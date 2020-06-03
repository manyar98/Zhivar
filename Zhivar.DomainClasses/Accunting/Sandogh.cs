using System.Collections.Generic;
using Zhivar.DomainClasses.Common;

namespace Zhivar.DomainClasses.Accunting
{
    public class Sandogh : BaseEntity
    {
        public string Onvan { get; set; }

        public string Code { get; set; }

        public int HesabId { get; set; }

        public int OrganId { get; set; }

    }

}