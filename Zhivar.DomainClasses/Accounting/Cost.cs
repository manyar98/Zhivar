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
    public class Cost : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public int OrganId { get; set; }
        public int ContactId { get; set; }
        public DateTime DateTime { get; set; }
        public string DisplayDate { get; set; }
        public List<CostItem> CostItems { get; set; }
        public string Explain { get; set; }
        public string Number { get; set; }
        public decimal Paid { get; set; }
        public decimal Payable { get; set; }
        public decimal Rest { get; set; }
        public ZhivarEnums.CostStatus Status { get; set; }
        public decimal Sum { get; set; }
        public int? DocumentId { get; set; }
        //public Contact Contact { get; set; }
    }
}
