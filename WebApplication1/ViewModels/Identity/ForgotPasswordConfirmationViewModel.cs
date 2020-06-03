using System.ComponentModel.DataAnnotations;

namespace Zhivar.Web.ViewModels.Identity
{

    public class ForgotPasswordConfirmationViewModel
    {

        [Required(ErrorMessage = "لطفا کد امنیتی را وارد نمائید")]
        [Display(Name = "کد امنیتی (به عدد)")]
        public string CaptchaInputText { get; set; }

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

        [Required(ErrorMessage = "لطفا کد تاییدیه ارسال شده را وارد نمایید")]
        [Display(Name = "کد تاییدیه")]
        public string Code { get; set; }

    }
}