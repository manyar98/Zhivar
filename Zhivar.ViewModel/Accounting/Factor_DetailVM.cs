using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zhivar.AutoMapperContracts;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.ViewModel.Accounting
{
    public class Factor_DetailVM : IHaveCustomMappings
    {
        public int? ID { get; set; }
        public int FactorId { get; set; }
        public string Tozehat { get; set; }
        public int Tedad { get; set; }
        public decimal Mablagh { get; set; }
        public decimal Takhfif { get; set; }
        public decimal Fi { get; set; }
        public decimal Maleat { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Factor_DetailVM, Factor_Detail>();

            configuration.CreateMap<Factor_Detail, Factor_DetailVM>();
        }

    }
}
