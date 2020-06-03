using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Security
{
    public class AccessUserRoleVM 
    {
        public AccessUserRoleVM()
        {
            UserRoles = new List<UserRoleVM>();
        }
        public int UserId { get; set; }
        public List<UserRoleVM> UserRoles { get; set; }
     
    }
}
