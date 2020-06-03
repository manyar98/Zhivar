using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Accounting
{
    public class MapItemSaze: LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

        public int ItemID { get; set; }
        public int SazeID { get; set; }
        public ZhivarEnums.MapItemSazeType Type { get; set; }
    
    }
}
