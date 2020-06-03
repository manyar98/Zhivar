using OMF.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.BaseInfo
{
    public class Cheque: LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public int OrganId { get; set; }
        [ForeignKey("Contact")]
        public int ContactId { get; set; }
        public decimal Amount { get; set; }
        public string BankBranch { get; set; }
        public string BankName { get; set; }
        public DateTime Date { get; set; }
        public string DisplayDate { get; set; }
        public string ChequeNumber { get; set; }
        public ZhivarEnums.ChequeStatus Status { get; set; }
        [ForeignKey("DepositBank")]
        public int? DepositBankId { get; set; }
        public virtual Bank DepositBank { get; set; }

        public DateTime? ReceiptDate { get; set; }
        public ZhivarEnums.ChequeType Type { get; set; }
        public virtual Contact Contact { get; set; }

    }
}
