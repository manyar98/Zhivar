using System.Collections.Generic;
using Zhivar.DomainClasses.Common;

namespace Zhivar.DomainClasses.Accunting
{
    public class Factor_Detail : BaseEntity
    {
        public int FactorId { get; set; }

        public string Tozehat { get; set; }

        public int Tedad { get; set; }

        public decimal Mablagh { get; set; }

        public decimal Takhfif { get; set; }

        public decimal Fi { get; set; }

        public decimal Maleat { get; set; }

    }

}