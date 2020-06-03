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
    public class TransferMoney: LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        //[ForeignKey("FinanYear")]
        //public int FinanYearId { get; set; }
        //public virtual FinanYear FinanYear { get; set; }
        public decimal Amount { get; set; }
        public string DisplayDate { get; set; }
       
        public DateTime Date { get; set; }
        public string Description { get; set; }
        //[ForeignKey("Document")]
        public int? DocumentId { get; set; }
        public int DocumentNumber { get; set; }
        public string From { get; set; }
        public string FromDetailAccountId { get; set; }
        public string FromDetailAccountName { get; set; }
        public string FromReference { get; set; }
        public string To { get; set; }
        public int ToDetailAccountId { get; set; }
        public string ToDetailAccountName { get; set; }
        public string ToReference { get; set; }
        public int OrganId { get; set; }
        public Document Document { get; set; }
        public ZhivarEnums.Status Status { get; set; }
    }
}
