using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Accounting;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.BaseInfo;
using Zhivar.ViewModel.Common;
using Zhivar.ViewModel.Security;

namespace Zhivar.ViewModel.Contract
{
    public class ContractData
    {
        public ContractVM contract { get; set; }
        public List<ContactVM> contacts { get; set; }
        public List<SazeVM> items { get; set; }
        public List<NoeEjare> noeEjares { get; set; }

        public ContractSettings contractSettings { get; set; }

        public List<UsersForRule> nasabs { get; set; }
        public List<UsersForRule> chapkhanes { get; set; }
        public List<UsersForRule> tarahs { get; set; }
        public List<UsersForRule> bazaryabs { get; set; }
        public List<NoeChap> noeChaps { get; set; }
        public List<Contract_PayRecevieVM> Contract_PayRecevie { get; set; }
        public List<CashVM> cashes { get; set; }
        public List<BankVM> banks { get; set; }
    }
}
