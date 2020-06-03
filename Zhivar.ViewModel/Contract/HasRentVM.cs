using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Contract
{
    public class HasRentVM
    {
        public int SazeID { get; set; }
        public string DisplayTarikhShorou { get; set; }
        public int Quantity { get; set; }
        public int NoeEjare { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ContractID { get; set; }
        
    }
}
