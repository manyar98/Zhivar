using System.ComponentModel.DataAnnotations;

namespace Zhivar.Web.ViewModels.Identity
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}