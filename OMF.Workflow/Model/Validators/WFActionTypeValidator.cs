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

namespace OMF.Workflow.Model.Validators
{
    public class WFActionTypeValidator : EntityValidator<WFActionType>
    {
        public WFActionTypeValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
         //   this.RuleFor<string>((Expression<Func<WFActionType, string>>)(actionTypes => actionTypes.Title)).NotNull<WFActionType, string>().WithMessage<WFActionType, string>("{0} اجباری می باشد", (object)"عنوان نوع فعالیت").NotEmpty<WFActionType, string>().WithMessage<WFActionType, string>("{0} اجباری می باشد", (object)"عنوان نوع فعالیت").MustAsync<WFActionType, string>((Func<WFActionType, string, Task<bool>>)((entity, title) =>
        //    {
        //        bool flag;
        //        using (UnitOfWork uow = new UnitOfWork())
        //        {
        //            IQueryable<WFActionType> actionTypesQuery = uow.RepositoryAsync<WFActionType>().Queryable(false, true, (List<Expression<Func<WFActionType, object>>>)null).Where<WFActionType>((Expression<Func<WFActionType, bool>>)(actionTypes => actionTypes.Title == title));
        //            if (entity.ObjectState == ObjectState.Modified)
        //                actionTypesQuery = actionTypesQuery.Where<WFActionType>((Expression<Func<WFActionType, bool>>)(actionTypes => actionTypes.ID != entity.ID));
        //            int count = await actionTypesQuery.CountAsync<WFActionType>();
        //            flag = count == 0;
        //        }
        //        return flag;
        //    })).WithMessage<WFActionType, string>("{0} با مقدار '{1}' تکراری می باشد", new Func<WFActionType, object>[2]
        //    {
        //(Func<WFActionType, object>) (n => (object) "عنوان نوع فعالیت"),
        //(Func<WFActionType, object>) (actionTypes => (object) actionTypes.Title)
        //    });
        }
    }
}
