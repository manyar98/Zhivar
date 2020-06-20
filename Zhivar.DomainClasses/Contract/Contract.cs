using OMF.Common;
using System;
using System.Collections.Generic;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Contract
{
    public class Contract : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

        public int OrganId { get; set; }
        public int ContactId { get; set; }
        public string ContractTitle { get; set; }
        public DateTime DateTime { get; set; }
        public string DisplayDate { get; set; }
        public string DisplayDueDate { get; set; }
        public DateTime DueDate { get; set; }
        public List<Contract_Saze> Contract_Sazes { get; set; }
        public bool IsDraft { get; set; }
        public string Note { get; set; }
        public string Number { get; set; }
        public string Refrence { get; set; }
        public decimal Paid { get; set; }
        public decimal Payable { get; set; }
        public decimal Profit { get; set; }
        public decimal Rest { get; set; }
        public bool Sent { get; set; }
        public ZhivarEnums.Status Status { get; set; }
        public decimal Sum { get; set; }
        public string Tag { get; set; }
        public int? InvoiceId { get; set; }
        public List<Contract_PayRecevies> Contract_PayRecevies { get; set; }
        public ZhivarEnums.ContractType ContractType { get; set; }
        public bool? AutoTax { get; set; }
        public int? DocumentID { get; set; }
    }
}