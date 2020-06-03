using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ViewModel.BaseInfo
{
    public class NoeChapVM 
    {
        public int? ID { get; set; }
        public int OrganId { get; set; }
        public string Title { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<NoeChapVM, NoeChap>();

            configuration.CreateMap<NoeChap, NoeChapVM>();
        }

    }
}