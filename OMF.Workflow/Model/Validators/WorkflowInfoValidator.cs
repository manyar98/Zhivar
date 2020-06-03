using OMF.Common;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace OMF.Workflow.Model.Validators
{
    public class WorkflowInfoValidator : EntityValidator<WorkflowInfo>
    {
        public WorkflowInfoValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor<string>((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Version)).NotNull<WorkflowInfo, string>().WithMessage("{0} اجباری می باشد ورژن").NotEmpty<WorkflowInfo, string>().WithMessage("{0} اجباری می باشد ورژن");
        //    this.RuleFor<string>((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Title)).NotNull<WorkflowInfo, string>().WithMessage<WorkflowInfo, string>("{0} اجباری می باشد", (object)"عنوان نوع فعالیت").NotEmpty<WorkflowInfo, string>().WithMessage<WorkflowInfo, string>("{0} اجباری می باشد", (object)"عنوان نوع فعالیت").MustAsync<WorkflowInfo, string>((Func<WorkflowInfo, string, Task<bool>>)((entity, title) =>
        //    {
        //        bool flag;
        //        using (UnitOfWork uow = new UnitOfWork())
        //        {
        //            IQueryable<WorkflowInfo> workflowInfoQuery = uow.RepositoryAsync<WorkflowInfo>().Queryable(false, true, (List<Expression<Func<WorkflowInfo, object>>>)null).Where<WorkflowInfo>((Expression<Func<WorkflowInfo, bool>>)(workflowInfo => workflowInfo.Title == title));
        //            bool status = false;
        //            if (entity.ObjectState == ObjectState.Modified)
        //                status = await workflowInfoQuery.AnyAsync<WorkflowInfo>((Expression<Func<WorkflowInfo, bool>>)(workflowInfo => workflowInfo.ID != entity.ID));
        //            flag = status;
        //        }
        //        return flag;
        //    })).WithMessage<WorkflowInfo, string>("{0} با مقدار '{1}' تکراری می باشد", new Func<WorkflowInfo, object>[2]
        //    {
        //(Func<WorkflowInfo, object>) (n => (object) "عنوان نوع فعالیت"),
        //(Func<WorkflowInfo, object>) (workflowInfo => (object) workflowInfo.Title)
        //    });
        }
    }
}
