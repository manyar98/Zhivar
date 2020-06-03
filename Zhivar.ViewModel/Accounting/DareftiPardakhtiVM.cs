using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zhivar.AutoMapperContracts;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.ViewModel.Accounting
{
    public class DareftiPardakhtiVM : IHaveCustomMappings
    {
        public int? ID { get; set; }
        public int OrganId { get; set; }
        public int Noe { get; set; }
        public int FactorId { get; set; }
        public int SandoghId { get; set; }
        public decimal Mablagh { get; set; }
        public int BankHesabId { get; set; }
        public string ShomareFish { get; set; }
        public string ShomareCheck { get; set; }
        public string NamBank { get; set; }
        public DateTime TarikhCheck { get; set; }
        public string Shob { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<DareftiPardakhtiVM, DareftiPardakhti>();

            configuration.CreateMap<DareftiPardakhti, DareftiPardakhtiVM>();
        }

    }
}
