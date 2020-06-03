using OMF.Common.Validation;
using FluentValidation;
using System;
using System.Linq.Expressions;

namespace OMF.Workflow.Model.Validators
{
    public class WorkflowStepValidator : EntityValidator<WorkflowStep>
    {
        public WorkflowStepValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            //this.RuleFor<int>((Expression<Func<WorkflowStep, int>>)(workflowStep => workflowStep.StepNo)).NotNull<WorkflowStep, int>().WithMessage<WorkflowStep, int>("{0} اجباری می باشد", (object)"شماره مرحله").NotEmpty<WorkflowStep, int>().WithMessage<WorkflowStep, int>("{0} اجباری می باشد", (object)"شماره مرحله");
            //this.RuleFor<int>((Expression<Func<WorkflowStep, int>>)(workflowStep => workflowStep.StepLevel)).NotNull<WorkflowStep, int>().WithMessage<WorkflowStep, int>("{0} اجباری می باشد", (object)"سطح مرحله").NotEmpty<WorkflowStep, int>().WithMessage<WorkflowStep, int>("{0} اجباری می باشد", (object)"سطح مرحله");
        }
    }
}
