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
    public class GoroheSaze: LoggableEntity , IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public string Title { get; set; }
        [ForeignKey("Person")]
        public int OrganID { get; set; }
      
        public virtual ICollection<Saze> Sazes { get; set; }
       public virtual Person  Person { get; set; }
    }
}
