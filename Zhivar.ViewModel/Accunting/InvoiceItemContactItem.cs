using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;

namespace Zhivar.ViewModel.Accunting
{
    public class InvoiceItemContactItem
    {
        public string Code { get; set; }
        public string DateTime { get; set; }
        public decimal Discount { get; set; }
        public int DocId { get; set; }
        public int InvoiceId { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public decimal Quantity { get; set; }
        public string Reference { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public ZhivarEnums.NoeFactor Type { get; set; }
        public int Unit { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
