using System.Collections.Generic;
using Zhivar.DomainClasses.Common;

namespace Zhivar.DomainClasses.Accunting
{
    public class KalaKhadmat : BaseEntity
    {
        public int GoroheKalaID { get; set; }

        public int Noe { get; set; }

        public string Code { get; set; }

        public string Barcode { get; set; }

        public string Onvan { get; set; }

        public decimal GhematKharid { get; set; }

        public string OnvanKharid { get; set; }

        public decimal GhematFrosh { get; set; }

        public string OnvanFrosh { get; set; }

    }
 
}