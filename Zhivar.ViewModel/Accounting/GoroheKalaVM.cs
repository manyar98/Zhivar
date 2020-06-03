using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zhivar.AutoMapperContracts;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.ViewModel.Accounting
{
    public class GoroheKalaVM : IHaveCustomMappings
    {
        public int? ID { get; set; }
        public string Title { get; set; }
        public int OrganID { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<GoroheKalaVM, GoroheKala>();

            configuration.CreateMap<GoroheKala, GoroheKalaVM>();
        }

    }

}