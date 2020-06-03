using System;
using System.ComponentModel.DataAnnotations;
using Zhivar.DomainClasses;

namespace Zhivar.Web.ViewModels.Identity
{
    public class RegisterViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "لطفا شماره موبایل را وارد نمایید")]
        [Display(Name = "شماره موبایل")]
        [RegularExpression(@"^[0][9][0-9]{9,9}$", ErrorMessage = "لطفا شماره موبایل را صحیح وارد نمایید")]
        public string UserName { get; set; }

        // [Required(ErrorMessage = "لطفا پست الکترونیکی را وارد نمایید")]
        [EmailAddress]
        [Display(Name = "پست الکترونیکی")]
        public string Email { get; set; }

        [Required(ErrorMessage = "لطفا کلمه عبور را وارد نمایید")]
        [StringLength(100, ErrorMessage = "کلمه عبور باید حداقل 6 حرف باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "تکرار کلمه عبور، با کلمه عبور یکسان نیست")]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }
        public string Code { get; set; }
        [Required(ErrorMessage = "لطفا کد امنیتی را وارد نمائید")]
        [Display(Name = "کد امنیتی (به عدد)")]
        public string CaptchaInputText { get; set; }

        [Required(ErrorMessage = "لطفا نوع را انتخاب نمایید")]
        [Display(Name = "نوع ")]
        public ZhivarEnums.NoeShakhs NoeShakhs { get; set; }

       // [Required(ErrorMessage = "لطفا نوع شخص حقوقی را انتخاب نمایید")]
        [Display(Name = "نوع شخص حقوقی ")]
        public ZhivarEnums.TypeHoghoghi? TypeHoghoghi { get; set; }

        [Required(ErrorMessage = "لطفا نام را وارد نمایید")]
        [Display(Name = "نام ")]
        public string Nam { get; set; }
        public string NamKhanvadegi { get; set; }
        [Required(ErrorMessage = "لطفا آدرس را وارد نمایید")]
        [Display(Name = "آدرس ")]
        public string Address { get; set; }
        [Required(ErrorMessage = "لطفا تلفن را وارد نمایید")]
        [Display(Name = "تلفن ")]
        public string Tel { get; set; }
        public string CodeMeli { get; set; }
        public string NamPedar { get; set; }
        public string CodeEghtesadi { get; set; }
        [Required(ErrorMessage = "لطفا کدپستی را وارد نمایید")]
        [Display(Name = "کدپستی ")]
        public string CodePosti { get; set; }
        public string ShomareSabt { get; set; }
        public DateTime? TarikhSoudor { get; set; }
        public string ModateEtebar { get; set; }
        public string SahebEmtiaz { get; set; }

    }
}