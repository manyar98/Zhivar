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
    public class DetailPayRecevie: LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public decimal Amount { get; set; }
        public int? BankId { get; set; }
        public int? CashId { get; set; }
        public int? ChequeId { get; set; }
        public int? ChequeBankId { get; set; }
        public Bank Bank { get; set; }
        public Cash Cash { get; set; }
        public Cheque Cheque { get; set; }
        public ChequeBank ChequeBank { get; set; }
        public int PayRecevieId { get; set; }
        public string Reference { get; set; }
        public ZhivarEnums.DetailPayReceiveType Type { get; set; }

    }
}
