using OMF.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.BaseInfo
{
    public class Saze: LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

        public string Title { get; set; }
        public string Address { get; set; }
        public decimal Tol { get; set; }
        public decimal Arz { get; set; }
        public int GoroheSazeID { get; set; }
        public int NoeSazeId { get; set; }
        public int NoeEjareID { get; set; }
        public int OrganId { get; set; }
        public string Code { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public bool? NoorDard { get; set; }
        public bool Status { get; set; }
        public List<SazeImage> Images { get; set; }
    }
}
