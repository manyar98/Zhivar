using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Common
{
    public class Person:Entity,IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

        public ZhivarEnums.Jenseat? Jenseat { get; set; }
        public ZhivarEnums.NoeShakhs NoeShakhs { get; set; }
        public ZhivarEnums.TypeHoghoghi? TypeHoghoghi { get; set; }
        public string Nam { get; set; }
        public string NamKhanvadegi { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string CodeMeli { get; set; }
        public string NamPedar { get; set; }
        public string CodeEghtesadi { get; set; }
        public string CodePosti { get; set; }
        public string ShomareSabt { get; set; }
        public DateTime? TarikhSoudor { get; set; }
        public string ModateEtebar { get; set; }
        public string SahebEmtiaz { get; set; }
        public bool IsOrgan { get; set; }
        public int? OrganId { get; set; }
        public ZhivarEnums.Vaziat Vaziat { get; set; }
        public virtual ICollection<GoroheSaze> GoroheSazes { get; set; }
    }
}
