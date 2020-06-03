using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class OpeningBalance
    {
        public TransObj transObj { get; set; }
        public List<TransactionVM> transactions { get; set; }
    }
}
