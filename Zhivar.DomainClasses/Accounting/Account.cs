using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Accounting
{
    public class Account:LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public int OrganId { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public string Coding { get; set; }
        public string ComplteCoding { get; set; }
        public ZhivarEnums.AccountType Level { get; set; }
    }
}
