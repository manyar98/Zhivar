using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ViewModel.BaseInfo
{
    public class VahedTolVM 
    {
        public int? ID { get; set; }
        public int OrganId { get; set; }
        public string Title { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<VahedTolVM, VahedTol>();

            configuration.CreateMap<VahedTol, VahedTolVM>();
        }

    }
}