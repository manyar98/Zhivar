using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.ViewModel.Accounting;

namespace Zhivar.ViewModel.Accunting
{
    public class InvoiceTransObj
    {
        public InvoiceVM invoice { get; set; }
        public List<InvoiceItemVM> invoiceItems { get; set; }
        public List<CashVM> cashes { get; set; }
        public List<BankVM> banks { get; set; }
        public List<TransactionVM> payments { get; set; }
        public List<DetailPayRecevieVM> tempPayments { get; set; }
        
    }
}
