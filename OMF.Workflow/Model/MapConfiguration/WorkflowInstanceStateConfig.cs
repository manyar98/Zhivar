using OMF.Common.Configuration;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Workflow.Model.Validators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static OMF.Workflow.Enums;

namespace OMF.Workflow.Model.MapConfiguration
{
    public class WorkflowInstanceStateConfig : BaseEntityTypeConfig<WorkflowInstanceState>
    {
        public WorkflowInstanceStateConfig()
        {
            this.ToTable("WF_INSTANCE_STATES");//, ConfigurationController.SecurityDbSchema);
            this.Property((Expression<Func<WorkflowInstanceState, string>>)(instanceState => instanceState.Title)).HasColumnName("TITLE").HasMaxLength(new int?(200));
            this.Property<WfStateStatus>((Expression<Func<WorkflowInstanceState, WfStateStatus>>)(instanceState => instanceState.StateStatus)).HasColumnName("STATE_STATUS");
            this.Property((Expression<Func<WorkflowInstanceState, DateTime>>)(instanceState => instanceState.InstantiationTime)).HasColumnName("INSTANTIATION_TIME");
            this.Property((Expression<Func<WorkflowInstanceState, DateTime?>>)(instanceState => instanceState.AccomplishTime)).HasColumnName("ACCOMPLISH_TIME");
            this.Property((Expression<Func<WorkflowInstanceState, DateTime?>>)(instanceState => instanceState.ViewDateTime)).HasColumnName("VIEW_TIME");
            this.Property<int>((Expression<Func<WorkflowInstanceState, int?>>)(instanceState => instanceState.AccomplishActionId)).HasColumnName("ACCOMPLISH_ACTION_ID");
            this.Property((Expression<Func<WorkflowInstanceState, string>>)(instanceState => instanceState.UserComment)).HasColumnName("USER_COMMENT").HasMaxLength(new int?(2000));
            this.Property((Expression<Func<WorkflowInstanceState, string>>)(instanceState => instanceState.ExchangeData)).HasColumnName("EXCHANGE_DATA").HasMaxLength(new int?(2000));
            this.Property<int>((Expression<Func<WorkflowInstanceState, int>>)(instanceState => instanceState.UserId)).HasColumnName("USER_ID");
            this.Property<int>((Expression<Func<WorkflowInstanceState, int>>)(instanceState => instanceState.WorkflowInstanceId)).HasColumnName("WF_INSTANCE_ID");
            this.Property<int>((Expression<Func<WorkflowInstanceState, int>>)(instanceState => instanceState.SubWorkflowInstanceId)).HasColumnName("SUB_PRSS_ID");
            this.Property<int>((Expression<Func<WorkflowInstanceState, int>>)(instanceState => instanceState.WorkflowStepId)).HasColumnName("WF_STEP_ID");
            this.Property<int>((Expression<Func<WorkflowInstanceState, int?>>)(instanceState => instanceState.SenderWorkflowInstanceStateId)).HasColumnName("SENDER_STATE_ID");
            this.HasOptional<WorkflowInstanceState>((Expression<Func<WorkflowInstanceState, WorkflowInstanceState>>)(instanceState => instanceState.SenderWorkflowInstanceState)).WithMany().HasForeignKey<int?>((Expression<Func<WorkflowInstanceState, int?>>)(instanceState => instanceState.SenderWorkflowInstanceStateId));
            this.HasRequired<WorkflowInstance>((Expression<Func<WorkflowInstanceState, WorkflowInstance>>)(instanceState => instanceState.WorkflowInstance)).WithMany((Expression<Func<WorkflowInstance, ICollection<WorkflowInstanceState>>>)(wfInstance => wfInstance.WorkflowInstanceStates)).HasForeignKey<int>((Expression<Func<WorkflowInstanceState, int>>)(instanceState => instanceState.WorkflowInstanceId));
            this.MapEntityValidator((EntityValidator<WorkflowInstanceState>)new WorkflowInstanceStateValidator());
        }
    }
}
