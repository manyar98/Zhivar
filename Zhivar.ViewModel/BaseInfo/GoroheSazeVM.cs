using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.BaseInfo
{
    public class GoroheSazeVM 
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int OrganID { get; set; }
        public List<SazeVM> Items { get; set; }
        public double Occupy { get; set; }
        public decimal CostRentTo { get; set; }
        public decimal CostRentFrom { get; set; }
        public int CountOfRent { get; set; }
        public bool ShowCostFrom { get; set; }
    }
}
