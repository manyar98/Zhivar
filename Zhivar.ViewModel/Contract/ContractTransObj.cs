using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Contract
{
    public class ContractTransObj
    {
        public ContractVM contract { get; set; }
        public List<Contract_SazeVM> Contract_Sazes { get; set; }
        //public List<CashVM> cashes { get; set; }
        //public List<BankVM> banks { get; set; }
        //public List<TransactionVM> payments { get; set; }
    }
}
