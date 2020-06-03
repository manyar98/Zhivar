using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zhivar.AutoMapperContracts;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.ViewModel.Accounting
{
    public class BankHesabVM : IHaveCustomMappings
    {
        public int? ID { get; set; }
        public string Code { get; set; }
        public string Onvan { get; set; }
        public string Shomare { get; set; }
        public string Shobe { get; set; }
        public int OrganId { get; set; }
        public int HesabId { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<BankHesabVM, BankHesab>();

            configuration.CreateMap<BankHesab, BankHesabVM>();
        }

    }
}
