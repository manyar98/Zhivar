using OMF.Common;
using System.Collections.Generic;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Accunting
{
    public class BaseAccount : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public string Onvan { get; set; }

        public int ParentId { get; set; }

        public string Coding { get; set; }
        public string ComplteCoding { get; set; }

        public ZhivarEnums.AccountType Level { get; set; }

    }
}