using System.ComponentModel.DataAnnotations;

namespace Zhivar.Web.ViewModels.Identity
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
    }
}