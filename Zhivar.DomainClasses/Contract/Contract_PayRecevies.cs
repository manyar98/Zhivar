using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Contract
{
    public class Contract_PayRecevies : LoggableEntityName, IActivityLoggable
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
        public int? ContractId { get; set; }
        //public int? CostId { get; set; }
        public bool IsReceive { get; set; }
        public Contract Contract { get; set; }
        public Document Document { get; set; }
        public Contact Contact { get; set; }
        public Accounting.Account Account { get; set; }
        public ZhivarEnums.PayRecevieType Type { get; set; }
        public int Number { get; set; }
        public ZhivarEnums.Status Status { get; set; }
        public List<Contract_DetailPayRecevies> Contract_DetailPayRecevies { get; set; }
    }
}
