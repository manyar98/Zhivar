using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ViewModel.BaseInfo
{
    public class NoeEjareVM 
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int OrganId { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<NoeEjareVM, NoeEjare>();

            configuration.CreateMap<NoeEjare, NoeEjareVM>();
        }
    }

}
