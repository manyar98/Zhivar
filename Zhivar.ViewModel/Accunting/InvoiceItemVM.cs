using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.ViewModel.Accunting
{
    public class InvoiceItemVM 
    {
        public int ID { get; set; }
        public int InvoiceId { get; set; }
        public string Description { get; set; }
        public decimal Discount { get; set; }
        public string Inv { get; set; }
        public int ItemId { get; set; }
        public string ItemInput { get; set; }
        public decimal Quantity { get; set; }
        public int RowNumber { get; set; }
        public decimal Sum { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public int UnitInvoiceItem { get; set; }
        public decimal UnitPrice { get; set; }
        public bool CalcTax { get; set; }
        // public Item Item { get; set; }
        public decimal? PriceBazareab { get; set; }
        public decimal? PriceChap { get; set; }
        public decimal? PriceNasab { get; set; }
        public decimal? PriceTarah { get; set; }
        public int Unit { get; set; }
        public ItemVM Item { get; set; }
        //public Invoice Invoice { get; set; }
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<InvoiceItemVM, InvoiceItem>();

            configuration.CreateMap<InvoiceItem, InvoiceItemVM>();

            Mapper.CreateMap<InvoiceItemVM, InvoiceItem>()
    .ForMember(dest => dest.SumInvoiceItem, opt => opt.MapFrom(src => src.Sum));

            Mapper.CreateMap<InvoiceItem, InvoiceItemVM>()
.ForMember(dest => dest.Sum, opt => opt.MapFrom(src => src.SumInvoiceItem));
        }

    }
}
