using OMF.Common.Validation;
using FluentValidation;
using System;
using System.Linq.Expressions;

namespace OMF.Workflow.Model.Validators
{
    public class WorkflowStepActionValidator : EntityValidator<WorkflowStepAction>
    {
        public WorkflowStepActionValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            //this.RuleFor<int>((Expression<Func<WorkflowStepAction, int>>)(workflowStepAction => workflowStepAction.WorkflowStepId)).NotNull<WorkflowStepAction, int>().WithMessage<WorkflowStepAction, int>("{0} اجباری می باشد", (object)"مراحل فرآیند ").GreaterThan<WorkflowStepAction, int>(0).WithMessage<WorkflowStepAction, int>("{0} معتبر نمی باشد", (object)"مراحل فرآیند ");
            //this.RuleFor<int>((Expression<Func<WorkflowStepAction, int>>)(workflowStepAction => workflowStepAction.WFActionTypeId)).NotNull<WorkflowStepAction, int>().WithMessage<WorkflowStepAction, int>("{0} اجباری می باشد", (object)"نوع فرآیند ").GreaterThan<WorkflowStepAction, int>(0).WithMessage<WorkflowStepAction, int>("{0} معتبر نمی باشد", (object)"نوع فرآیند ");
        }
    }
}
