using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.BaseInfo
{
    public class DocType : LoggableEntityName, ISelfReferenceEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog
        {
            get
            {
                return ActionLog.Insert | ActionLog.Update;
            }
        }
        public string DocName { get; set; }
        public ZhivarEnums.DocRequest DocRequestType { get; set; }
        public bool IsActive { get; set; }


    }
}

