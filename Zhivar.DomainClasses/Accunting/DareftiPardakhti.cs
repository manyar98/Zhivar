using System;
using System.Collections.Generic;
using Zhivar.DomainClasses.Common;

namespace Zhivar.DomainClasses.Accunting
{
    public class DareftiPardakhti : BaseEntity
    {
        public int OrganId { get; set; }

        public int Noe { get; set; }

        public int FactorId { get; set; }

        public int SandoghId { get; set; }

        public decimal Mablagh { get; set; }

        public int BankHesabId { get; set; }

        public string ShomareFish { get; set; }

        public string ShomareCheck { get; set; }

        public string NamBank { get; set; }

        public DateTime TarikhCheck { get; set; }

        public string Shob { get; set; }

    }
}