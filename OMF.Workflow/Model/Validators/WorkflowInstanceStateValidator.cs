using OMF.Common.Validation;
using FluentValidation;
using System;
using System.Collections;
using System.Linq.Expressions;
using static OMF.Workflow.Enums;

namespace OMF.Workflow.Model.Validators
{
    public class WorkflowInstanceStateValidator : EntityValidator<WorkflowInstanceState>
    {
        public WorkflowInstanceStateValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            //this.RuleFor<WfStateStatus>((Expression<Func<WorkflowInstanceState, WfStateStatus>>)(workflowInstanceState => workflowInstanceState.StateStatus)).NotNull<WorkflowInstanceState, WfStateStatus>().WithMessage<WorkflowInstanceState, WfStateStatus>("{0} اجباری می باشد", (object)"وضعیت").NotEmpty<WorkflowInstanceState, WfStateStatus>().WithMessage<WorkflowInstanceState, WfStateStatus>("{0} اجباری می باشد", (object)"وضعیت");
            //this.RuleFor<DateTime>((Expression<Func<WorkflowInstanceState, DateTime>>)(workflowInstanceState => workflowInstanceState.InstantiationTime)).NotNull<WorkflowInstanceState, DateTime>().WithMessage<WorkflowInstanceState, DateTime>("{0} اجباری می باشد", (object)"زمان نمونه").NotEqual<WorkflowInstanceState, DateTime>(DateTime.MinValue, (IEqualityComparer)null).WithMessage<WorkflowInstanceState, DateTime>("{0} معتبر نمی باشد", (object)"زمان نمونه");
            //this.RuleFor<int>((Expression<Func<WorkflowInstanceState, int>>)(workflowInstanceState => workflowInstanceState.UserId)).NotNull<WorkflowInstanceState, int>().WithMessage<WorkflowInstanceState, int>("{0} اجباری می باشد", (object)"پرسنل").GreaterThan<WorkflowInstanceState, int>(0).WithMessage<WorkflowInstanceState, int>("{0} معتبر نمی باشد", (object)"پرسنل");
            //this.RuleFor<int>((Expression<Func<WorkflowInstanceState, int>>)(workflowInstanceState => workflowInstanceState.SubWorkflowInstanceId)).NotNull<WorkflowInstanceState, int>().WithMessage<WorkflowInstanceState, int>("{0} اجباری می باشد", (object)"فرآیند ایجاد شده زیر").GreaterThan<WorkflowInstanceState, int>(0).WithMessage<WorkflowInstanceState, int>("{0} معتبر نمی باشد", (object)"فرآیند ایجاد شده زیر");
            //this.RuleFor<int>((Expression<Func<WorkflowInstanceState, int>>)(workflowInstanceState => workflowInstanceState.WorkflowInstanceId)).NotNull<WorkflowInstanceState, int>().WithMessage<WorkflowInstanceState, int>("{0} اجباری می باشد", (object)"فرآیند ایجاد شده").GreaterThan<WorkflowInstanceState, int>(0).WithMessage<WorkflowInstanceState, int>("{0} معتبر نمی باشد", (object)"فرآیند ایجاد شده");
            //this.RuleFor<int>((Expression<Func<WorkflowInstanceState, int>>)(workflowInstanceState => workflowInstanceState.WorkflowStepId)).NotNull<WorkflowInstanceState, int>().WithMessage<WorkflowInstanceState, int>("{0} اجباری می باشد", (object)"مراحل فرآیند").GreaterThan<WorkflowInstanceState, int>(0).WithMessage<WorkflowInstanceState, int>("{0} معتبر نمی باشد", (object)"مراحل فرآیند");
        }
    }
}
