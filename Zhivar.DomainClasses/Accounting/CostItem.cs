using OMF.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Accounting
{
    public class CostItem : LoggableEntity , IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
       // [ForeignKey("Cost")]
        public int CostId { get; set; }
       // public string Title { get; set; }
       // [ForeignKey("Item")]
        public int ItemId { get; set; }
        public string Description { get; set; }
        public int RowNumber { get; set; }
        public decimal Sum { get; set; }
        public decimal Rest { get; set; }
        //public Account Item { get; set; }
      //  public virtual Cost Cost { get; set; }
    }
}
