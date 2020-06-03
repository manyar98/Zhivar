using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ViewModel.BaseInfo
{
    public class ItemVM 
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public int OrganId { get; set; }
        public int ItemGroupId { get; set; }
        public string Barcode { get; set; }
        public decimal BuyPrice { get; set; }
        public ZhivarEnums.NoeItem ItemType { get; set; }
        public int MoneyStock { get; set; }
        public string Name { get; set; }
        public string PurchasesTitle { get; set; }
        public string SalesTitle { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Stock { get; set; }
        public int? UnitID { get; set; }
        public bool? IsGoods { get; set; }
        public bool? IsService { get; set; }
        public string GroupName { get; set; }
        public DetailAccount DetailAccount { get; set; }
        public bool Next { get; set; }
        public ItemUnit Unit { get; set; }
        public List<ItemUnit> itemUnits { get; set; }
        
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<ItemVM, Item>();
            configuration.CreateMap<Item, ItemVM>();
        }


    }
}
