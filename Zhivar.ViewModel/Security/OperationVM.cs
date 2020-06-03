using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace Zhivar.ViewModel.Security
{
    public class OperationVM 
    {
        public int ID { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public OperationType OperationType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int OrderNo { get; set; }
        public string Tag1 { get; set; }
        public string Tag2 { get; set; }
        public string Tag3 { get; set; }
        public string Tag4 { get; set; }
        public bool IsSystem { get; set; }
        public int ApplicationId { get; set; }
    }
}
