using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ViewModel.BaseInfo
{
    public class ItemCard
    {
        public List<Chart> chart { get; set; }
        public List<InvoiceItemVM> invoiceItems { get; set; }
        public ItemVM item { get; set; }
        public List<List> list { get; set; }

    }
}
