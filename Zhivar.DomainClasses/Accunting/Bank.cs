using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;

namespace Zhivar.DomainClasses.Accunting
{
    public class Bank:BaseEntity
    {
        public string Nam { get; set; }
        public string ShomareHesab { get; set; }
        public string ShomareShobe { get; set; }
        public string CodeShobe { get; set; }
        //public virtual TarfeHesab TarfeHesab { get; set; }
    }
}
