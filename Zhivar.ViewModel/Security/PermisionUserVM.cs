using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Security
{
    public class PermisionUserVM 
    {
        public PermisionUserVM()
        {
            OperationUsers = new List<UserOperationVM>();
        }
        public int UserId { get; set; }
        public List<UserOperationVM> OperationUsers { get; set; }
     
    }
}
