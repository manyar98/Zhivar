using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class CapitalStatement
    {
        public string reportDate { get; set; }
        public List<SummeryAccount> tableCapitals { get; set; }
    }
}
