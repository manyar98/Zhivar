using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Common;
using System;

namespace Zhivar.ViewModel.Accunting
{
    public class SahamdaranVM: IValidatableObject
    {
        public int? ID { get; set; }
        [Display(Name ="تعریف نوع شخص")]
        [Required(ErrorMessage ="لطفا تغریف نوع شخص را انتخاب کنید")]
        public int HasShakhsInDatabase { get; set; }
        [Display(Name = "انتخاب از لیست اشخاص")]
        public int? ShakhsId { get; set; }
        public int OrganId { get; set; }
        [Display(Name = "درصد سهم ")]
        [Required(ErrorMessage = "لطفا درصد سهم  را انتخاب کنید")]
        public decimal Sahm { get; set; }

        [Display(Name = "جنسیت")]
        public ZhivarEnums.Jenseat? Jenseat { get; set; }

        [Display(Name = "نام ")]
        public string Nam { get; set; }
        [Display(Name = "نام خانوادگی")]
        public string NamKhanvadegi { get; set; }
        [Display(Name = "آدرس ")]
        public string Address { get; set; }
        [Display(Name = "تلفن ")]
        public string Tel { get; set; }
        [Display(Name = "کد ملی/شناسه ملی")]
        public string CodeMeli { get; set; }
        [Display(Name = "نام پدر")]
        public string NamPedar { get; set; }

        [Display(Name = "کدپستی ")]
        public string CodePosti { get; set; }
        public string FullName { get; set; }
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<SahamdaranVM, Sahamdaran>();

            configuration.CreateMap<Sahamdaran, SahamdaranVM>();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HasShakhsInDatabase == 0)
            {
                if (string.IsNullOrEmpty(Nam))
                    yield return new ValidationResult("لطفا نام را وارد نمایید");

                if (string.IsNullOrEmpty(NamKhanvadegi))
                    yield return new ValidationResult("لطفا نام خانوادگی را وارد نمایید");

                if (string.IsNullOrEmpty(Address))
                    yield return new ValidationResult("لطفا آدرس را وارد نمایید");


                if (string.IsNullOrEmpty(Tel))
                    yield return new ValidationResult("لطفا تلفن را وارد نمایید");


                if (string.IsNullOrEmpty(CodeMeli))
                    yield return new ValidationResult("لطفا کد ملی را وارد نمایید");


            }
            else
            {
                if (!ShakhsId.HasValue)
                    yield return new ValidationResult("لطفا شخص مورد نظر را انتخاب نمایید");
            }
        }
    }
}
