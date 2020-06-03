using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.Web.ViewModels.Identity
{
    public class VerificationViewModel
    {
        [Required(ErrorMessage = "لطفا کد تایید را صحیح وارد نمایید")]
        [StringLength(5, ErrorMessage = "کد تایید باید 5 حرف باشد", MinimumLength = 5)]

        [Display(Name = "کد تایید")]
        public string Code { get; set; }
    }
}
