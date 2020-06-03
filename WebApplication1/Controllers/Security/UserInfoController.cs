using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Expressions;
using OMF.Common;
using OMF.Enterprise.MVC;
using OMF.Business;
using System.Threading.Tasks;
using OMF.Security.Model;
using OMF.Common.Cryptography;
using OMF.Common.Configuration;
using System.Data.Entity;
using OMF.Common.Security;
using OMF.EntityFramework.Query;
using OMF.Common.Extensions;
using Zhivar.DomainClasses.Security;
using Zhivar.DomainClasses;
using Zhivar.ViewModel.Security;
using static OMF.Common.Enums;
using static Zhivar.DomainClasses.ZhivarEnums;
using Zhivar.ViewModel.Accunting;
using OMF.Common.ExceptionManagement.Exceptions;

namespace Zhivar.Web.Controllers.Security
{
    public class UserInfoController : ApiControllerCustomQueryBaseAsync<UserInfo, UserInfoVM, UserInfoVM>
    {
        protected override Expression<Func<UserInfo, bool>> CreateDefaultSearchExpression()
        {
            SecurityManager.ThrowIfUserContextNull();

            //if (SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == BehshoConstants.CALLCENTER_ROLE_CODE || x.RoleCode == BehshoConstants.MODIR_ROLE_CODE))
            //{
            //    return user => user.AuthenticationType == (int)BehshoEnums.BehshoUserType.MobileAppUser || user.AuthenticationType == (int)BehshoEnums.BehshoUserType.PezeshkUser;
            //}
            //else if (SecurityManager.CurrentUserContext.AuthenticationType != (int)BehshoEnums.BehshoUserType.Developers)
            //    return user => user.AuthenticationType != (int)BehshoEnums.BehshoUserType.Developers;


            return base.CreateDefaultSearchExpression();
        }

        protected override IOrderedQueryable<UserInfoVM> CreateOrderedQuery(IQueryable<UserInfoVM> query, List<SortInfo> sortInfoList)
        {
            if (sortInfoList != null && sortInfoList.Any())
            {
                string fullNamePropName = Entity.GetPropertyName<UserInfoVM>(uVm => uVm.FullName);
                SortInfo fullNameSortInfo = sortInfoList.FirstOrDefault(s => s.Field == fullNamePropName);
                if (fullNameSortInfo != null)
                    fullNameSortInfo.Field = Entity.GetPropertyName<UserInfoVM>(uVm => uVm.FirstName);
            }

            return base.CreateOrderedQuery(query, sortInfoList);
        }

        protected override IQueryable<UserInfoVM> CreateCustomQuery(IQueryable<UserInfo> query, QueryInfo searchRequestInfo)
        {
            var userRoleQuery = BusinessContext.GetBusinessRule<UserRole>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork)
                                               .Queryable();

            var roleQuery = this.BusinessRule.UnitOfWork.RepositoryAsync<RoleBase>()
                                                        .Queryable()
                                                        .Where(r => r.IsActive);

            #region FullName Filteration
            if (searchRequestInfo.Filter != null && searchRequestInfo.Filter.Filters != null && searchRequestInfo.Filter.Filters.Any())
            {
                string userFullNamePropName = Entity.GetPropertyName<UserInfoVM>(uVm => uVm.FullName);
                FilterData userFullNameFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault(filter => filter.Field == userFullNamePropName);
                if (userFullNameFilterData != null)
                {
                    FilterInfo userFullNamefilterInfo = new FilterInfo();
                    FilterData filterData = new FilterData() { Logic = "or", Filters = new List<FilterData>() };

                    string firstNameProp = Entity.GetPropertyName<UserInfo>(u => u.FirstName);
                    filterData.Filters.Add(new FilterData() { Field = firstNameProp, Operator = userFullNameFilterData.Operator, Value = userFullNameFilterData.Value });

                    string lastNameProp = Entity.GetPropertyName<UserInfo>(u => u.LastName);
                    filterData.Filters.Add(new FilterData() { Field = lastNameProp, Operator = userFullNameFilterData.Operator, Value = userFullNameFilterData.Value });

                    userFullNamefilterInfo.Filters.Add(filterData);
                    var userExpression = userFullNamefilterInfo.TranslateFilter<UserInfo>();
                    query = query.Where(userExpression);

                    searchRequestInfo.Filter.Filters.Remove(userFullNameFilterData);
                }
            }

            FilterInfo roleFilterInfo = new FilterInfo();
            if (searchRequestInfo.Filter != null && searchRequestInfo.Filter.Filters != null && searchRequestInfo.Filter.Filters.Any())
            {
                string roleNamesStrPropName = Entity.GetPropertyName<UserInfoVM>(uVm => uVm.RoleNamesStr);
                if (searchRequestInfo.Filter.Filters.Any(filter => filter.Field == roleNamesStrPropName))
                {
                    FilterData roleNamesStrFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault(filter => filter.Field == roleNamesStrPropName);
                    string roleNamesProp = Entity.GetPropertyName<RoleBase>(role => role.Name);

                    roleFilterInfo.Filters.Add(new FilterData() { Field = roleNamesProp, Operator = roleNamesStrFilterData.Operator, Value = roleNamesStrFilterData.Value });

                    var roleExpression = roleFilterInfo.TranslateFilter<RoleBase>();
                    roleQuery = roleQuery.Where(roleExpression);
                }
            }
            #endregion

            #region Create CustomQuery
            var joinQuery = from user in query
                            join userRole in
                                (from userRole in userRoleQuery
                                 join role in roleQuery
                                 on userRole.RoleId equals role.ID
                                 select new
                                 {
                                     UserID = userRole.UserId,
                                     RoleName = role.Name
                                 })
                            on user.ID equals userRole.UserID into userRoleGroup
                            select new
                            {
                                ID = user.ID,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                UserName = user.UserName,
                                MobileNo = user.MobileNo,
                                AuthenticationType = (int)user.AuthenticationType,
                                Gender = user.Gender,
                                NationalCode = user.NationalCode,
                                UserRoleGroup = userRoleGroup,
                                IsActive = user.IsActive,
                                ApplicationId = user.ApplicationId
                            };

            if (roleFilterInfo.Filters.Any())
                joinQuery = joinQuery.Where(q => q.UserRoleGroup.Any());


            IQueryable<UserInfoVM> endQuery = joinQuery.Select(item => new UserInfoVM
            {
                ID = item.ID,
                LastName = item.LastName,
                FirstName = item.FirstName,
                UserName = item.UserName,
                MobileNo = item.MobileNo,
                AuthenticationType = item.AuthenticationType,
                NationalCode = item.NationalCode,
                Gender = item.Gender,
                RoleNames = item.UserRoleGroup.Select(ur => ur.RoleName),
                IsActive = item.IsActive,
                ApplicationId = item.ApplicationId
            });

            return endQuery;
            #endregion
        }

        protected override UserInfoVM TranslateEntityToCustomQueryEntity(UserInfo entity)
        {
            var userInfoVm = base.TranslateEntityToCustomQueryEntity(entity);

            userInfoVm.ConfirmPassword = entity.Password;
            userInfoVm.UserRoles = entity.UserRoles == null ? null : entity.UserRoles.Select(userRole => userRole.Translate<UserRoleVM>());
            userInfoVm.UserOperations = entity.UserOperations == null ? null : entity.UserOperations.Select(userOperation => userOperation.Translate<UserOperationVM>());

            //TODO For Other AuthenticationType for example
            //if (userInfoVM.AuthenticationType != (int)BehshoEnums.BehshoUserType.LOBLOBLOB)
            //{
            //    userInfoVm.LOBLOBLOB = userInfo.TagInt1;
            //}

            return userInfoVm;
        }

        protected override UserInfo TranslateEntityVMToEntity(UserInfoVM entityVM)
        {
            var userInfo = new UserInfo()
            {
                ApplicationId = ConfigurationController.ApplicationID,
                AuthenticationType = entityVM.AuthenticationType,
                Email = entityVM.Email,
                FirstName = entityVM.FirstName,
                ID = entityVM.ID,
                IsActive = entityVM.IsActive,
                IsDeleted = entityVM.IsDeleted,
                LastLoginDate = entityVM.LastLoginDate,
                LastName = entityVM.LastName,
                LoginTryTime = entityVM.LoginTryTime,
                MobileNo = entityVM.MobileNo,
                Tel = entityVM.Tel,
                Gender = entityVM.Gender,
                UserName = entityVM.UserName,
                NationalCode = entityVM.NationalCode
                //TODO For Other AuthenticationType for example
                //TagInt1 = entityVM.LOBLOBLOB;                
            };

            return userInfo;
        }

        public async Task<HttpResponseMessage> GetUserOperationOperationAccess()
        {
            using (var userOperationRule = BusinessContext.GetBusinessRule<UserOperation>())
            {
                OperationAccess oprAccess = await userOperationRule.CreateOperationAccessAsync();
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = oprAccess });
            }
        }

        public async Task<HttpResponseMessage> GetUserRoleOperationAccess()
        {
            using (var userRoleRule = BusinessContext.GetBusinessRule<UserRole>())
            {
                OperationAccess oprAccess = await userRoleRule.CreateOperationAccessAsync();
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = oprAccess });
            }
        }

        public async Task<HttpResponseMessage> AddUserInfo(UserInfoVM userInfoVM)
        {
            try
            {
                var userInfo = TranslateEntityVMToEntity(userInfoVM);
                userInfo.PlainPassword = userInfoVM.Password;
                userInfo.Password = CryptoHelper.ComputeHash(userInfoVM.Password);
                userInfo.ObjectState = ObjectState.Added;
                userInfo.AuthenticationType = 3;
                userInfo.OrganizationId = 22;
                //TODO For Other AuthenticationType for example
                //if (userInfoVM.AuthenticationType != (int)BehshoEnums.BehshoUserType.LOBLOBLOB)
                //{
                //    userInfo.TagInt1 = null;
                //}

                var oldUsers = this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().Queryable().Where(x => x.UserName == userInfoVM.UserName);

                if (oldUsers.Any())
                {
                     throw new OMFValidationException("این نام کاربری قبلا استفاده شده است.");
                }

                this.BusinessRule.Insert(userInfo);
                await this.BusinessRule.SaveChangesAsync();

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = userInfo.ID });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        public async Task<HttpResponseMessage> UpdateUserInfo(UserInfoVM userInfoVM)
        {
            try
            {
                var userInfo = TranslateEntityVMToEntity(userInfoVM);
                userInfo.ObjectState = ObjectState.Modified;
                userInfo.Password = await this.BusinessRule.Queryable().Where(u => u.ID == userInfo.ID).Select(u => u.Password).SingleOrDefaultAsync();

                //TODO For Other AuthenticationType for example
                //if (userInfoVM.AuthenticationType != (int)BehshoEnums.BehshoUserType.LOBLOBLOB)
                //{
                //    userInfo.TagInt1 = null;
                //}

                this.BusinessRule.Update(userInfo);
                await this.BusinessRule.SaveChangesAsync();

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = userInfo.ID });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        public async Task<HttpResponseMessage> UpdateOperations(PermisionUserVM permisionUser)
        {
            try
            {
                using (var operationBusiness = BusinessContext.GetBusinessRule<Operation>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork))
                using (var userOperationBusiness = BusinessContext.GetBusinessRule<UserOperation>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork))
                {
                    var operationQuery = operationBusiness.Queryable()
                                                          .Where(opr => opr.OperationType != OperationType.Other);

                    var dbUserOperations = userOperationBusiness.Queryable()
                                                                .Where(userOpr => userOpr.UserId == permisionUser.UserId);

                    var dbUserOperationsList = await (from opr in operationQuery
                                                      join userOpr in dbUserOperations
                                                      on opr.ID equals userOpr.OperationId
                                                      select userOpr).ToListAsync();

                    var currentUserOperationIds = permisionUser.OperationUsers.Select(s => s.OperationId);

                    var dbUserOprOperationIds = dbUserOperationsList.Select(dbUserOpr => dbUserOpr.OperationId);
                    var userOprOperationIdsForDelete = dbUserOprOperationIds.Except(currentUserOperationIds);
                    foreach (var userOprOperaionID in userOprOperationIdsForDelete)
                    {
                        UserOperation userOperation = dbUserOperationsList.FirstOrDefault(dbUserOpr => dbUserOpr.OperationId == userOprOperaionID);
                        userOperationBusiness.Delete(userOperation);
                    }

                    var userOperationIdsForInsert = permisionUser.OperationUsers.Select(currentUserOpr => currentUserOpr.OperationId).Except(dbUserOprOperationIds);
                    foreach (var userOprIdForInsert in userOperationIdsForInsert)
                    {
                        UserOperation userOperationForInsert = new UserOperation() { OperationId = userOprIdForInsert, UserId = permisionUser.UserId };
                        userOperationBusiness.Insert(userOperationForInsert);
                    }

                    await userOperationBusiness.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = permisionUser.UserId });
                }
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
            finally
            {
                BusinessRule.Dispose();
            }
        }

        public async Task<HttpResponseMessage> UpdateUserRole(AccessUserRoleVM accessUserRole)
        {
            try
            {
                using (var userRoleBusiness = BusinessContext.GetBusinessRule<UserRole>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork))
                {
                    var dbUserRoles = await userRoleBusiness.Queryable().Where(userRole => userRole.UserId == accessUserRole.UserId).ToListAsync();
                    var currentUserRoleIds = accessUserRole.UserRoles.Select(s => s.RoleId);

                    var dbUserRoleIds = dbUserRoles.Select(dbUserRole => dbUserRole.RoleId);
                    var userRoleIdsForDelete = dbUserRoleIds.Except(currentUserRoleIds);
                    foreach (var userRoleID in userRoleIdsForDelete)
                    {
                        UserRole userRole = dbUserRoles.FirstOrDefault(dbUserRole => dbUserRole.RoleId == userRoleID);
                        userRoleBusiness.Delete(userRole);
                    }

                    var userRoleIdsForInsert = accessUserRole.UserRoles.Select(currentUserRole => currentUserRole.RoleId).Except(dbUserRoleIds);
                    foreach (var userRoleIdForInsert in userRoleIdsForInsert)
                    {
                        UserRole userRoleForInsert = new UserRole() { RoleId = userRoleIdForInsert, UserId = accessUserRole.UserId };
                        userRoleBusiness.Insert(userRoleForInsert);
                    }

                    await userRoleBusiness.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = accessUserRole.UserId });
                }

            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
            finally
            {
                BusinessRule.Dispose();
            }
        }

        [HttpGet]
        public async Task<HttpResponseMessage> FindUserInfo(int userID)
        {
            try
            {
                UserInfoVM userInfoVm = null;
                UserInfo userInfo = await BusinessRule.FindAsync(userID);
                userInfoVm = TranslateEntityToCustomQueryEntity(userInfo);

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = userInfoVm });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
            finally
            {
                BusinessRule.Dispose();
            }
        }

        public async Task<HttpResponseMessage> GetChangeUserPasswordOperationAccess()
        {
            var hasAccess = await SecurityManager.HasAccessAsync(ZhivarResourceIds.Zhivar_Security_ChangePassword);
            var oprAccess = new OperationAccess() { CanView = hasAccess, CanUpdate = hasAccess };
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = oprAccess });
        }
        //public async Task<HttpResponseMessage> GetUserMobileOperationAccess()
        //{
        //    var hasAccess = await SecurityManager.HasAccessAsync(BehshoResourceIds.Behsho_Security_MobileUser_View);
        //    var oprAccess = new OperationAccess() { CanView = hasAccess, CanUpdate = hasAccess };
        //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = oprAccess });
        //}
        public async Task<HttpResponseMessage> GetUserInfoAsync()
        {
            try
            {
                var userQuery = this.BusinessRule.Queryable();

                var userList = await userQuery.Select(user => new
                {
                    ID = user.ID,
                    OrganizationId = user.OrganizationId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName
                }).ToListAsync();

                var result = userList.ConvertAll(user => new KeyValueVM
                {
                    Key = user.ID,
                    Value = $"{user.FirstName} {user.LastName} | {user.UserName}"
                });

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = result });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
            finally
            {
                BusinessRule.Dispose();
            }
        }

        public async Task<HttpResponseMessage> GetUserMobileListByFilter([FromUri]QueryInfo searchRequestInfo)
        {
            try
            {
                if (searchRequestInfo?.Filter?.Filters == null)
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = new List<KeyValueVM>() });

                string nameFilterValue = searchRequestInfo.Filter.Filters.FirstOrDefault(x => x.Field.ToLower() == "value")?.Value;

                if (nameFilterValue == null)
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = new List<KeyValueVM>() });

                var userQuery = from user in this.BusinessRule.Queryable()
                                select new KeyValueVM
                                {
                                    Key = user.ID,
                                    Value = user.MobileNo
                                };
                var userList = await userQuery.ToListAsync2();
                List<KeyValueVM> result = null;

                result = userList.Where(md => md.Value.Contains(nameFilterValue)).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = result });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }


        [HttpPost]
        public async Task<HttpResponseMessage> UsersByRole()
        {
            try
            {

                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);
                using (var uow = new UnitOfWork())
                {
                    var userInfosQuery = uow.Repository<UserInfo>().Queryable().Where(x => x.OrganizationId == organId);
                    var userRoleQuery = uow.Repository<UserRole>().Queryable();
                    var roleQuery = uow.Repository<Role>().Queryable();


                    var joinQuery = from userInfo in userInfosQuery
                                    join userRole in userRoleQuery
                                    on userInfo.ID equals userRole.UserId
                                    join role in roleQuery
                                    on userRole.RoleId equals role.ID
                                    select new
                                    {
                                        ID = userInfo.ID,
                                        Name = userInfo.FirstName + " " + userInfo.LastName,
                                        RoleId = userRole.RoleId,
                                        RoleName = role.Name,
                                        //  DetailAccount = new DetailAccount { }

                                    };

                    var nasabs = await joinQuery.Where(x => x.RoleId == 10).ToListAsync();
                    var chapkhanes = await joinQuery.Where(x => x.RoleId == 9).ToListAsync();
                    var tarahs = await joinQuery.Where(x => x.RoleId == 8).ToListAsync();
                    var bazaryabs = await joinQuery.Where(x => x.RoleId == 7).ToListAsync();

                    //var nasabs = await joinQuery.ToListAsync();
                    //var chapkhanes = await joinQuery.ToListAsync();
                    //var tarahs = await joinQuery.ToListAsync();
                    //var bazaryabs = await joinQuery.ToListAsync();


                    // UserInfoVM userInfoVm = null;
                    //  UserInfo userInfo = await BusinessRule.(userID);
                    //userInfoVm = TranslateEntityToCustomQueryEntity(userInfo);

                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = new { nasabs, chapkhanes, tarahs, bazaryabs } });
            }}
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
            finally
            {
                BusinessRule.Dispose();
            }
        }

        public async Task<HttpResponseMessage> AddUserInfo2(UserInfoVM userInfoVM)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);
                var userInfo = TranslateEntityVMToEntity(userInfoVM);
                userInfo.PlainPassword = userInfoVM.Password;
                userInfo.Password = CryptoHelper.ComputeHash(userInfoVM.Password);
                userInfo.ObjectState = ObjectState.Added;
                userInfo.OrganizationId = organId;
                userInfo.AuthenticationType = 3;

                userInfo.UserRoles = new List<UserRole>() { new UserRole() { ObjectState = ObjectState.Added, RoleId = (int)userInfoVM.RoleId } };

                var oldUsers = this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().Queryable().Where(x => x.UserName == userInfoVM.UserName);

                if (oldUsers.Any())
                {
                    throw new OMFValidationException("این نام کاربری قبلا استفاده شده است.");
                }

                using (var uow = new UnitOfWork())
                {
                    uow.Repository<UserInfo>().Insert(userInfo);
                    await uow.SaveChangesAsync();
                }

                //    this.BusinessRule.Insert(userInfo);
            //    await this.BusinessRule.SaveChangesAsync();

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = userInfo });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }
    }
}

