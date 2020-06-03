using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class OpeningBalanceStat
    {
        public decimal bank { get; set; }
        public decimal cash { get; set; }
        public decimal creditors { get; set; }
        public decimal debtors { get; set; }
        public string docDate { get; set; }
        public decimal inProgress { get; set; }
        public decimal inventory { get; set; }
        public decimal otherAssets { get; set; }
        public decimal otherLiabilities { get; set; }
        public decimal payables { get; set; }
        public decimal receivables { get; set; }
        public decimal share { get; set; }
        public decimal withdrawals { get; set; }

    }
}
