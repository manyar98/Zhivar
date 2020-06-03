using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Contract;
using Zhivar.ViewModel.Security;

namespace Zhivar.ViewModel.Contract
{
    public class Contract_Saze_BazareabVM 
    {
        public int ID { get; set; }
        public int Contarct_SazeId { get; set; }
        public int UserID { get; set; }
        public decimal? Hazine { get; set; }
        public UsersForRule User { get; set; }
        public ZhivarEnums.NoeMozd NoeMozdBazryab { get; set; }
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Contract_Saze_BazareabVM, Contract_Saze_Bazareab>();

            configuration.CreateMap<Contract_Saze_Bazareab, Contract_Saze_BazareabVM>();
        }

    }
}
