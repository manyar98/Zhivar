using OMF.Common.ActivityLog;
using OMF.EntityFramework.Ef6;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace OMF.EntityFramework
{
    public class ActivityLogDbContext : Ef6.DataContext
    {
        public ActivityLogDbContext()
          : base(ActivityLogDbConnectionManager.CreateConnection(), true)
        {
            Database.SetInitializer<ActivityLogDbContext>((IDatabaseInitializer<ActivityLogDbContext>)new NullDatabaseInitializer<ActivityLogDbContext>());
            this.Database.Initialize(false);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add<OMF.Common.ActivityLog.ActivityLog>((EntityTypeConfiguration<OMF.Common.ActivityLog.ActivityLog>)new ActivityLogConfig());
            modelBuilder.Configurations.Add<ActivityLogData>((EntityTypeConfiguration<ActivityLogData>)new ActivityLogDataConfig());
        }
    }
}
