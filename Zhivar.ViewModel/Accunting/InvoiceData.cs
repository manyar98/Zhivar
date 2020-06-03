using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.BaseInfo;
using Zhivar.ViewModel.Common;

namespace Zhivar.ViewModel.Accunting
{
    public class InvoiceData
    {
        public InvoiceVM invoice { get; set; }
        public List<ContactVM> contacts { get; set; }
        public List<ItemVM> items { get; set; }
        public InvoiceSettings invoiceSettings { get; set; }
    }
}
