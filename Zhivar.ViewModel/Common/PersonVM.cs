using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Common;

namespace Zhivar.ViewModel.Common
{
    public class PersonVM
    {
        public int? ID { get; set; }
        [Display(Name = "جنسیت")]
        public ZhivarEnums.Jenseat? Jenseat { get; set; }

        [Required(ErrorMessage = "لطفا نوع شخص را انتخاب نمایید")]
        [Display(Name = " نوع شخص ")]
        public ZhivarEnums.NoeShakhs NoeShakhs { get; set; }
        [Display(Name = "نوع حقوقی")]
        public ZhivarEnums.TypeHoghoghi? TypeHoghoghi { get; set; }
        [Required(ErrorMessage = "لطفا نام را وارد نمایید")]
        [Display(Name = "نام ")]
        public string Nam { get; set; }
        [Display(Name = "نام خانوادگی")]
        public string NamKhanvadegi { get; set; }
        [Required(ErrorMessage = "لطفا آدرس را وارد نمایید")]
        [Display(Name = "آدرس ")]
        public string Address { get; set; }
        [Required(ErrorMessage = "لطفا تلفن را وارد نمایید")]
        [Display(Name = "تلفن ")]
        public string Tel { get; set; }
        [Display(Name = "کد ملی/شناسه ملی")]
        public string CodeMeli { get; set; }
        [Display(Name = "نام پدر")]
        public string NamPedar { get; set; }
        [Display(Name = "کد اقتصادی")]
        public string CodeEghtesadi { get; set; }
        [Required(ErrorMessage = "لطفا کدپستی را وارد نمایید")]
        [Display(Name = "کدپستی ")]
        public string CodePosti { get; set; }
        [Display(Name = "شماره ثبت")]
        public string ShomareSabt { get; set; }
        [Display(Name = "تاریخ صدور")]
        public DateTime? TarikhSoudor { get; set; }
        [Display(Name = "مدت اعتبار")]
        public string ModateEtebar { get; set; }
        [Display(Name = "صاحب امتیاز")]
        public string SahebEmtiaz { get; set; }

        [Display(Name = "وضعیت")]
        public ZhivarEnums.Vaziat Vaziat { get; set; }
        public string FullName { get { return Nam + " " + NamKhanvadegi; } }
        public bool IsOrgan { get; set; }
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<PersonVM, Person>();
            configuration.CreateMap<Person, PersonVM>();
        }
      
    }
}
