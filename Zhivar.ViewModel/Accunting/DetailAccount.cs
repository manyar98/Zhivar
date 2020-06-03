using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class DetailAccount
    {
        public string Code { get; set; }
        public int Id { get; set; }
        public Node Node { get; set; }
        public List<AccountVM> Accounts { get; set; }
        public decimal Balance { get; set; }
        public int BalanceType { get; set; }
        public string Name { get; set; }
        public string RelatedAccounts { get; set; }
        public decimal credit { get; set; }
        public decimal debit { get; set; }


    }
}
