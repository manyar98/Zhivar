using OMF.Common;
using OMF.Common.Validation;
using OMF.Security.Model.Validators;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMF.EntityFramework.Ef6;
using System.Data.Entity;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.DataLayer.Validation.BaseInfo
{
    public partial class SazeValidator : AbstractValidator<Saze>
    {
        public SazeValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(saze => saze.Code).Must((entity, code) =>
            {
                using (var uow = new UnitOfWork())
                {
                    if (entity.ObjectState == Enums.ObjectState.Added)
                    {
                        var sazeQuery = uow.RepositoryAsync<Saze>().Queryable().Where(x => x.Code == entity.Code && x.OrganId == entity.OrganId);

                        if (sazeQuery.Any())
                            return false;

                        return true;
                    }
                    else
                        return true;
               
                }


            }).WithMessage("کد رسانه تکراری می باشد.");

            RuleFor(saze => saze.NoeEjareID)
                //.NotNull().WithMessage("نوع اجاره رسانه را مشخص کنید.")
                                            .NotEqual(5).WithMessage("نوع اجاره رسانه را مشخص کنید.");

            RuleFor(saze => saze.NoeSazeId)
                //.NotNull().WithMessage("نوع رسانه را مشخص کنید.")
                                            .NotEqual(4).WithMessage("نوع رسانه را مشخص کنید.");

            RuleFor(saze => saze.GoroheSazeID)
                //.NotNull().WithMessage("نوع گروه رسانه را مشخص کنید.")
                                          .NotEqual(2).WithMessage("نوع گروه رسانه را مشخص کنید.");

            RuleFor(saze => saze.Longitude)
                              //.NotNull().WithMessage("نوع گروه رسانه را مشخص کنید.")
                              .NotEqual(0).WithMessage("مشخص کردن موقعیت جغرافیایی رسانه الزامیست.");
            RuleFor(saze => saze.Latitude)
                  //.NotNull().WithMessage("نوع گروه رسانه را مشخص کنید.")
                  .NotEqual(0).WithMessage("مشخص کردن موقعیت جغرافیایی رسانه الزامیست.");

            RuleFor(saze => saze.Tol)
                //.NotNull().WithMessage("نوع گروه رسانه را مشخص کنید.")
                .NotEqual(0).WithMessage("وارد کردن طول رسانه الزامیست.");

            RuleFor(saze => saze.Arz)
               //.NotNull().WithMessage("نوع گروه رسانه را مشخص کنید.")
               .NotEqual(0).WithMessage("وارد کردن عرض رسانه الزامیست.");

            //.Equals();

        }
    }
}