using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;

namespace Zhivar.DomainClasses.Account
{
    public class ForgetPasswordUser:Entity
    {
        public int UserId { get; set; }
        public DateTime Date { get; set; }
    }
}
