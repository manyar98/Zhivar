using OMF.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.BaseInfo
{
    public class Item: LoggableEntity,IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public string Code { get; set; }
        public int OrganIdItem { get; set; }
        //[ForeignKey("ItemGroup")]
        public int ItemGroupId { get; set; }
        public string Barcode { get; set; }
        public decimal BuyPrice { get; set; }
        //public DetailAccount DetailAccount { get; set; }
        public ZhivarEnums.NoeItem ItemType { get; set; }
        public int MoneyStock { get; set; }
        public string Name { get; set; }
        public string PurchasesTitle { get; set; }
        public string SalesTitle { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Stock { get; set; }
        public int? UnitID { get; set; }
        public bool? IsGoods { get; set; }
        public bool? IsService { get; set; }
        public bool? IsSaze { get; set; }
        //  public virtual ItemGroup ItemGroup { get; set; }



    }
}
