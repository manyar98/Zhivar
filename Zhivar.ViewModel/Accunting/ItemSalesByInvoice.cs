using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class ItemSalesByInvoice
    {
        public string Contact { get; set; }
        public int ContactId { get; set; }
        public string Date { get; set; }
        public decimal Discount { get; set; }
        public int Id { get; set; }
        public string Number { get; set; }
        public int Quantity { get; set; }
        public string Reference { get; set; }
        public decimal Sum { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
