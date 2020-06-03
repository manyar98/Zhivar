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
    public class PayRecevie:LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public int? AccountId { get; set; }
        public int OrganId { get; set; }
        public decimal Amount { get; set; }
        public int? ContactId { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string DisplayDate { get; set; }
        public int? DocumentId { get; set; }
        public int? InvoiceId { get; set; }
        public int? CostId { get; set; }
        public bool IsReceive { get; set; }
        public Invoice Invoice { get; set; }
        public Cost Cost { get; set; }
        public Document Document { get; set; }
        public Contact Contact { get; set; }
        public Account Account { get; set; }
        public List<DetailPayRecevie> Items { get; set; }
        public ZhivarEnums.PayRecevieType Type { get; set; }

        public int Number { get; set; }
        public ZhivarEnums.Status Status { get; set; }
    }
}
