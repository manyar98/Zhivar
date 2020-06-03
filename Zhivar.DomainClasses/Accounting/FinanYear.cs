using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Accounting
{
    public class FinanYear: LoggableEntity , IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public int OrganId { get; set; }
        public bool Closed { get; set; }
        public string DisplayEndDate { get; set; }
        public string DisplayStartDate { get; set; }
        public bool FirstYear { get; set; }
        public string Name { get; set; }

    }
}
