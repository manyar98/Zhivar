using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class FinanYearVM
    {
        public int ID { get; set; }
        public bool Closed { get; set; }
        public string DisplayEndDate { get; set; }
        public string DisplayStartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int EndDateDay { get; set; }
        public int EndDateMonth { get; set; }
        public int EndDateYear { get; set; }
        public bool FirstYear { get; set; }
        public int Id { get; set; }
        public bool IsMoreThanOneYear { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public DateTime StartDate { get; set; }
        public int StartDateDay { get; set; }
        public int StartDateMonth { get; set; }
        public int StartDateYear { get; set; }

        public int OrganId { get; set; }
    }
}
