using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class LedgerRow
    {
        public string Code { get; set; }
        public decimal Credit { get; set; }
        public string Date{ get; set; }
        public decimal Debit { get; set; }
        public string Description { get; set; }
        public int DocId { get; set; }
        public int DocNum { get; set; }
        public string DtName { get; set; }
        public int InvoiceId { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }
        public decimal Remain { get; set; }
        public string RemainType { get; set; }
    }
}
