using OMF.Common;
using System;
using System.Collections.Generic;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Contract
{
    public class ContractStops : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

        public int ContractID { get; set; }
        public ZhivarEnums.ContractStopType Type { get; set; }
        public int? OtherContractID { get; set; }
        public int? InvoiceID { get; set; }
        public string DisplayDateRegister { get; set; }
        public DateTime DateRegister { get; set; }
        public string Description { get; set; }
        public List<ContractStopDetails> ContractStopDetails { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public byte[] Blob { get; set; }
        public int? Ratio { get; set; }

    }
}
