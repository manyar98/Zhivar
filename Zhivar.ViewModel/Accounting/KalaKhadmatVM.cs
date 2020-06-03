using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zhivar.AutoMapperContracts;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.ViewModel.Accounting
{
    public class KalaKhadmatVM : IHaveCustomMappings
    {
        public int? ID { get; set; }
        [Required(ErrorMessage = "لطفا گروه کالا/خدمات را وارد کنید")]
        [Display(Name = "گروه کالا/خدمات")]
        public int GoroheKalaID { get; set; }
        [Required(ErrorMessage = "لطفا نوع کالا/خدمات را وارد کنید")]
        [Display(Name = "نوع کالا/خدمات")]
        public int Noe { get; set; }
        [Required(ErrorMessage = "لطفا کد را وارد کنید")]
        [Display(Name = "کد")]
        public string Code { get; set; }
        //public string Barcode { get; set; }
        [Required(ErrorMessage = "لطفا عنوان را وارد کنید")]
        [Display(Name = "عنوان")]
        public string Onvan { get; set; }
        [Required(ErrorMessage = "لطفا قیمت خربد را وارد کنید")]
        [Display(Name = "قیمت خرید")]
        public decimal GhematKharid { get; set; }
        [Required(ErrorMessage = "لطفا عنوان در فاکتور خرید را وارد کنید")]
        [Display(Name = "عنواندر فاکتور خرید")]
        public string OnvanKharid { get; set; }
      //  [Required(ErrorMessage = "لطفا قیمت خربد را وارد کنید")]
        [Display(Name = "قیمت فروش")]
        public decimal GhematFrosh { get; set; }
      //  [Required(ErrorMessage = "لطفا عنوان کالا/ خدمات در فاکتور فروش را وارد کنید")]
        [Display(Name = "عنوان در فاکتور فروش")]
        public string OnvanFrosh { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<KalaKhadmatVM, KalaKhadmat>();

            configuration.CreateMap<KalaKhadmat, KalaKhadmatVM>();
        }

    }
}
