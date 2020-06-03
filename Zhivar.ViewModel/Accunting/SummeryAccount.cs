using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class SummeryAccount
    {
        public decimal Amount { get; set; }
        public int? Class { get; set; }
        public string Date{ get; set; }
        public string Description { get; set; }
        public int? GroupCode { get; set; }
        public string Type { get; set; }
    }
}
