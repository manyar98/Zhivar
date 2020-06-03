using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Contract
{
    public class Contract_DetailPayRecevies : LoggableEntityName, IActivityLoggable
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
        public int Contract_PayRecevieId { get; set; }
        public string Reference { get; set; }
        public ZhivarEnums.DetailPayReceiveType Type { get; set; }

        public string BankBranchCheque { get; set; }
        public string BankNameCheque { get; set; }
        public DateTime? DateCheque { get; set; }
        public string DisplayDateCheque { get; set; }
        public string ChequeNumber { get; set; }

    }
}
