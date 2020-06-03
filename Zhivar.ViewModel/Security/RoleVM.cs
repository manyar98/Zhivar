using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zhivar.ViewModel.Security
{
    public class RoleVM
    {
        public int ID { get; set; }
        public int RoleType { get; set; }
        public int CHRTId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ParentId { get; set; }
        public List<RoleOperationVM> RoleOperations { get; set; }
    }
}