using System.ComponentModel.DataAnnotations;

namespace Zhivar.Web.ViewModels.Identity
{
    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }
}