using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.ViewModel.Contract
{
    public class ContractStopDetailsVM
    {
        public int ID { get; set; }
        public int ContractStopID { get; set; }
        public int SazeID { get; set; }
        public int Quantity { get; set; }
        public string DisplayStartDate { get; set; }
        public DateTime StartDate { get; set; }
        public string DisplayEndDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NoeEjareID { get; set; }
        public SazeVM Saze { get; set; }
        public NoeEjareVM NoeEjare { get; set; }
        public int RowNumber { get; set; }
    }
}
