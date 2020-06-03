using OMF.Common;
using OMF.Common.Cache;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Security;
using OMF.EntityFramework.DataContext;
using OMF.EntityFramework.Ef6;
//using OMF.EntityFramework.DataContext;
//using OMF.EntityFramework.Ef6;
using OMF.Security.Model;
using OMF.Security.TokenManagement;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace OMF.Security
{
    public class OMFSecurityProvider : ISecurityProviderAsync, ISecurityProvider
    {
        private static OMFSecurityProvider instance;

        private OMFSecurityProvider()
        {
        }

        public static OMFSecurityProvider Instance
        {
            get
            {
                if (OMFSecurityProvider.instance == null)
                    OMFSecurityProvider.instance = new OMFSecurityProvider();
                return OMFSecurityProvider.instance;
            }
        }

        public List<Operation> GetUserOperations()
        {
            SecurityManager.ThrowIfUserContextNull();
            string key = "Operations" + SecurityManager.CurrentUserToken;
            if (SessionManager.Contains(key))
                return SessionManager.GetData(key) as List<Operation>;
            List<Operation> operationList;
            if (!ConfigurationController.EnableSecurityCheck)
                operationList = this.GetAllOperations();
            else if (this.UserInRole("administrator"))
            {
                operationList = this.GetAllOperations().Where<Operation>((Func<Operation, bool>)(opr => !opr.IsSystem)).ToList<Operation>();
            }
            else
            {
                SecurityManager.ThrowIfUserContextNull();
                using (SecurityDbContext securityDbContext = new SecurityDbContext())
                    operationList = securityDbContext.GetOperationsByUserID(SecurityManager.CurrentUserContext.UserId);
            }
            SessionManager.Add(key, (object)operationList);
            return operationList;
        }

        public List<Operation> GetOperationsByRoleCode(string roleCode, int? applicationId = null)
        {
            using (SecurityDbContext securityDbContext = new SecurityDbContext())
            {
                if (!applicationId.HasValue)
                    applicationId = ConfigurationController.ApplicationID;
                return securityDbContext.GetOperationsByRoleCode(roleCode, applicationId);
            }
        }

        private List<Operation> GetAllOperations()
        {
            using (SecurityDbContext securityDbContext = new SecurityDbContext())
                return  securityDbContext.Operations.Where<Operation>((Expression<Func<Operation, bool>>)(opr => opr.IsActive && !opr.IsDeleted)).ToList<Operation>();
        }

        public List<Operation> GetUserOperations(OperationType operationType)
        {
            return this.GetUserOperations().Where<Operation>((Func<Operation, bool>)(op => op.OperationType == operationType)).OrderBy<Operation, int>((Func<Operation, int>)(op => op.OrderNo)).ToList<Operation>();
        }

        public bool ExistsUser(string userName, int? applicationId = null)
        {
            using (SecurityDbContext securityDbContext = new SecurityDbContext())
            {
                IQueryable<UserInfo> source = securityDbContext.Users.Where<UserInfo>((Expression<Func<UserInfo, bool>>)(u => u.UserName == userName && !u.IsDeleted && u.IsActive));
                if (!applicationId.HasValue)
                    applicationId = ConfigurationController.ApplicationID;
                if (applicationId.HasValue)
                    source = source.Where<UserInfo>((Expression<Func<UserInfo, bool>>)(u => u.ApplicationId == applicationId));
                return source.Any<UserInfo>();
            }
        }

        public bool UserInRole(string roleCode)
        {
            if (SecurityManager.CurrentUserContext == null || (SecurityManager.CurrentUserContext.Roles == null || SecurityManager.CurrentUserContext.Roles.Count == 0))
                return false;
            string lowerRoleCode = roleCode.ToLower();
            return SecurityManager.CurrentUserContext.Roles.Any<RoleData>((Func<RoleData, bool>)(r => r.RoleCode.ToLower() == lowerRoleCode));
        }

        public bool HasAccess(string operationCode)
        {
            if (string.IsNullOrWhiteSpace(operationCode))
                return false;
            SecurityManager.ThrowIfUserContextNull();
            if (!ConfigurationController.EnableSecurityCheck)
                return true;
            return this.GetUserOperations().Any(opr => opr.Code.ToLower() == operationCode.ToLower());
        }

        public bool HasAccess(int userId, string operationCode)
        {
            using (SecurityDbContext securityDbContext = new SecurityDbContext())
                return securityDbContext.CheckSecurityAccess(userId, operationCode);
        }

        public bool HasPageAccess(string operationCode)
        {
            if (!ConfigurationController.EnableSecurityCheck)
                return true;
            if (string.IsNullOrWhiteSpace(operationCode))
                return false;
            string key = "PageAccess" + SecurityManager.CurrentUserToken;
            List<string> stringList;
            if (SessionManager.Contains(key))
            {
                stringList = SessionManager.GetData(key) as List<string>;
            }
            else
            {
                List<Operation> userOperations = this.GetUserOperations();
                stringList = userOperations.Select<Operation, string>((Func<Operation, string>)(opr => opr.Code.ToLower())).ToList<string>();
                stringList.AddRange(userOperations.Where<Operation>((Func<Operation, bool>)(opr => !string.IsNullOrWhiteSpace(opr.Tag1))).Select<Operation, string>((Func<Operation, string>)(opr =>
                {
                    if (!opr.Tag1.Contains(";;"))
                        return opr.Tag1.Replace("~/", "").ToLower();
                    return opr.Tag1.Split(new string[1] { ";;" }, StringSplitOptions.None)[0].Replace("~/", "").ToLower();
                })));
                SessionManager.Add(key, (object)stringList);
            }
            return stringList.Contains(operationCode.Replace("~/", "").ToLower());
        }

        public UserContext Login(string userName, string password)
        {
            using (SecurityDbContext securityDbContext = new SecurityDbContext())
            {
                UserInfo user = this.GetUser(userName, new int?());
                if (user == null)
                    return (UserContext)null;
                if (!OperationAccess.PasswordEqual(user.Password, password))
                {
                    if (user.LoginTryTime >= (int)ConfigurationController.LoginTryNo)
                        user.IsActive = false;
                    ++user.LoginTryTime;
                    //user.ObjectState = ObjectState.Modified;
                    //securityDbContext.SyncObjectState<UserInfo>(user);
                    securityDbContext.SaveChanges();
                    return (UserContext)null;
                }
                if (!user.IsActive)
                {
                    SecurityManager.SaveActivityLogForLoginFailedAction(userName, "کاربر غیر فعال");
                    throw new LoginException(ConfigurationController.ApplicationLanguage == AppLanguage.English ? "User is not Active." : "کاربر غیر فعال می باشد.");
                }
                user.LastLoginDate = new DateTime?(DateTime.Now);
                user.LoginTryTime = 0;
                //user.ObjectState = ObjectState.Modified;
                //securityDbContext.SyncObjectState<UserInfo>(user);
                securityDbContext.SaveChanges();
                return this.InitiateUserContext(user);
            }
        }

        public UserContext Logoff(string userToken)
        {
            UserContext user = TokenManager.GetUser(userToken);
            if (user != null)
                TokenManager.DeleteToken(userToken);
            Action<string> afterLogoffHandler = this.AfterLogoffHandler;
            if (afterLogoffHandler != null)
                afterLogoffHandler(userToken);
            return user;
        }

        public event Action<string> AfterLogoffHandler;

        public UserInfo GetUser(int userId)
        {
            using (SecurityDbContext securityDbContext = new SecurityDbContext())
                return securityDbContext.Users.Find((object)userId);
        }

        public UserInfo GetUser(string userName, int? applicationId = null)
        {
            
            using (SecurityDbContext securityDbContext = new SecurityDbContext())
            {
                IQueryable<UserInfo> source = securityDbContext.Users.Where<UserInfo>((Expression<Func<UserInfo, bool>>)(u => u.UserName == userName && !u.IsDeleted));
                if (!applicationId.HasValue)
                    applicationId = ConfigurationController.ApplicationID;
                if (applicationId.HasValue)
                    source = source.Where<UserInfo>((Expression<Func<UserInfo, bool>>)(u => u.ApplicationId == applicationId));
                return source.FirstOrDefault<UserInfo>();
            }
        }

        public async Task<UserContext> LoginAsync(string userName, string password)
        {
            using (SecurityDbContext context = new SecurityDbContext())
            {
                UserInfo uInfo = await this.GetUserAsync(userName, new int?());
                if (uInfo == null)
                    return (UserContext)null;
                if (!OperationAccess.PasswordEqual(uInfo.Password, password))
                {
                    if (uInfo.LoginTryTime >= (int)ConfigurationController.LoginTryNo)
                        uInfo.IsActive = false;
                    ++uInfo.LoginTryTime;
                    //uInfo.ObjectState = ObjectState.Modified;
                    //context.SyncObjectState<UserInfo>(uInfo);
                    int num = await context.SaveChangesAsync();
                    return (UserContext)null;
                }
                if (!uInfo.IsActive)
                {
                    await SecurityManager.SaveActivityLogForLoginFailedActionAsync(userName, "کاربر غیر فعال");
                    throw new LoginException( "کاربر غیر فعال می باشد.");
                }
                uInfo.LastLoginDate = new DateTime?(DateTime.Now);
                uInfo.LoginTryTime = 0;
                //uInfo.ObjectState = ObjectState.Modified;
                //context.SyncObjectState<UserInfo>(uInfo);
                int num1 = await context.SaveChangesAsync();
                UserContext userContext = this.InitiateUserContext(uInfo);
                return userContext;
            }
        }

        public async Task<bool> HasAccessAsync(int userId, string operationCode)
        {
            bool flag;
            using (SecurityDbContext db = new SecurityDbContext())
                flag = await db.CheckSecurityAccessAsync(userId, operationCode);
            return flag;
        }

        public async Task<bool> HasAccessAsync(string operationCode)
        {
            if (string.IsNullOrWhiteSpace(operationCode))
                return false;
            SecurityManager.ThrowIfUserContextNull();
            if (!ConfigurationController.EnableSecurityCheck)
                return true;
            List<Operation> userOperations = await this.GetUserOperationsAsync();
            return userOperations.Any<Operation>((Func<Operation, bool>)(opr => opr.Code.ToLower() == operationCode.ToLower()));
        }

        public async Task<bool> UserInRoleAsync(string roleCode)
        {
            if (SecurityManager.CurrentUserContext == null || (SecurityManager.CurrentUserContext.Roles == null || SecurityManager.CurrentUserContext.Roles.Count == 0))
                return false;
            string lowerRoleCode = roleCode.ToLower();
            return SecurityManager.CurrentUserContext.Roles.Any<RoleData>((Func<RoleData, bool>)(r => r.RoleCode.ToLower() == lowerRoleCode));
        }

        public async Task<bool> HasPageAccessAsync(string operationCode)
        {
            if (!ConfigurationController.EnableSecurityCheck)
                return true;
            if (string.IsNullOrWhiteSpace(operationCode))
                return false;
            string key = "PageAccess" + SecurityManager.CurrentUserToken;
            List<string> accessList;
            if (SessionManager.Contains(key))
            {
                accessList = SessionManager.GetData(key) as List<string>;
            }
            else
            {
                List<Operation> operationList = await this.GetUserOperationsAsync();
                accessList = operationList.Select<Operation, string>((Func<Operation, string>)(opr => opr.Code.ToLower())).ToList<string>();
                accessList.AddRange(operationList.Where<Operation>((Func<Operation, bool>)(opr => !string.IsNullOrWhiteSpace(opr.Tag1))).Select<Operation, string>((Func<Operation, string>)(opr =>
                {
                    if (!opr.Tag1.Contains(";;"))
                        return opr.Tag1.Replace("~/", "").ToLower();
                    return opr.Tag1.Split(new string[1] { ";;" }, StringSplitOptions.None)[0].Replace("~/", "").ToLower();
                })));
                SessionManager.Add(key, (object)accessList);
                operationList = (List<Operation>)null;
            }
            return accessList.Contains(operationCode.Replace("~/", "").ToLower());
        }

        public async Task<List<Operation>> GetUserOperationsAsync()
        {
            SecurityManager.ThrowIfUserContextNull();
            string key = "Operations" + SecurityManager.CurrentUserToken;
            if (SessionManager.Contains(key))
                return SessionManager.GetData(key) as List<Operation>;
            List<Operation> accessList;
            if (!ConfigurationController.EnableSecurityCheck)
                accessList = await this.GetAllOperationsAsync();
            else if (this.UserInRole("administrator"))
            {
                accessList = await this.GetAllOperationsAsync();
                accessList = accessList.Where<Operation>((Func<Operation, bool>)(opr => !opr.IsSystem)).ToList<Operation>();
        }
            else
            {
                SecurityManager.ThrowIfUserContextNull();
                using (SecurityDbContext db = new SecurityDbContext())
                {
                    accessList = await db.GetOperationsByUserIDAsync(SecurityManager.CurrentUserContext.UserId);
                    if (ConfigurationController.ApplicationID.HasValue)
                        accessList = accessList.Where<Operation>((Func<Operation, bool>)(opr =>
                        {
                            int applicationId1 = opr.ApplicationId;
                            int? applicationId2 = ConfigurationController.ApplicationID;
                            int valueOrDefault = applicationId2.GetValueOrDefault();
                            if (applicationId1 != valueOrDefault)
                                return false;
                            return applicationId2.HasValue;
                        })).ToList<Operation>();
    }
}
SessionManager.Add(key, (object)accessList);
            return accessList ?? new List<Operation>();
        }

        public async Task<List<Operation>> GetOperationsByRoleCodeAsync(string roleCode)
        {
            List<Operation> operationList1;
            using (SecurityDbContext context = new SecurityDbContext())
            {
                List<Operation> operationList = await context.GetOperationsByRoleCodeAsync(roleCode, ConfigurationController.ApplicationID);
                IEnumerable<Operation> source = (IEnumerable<Operation>)operationList;
                List<Operation> operations = source.ToList<Operation>();
                operationList = (List<Operation>)null;
                source = (IEnumerable<Operation>)null;
                operationList1 = operations;
            }
            return operationList1;
        }

        private async Task<List<Operation>> GetAllOperationsAsync()
        {
            List<Operation> operationList;
            using (SecurityDbContext context = new SecurityDbContext())
            {
                IQueryable<Operation> operationQuery =  context.Operations.Where(opr => opr.IsActive && !opr.IsDeleted);

                if (ConfigurationController.ApplicationID.HasValue)
                    operationQuery = operationQuery.Where(opr => opr.ApplicationId == ConfigurationController.ApplicationID);
                List<Operation> operations = await operationQuery.ToListAsync();//.ToListAsync<Operation>();
                operationList = operations;
            }
            return operationList;
        }

        public async Task<List<Operation>> GetUserOperationsAsync(
          OperationType operationType)
        {
            List<Operation> accessList = await this.GetUserOperationsAsync();
            accessList = accessList.Where<Operation>((Func<Operation, bool>)(op => op.OperationType == operationType)).OrderBy<Operation, int>((Func<Operation, int>)(op => op.OrderNo)).ToList<Operation>();
            return accessList;
        }

        public async Task<UserInfo> GetUserAsync(int userId)
        {
            UserInfo userInfo;
            using (SecurityDbContext context = new SecurityDbContext())
            {
                UserInfo user = await context.Users.FindAsync((object)userId);
                userInfo = user;
            }
            return userInfo;
        }

        public async Task<UserInfo> GetUserAsync(string userName, int? applicationId = null)
        {
            try
            {
                UserInfo userInfo = new UserInfo();
                using (SecurityDbContext context = new SecurityDbContext())
                {
                    IQueryable<UserInfo> userQuery = context.Users.Where(u => u.UserName == userName && !u.IsDeleted);
                    if (!applicationId.HasValue)
                        applicationId = ConfigurationController.ApplicationID;
                    if (applicationId.HasValue)
                        userQuery = userQuery.Where(u => u.ApplicationId == applicationId);
                    UserInfo user = await userQuery.FirstOrDefaultAsync();
                    userInfo = user;
                }
                return userInfo;
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }

        public async Task<bool> ExistsUserAsync(string userName, int? applicationId = null)
        {
            bool flag;
            using (SecurityDbContext context = new SecurityDbContext())
            {
                IQueryable<UserInfo> userQuery = context.Users.Where<UserInfo>((Expression<Func<UserInfo, bool>>)(u => u.UserName == userName && !u.IsDeleted && u.IsActive));
                if (!applicationId.HasValue)
                    applicationId = ConfigurationController.ApplicationID;
                if (applicationId.HasValue)
                    userQuery = userQuery.Where<UserInfo>((Expression<Func<UserInfo, bool>>)(u => u.ApplicationId == applicationId));
                flag = await userQuery.AnyAsync<UserInfo>();
            }
            return flag;
        }

        private UserContext InitiateUserContext(UserInfo uInfo)
        {
            UserContext userContext =  uInfo.ConvertToUserContext();
            TokenManager.TokenizeUser(userContext);
            return userContext;
        }

        public RoleDataCollection GetUserRoles(int userId)
        {
           
            using (UnitOfWork unitOfWork = new UnitOfWork((IDataContextAsync)new SecurityDbContext()))
            {
                IQueryable<RoleBase> queryable = unitOfWork.RepositoryAsync<RoleBase>().Queryable(false, true, (List<Expression<Func<RoleBase, object>>>)null).Where(x => x.IsActive && !x.IsDeleted);
                if (ConfigurationController.ApplicationID.HasValue)
                    queryable = queryable.Where<RoleBase>((Expression<Func<RoleBase, bool>>)(role => role.ApplicationId == ConfigurationController.ApplicationID));


                IQueryable<RoleData> source = queryable.Join((IEnumerable<UserRole>)unitOfWork.RepositoryAsync<UserRole>().Queryable(false, true, (List<Expression<Func<UserRole, object>>>)null).Where(x => x.UserId == userId),
                    (Expression<Func<RoleBase, int>>)(role => role.ID),
                    (Expression<Func<UserRole, int>>)(userRole => userRole.RoleId),
                    (role, userRole) => new RoleData
                    {
                        RoleCode = role.Code,
                        RoleID = role.ID,
                        RoleName = role.Name,
                        RelatedUserId = userRole.UserId
                        //role = role,
                        //userRole = userRole
                    });//.Where(data => data.role.IsActive &&
                             //   !data.role.IsDeleted &&
                              //   data.userRole.UserId == userId);
                RoleDataCollection roleDataCollection = new RoleDataCollection();
                foreach (RoleData roleData in source.Distinct<RoleData>().ToList<RoleData>())
                    roleDataCollection.Add(roleData);
                return roleDataCollection;
            }
        }

        public void ThrowIfUserContextNull()
        {
            if (SecurityManager.CurrentUserContext == null)
            {
                SessionManager.Clear();
                throw new UserContextNullException();
            }
            TokenInfo tokenInfoByToken = TokenManager.GetTokenInfoByToken(SecurityManager.CurrentUserToken);
            if (tokenInfoByToken == null)
            {
                SessionManager.Clear();
                throw new UserContextNullException();
            }
            tokenInfoByToken.LastAccess = DateTime.Now;
        }
    }
}
