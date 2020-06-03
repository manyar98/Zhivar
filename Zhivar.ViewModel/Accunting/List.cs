using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class List
    {
        public decimal Amount { get; set; }
        public string DateTime { get; set; }
        public int DocId { get; set; }
        public decimal In { get; set; }
        public int InvoiceId { get; set; }
        public int Number { get; set; }
        public decimal Out { get; set; }
        public decimal Remain { get; set; }
        public string Text { get; set; }
    }
}
