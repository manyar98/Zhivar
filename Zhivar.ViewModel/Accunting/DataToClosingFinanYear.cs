using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.ViewModel.Accunting
{
    public class DataToClosingFinanYear
    {
        public string closingDate { get; set; }
        public decimal netIncome { get; set; }
        public FinanYearVM newFinanYear { get; set; }
        public List<ShareholderVM> shareholders { get; set; }
    }
}
