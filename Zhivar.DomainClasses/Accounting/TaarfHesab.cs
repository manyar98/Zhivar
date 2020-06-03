using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.DomainClasses.Accunting
{
    public class TaarfHesab
    {
        public int ID { get; set; }
        public Enums.Jenseat Jenseat { get; set; }
        public string Nam { get; set; }
        public string NamKhanvadegi { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string Mobile { get; set; }
        public virtual NoeHesab NoeHesab { get; set; }
    }
}
