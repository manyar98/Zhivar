using System.ComponentModel.DataAnnotations;

namespace Zhivar.Web.ViewModels.Identity
{
    public class LoginViewModel
    {
    //    [Required(AllowEmptyStrings = false, ErrorMessage = "وارد کردن پست الکترونیکی ضروری است")]
    //    [Display(Name = "پست الکترونیکی")]
    //    [EmailAddress(ErrorMessage = "لطفا ایمیل معتبر وارد نمایید")]
    //    public string Email { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "وارد کردن موبایل ضروری است")]
        [Display(Name = "موبایل ")]
        [RegularExpression(@"^[0][9][0-9]{9,9}$", ErrorMessage = "لطفا شماره موبایل را صحیح وارد نمایید")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "وارد کردن کلمه عبور ضروری است")]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        [Display(Name = "مرا به یاد داشته باش؟")]
        public bool RememberMe { get; set; }

        [Required(ErrorMessage = "لطفا کد امنیتی را وارد نمائید")]
        [Display(Name = "کد امنیتی (به عدد)")]
        public string CaptchaInputText { get; set; }
    }
}