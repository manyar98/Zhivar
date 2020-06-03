using System.Collections.Generic;
using Zhivar.DomainClasses.Common;

namespace Zhivar.DomainClasses.Accunting
{
    public class Hesabha : BaseEntity
    {
        public string Onvan { get; set; }

        public int ParentId { get; set; }

        public string Code { get; set; }

        public int GoroheHesabId { get; set; }

    }
}