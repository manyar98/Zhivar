using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.Security;

namespace Zhivar.ViewModel.Contract
{
    public class Contract_SazeTransObj
    {
        public Contract_SazeVM Contract_Saze { get; set; }
        public List<Contract_Saze_ChapkhaneVM> Contract_Saze_Chapkhanes { get; set; }
        public List<Contract_Saze_NasabVM> Contract_Saze_Nasabs { get; set; }
        public List<Contract_Saze_BazareabVM> Contarct_Saze_Bazareabs { get; set; }
        public List<Contract_Saze_TarahVM> Contract_Saze_Tarahs { get; set; }
        public List<ContractSazeImagesVM> ContractSazeImages { get; set; }
        public List<UsersForRule> bazaryabs { get; set; }
        public List<UsersForRule> tarahs { get; set; }
        public List<UsersForRule> chapkhanes { get; set; }
        public List<UsersForRule> nasabs { get; set; }
 
    }
}
