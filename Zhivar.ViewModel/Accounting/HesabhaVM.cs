using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zhivar.AutoMapperContracts;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.ViewModel.Accounting
{
    public class HesabhaVM : IHaveCustomMappings
    {
        public int? ID { get; set; }
        public string Onvan { get; set; }
        public int ParentId { get; set; }
        public string Code { get; set; }
        public int GoroheHesabId { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<HesabhaVM, Hesabha>();

            configuration.CreateMap<Hesabha, HesabhaVM>();
        }

    }
}
