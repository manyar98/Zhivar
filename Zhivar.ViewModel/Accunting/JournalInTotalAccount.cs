using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class JournalInTotalAccountVM
    {
        public int? ID { get; set; }
        public int AccountId { get; set; }
        public string Code { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string DisplayDate { get; set; }
        //public int Id { get; set; }
        public bool IsDebit { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
}
