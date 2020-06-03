using System.Collections.Generic;
using Zhivar.DomainClasses.Common;

namespace Zhivar.DomainClasses.Accunting
{
    public class Sahamdaran : BaseEntity
    {
        public int ShakhsId { get; set; }

        public int OrganId { get; set; }

        public decimal Sahm { get; set; }

    }

}