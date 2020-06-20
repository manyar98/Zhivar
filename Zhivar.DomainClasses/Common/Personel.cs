using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Account;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Common
{
    public class Personel: Entity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public int OrganID { get; set; }

        public int UserID { get; set; }
        // public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Person Person { get; set; }

    }
}
