using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Contract;
using Zhivar.ViewModel.Security;

namespace Zhivar.ViewModel.Contract
{

    public class Contract_Saze_NasabVM 
    {
        public int ID { get; set; }
        public int Contract_SazeId { get; set; }
        public int UserID { get; set; }
        public decimal? Hazine { get; set; }
        public decimal HazineMoshtari { get; set; }
        public UsersForRule User { get; set; }
        public ZhivarEnums.NoeMozd NoeMozdNasab { get; set; }
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Contract_Saze_NasabVM, Contract_Saze_Nasab>();

            configuration.CreateMap<Contract_Saze_Nasab, Contract_Saze_NasabVM>();
        }

    }
}
