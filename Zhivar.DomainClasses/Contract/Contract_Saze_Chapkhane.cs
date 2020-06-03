using OMF.Common;
using System.Collections.Generic;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.DomainClasses.Contract
{
    public class Contract_Saze_Chapkhane : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        //public int Contarct_SazeId { get; set; }

        //public int NoeChap { get; set; }

        //public decimal FiChap { get; set; }

        //public decimal Metrazh { get; set; }

        //public int VaedId { get; set; }
        public int ContarctSazeID { get; set; }
        public decimal FiMoshtari { get; set; }
        public decimal? FiChapkhane { get; set; }
        public decimal MetrazhMoshtari { get; set; }
        public decimal? MetrazhChapkhane { get; set; }
        public decimal TotalMoshtari { get; set; }
        public decimal? TotalChapkhane { get; set; }
        public int? NoeChapID { get; set; }
        public int UserID { get; set; }
        public ChapkhaneType ChapkhaneType { get; set; }


    }
}
