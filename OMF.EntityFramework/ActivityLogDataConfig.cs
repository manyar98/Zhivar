using OMF.Common.ActivityLog;
using OMF.Common.Configuration;
using OMF.EntityFramework.Ef6;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace OMF.EntityFramework
{
    public class ActivityLogDataConfig : BaseEntityTypeConfig<ActivityLogData>
    {
        public ActivityLogDataConfig()
        {
            this.ToTable("TBL_ACTIVITYLOG");
            if (ConfigurationController.CustomIdentityEnabled)
                this.Property<int>((Expression<Func<ActivityLogData, int>>)(aLogData => aLogData.ID)).HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
            this.Property((Expression<Func<ActivityLogData, byte[]>>)(aLogData => aLogData.Data)).HasColumnName("LOG_DATA").IsRequired().HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
            //this.MapViewKey("Infrastructure-ActivityLog-View");
        }
    }
}
