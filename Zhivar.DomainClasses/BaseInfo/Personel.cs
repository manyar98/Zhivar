using OMF.Common;
using OMF.Security.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.BaseInfo
{
    public class Personel : LoggableEntityName, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update;
        public DateTime AzTarikh { get; set; }
        public DateTime? TaTarikh { get; set; }
        public bool DarHalKhedmat { get; set; }
        public int OrganizationId { get; set; }
        public int RoleID { get; set; }
        public int UserID { get; set; }
        public ZhivarEnums.NoeMozd NoeMozd { get; set; }
        public decimal Darsd { get; set; }
        public decimal Salary { get; set; }
        
        public UserInfo User { get; set; }
    }
}