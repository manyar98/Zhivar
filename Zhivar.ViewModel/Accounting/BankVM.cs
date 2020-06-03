using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.AutoMapperContracts;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.ViewModel.Accounting
{
    public class BankVM : IHaveCustomMappings
    {
        public int? ID { get; set; }
        [Display(Name = "نام بانک")]
        public string Nam { get; set; }
        [Display(Name = "شماره حساب")]
        public string ShomareHesab { get; set; }
        [Display(Name = "شماره شعبه")]
        public string ShomareShobe { get; set; }
        [Display(Name = "کد شعبه")]
        public string CodeShobe { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<BankVM, Bank>();
            configuration.CreateMap<Bank, BankVM>();
        }
    }
}
