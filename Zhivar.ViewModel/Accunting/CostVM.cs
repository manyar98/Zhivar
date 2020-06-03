using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Common;

namespace Zhivar.ViewModel.Accunting
{
    public class CostVM 
    {
        public int ID { get; set; }
        public int OrganId { get; set; }
        public int ContactId { get; set; }
        public string ContactTitle { get; set; }
        public DateTime DateTime { get; set; }
        public string DisplayDate { get; set; }
        public List<CostItemVM> CostItems { get; set; }
        public string Explain { get; set; }
        public string Number { get; set; }
        public decimal Paid { get; set; }
        public decimal Payable { get; set; }
        public decimal Rest { get; set; }
        public ZhivarEnums.CostStatus Status { get; set; }

        public decimal Sum { get; set; }
        public ContactVM Contact { get; set; }
        public int? DocumentId { get; set; }
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<CostVM, Cost>();//.ForMember(x => x.Contact, opt => opt.MapFrom(src => src.Contact));

            configuration.CreateMap<Cost, CostVM>();//.ForMember(x => x.Contact, opt => opt.MapFrom(src => src.Contact));

   

        }

       
    }
}