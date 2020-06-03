using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ViewModel.BaseInfo
{
    public class ChequesAndStatsVM
    {
        public string change { get; set; }
        public int chequeId { get; set; }
        public string date { get; set; }
        public DetailAccount detailAccount { get; set; }
        public string description { get; set; }
        public string receiveType { get; set; }
        public string reference { get; set; }

    }
}
