using System.ComponentModel.DataAnnotations;

namespace Zhivar.Web.ViewModels.Identity
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "وارد کردن شماره موبایل ضروری است")]
        [RegularExpression(@"^[0][9][0-9]{9,9}$", ErrorMessage = "لطفا شماره موبایل را صحیح وارد نمایید")]
        [Display(Name = "شماره موبایل")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "لطفا کد امنیتی را وارد نمائید")]
        [Display(Name = "کد امنیتی (به عدد)")]
        public string CaptchaInputText { get; set; }
    }
}