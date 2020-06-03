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
    public class Document: LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        //[ForeignKey("FinanYear")]
        public int FinanYearId { get; set; }
        public FinanYear FinanYear { get; set; }
        public int OrganId { get; set; }
        public decimal Credit { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Debit { get; set; }
        public string Description { get; set; }
        public string DisplayDate { get; set; }
        public bool IsManual { get; set; }
        public int Number { get; set; }
        public int Number2 { get; set; }
        public ZhivarEnums.DocumentStatus Status { get; set; }
        public string StatusString { get; set; }
        public List<Transaction> Transactions { get; set; }
        public bool? IsFirsDocument { get; set; }

        public ZhivarEnums.NoeDoc Type { get; set; }

    }
}
