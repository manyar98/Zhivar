using OMF.Common;
using System;
using System.Collections.Generic;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.BaseInfo
{
    public class Organ_Color : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

        public int OrganID { get; set; }
        public int LastColorIDUsed { get; set; }


    }
}

