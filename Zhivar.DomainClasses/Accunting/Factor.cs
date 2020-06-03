using System;
using System.Collections.Generic;
using Zhivar.DomainClasses.Common;

namespace Zhivar.DomainClasses.Accunting
{
    public class Factor : BaseEntity
    {
        public int OrganId { get; set; }

        public int MoshtariId { get; set; }

        public DateTime TarikhSarresid { get; set; }

        public string Code { get; set; }

        public int Erjae { get; set; }

        public string OnvanMoshtari { get; set; }

        public decimal Mablagh { get; set; }

        public decimal Maleat { get; set; }

        public decimal JameKol { get; set; }

        public int Noe { get; set; }

    }
}