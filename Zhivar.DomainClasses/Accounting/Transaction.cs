using OMF.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Accounting
{
    public class Transaction: LoggableEntity, IActivityLoggable
    {

        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        //[ForeignKey("AccDocument")]
        public int DocumentId { get; set; }
        //public Document AccDocument { get; set; }
        //[ForeignKey("Account")]
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public decimal Amount { get; set; }

        public int? ChequeId { get; set; }

        public int? ContactId { get; set; }

        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string Description { get; set; }
        //public DetailAccount DetailAccount { get; set; }
        public int InvoiceId { get; set; }
        public int CostId { get; set; }
        public bool IsCredit { get; set; }
        public bool IsDebit { get; set; }
        public decimal PaymentMethod { get; set; }
        public string PaymentMethodString { get; set; }
        public string Reference { get; set; }
        public decimal Remaining { get; set; }
        public string RemainingType { get; set; }
        public int RowNumber { get; set; }
        public int Stock { get; set; }
        public string TransactionTypeString { get; set; }
        public int Type { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime Date { get; set; }
        public string DisplayDate { get; set; }
        //[ForeignKey("Transactions")]
        public int? RefTrans { get; set; }
       // public List<Transaction> Transactions { get; set; }

        public Cheque Cheque { get; set; }

    }
}
