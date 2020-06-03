//using OMF.Common.Validation;
//using OMF.EntityFramework.Ef6;
//using FluentValidation;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;

//namespace OMF.Security.Model.Validators
//{
//    internal class OrganizationUnitChartValidator : EntityValidator<OrganizationUnitChart>
//    {
//        public OrganizationUnitChartValidator()
//        {
//            this.CascadeMode = CascadeMode.StopOnFirstFailure;
//            this.RuleFor<string>((Expression<Func<OrganizationUnitChart, string>>)(ouc => ouc.Title)).NotNull<OrganizationUnitChart, string>().WithMessage("{0} اجباری می باشدعنوان چارت").NotEmpty<OrganizationUnitChart, string>().WithMessage("{0} اجباری می باشدعنوان چارت");
//            this.RuleFor<string>((Expression<Func<OrganizationUnitChart, string>>)(ouc => ouc.Code)).NotNull<OrganizationUnitChart, string>().WithMessage("{0} اجباری می باشدکد چارت").NotEmpty<OrganizationUnitChart, string>().WithMessage("{0} اجباری می باشدکد چارت").Must<OrganizationUnitChart, string>((Func<OrganizationUnitChart, string, bool>)((entity, code) =>
//            {
//                using (UnitOfWork unitOfWork = new UnitOfWork())
//                {
//                    IQueryable<OrganizationUnitChart> source = unitOfWork.RepositoryAsync<OrganizationUnitChart>().Queryable(false, true, (List<Expression<Func<OrganizationUnitChart, object>>>)null).Where<OrganizationUnitChart>((Expression<Func<OrganizationUnitChart, bool>>)(c => c.Code == code));
//                    if (entity.ID > 0)
//                        source = source.Where<OrganizationUnitChart>((Expression<Func<OrganizationUnitChart, bool>>)(c => c.ID != entity.ID));
//                    return source.Count<OrganizationUnitChart>() == 0;
//                }
//            })).WithMessage("کد چارت سازمانی با مقدار '{1}' تکراری می باشد");
//        }
//    }
//}
