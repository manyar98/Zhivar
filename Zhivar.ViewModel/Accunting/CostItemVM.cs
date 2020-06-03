using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.ViewModel.Accunting
{
    public class CostItemVM 
    {
        public int? ID { get; set; }
        public int CostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int RowNumber { get; set; }
        public decimal Sum { get; set; }
        public decimal Rest { get; set; }
        public int ItemId { get; set; }
        public virtual Account Item { get; set; }


    }
}
