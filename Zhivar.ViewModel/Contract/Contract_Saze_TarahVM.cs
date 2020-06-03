using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Contract;
using Zhivar.ViewModel.Security;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.ViewModel.Contract
{
    public class Contract_Saze_TarahVM 
    {
        public int? ID { get; set; }
        public int Contarct_SazeId { get; set; }
        public int UserID { get; set; }
        public decimal? Hazine { get; set; }
        public decimal HazineMoshtari { get; set; }
        public UsersForRule User { get; set; }
        public NoeMozd NoeMozdTarah { get; set; }
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Contract_Saze_TarahVM, Contract_Saze_Tarah>();

            configuration.CreateMap<Contract_Saze_Tarah, Contract_Saze_TarahVM>();
        }

    }
}
