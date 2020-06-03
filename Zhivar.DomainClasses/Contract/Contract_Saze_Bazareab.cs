using OMF.Common;
using System.Collections.Generic;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Contract
{

    public class Contract_Saze_Bazareab : LoggableEntity,IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public int ContarctSazeID { get; set; }
        public int UserID { get; set; }
        public decimal? Hazine { get; set; }
        public ZhivarEnums.NoeMozd NoeMozdBazryab { get; set; }
        
        // public virtual Contract_Saze Contract_Saze { get; set; }
    }
}
