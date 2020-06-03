using OMF.Common.Validation;
using OMF.EntityFramework.DataContext;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OMF.Security.Model.Validators
{
    public class RoleBaseValidator : EntityValidator<RoleBase>
    {
        public RoleBaseValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor<string>((Expression<Func<RoleBase, string>>)(role => role.Name)).NotNull<RoleBase, string>().WithMessage("{0} اجباری می باشد نام").NotEmpty<RoleBase, string>().WithMessage<RoleBase, string>("{0} اجباری می باشد نام");
            //this.RuleFor<string>((Expression<Func<RoleBase, string>>)(role => role.Code)).NotNull<RoleBase, string>().WithMessage<RoleBase, string>("{0} اجباری می باشد کد").NotEmpty<RoleBase, string>().WithMessage<RoleBase, string>("{0} اجباری می باشد ").Must<RoleBase, string>((Func<RoleBase, string, Task<bool>>)((entity, roleCode) =>
            //{
            //    bool flag;
            //    using (UnitOfWork uow = new UnitOfWork((IDataContextAsync)new SecurityDbContext()))
            //    {
            //        IRepositoryAsync<RoleBase> roleRep = uow.RepositoryAsync<RoleBase>();
            //        IQueryable<RoleBase> query = roleRep.Queryable(false, true, (List<Expression<Func<RoleBase, object>>>)null).Where<RoleBase>((Expression<Func<RoleBase, bool>>)(role => role.Code == roleCode && role.ApplicationId == entity.ApplicationId));
            //        if (entity.ID > 0)
            //            query = query.Where<RoleBase>((Expression<Func<RoleBase, bool>>)(role => role.ID != entity.ID));
            //        int count =  query.Count<RoleBase>();
            //        flag = count == 0;
            //    }
            //    return  flag;
            //})).WithMessage<RoleBase, string>("{0} با مقدار '{1}' تکراری می باشد", new Func<RoleBase, object>[2]
            //{
            //(Func<RoleBase, object>) (user => (object) "کد"),
            //(Func<RoleBase, object>) (role => (object) role.Code)
            //});
        }
    }
}
