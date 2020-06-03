using OMF.Common;
using System;
using System.Collections.Generic;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.BaseInfo
{
    public class BaseColor : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

        public int? Number { get; set; }
        public string Name { get; set; }
        public string HEX { get; set; }

    }
}

