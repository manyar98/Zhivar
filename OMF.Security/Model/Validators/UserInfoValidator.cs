using OMF.Common;
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
using static OMF.Common.Enums;

namespace OMF.Security.Model.Validators
{
    public class UserInfoValidator : EntityValidator<UserInfo>
    {
        public UserInfoValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor<string>((Expression<Func<UserInfo, string>>)(user => user.FirstName)).NotNull<UserInfo, string>().WithMessage<UserInfo, string>("{0} اجباری می باشد نام").NotEmpty<UserInfo, string>().WithMessage<UserInfo, string>("{0} اجباری می باشد نام");
            this.RuleFor<string>((Expression<Func<UserInfo, string>>)(user => user.LastName)).NotNull<UserInfo, string>().WithMessage<UserInfo, string>("{0} اجباری می باشد نام خانوادگی").NotEmpty<UserInfo, string>().WithMessage<UserInfo, string>("{0} اجباری می باشد نام خانوادگی");
            this.RuleFor<string>((Expression<Func<UserInfo, string>>)(user => user.Password)).NotNull<UserInfo, string>().WithMessage<UserInfo, string>("{0} اجباری می باشد رمز عبور").NotEmpty<UserInfo, string>().WithMessage<UserInfo, string>("{0} اجباری می باشد رمز عبور");
            this.RuleFor<string>((Expression<Func<UserInfo, string>>)(user => user.PlainPassword)).Length<UserInfo>(1, 30).WithMessage<UserInfo, string>("مقدار {0} می بایست از '{1}' تا '{2}' کاراکتر باشد رمز عبور");
            //this.RuleFor<string>((Expression<Func<UserInfo, string>>)(user => user.NationalCode)).WithMessage<UserInfo, string>("{0} معتبر نمی باشد");
            //this.When((Func<UserInfo, bool>)(user => !string.IsNullOrWhiteSpace(user.MobileNo)), (Action)(() => this.RuleFor<string>((Expression<Func<UserInfo, string>>)(user => user.MobileNo)).Matches<UserInfo>("^9[0|1|2|3|4|9][0-9]{8}$").WithMessage<UserInfo, string>("{0} معتبر نمی باشد");
            //    this.RuleFor<string>((Expression<Func<UserInfo, string>>)(user => user.UserName)).NotNull<UserInfo, string>().WithMessage<UserInfo, string>("{0} اجباری می باشد نام کاربری").NotEmpty<UserInfo, string>().WithMessage<UserInfo, string>("{0} اجباری می باشد نام کاربری").MustAsync<UserInfo, string>((Func<UserInfo, string, Task<bool>>)((entity, userName) =>
            //    {
            //        if (entity.ObjectState == ObjectState.Deleted)
            //            return true;
            //        using (UnitOfWork uow = new UnitOfWork((IDataContextAsync)new SecurityDbContext()))
            //        {
            //            IRepositoryAsync<UserInfo> userInfoRep = uow.RepositoryAsync<UserInfo>();
            //            IQueryable<UserInfo> query = userInfoRep.Queryable(false, true, (List<Expression<Func<UserInfo, object>>>)null).Where<UserInfo>((Expression<Func<UserInfo, bool>>)(user => user.UserName == userName && user.ApplicationId == entity.ApplicationId));
            //            if (entity.ObjectState == ObjectState.Modified)
            //                query = query.Where<UserInfo>((Expression<Func<UserInfo, bool>>)(user => user.ID != entity.ID));
            //            int count = await query.CountAsync<UserInfo>();
            //            return count == 0;
            //        }
            //    })).WithMessage<UserInfo, string>("{0} با مقدار '{1}' تکراری می باشد", new Func<UserInfo, object>[2]
            //    {
            //(Func<UserInfo, object>) (user => (object) "نام کاربری"),
            //(Func<UserInfo, object>) (user => (object) user.UserName)
            //    });
        }
    }
}
