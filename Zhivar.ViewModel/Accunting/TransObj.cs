using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;

namespace Zhivar.ViewModel.Accunting
{
    public class TransObj
    {
        public DocumentVM AccDocument { get; set; }
        public AccountVM Account { get; set; }
        public decimal Amount{ get; set; }
        public Cheque Cheque{ get; set; }
        public Contact Contact{ get; set; }
        public decimal Credit{ get; set; }
        public decimal Debit{ get; set; }
        public string Description{ get; set; }
        public DetailAccount DetailAccount{ get; set; }
        public int Id{ get; set; }
        public Invoice Invoice{ get; set; }
        public bool IsCredit{ get; set; }
        public bool IsDebit{ get; set; }
        public int PaymentMethod{ get; set; }
        public string PaymentMethodString{ get; set; }
        public string Reference{ get; set; }
        public decimal Remaining{ get; set; }
        public string RemainingType{ get; set; }
        public int RowNumber{ get; set; }
        public decimal Stock{ get; set; }
        public string TransactionTypeString{ get; set; }
        public int Type{ get; set; }
        public decimal UnitPrice{ get; set; }
    }
}
