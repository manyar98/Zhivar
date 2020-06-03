using OMF.Common.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Data.Entity;
using OMF.EntityFramework.Ef6;
using OMF.Common;
using OMF.Security.Model;
using OMF.Security;
using OMF.Common.Configuration;
using OMF.Security.Captcha;
using Zhivar.DomainClasses.Account;
using Zhivar.DomainClasses.Security;

namespace Zhivar.DataLayer.Validation
{
    public class RegisterationValidator : AbstractValidator<BussinessRegisteration>
    {
        public RegisterationValidator()
        {

            this.RuleFor(dr => dr.Password).NotNull().WithMessage("رمز عبور اجباری می باشد.")
                                            .NotEmpty().WithMessage("رمز عبور اجباری می باشد.")
                                            .Must(password => password.Length > 5).WithMessage(" رمز عبور باید حداقل 6 کاراکتر  باشد")
                                            .Matches("^([0-9]+[a-zA-Z]+|[a-zA-Z]+[0-9]+)[0-9a-zA-Z]*$").WithMessage(" رمز عبور باید ترکیب کاراکتر و عدد  باشد");

            this.RuleFor(dr => dr.MobileNo).NotNull().WithMessage( " تلفن همراه اجباری می باشد.")
                                                     .NotEmpty().WithMessage("تلفن همراه اجباری می باشد.")
                                                     .Matches(RegExPatterns.Mobile).WithMessage( dr => $"معتبر نمی باشد تلفن همراه '{dr.MobileNo}'"); ;
            When(dr => dr.IsFinalStep, () =>
            {
                this.RuleFor(dr => dr.MobileCaptcha).NotNull().WithMessage( "کد تایید اجباری می باشد.")
                                                    .NotEmpty().WithMessage("کد تایید اجباری می باشد.");
            });

            UserInfo uInfo = null;
            this.RuleFor(entity => entity.MobileNo).MustAsync(async (entity, userName) =>
            {



                  using (UnitOfWork uow = new UnitOfWork())
                  {
                    //var userQuery = uow.RepositoryAsync<ZhivarUserInfo>()
                    //                   .Queryable()
                    //                   .Where(user => !user.IsDeleted);// &&
                    //                                   //user.ApplicationId == ConfigurationController.ApplicationID);


                    //uInfo = await userQuery.Where(user => //user.UserName == userName
                    //                     //&& 
                    //                     user.IsActive == true
                    //                                     && user.IsDeleted == false)
                    //                                     .FirstOrDefaultAsync();
                    if (uInfo == null)
                        return true;
                    else
                        return false;
                }
            }).WithMessage("نام کاربری بااین تلفن همراه قبلاً در سیستم ثبت شده است.");

            When(dr => !dr.IsFinalStep, () =>
            {
                this.RuleFor(dr => dr.Captcha).NotNull().WithMessage("کد امنیتی اجباری می باشد.")
                                              .NotEmpty().WithMessage("کد امنیتی اجباری می باشد.")
                                              .Must(captcha => {
                                                  return CaptchaManager.VerifyCaptcha(captcha);
                                              }).WithMessage( " کد امنیتی معتبر می باشد.");
            });
        }
    }
}
