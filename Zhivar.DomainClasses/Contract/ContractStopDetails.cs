using OMF.Common;
using System;
using System.Collections.Generic;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Contract
{
    public class ContractStopDetails : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

        public int ContractStopID { get; set; }
        public int SazeID { get; set; }
        public int Quantity { get; set; }
        public string DisplayStartDate { get; set; }
        public DateTime StartDate { get; set; }
        public string DisplayEndDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NoeEjareID { get; set; }

    }
}
