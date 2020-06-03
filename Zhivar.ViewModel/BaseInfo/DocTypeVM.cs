using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zhivar.DomainClasses;

namespace Zhivar.ViewModel.BaseInfo
{
    public class DocTypeVM
    {
        public string ID { get; set; }
        public string DocName { get; set; }
        public ZhivarEnums.DocRequest DocRequestType { get; set; }
        public bool IsActive { get; set; }
    }
}