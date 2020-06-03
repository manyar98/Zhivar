using OMF.Common.Configuration;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Workflow.Model.Validators;
using System;
using System.Linq.Expressions;

namespace OMF.Workflow.Model.MapConfiguration
{
    public class WorkflowStepActionConfig : BaseEntityTypeConfig<WorkflowStepAction>
    {
        public WorkflowStepActionConfig()
        {
            this.ToTable("WF_STEP_ACTIONS");//, ConfigurationController.SecurityDbSchema);
            this.Property<bool>((Expression<Func<WorkflowStepAction, bool?>>)(stepAction => stepAction.TerminationStatus)).HasColumnName("TERMINATION_STATUS");
            this.Property<int>((Expression<Func<WorkflowStepAction, int>>)(stepAction => stepAction.NextWorkflowStepId)).HasColumnName("NEXT_WFST_ID");
            this.Property<int>((Expression<Func<WorkflowStepAction, int>>)(stepAction => stepAction.SubWorkflowInfoId)).HasColumnName("SUB_WRKF_ID");
            this.Property<int>((Expression<Func<WorkflowStepAction, int>>)(stepAction => stepAction.WFActionTypeId)).HasColumnName("WFAT_ID");
            this.Property<int>((Expression<Func<WorkflowStepAction, int>>)(stepAction => stepAction.WorkflowStepId)).HasColumnName("WFST_ID");
            this.Property((Expression<Func<WorkflowStepAction, string>>)(stepAction => stepAction.DesignMetaData)).HasColumnName("DESIGN_METADATA").HasMaxLength(new int?(500));
            this.MapEntityValidator((EntityValidator<WorkflowStepAction>)new WorkflowStepActionValidator());
        }
    }
}
