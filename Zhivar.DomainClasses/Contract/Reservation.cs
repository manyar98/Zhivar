using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Contract
{
    public class Reservation : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

        public int OrganID { get; set; }
        public int ContactID { get; set; }
        public DateTime RegisterDate { get; set; }
        public string DisplayRegisterDate { get; set; }
        public int ValiditDuration { get; set; }
        
        public List<Reservation_Detail> ReservationDetails { get; set; }
    
    }
}
