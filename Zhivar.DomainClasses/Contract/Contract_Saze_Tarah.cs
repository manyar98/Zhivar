using OMF.Common;
using System.Collections.Generic;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.DomainClasses.Contract
{

    public class Contract_Saze_Tarah : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        //public int Contarct_SazeId { get; set; }

        public int UserID { get; set; }
        public decimal? Hazine { get; set; }
        public decimal HazineMoshtari { get; set; }
        public int ContarctSazeID { get; set; }
        public NoeMozd NoeMozdTarah { get; set; }
    }
}
