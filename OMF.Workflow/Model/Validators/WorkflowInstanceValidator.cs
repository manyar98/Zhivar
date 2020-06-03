using OMF.Common;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using FluentValidation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OMF.Workflow.Model.Validators
{
    public class WorkflowInstanceValidator : EntityValidator<WorkflowInstance>
    {
        public WorkflowInstanceValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor<DateTime>((Expression<Func<WorkflowInstance, DateTime>>)(workflowInstance => workflowInstance.StartTime)).NotNull<WorkflowInstance, DateTime>().WithMessage("{0} اجباری می باشد تاریخ شروع").NotEqual<WorkflowInstance, DateTime>(DateTime.Now, (IEqualityComparer)null).WithMessage("{0} معتبر نمی باشد تاریخ شروع");
            this.RuleFor<int>((Expression<Func<WorkflowInstance, int>>)(workflowInstance => workflowInstance.WorkflowInfoId)).NotNull<WorkflowInstance, int>().WithMessage("{0} اجباری می باشدفرآیند").GreaterThan<WorkflowInstance, int>(0).WithMessage("{0} معتبر نمی باشد فرآیند");
        //    this.RuleFor<int>((Expression<Func<WorkflowInstance, int>>)(workflowInstance => workflowInstance.RelatedRecordId)).NotNull<WorkflowInstance, int>().WithMessage<WorkflowInstance, int>("{0} اجباری می باشد", (object)"شناسه رکورد مرتبط").NotEmpty<WorkflowInstance, int>().WithMessage<WorkflowInstance, int>("{0} اجباری می باشد", (object)"شناسه رکورد مرتبط").MustAsync<WorkflowInstance, int>((Func<WorkflowInstance, int, Task<bool>>)((entity, RelatedRecordId) =>
        //    {
        //        bool flag;
        //        using (UnitOfWork uow = new UnitOfWork())
        //        {
        //            IQueryable<WorkflowInstance> actionTypesQuery = uow.RepositoryAsync<WorkflowInstance>().Queryable(false, true, (List<Expression<Func<WorkflowInstance, object>>>)null).Where<WorkflowInstance>((Expression<Func<WorkflowInstance, bool>>)(workflowInstance => workflowInstance.RelatedRecordId == RelatedRecordId));
        //            if (entity.ObjectState == ObjectState.Modified)
        //                actionTypesQuery = actionTypesQuery.Where<WorkflowInstance>((Expression<Func<WorkflowInstance, bool>>)(actionTypes => actionTypes.ID != entity.ID));
        //            int count = await actionTypesQuery.CountAsync<WorkflowInstance>();
        //            flag = count == 0;
        //        }
        //        return flag;
        //    })).WithMessage<WorkflowInstance, int>("{0} با مقدار '{1}' تکراری می باشد", new Func<WorkflowInstance, object>[2]
        //    {
        //(Func<WorkflowInstance, object>) (n => (object) "شناسه رکورد مرتبط"),
        //(Func<WorkflowInstance, object>) (workflowInstance => (object) workflowInstance.RelatedRecordId)
        //    });
        }
    }
}
