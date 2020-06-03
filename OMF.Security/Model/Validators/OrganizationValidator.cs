//using OMF.Common;
//using OMF.Common.Validation;
//using OMF.EntityFramework.Ef6;
//using FluentValidation;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using static OMF.Common.Enums;

//namespace OMF.Security.Model.Validators
//{
//    public class OrganizationValidator : EntityValidator<Organization>
//    {
//        public OrganizationValidator()
//        {
//            this.CascadeMode = CascadeMode.StopOnFirstFailure;
//            this.RuleFor<int>((Expression<Func<Organization, int>>)(org => org.OrganizationUnitChartId)).NotNull<Organization, int>().WithMessage(" اجباری می باشد انتخاب چارت سازمانی").NotEmpty<Organization, int>().WithMessage<Organization, int>(" اجباری می باشدانتخاب چارت سازمانی").GreaterThan<Organization, int>(0).WithMessage(" اجباری می باشدانتخاب چارت سازمانی");
//            this.RuleFor<int>((Expression<Func<Organization, int>>)(org => org.CityId)).NotNull<Organization, int>().WithMessage("انتخاب شهراجباری می باشد").NotEmpty<Organization, int>().WithMessage("{0} اجباری می باشدانتخاب شهر").GreaterThan<Organization, int>(0).WithMessage(" اجباری می باشدانتخاب شهر");
//            this.RuleFor<string>((Expression<Func<Organization, string>>)(org => org.Title)).NotNull<Organization, string>().WithMessage(" اجباری می باشدعنوان سازمان").NotEmpty<Organization, string>().WithMessage<Organization, string>(" اجباری می باشد عنوان سازمان");
//            this.RuleFor<string>((Expression<Func<Organization, string>>)(org => org.Code)).NotNull<Organization, string>().WithMessage(" اجباری می باشدکد سازمان").NotEmpty<Organization, string>().WithMessage<Organization, string>("اجباری می باشد کد سازمان").Must<Organization, string>((Func<Organization, string, bool>)((entity, code) =>
//            {
//                using (UnitOfWork unitOfWork = new UnitOfWork())
//                {
//                    IQueryable<Organization> source = unitOfWork.RepositoryAsync<Organization>().Queryable(false, true, (List<Expression<Func<Organization, object>>>)null).Where<Organization>((Expression<Func<Organization, bool>>)(o => o.Code == code));
//                    if (entity.ObjectState == ObjectState.Modified)
//                        source = source.Where<Organization>((Expression<Func<Organization, bool>>)(o => o.ID != entity.ID));
//                    return source.Count<Organization>() == 0;
//                }
//            })).WithMessage("کد ارگان با مقدار '{1}' تکراری می باشد");
//        }
//    }
//}
