using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class BalanceSheet
    {
        public List<SummeryAccount> assets { get; set; }
        public decimal assetsSum { get; set; }
        public string balanceSheetDate { get; set; }
        public decimal capitalSum { get; set; }
        public decimal liabilitiesSum { get; set; }
        public decimal sumLeft { get; set; }
        public decimal sumRight { get; set; }
        public List<SummeryAccount> capitals { get; set; }
        public List<SummeryAccount> liabilities { get; set; }

    }
}
