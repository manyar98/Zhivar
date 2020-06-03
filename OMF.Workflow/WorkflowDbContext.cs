using OMF.Common;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model;
using OMF.Security.Model.MappingConfiguration;
using OMF.Workflow.Model;
using OMF.Workflow.Model.MapConfiguration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace OMF.Workflow
{
    internal class WorkflowDbContext : DataContext
    {
        public WorkflowDbContext()
          : base(WorkflowDbConnectionManager.CreateConnection(), true)
        {
            Database.SetInitializer<WorkflowDbContext>((IDatabaseInitializer<WorkflowDbContext>)new NullDatabaseInitializer<WorkflowDbContext>());
            this.Database.Initialize(false);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add<WorkflowInfo>((EntityTypeConfiguration<WorkflowInfo>)new WorkflowInfoConfig());
            modelBuilder.Configurations.Add<WFActionType>((EntityTypeConfiguration<WFActionType>)new WFActionTypeConfig());
            modelBuilder.Configurations.Add<WorkflowInstance>((EntityTypeConfiguration<WorkflowInstance>)new WorkflowInstanceConfig());
            modelBuilder.Configurations.Add<WorkflowInstanceState>((EntityTypeConfiguration<WorkflowInstanceState>)new WorkflowInstanceStateConfig());
            modelBuilder.Configurations.Add<WorkflowStepAction>((EntityTypeConfiguration<WorkflowStepAction>)new WorkflowStepActionConfig());
            modelBuilder.Configurations.Add<WorkflowStep>((EntityTypeConfiguration<WorkflowStep>)new WorkflowStepConfig());
            modelBuilder.Configurations.Add<UserInfo>((EntityTypeConfiguration<UserInfo>)new UserInfoConfig());
            modelBuilder.Configurations.Add<RoleBase>((EntityTypeConfiguration<RoleBase>)new RoleBaseConfig());
            modelBuilder.Configurations.Add<Role>((EntityTypeConfiguration<Role>)new RoleConfig());
            modelBuilder.Configurations.Add<Position>((EntityTypeConfiguration<Position>)new PositionConfig());
            modelBuilder.Configurations.Add<UserRole>((EntityTypeConfiguration<UserRole>)new UserRoleConfig());
            modelBuilder.Configurations.Add<EntityIDLogData>((ComplexTypeConfiguration<EntityIDLogData>)new LoggableEntityIDTypeConfig());
            modelBuilder.Configurations.Add<EntityNameLogData>((ComplexTypeConfiguration<EntityNameLogData>)new LoggableEntityNameTypeConfig());
        }
    }
}
