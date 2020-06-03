using OMF.Common;
using OMF.Security.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Security
{
    public class PositionVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsMultiAssignable { get; set; }
        public int? ParentId { get; set; }
        public int OrganizationUnitChartId { get; set; }
        public string OrganizationUnitChartTitle { get; set; }
        public int? OrganizationId { get; set; }
        public List<RoleOperation> RoleOperations { get; set; }
        public string OrganizationTitle { get; set; }
    }
}
