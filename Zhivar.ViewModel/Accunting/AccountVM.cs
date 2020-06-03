using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;

namespace Zhivar.ViewModel.Accunting
{
    public class AccountVM
    {
        public int ID { get; set; }
        public decimal Balance { get; set; }
        public int BalanceType { get; set; }
        public string Code { get; set; }
        public string Coding { get; set; }
        public string GroupCode { get; set; }
        //public int Id { get; set; }
        public ZhivarEnums.AccountType Level { get; set; }
        public string LevelString { get; set; }
        public string Name { get; set; }
        public string ParentCoding { get; set; }
        public int SystemAccount { get; set; }
        public string SystemAccountName { get; set; }
        public decimal credit { get; set; }
        public decimal debit { get; set; }
        public int ParentId { get; set; }
        public int OrganId { get; set; }
        public string ComplteCoding { get; set; }
    }
}
