using OMF.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Accounting
{
    public class Invoice : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

        public int OrganId { get; set; }
        public int ContactId { get; set; }
        public string ContactTitle { get; set; }
        public DateTime DateTime { get; set; }
        public string DisplayDate { get; set; }
        public string DisplayDueDate { get; set; }
        public DateTime DueDate { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; }
        public string InvoiceStatusString { get; set; }
        public ZhivarEnums.NoeFactor InvoiceType { get; set; }
        public string InvoiceTypeString { get; set; }
        public bool IsDraft { get; set; }
        public bool IsPurchase { get; set; }
        public bool IsPurchaseReturn { get; set; }
        public bool IsSale { get; set; }
        public bool IsSaleReturn { get; set; }
        public string Note { get; set; }
        public string Number { get; set; }
        public string Refrence { get; set; }
        public decimal Paid { get; set; }
        public decimal Payable { get; set; }
        public decimal Profit { get; set; }
        public decimal Rest { get; set; }
        public bool Returned { get; set; }
        public bool Sent { get; set; }
        public ZhivarEnums.NoeInsertFactor Status { get; set; }
        public decimal Sum { get; set; }
        public string Tag { get; set; }
        // public Contact Contact { get; set; }
        public bool? IsContract { get; set; }

        public int? DocumentID { get; set;}


    }
}
