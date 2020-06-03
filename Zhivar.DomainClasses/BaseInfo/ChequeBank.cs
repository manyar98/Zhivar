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
    public class ChequeBank : Entity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public int OrganId { get; set; }
        [ForeignKey("Cheque")]
        public int ChequeId { get; set; }
        public Cheque Cheque { get; set; }
        [ForeignKey("Bank")]
        public int BankId { get; set; }
        public Bank Bank { get; set; }
      
    
    }
}
