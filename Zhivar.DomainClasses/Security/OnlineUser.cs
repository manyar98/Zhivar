using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Behsho.Common.Security
{
    public class OnlineUser : Entity
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
        public DateTime LastLoginDate { get; set; }
        public Zhivar.DomainClasses.ZhivarEnums.ZhivarUserType UserType { get; set; }
    }
}
