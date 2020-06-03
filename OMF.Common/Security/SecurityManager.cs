using System;
using System.Diagnostics;
using System.Threading.Tasks;
using OMF.Common.ActivityLog;
using OMF.Common.Cache;
using OMF.Common.ExceptionManagement;
using OMF.Common.Log;
using OMF.Common.Serialization;
using static OMF.Common.Enums;

namespace OMF.Common.Security
{
    public static class SecurityManager
    {
        private static ISecurityProviderAsync security;

        private static ISecurityProviderAsync SecurityProvider
        {
            get
            {
                return SecurityManager.security;
            }
        }

        public static void InitiateSecurityProvider(ISecurityProviderAsync securityProvider)
        {
            SecurityManager.security = securityProvider;
        }

        public static bool HasAccess(string accessKey)
        {
            return SecurityManager.SecurityProvider.HasAccess(accessKey);
        }

        public static bool HasAccess(int userId, string operationCode)
        {
            return SecurityManager.SecurityProvider.HasAccess(userId, operationCode);
        }

        public static bool HasPageAccess(string securityKey)
        {
            return SecurityManager.SecurityProvider.HasPageAccess(securityKey);
        }

        public static UserContext Login(string userName, string password, bool needToLog)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SecurityManager.CurrentUserToken))
                    SecurityManager.Logoff(SecurityManager.CurrentUserToken);
                UserContext userContext = SecurityManager.SecurityProvider.Login(userName, password);
                if (userContext == null)
                {
                    SecurityManager.SaveActivityLogForLoginFailedAction(userName, "نام کاربری یا رمز عبور نادرست");
                }
                else
                {
                    if (needToLog)
                        SecurityManager.SaveActivityLogForLoginAction(userName);
                    SecurityManager.CurrentUserToken = userContext.Token;
                    SecurityManager.CurrentUserContext = userContext;
                }
                return userContext;
            }
            catch (Exception ex)
            {
                EventLogHelper.Write(ExceptionManager.GetExceptionMessageWithDebugInfo(ex), EventLogEntryType.Error);
                throw;
            }
        }

        public static void Logoff(string userToken)
        {
            UserContext userContext = SecurityManager.SecurityProvider.Logoff(userToken);
            if (userContext == null)
                return;
            SecurityManager.SaveActivityLogForLogOffAction(userContext);
        }

        public static bool UserInRole(string roleCode)
        {
            return SecurityManager.SecurityProvider.UserInRole(roleCode);
        }

        public static async Task<bool> HasAccessAsync(int userId, string operationCode)
        {
            bool flag = await SecurityManager.SecurityProvider.HasAccessAsync(userId, operationCode);
            return flag;
        }

        public static async Task<bool> HasAccessAsync(string accessKey)
        {
            bool result = await SecurityManager.SecurityProvider.HasAccessAsync(accessKey);
            return result;
        }

        public static async Task<bool> HasPageAccessAsync(string securityKey)
        {
            bool result = await SecurityManager.SecurityProvider.HasPageAccessAsync(securityKey);
            return result;
        }

        public static async Task<UserContext> LoginAsync(
          string userName,
          string password,
          bool needToLog)
        {
            if (!string.IsNullOrWhiteSpace(SecurityManager.CurrentUserToken))
                await SecurityManager.LogoffAsync(SecurityManager.CurrentUserToken);
            UserContext userContext = await SecurityManager.SecurityProvider.LoginAsync(userName, password);
            if (userContext == null)
            {
                await SecurityManager.SaveActivityLogForLoginFailedActionAsync(userName, "نام کاربری یا رمز عبور نادرست");
            }
            else
            {
                if (needToLog)
                    await SecurityManager.SaveActivityLogForLoginActionAsync(userName);
                SecurityManager.CurrentUserToken = userContext.Token;
                SecurityManager.CurrentUserContext = userContext;
            }
            return userContext;
        }

        public static async Task LogoffAsync(string userToken)
        {
            UserContext userContext = SecurityManager.SecurityProvider.Logoff(userToken);
            if (userContext == null)
                return;
            await SecurityManager.SaveActivityLogForLogOffActionAsync(userContext);
        }

        public static async Task<bool> UserInRoleAsync(string roleCode)
        {
            bool result = await SecurityManager.SecurityProvider.UserInRoleAsync(roleCode);
            return result;
        }

        public static void ThrowIfUserContextNull()
        {
            SecurityManager.SecurityProvider.ThrowIfUserContextNull();
        }

        public static UserContext CurrentUserContext
        {
            get
            {
                return SessionManager.GetData("__CurrentUserContext__") as UserContext;
            }
            set
            {
                SessionManager.Add("__CurrentUserContext__", (object)value);
            }
        }

        public static string CurrentUserToken
        {
            get
            {
                return SessionManager.GetData("__CurrentUserToken__")?.ToString();
            }
            set
            {
                SessionManager.Add("__CurrentUserToken__", (object)value);
            }
        }

        public static async Task SaveActivityLogForLoginActionAsync(string userName)
        {
            ActivityLog.ActivityLog activityLog = ActivityLogManager.CreateActivityLog();
            activityLog.ForceLog = true;
            activityLog.EntityName = "ورود";
            activityLog.UserName = userName;
            activityLog.VisibleForEndUser = true;
            activityLog.EntityID = SecurityManager.CurrentUserToken;
            activityLog.Action = 6;
            ActivityLog.ActivityLog activityLog1 = activityLog;
            ActivityLogData activityLogData = new ActivityLogData();
            activityLogData.Data = Serialization.SerializationHelper.SerializeCustom((object)new
            {
                UserName = userName,
                UserToken = activityLog.EntityID,
                RecordDateTime = DateTime.Now,
                Action = ActionType.Login.ToString()
            });
            activityLogData.ObjectState = ObjectState.Added;
            activityLog1.LogData = activityLogData;
            await ActivityLogManager.SaveAsync(activityLog);
        }

        public static void SaveActivityLogForLoginAction(string userName)
        {
            ActivityLog.ActivityLog activityLog1 = ActivityLogManager.CreateActivityLog();
            activityLog1.ForceLog = true;
            activityLog1.UserName = userName;
            activityLog1.EntityName = "ورود";
            activityLog1.VisibleForEndUser = true;
            activityLog1.EntityID = SecurityManager.CurrentUserToken;
            activityLog1.Action = 6;
            ActivityLog.ActivityLog activityLog2 = activityLog1;
            ActivityLogData activityLogData = new ActivityLogData();
            activityLogData.Data = SerializationHelper.SerializeCustom((object)new
            {
                UserName = userName,
                UserToken = activityLog1.EntityID,
                RecordDateTime = DateTime.Now,
                Action = ActionType.Login.ToString()
            });
            activityLogData.ObjectState = ObjectState.Added;
            activityLog2.LogData = activityLogData;
            ActivityLogManager.Save(activityLog1);
        }

        public static async Task SaveActivityLogForLoginFailedActionAsync(
          string userName,
          string reason)
        {
            ActivityLog.ActivityLog activityLog = ActivityLogManager.CreateActivityLog();
            activityLog.ForceLog = true;
            activityLog.UserName = userName;
            activityLog.EntityName = string.Format("ورود ناموفق-{0}", (object)reason);
            activityLog.VisibleForEndUser = true;
            activityLog.EntityID = " ";
            activityLog.Action = 8;
            ActivityLog.ActivityLog activityLog1 = activityLog;
            ActivityLogData activityLogData = new ActivityLogData();
            activityLogData.Data = SerializationHelper.SerializeCustom((object)new
            {
                UserName = userName,
                UserToken = activityLog.EntityID,
                RecordDateTime = DateTime.Now,
                Action = ActionType.LoginFailed.ToString()
            });
            activityLogData.ObjectState = ObjectState.Added;
            activityLog1.LogData = activityLogData;
            await ActivityLogManager.SaveAsync(activityLog);
        }

        public static void SaveActivityLogForLoginFailedAction(string userName, string reason)
        {
            ActivityLog.ActivityLog activityLog1 = ActivityLogManager.CreateActivityLog();
            activityLog1.ForceLog = true;
            activityLog1.UserName = userName;
            activityLog1.EntityName = string.Format("ورود ناموفق-{0}", (object)reason);
            activityLog1.VisibleForEndUser = true;
            activityLog1.EntityID = " ";
            activityLog1.Action = 8;
            ActivityLog.ActivityLog activityLog2 = activityLog1;
            ActivityLogData activityLogData = new ActivityLogData();
            activityLogData.Data = SerializationHelper.SerializeCustom((object)new
            {
                UserName = userName,
                UserToken = activityLog1.EntityID,
                RecordDateTime = DateTime.Now,
                Action = ActionType.LoginFailed.ToString()
            });
            activityLogData.ObjectState = ObjectState.Added;
            activityLog2.LogData = activityLogData;
            ActivityLogManager.Save(activityLog1);
        }

        public static async Task SaveActivityLogForChangePassActionAsync(
          int userId,
          string userName)
        {
            ActivityLog.ActivityLog activityLog = ActivityLogManager.CreateActivityLog();
            activityLog.ForceLog = true;
            activityLog.EntityName = "تغییر رمز عبور";
            activityLog.UserName = userName;
            activityLog.VisibleForEndUser = true;
            activityLog.EntityID = userId.ToString();
            activityLog.Action = 9;
            ActivityLog.ActivityLog activityLog1 = activityLog;
            ActivityLogData activityLogData = new ActivityLogData();
            activityLogData.Data = SerializationHelper.SerializeCustom((object)new
            {
                UserName = userName,
                UserToken = activityLog.EntityID,
                RecordDateTime = DateTime.Now,
                Action = ActionType.ChangePassword.ToString()
            });
            activityLogData.ObjectState = ObjectState.Added;
            activityLog1.LogData = activityLogData;
            await ActivityLogManager.SaveAsync(activityLog);
        }

        public static void SaveActivityLogForChangePassAction(int userId, string userName)
        {
            ActivityLog.ActivityLog activityLog1 = ActivityLogManager.CreateActivityLog();
            activityLog1.ForceLog = true;
            activityLog1.UserName = userName;
            activityLog1.EntityName = "تغییر رمز عبور";
            activityLog1.VisibleForEndUser = true;
            activityLog1.EntityID = userId.ToString();
            activityLog1.Action = 9;
            ActivityLog.ActivityLog activityLog2 = activityLog1;
            ActivityLogData activityLogData = new ActivityLogData();
            activityLogData.Data = SerializationHelper.SerializeCustom((object)new
            {
                UserName = userName,
                UserToken = activityLog1.EntityID,
                RecordDateTime = DateTime.Now,
                Action = ActionType.ChangePassword.ToString()
            });
            activityLogData.ObjectState = ObjectState.Added;
            activityLog2.LogData = activityLogData;
            ActivityLogManager.Save(activityLog1);
        }

        public static async Task SaveActivityLogForLogOffActionAsync(UserContext userContext)
        {
            ActivityLog.ActivityLog activityLog = ActivityLogManager.CreateActivityLog();
            activityLog.ForceLog = true;
            activityLog.EntityName = "خروج";
            activityLog.EntityID = userContext.Token;
            activityLog.Action = 7;
            activityLog.UserID = userContext.UserId;
            activityLog.UserName = userContext.UserName;
            activityLog.ClientIP = userContext.ClientIP;
            activityLog.VisibleForEndUser = true;
            ActivityLog.ActivityLog activityLog1 = activityLog;
            ActivityLogData activityLogData = new ActivityLogData();
            activityLogData.Data = SerializationHelper.SerializeCustom((object)new
            {
                UserName = userContext.UserName,
                UserToken = userContext.Token,
                RecordDateTime = DateTime.Now,
                Action = ActionType.Logout.ToString()
            });
            activityLogData.ObjectState = ObjectState.Added;
            activityLog1.LogData = activityLogData;
            await ActivityLogManager.SaveAsync(activityLog);
        }

        public static void SaveActivityLogForLogOffAction(UserContext userContext)
        {
            ActivityLog.ActivityLog activityLog1 = ActivityLogManager.CreateActivityLog();
            activityLog1.ForceLog = true;
            activityLog1.EntityName = "خروج";
            activityLog1.EntityID = userContext.Token;
            activityLog1.Action = 7;
            activityLog1.UserID = userContext.UserId;
            activityLog1.UserName = userContext.UserName;
            activityLog1.ClientIP = userContext.ClientIP;
            activityLog1.VisibleForEndUser = true;
            ActivityLog.ActivityLog activityLog2 = activityLog1;
            ActivityLogData activityLogData = new ActivityLogData();
            activityLogData.Data = SerializationHelper.SerializeCustom((object)new
            {
                UserName = userContext.UserName,
                UserToken = userContext.Token,
                RecordDateTime = DateTime.Now,
                Action = ActionType.Logout.ToString()
            });
            activityLogData.ObjectState = ObjectState.Added;
            activityLog2.LogData = activityLogData;
            ActivityLogManager.Save(activityLog1);
        }

        public static async Task<bool> ExistsUserAsync(string userName)
        {
            bool result = await SecurityManager.SecurityProvider.ExistsUserAsync(userName, new int?());
            return result;
        }

        public static bool ExistsUser(string userName)
        {
            return SecurityManager.SecurityProvider.ExistsUser(userName, new int?());
        }

        public static RoleDataCollection GetUserRoles(int userId)
        {
            return SecurityManager.SecurityProvider.GetUserRoles(userId);
        }
    }
}
