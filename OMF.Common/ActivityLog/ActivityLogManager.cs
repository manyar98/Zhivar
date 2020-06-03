using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement;
using OMF.Common.Serialization;
using static OMF.Common.Enums;
using OMF.Common.Security;

namespace OMF.Common.ActivityLog
{
    public class ActivityLogManager
    {
        
        private static IActivityLogger actLogger = ConfigurationController.ActivityLogger;
        public static Func<string> ClientIpCatcher = (Func<string>)(() => "undefined");

        public static void InitiateActivityLogger(IActivityLogger activityLogger)
        {
            ActivityLogManager.actLogger = activityLogger;
        }

        public static void Save(ActivityLog activityLogData)
        {
            try
            {
                if (!activityLogData.ForceLog && !ConfigurationController.EnableActivityLog)
                    return;
                ActivityLogManager.actLogger.Save(activityLogData);
            }
            catch (Exception ex)
            {
                ExceptionManager.SaveException(ex);
            }
        }

        public static void Save(IEnumerable<ActivityLog> activityLogDatas)
        {
            try
            {
                if (!activityLogDatas.Any<ActivityLog>())
                    return;
                if (ConfigurationController.EnableActivityLog)
                {
                    ActivityLogManager.actLogger.Save(activityLogDatas);
                }
                else
                {
                    if (!activityLogDatas.Any<OMF.Common.ActivityLog.ActivityLog>((Func<OMF.Common.ActivityLog.ActivityLog, bool>)(actLog => actLog.ForceLog)))
                        return;
                    ActivityLogManager.actLogger.Save(activityLogDatas.Where<OMF.Common.ActivityLog.ActivityLog>((Func<OMF.Common.ActivityLog.ActivityLog, bool>)(actLog => actLog.ForceLog)));
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.SaveException(ex);
            }
        }

        public static async Task SaveAsync(ActivityLog activityLogData)
        {
            try
            {
                if (!activityLogData.ForceLog)// && !ConfigurationController.EnableActivityLog)
                    return;
                if (ActivityLogManager.actLogger is IActivityLoggerAsync)
                    await ((IActivityLoggerAsync)ActivityLogManager.actLogger).SaveAsync(activityLogData);
                else
                    ActivityLogManager.actLogger.Save(activityLogData);
            }
            catch (Exception ex)
            {
                await ExceptionManager.SaveExceptionAsync(ex);
            }
        }

        public static async Task SaveAsync(IEnumerable<ActivityLog> activityLogDatas)
        {
            try
            {
                if (!activityLogDatas.Any<ActivityLog>())
                    return;
                if (ConfigurationController.EnableActivityLog)
                {
                    if (ActivityLogManager.actLogger is IActivityLoggerAsync)
                        await ((IActivityLoggerAsync)ActivityLogManager.actLogger).SaveAsync(activityLogDatas);
                    else
                        ActivityLogManager.actLogger.Save(activityLogDatas);
                }
                else if (activityLogDatas.Any<ActivityLog>((Func<ActivityLog, bool>)(actLog => actLog.ForceLog)))
                {
                    if (ActivityLogManager.actLogger is IActivityLoggerAsync)
                        await ((IActivityLoggerAsync)ActivityLogManager.actLogger).SaveAsync(activityLogDatas.Where<ActivityLog>((Func<ActivityLog, bool>)(actLog => actLog.ForceLog)));
                    else
                        ActivityLogManager.actLogger.Save(activityLogDatas);
                }
            }
            catch (Exception ex)
            {
                await ExceptionManager.SaveExceptionAsync(ex);
            }
        }

        public static ActivityLog CreateActivityLog(
          IActivityLoggable entity)
        {
            ActivityLog activityLog1 = ActivityLogManager.CreateActivityLog();
            activityLog1.EntityID = entity.ID.ToString();
            activityLog1.EntityName = EntityConfigManager.GetPersianTitle(entity.GetType());
            switch (entity.ObjectState)
            {
                case ObjectState.Unchanged:
                case ObjectState.Detached:
                    activityLog1.Action = 1;
                    break;
                case ObjectState.Added:
                    activityLog1.Action = 2;
                    break;
                case ObjectState.Modified:
                    activityLog1.Action = 3;
                    break;
                case ObjectState.Deleted:
                    activityLog1.Action = 4;
                    break;
            }
            ActivityLog activityLog2 = activityLog1;
            ActivityLogData activityLogData = new ActivityLogData();
            activityLogData.Data = SerializationHelper.SerializeCustom((object)entity);
            activityLogData.ObjectState = ObjectState.Added;
            activityLog2.LogData = activityLogData;
            return activityLog1;
        }

        public static ActivityLog CreateActivityLog()
        {
            ActivityLog activityLog = new ActivityLog();
            activityLog.ApplicationID =  ConfigurationController.ApplicationID.HasValue ? ConfigurationController.ApplicationID.Value : -1;
            activityLog.ObjectState = ObjectState.Added;
            activityLog.RecordDateTime = DateTime.Now;
            activityLog.UserID = SecurityManager.CurrentUserContext == null ? -1 : SecurityManager.CurrentUserContext.UserId;
            activityLog.UserName = SecurityManager.CurrentUserContext == null ? "Anonymous" : SecurityManager.CurrentUserContext.UserName;
            activityLog.Action = 1;
            activityLog.ClientIP = ActivityLogManager.ClientIP;
            activityLog.ForceLog = false;
            activityLog.VisibleForEndUser = false;
            return activityLog;
        }

        public static string ClientIP
        {
            get
            {
                try
                {
                    return ActivityLogManager.ClientIpCatcher();
                }
                catch
                {
                    return "undefined";
                }
            }
        }
    }
}
