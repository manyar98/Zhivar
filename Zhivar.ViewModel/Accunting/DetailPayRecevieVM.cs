using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using Zhivar.ViewModel.Accounting;

namespace Zhivar.ViewModel.Accunting
{
    public class DetailPayRecevieVM
    {
        public int? ID { get; set; }
        public decimal Amount { get; set; }
        public BankVM BankVM { get; set; }
        public CashVM CashVM { get; set; }
        public ChequeVM ChequeVM { get; set; }
        //public ChequeBank ChequeBank { get; set; }
        public int PayRecevieId { get; set; }
        public string Reference { get; set; }
        public ZhivarEnums.DetailPayReceiveType Type { get; set; }

    }
}
