using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
  
namespace Zhivar.ViewModel.Security
{
    public class AccessRoleVM 
    {
        public AccessRoleVM()
        {
            RoleOperations = new List<RoleOperationVM>();
        }
        public int RoleId { get; set; }
        public List<RoleOperationVM> RoleOperations { get; set; }
     
    }
}
