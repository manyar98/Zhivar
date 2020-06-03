using OMF.Common;
using System.Collections.Generic;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Contract
{
    public class Contract_Saze_Nasab : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        //public int Contract_SazeId { get; set; }

        public int UserID { get; set; }
        public decimal? Hazine { get; set; }
        public decimal HazineMoshtari { get; set; }
        public int ContarctSazeID { get; set; }
        public ZhivarEnums.NoeMozd NoeMozdNasab { get; set; }


    }
}
