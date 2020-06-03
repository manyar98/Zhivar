using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Contract;
using Zhivar.ViewModel.Security;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.ViewModel.Contract
{

    public class Contract_Saze_ChapkhaneVM 
    {
        public int ID { get; set; }
        public int ContarctSazeID { get; set; }
        public decimal FiMoshtari { get; set; }
        public decimal? FiChapkhane { get; set; }
        public decimal MetrazhMoshtari { get; set; }
        public decimal? MetrazhChapkhane { get; set; }
        public decimal TotalMoshtari { get; set; }
        public decimal? TotalChapkhane { get; set; }
        public ChapkhaneType ChapkhaneType { get; set; }
        public int UserID { get; set; }
        public UsersForRule User { get; set; }
        public int? NoeChapID { get; set; }
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Contract_Saze_ChapkhaneVM, Contract_Saze_Chapkhane>();

            configuration.CreateMap<Contract_Saze_Chapkhane, Contract_Saze_ChapkhaneVM>();
        }

    }
}
