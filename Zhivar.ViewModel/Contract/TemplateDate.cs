using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Contract
{
    public class TemplateDate
    {
        public int ID { get; set; }
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Date { get; set; }
        public Dictionary<string,string> Styles { get; set; }
    }
}
