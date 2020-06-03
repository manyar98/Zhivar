using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMF.Workflow.Cartable.Model
{
    public class ChangeReceiverRequest
    {
        public List<int> FromUsersId { get; set; }
        public int ToUserId { get; set; }
        public int RoleId { get; set; }
    }
}
