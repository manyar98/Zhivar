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
    public class WorkflowStepConfig : BaseEntityTypeConfig<WorkflowStep>
    {
        public WorkflowStepConfig()
        {
            this.ToTable("WF_STEPS");//, ConfigurationController.SecurityDbSchema);
            this.Property<int>((Expression<Func<WorkflowStep, int>>)(step => step.WorkflowInfoId)).HasColumnName("WRKF_ID");
            this.Property((Expression<Func<WorkflowStep, string>>)(step => step.Title)).HasColumnName("TITLE");
            this.Property((Expression<Func<WorkflowStep, string>>)(step => step.Code)).HasColumnName("CODE");
            this.Property((Expression<Func<WorkflowStep, string>>)(step => step.Icon)).HasColumnName("ICON");
            this.Property<bool>((Expression<Func<WorkflowStep, bool>>)(step => step.MultiCheckAction)).HasColumnName("MULTI_CHECK_ACTION");
            this.Property<int>((Expression<Func<WorkflowStep, int?>>)(step => step.RoleId)).HasColumnName("POSI_ID");
            this.Property<bool>((Expression<Func<WorkflowStep, bool>>)(step => step.NeedToSign)).HasColumnName("NEED_TO_SIGN");
            this.Property<int>((Expression<Func<WorkflowStep, int>>)(step => step.StepNo)).HasColumnName("STEP_NO");
            this.Property<int>((Expression<Func<WorkflowStep, int>>)(step => step.StepLevel)).HasColumnName("STEP_LEVEL");
            this.Property<StepType>((Expression<Func<WorkflowStep, StepType>>)(step => step.StepType)).HasColumnName("STEP_TYPE");
            this.Property<Priority>((Expression<Func<WorkflowStep, Priority>>)(step => step.MessagePriority)).HasColumnName("MESSAGE_PRIORITY");
            this.Property<MessageType>((Expression<Func<WorkflowStep, MessageType>>)(step => step.MessageType)).HasColumnName("MESSAGE_TYPE");
            this.Property((Expression<Func<WorkflowStep, string>>)(step => step.DecisionLogic)).HasColumnName("DECISION_LOGIC").HasMaxLength(new int?(200));
            this.Property((Expression<Func<WorkflowStep, string>>)(step => step.ActionUriRoute)).HasColumnName("ACTION_URI_ROUTE").HasMaxLength(new int?(200));
            this.Property((Expression<Func<WorkflowStep, string>>)(step => step.MasterUriRoute)).HasColumnName("MASTER_URI_ROUTE").HasMaxLength(new int?(200));
            this.Property<int>((Expression<Func<WorkflowStep, int?>>)(step => step.OrganizationId)).HasColumnName("ORGN_ID");
            this.Property((Expression<Func<WorkflowStep, string>>)(step => step.PreActionMethod)).HasColumnName("PRE_ACTION_METHOD").HasMaxLength(new int?(200));
            this.Property((Expression<Func<WorkflowStep, string>>)(step => step.PostActionMethod)).HasColumnName("POST_ACTION_METHOD").HasMaxLength(new int?(200));
            this.Property((Expression<Func<WorkflowStep, string>>)(step => step.DesignMetaData)).HasColumnName("DESIGN_METADATA").HasMaxLength(new int?(500));
            this.HasMany<WorkflowInstanceState>((Expression<Func<WorkflowStep, ICollection<WorkflowInstanceState>>>)(step => step.WorkflowInstanceStates)).WithRequired().HasForeignKey<int>((Expression<Func<WorkflowInstanceState, int>>)(WFIS => WFIS.WorkflowStepId));
            this.HasMany<WorkflowStepAction>((Expression<Func<WorkflowStep, ICollection<WorkflowStepAction>>>)(step => step.WorkflowStepActions)).WithRequired().HasForeignKey<int>((Expression<Func<WorkflowStepAction, int>>)(WFIS => WFIS.WorkflowStepId));
            this.HasMany<WorkflowStepAction>((Expression<Func<WorkflowStep, ICollection<WorkflowStepAction>>>)(step => step.WorkflowStepActions)).WithRequired().HasForeignKey<int>((Expression<Func<WorkflowStepAction, int>>)(WFIS => WFIS.NextWorkflowStepId));
            this.MapEntityValidator((EntityValidator<WorkflowStep>)new WorkflowStepValidator());
        }
    }
}
