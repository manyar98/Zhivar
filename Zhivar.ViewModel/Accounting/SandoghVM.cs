using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zhivar.AutoMapperContracts;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.ViewModel.Accounting
{
    public class SandoghVM : IHaveCustomMappings
    {
        public int? ID { get; set; }
        public string Onvan { get; set; }
        public string Code { get; set; }
        public int HesabId { get; set; }
        public int OrganId { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<SandoghVM, Sandogh>();

            configuration.CreateMap<Sandogh, SandoghVM>();
        }

    }
}
