using OMF.Common.ActivityLog;
using OMF.Common.Configuration;
using OMF.EntityFramework.Ef6;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace OMF.EntityFramework
{
    public class ActivityLogConfig : BaseEntityTypeConfig<OMF.Common.ActivityLog.ActivityLog>
    {
        public ActivityLogConfig()
        {
            this.ToTable("TBL_ACTIVITYLOG");
            //if (ConfigurationController.CustomIdentityEnabled)
            //    this.Property<int>((Expression<Func<OMF.Common.ActivityLog.ActivityLog, int>>)(aLog => aLog.ID)).HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
            this.Ignore<bool>((Expression<Func<OMF.Common.ActivityLog.ActivityLog, bool>>)(aLog => aLog.ForceLog));
            this.Property<int>((Expression<Func<OMF.Common.ActivityLog.ActivityLog, int>>)(aLog => aLog.ApplicationID)).HasColumnName("APPLICATIONID");
            this.Property<int>((Expression<Func<OMF.Common.ActivityLog.ActivityLog, int>>)(aLog => aLog.Action)).HasColumnName("ACTION").IsRequired();
            this.Property((Expression<Func<OMF.Common.ActivityLog.ActivityLog, string>>)(aLog => aLog.EntityID)).HasColumnName("ENTITYID").HasMaxLength(new int?(50)).IsRequired();
            this.Property((Expression<Func<OMF.Common.ActivityLog.ActivityLog, string>>)(aLog => aLog.EntityName)).HasColumnName("ENTITYNAME").HasMaxLength(new int?(200)).IsRequired();
            this.Property((Expression<Func<OMF.Common.ActivityLog.ActivityLog, DateTime>>)(aLog => aLog.RecordDateTime)).HasColumnName("RECORDDATETIME").IsRequired();
            this.Property<int>((Expression<Func<OMF.Common.ActivityLog.ActivityLog, int>>)(aLog => aLog.UserID)).HasColumnName("USERID").IsRequired();
            this.Property((Expression<Func<OMF.Common.ActivityLog.ActivityLog, string>>)(aLog => aLog.UserName)).HasColumnName("USERNAME").HasMaxLength(new int?(1000)).IsRequired();
            this.Property((Expression<Func<OMF.Common.ActivityLog.ActivityLog, string>>)(aLog => aLog.ClientIP)).HasColumnName("CLIENTIP").HasMaxLength(new int?(50));
            this.Property<bool>((Expression<Func<OMF.Common.ActivityLog.ActivityLog, bool>>)(aLog => aLog.VisibleForEndUser)).HasColumnName("VISIBLEFORENDUSER");
            this.HasRequired<ActivityLogData>((Expression<Func<OMF.Common.ActivityLog.ActivityLog, ActivityLogData>>)(exLog => exLog.LogData)).WithRequiredPrincipal();
            //this.MapViewKey("Infrastructure-ActivityLog-View");
        }
    }
}
