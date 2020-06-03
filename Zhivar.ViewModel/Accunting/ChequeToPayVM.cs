using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;

namespace Zhivar.ViewModel.Accunting
{
    public class ChequeToPayVM
    {
        public decimal Amount { get; set; }
        public string BankBranch { get; set; }
        public string BankName { get; set; }
        public string ChequeNumber { get; set; }
        public Contact Contact { get; set; }
        public Bank DepositBank { get; set; }
        public string DepositDate { get; set; }
        public string DisplayDate { get; set; }
        public int Id { get; set; }
        public bool Overdue { get; set; }
        public string ReceiptDate { get; set; }
        public ZhivarEnums.ChequeStatus Status { get; set; }

    }
}
