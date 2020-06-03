using System.Collections.Generic;
using Zhivar.DomainClasses.Common;

namespace Zhivar.DomainClasses.Accunting
{
    public class BankHesab : BaseEntity
    {
        public string Code { get; set; }

        public string Onvan { get; set; }

        public string Shomare { get; set; }

        public string Shobe { get; set; }

        public int OrganId { get; set; }

        public int HesabId { get; set; }

    }
}