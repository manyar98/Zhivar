using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ViewModel.Accounting;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ViewModel.BaseInfo
{
    public class Cashe
    {
        public List<BankVM> banks { get; set; }
        public List<CashVM> cashes { get; set; }
    }
}
