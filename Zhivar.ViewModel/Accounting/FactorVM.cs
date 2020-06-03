using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zhivar.AutoMapperContracts;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.ViewModel.Accounting
{
    public class FactorVM : IHaveCustomMappings
    {
        public int? ID { get; set; }
        public int OrganId { get; set; }
        public int MoshtariId { get; set; }
        public DateTime TarikhSarresid { get; set; }
        public string Code { get; set; }
        public int Erjae { get; set; }
        public string OnvanMoshtari { get; set; }
        public Decimal Mablagh { get; set; }
        public Decimal Maleat { get; set; }
        public Decimal JameKol { get; set; }
        public int Noe { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<FactorVM, Factor>();

            configuration.CreateMap<Factor, FactorVM>();
        }

    }
}
