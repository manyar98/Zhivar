using OMF.Common;
using OMF.Common.Validation;
using OMF.EntityFramework.DataContext;
using OMF.EntityFramework.Ef6;
//using OMF.EntityFramework.Repositories;
//using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace OMF.Security.Model.Validators
{
    public class OperationValidator //: EntityValidator<Operation>
    {
        public OperationValidator()
        {
           // this.CascadeMode = CascadeMode.StopOnFirstFailure;
            //this.RuleFor<string>((Expression<Func<Operation, string>>)(opr => opr.Name)).NotNull<Operation, string>().WithMessage("{0} اجباری می باشد نام").NotEmpty<Operation, string>().WithMessage<Operation, string>(string.Format("{0} اجباری می باشد", (object)"نام"));
            //this.RuleFor<string>((Expression<Func<Operation, string>>)(opr => opr.Code)).NotNull<Operation, string>().WithMessage("{0} اجباری می باشد کد").NotEmpty<Operation, string>().WithMessage<Operation, string>("{0} اجباری می باشد کد").Must<Operation, string>((Func<Operation, string, Task<bool>>)((entity, oprCode) =>
            //{
            //    if (entity.ObjectState == ObjectState.Deleted)
            //        return true;
            //    using (UnitOfWork uow = new UnitOfWork((IDataContextAsync)new SecurityDbContext()))
            //    {
            //        IRepositoryAsync<Operation> operationRep = uow.RepositoryAsync<Operation>();
            //        IQueryable<Operation> query = operationRep.Queryable(false, true, (List<Expression<Func<Operation, object>>>)null).Where<Operation>((Expression<Func<Operation, bool>>)(opr => opr.Code == oprCode && opr.ApplicationId == entity.ApplicationId));
            //        if (entity.ObjectState == ObjectState.Modified)
            //            query = query.Where<Operation>((Expression<Func<Operation, bool>>)(opr => opr.ID != entity.ID));
            //        int count = await query.CountAsync<Operation>();
            //        return count == 0;
            //    }
            //})).WithMessage<Operation, string>("کد با مقدار '{0}' قبلتر استفاده شده است");
        }
    }
}
