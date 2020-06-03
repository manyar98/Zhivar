using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Accounting;

namespace Zhivar.ViewModel.Accunting
{
    public class TransactionVM
    {
        public int ID { get; set; }
        public DocumentVM AccDocument { get; set; }
        public AccountVM Account{ get; set; }
        public decimal Amount { get; set; }
        public Cheque Cheque { get; set; }
        public Contact Contact { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string Description { get; set; }
        public DetailAccount DetailAccount { get; set; }
        public int Id { get; set; }
        public Invoice Invoice { get; set; }
        public Cost Cost { get; set; }
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
        public int? RefTrans { get; set; }

        public int DocumentId { get; set; }
        public int AccountId { get; set; }

        public int? ChequeId { get; set; }

        public int? ContactId { get; set; }

        public int InvoiceId { get; set; }
        public int CostId { get; set; }
        public DateTime Date { get; set; }
        public string DisplayDate { get; set; }
        public string AccountName { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
        public string AccountComplteCoding { get; set; }

    }
}
