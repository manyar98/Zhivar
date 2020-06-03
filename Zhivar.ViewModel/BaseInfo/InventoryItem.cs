using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.BaseInfo
{
    public class InventoryItem
    {
        public decimal BuyPrice { get; set; }
        public string Code { get; set; }
        public int Id { get; set; }
        public decimal MoneyStock { get; set; }
        public string Name { get; set; }
        public string NodeName { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Stock { get; set; }
        public int? Unit { get; set; }
        public decimal Wap { get; set; }
    }
}
