using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;

namespace Zhivar.ViewModel.Common
{
    public class PersonelVM:IValidatableObject
    {
        public int? ID { get; set; }
        [Required(ErrorMessage = "لطفا نوع سمت را انتخاب نمایید")]
        [Display(Name = " نوع سمت ")]
        public int RoleID { get; set; }
        //[Required(ErrorMessage = "لطفا شماره موبایل را وارد نمایید")]
        [Display(Name = "شماره موبایل")]
        //[RegularExpression(@"^[0][9][0-9]{9,9}$", ErrorMessage = "لطفا شماره موبایل را صحیح وارد نمایید")]
        public string UserName { get; set; }

        //[Required(ErrorMessage = "لطفا کلمه عبور را وارد نمایید")]
        //[StringLength(100, ErrorMessage = "کلمه عبور باید حداقل 6 حرف باشد", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور")]
        //[Compare("Password", ErrorMessage = "تکرار کلمه عبور، با کلمه عبور یکسان نیست")]
        public string ConfirmPassword { get; set; }
        public PersonVM PersonVM { get; set; }
        public string RoleName { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!ID.HasValue)
            {
                if (string.IsNullOrEmpty(UserName))
                    yield return new ValidationResult("لطفا شماره موبایل را وارد نمایید");

                if (string.IsNullOrEmpty(Password))
                    yield return new ValidationResult("لطفا کلمه عبور را وارد نمایید");

                if(!string.Equals(Password,ConfirmPassword))
                    yield return new ValidationResult("تکرار کلمه عبور، با کلمه عبور یکسان نیست");

                if (!string.IsNullOrEmpty(Password))
                {
                    if (Password.Length < 6)
                        yield return new ValidationResult("کلمه عبور باید حداقل 6 حرف باشد");
                }
 

            }
     
        
        }
    }
}
