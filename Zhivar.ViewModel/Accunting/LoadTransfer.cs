using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class LoadTransfer
    {
        public decimal Amount { get; set; }
        public DateTime? Date { get; set; }
        public string DisplayDate { get; set; }
        public string Description { get; set; }
        public int DocumentId { get; set; }
        public int DocumentNumber { get; set; }
        public string From { get; set; }
        public int FromDetailAccountId{ get; set; }
        public string FromDetailAccountName { get; set; }
        public string FromReference { get; set; }
        public string To{ get; set; }
        public int ToDetailAccountId { get; set; }
        public string ToDetailAccountName{ get; set; }
        public string ToReference{ get; set; }
    }
}
