using OMF.Common;
using System;
using System.Collections.Generic;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using Zhivar.DomainClasses.Contract;
using static OMF.Common.Enums;

namespace Zhivar.ViewModel.Contract
{
    public class ContractStopsVM 
    {
        public int ID { get; set; }
        public int ContractID { get; set; }
        public ZhivarEnums.ContractStopType Type { get; set; }
        public int? OtherContractID { get; set; }
        public int? InvoiceID { get; set; }
        public string DisplayDateRegister { get; set; }
        public DateTime DateRegister { get; set; }
        public string Description { get; set; }
        public List<ContractStopDetailsVM> ContractStopDetails { get; set; }
        public List<ContractStopDetails> ContractStopDetailCommon { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public byte[] Blob { get; set; }
        public string TasvirBlobBase64 { get; set; }
        public int? Ratio { get; set; }

    }
}
