using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;

namespace Zhivar.ViewModel.Accunting
{
    public class DocumentVM
    {

        public int ID { get; set; }
        public decimal Credit { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Debit{ get; set; }
        public string Description{ get; set; }
        public string DisplayDate{ get; set; }
        public int Id{ get; set; }
        public bool IsManual{ get; set; }
        public int Number{ get; set; }
        public int Number2{ get; set; }
        public ZhivarEnums.DocumentStatus Status{ get; set; }
        public string StatusString{ get; set; }
        public List<TransactionVM> Transactions{ get; set; }
        public bool? IsFirsDocument { get; set; }
        public int OrganId { get; set; }
        public FinanYearVM FinanYear { get; set; }
        public int FinanYearId { get; set; }

        public ZhivarEnums.NoeDoc Type { get; set; }
    }
}
