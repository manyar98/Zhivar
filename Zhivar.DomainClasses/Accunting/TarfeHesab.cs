using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;

namespace Zhivar.DomainClasses.Accunting
{
    public class TarfeHesab : BaseEntity
    {
       
        public decimal HadeEtebar { get; set; }
        public decimal DarsadTakhfif { get; set; }
        //public int PersonID { get; set; }

        //[ForeignKey("PersonID")]
        public virtual Person Person { get; set; }
        public int OrganID { get; set; }
       // public int? BankID { get; set; }

       // [ForeignKey("BankID")]
        public virtual Bank Bank { get; set; }
    }
}
