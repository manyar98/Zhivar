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
    public class InvoiceItem : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

        public int InvoiceId { get; set; }
        public string Description { get; set; }
        public decimal Discount { get; set; }
        public string Inv { get; set; }

        public int ItemId { get; set; }
        public string ItemInput { get; set; }
        public decimal Quantity { get; set; }
        public int RowNumber { get; set; }
        public decimal SumInvoiceItem { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public int UnitInvoiceItem { get; set; }
        public decimal UnitPrice { get; set; }
        public bool CalcTax { get; set; }
        // public Item Item { get; set; }
        public decimal? PriceBazareab { get; set; }
        public decimal? PriceChap { get; set; }
        public decimal? PriceNasab { get; set; }
        public decimal? PriceTarah { get; set; }

    }
}
