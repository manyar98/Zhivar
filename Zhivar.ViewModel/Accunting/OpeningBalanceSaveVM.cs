using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.ViewModel.Accunting
{
    public class OpeningBalanceSaveVM
    {
        public string systemAccount { get; set; }
        public string docDate { get; set; }
        public List<TransactionVM> transactions { get; set; }
        public List<ItemInfo> items { get; set; }
    }
}
