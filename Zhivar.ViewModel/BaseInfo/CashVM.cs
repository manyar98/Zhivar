using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ViewModel.Accounting
{
    public class CashVM 
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string Code { get; set; }
        public int OrganId { get; set; }
        public DetailAccount DetailAccount { get; set; }
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<CashVM, Cash>();

            configuration.CreateMap<Cash, CashVM>();
        }

    }
}