using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zhivar.AutoMapperContracts;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.ViewModel.Accounting
{
    public class SahamdaranVM : IHaveCustomMappings
    {
        public int? ID { get; set; }
        public int ShakhsId { get; set; }
        public int OrganId { get; set; }
        public decimal Sahm { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<SahamdaranVM, Sahamdaran>();

            configuration.CreateMap<Sahamdaran, SahamdaranVM>();
        }

    }
}
