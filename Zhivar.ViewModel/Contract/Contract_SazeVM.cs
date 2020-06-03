using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Contract;

using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.ViewModel.Contract
{
    public class Contract_SazeVM 
    {
        public int? ID { get; set; }
        public int SazeId { get; set; }
        public bool HasBazareab { get; set; }
        public bool HasChap { get; set; }
        public bool HasNasab { get; set; }
        public bool HasTarah { get; set; }
        public DateTime TarikhShorou { get; set; }
        public DateTime TarikhPayan { get; set; }
        public int Roz { get; set; }
        public int Mah { get; set; }
        public int Saal { get; set; }
        public int ContractID { get; set; }
        public string Description { get; set; }
        public decimal Discount { get; set; }
        public string ItemInput { get; set; }
        public decimal Quantity { get; set; }
        public int RowNumber { get; set; }
        public decimal Sum { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public int UnitItem { get; set; }
        public decimal UnitPrice { get; set; }
        public bool CalcTax { get; set; }
        public NoeEjareVM NoeEjare { get; set; }
        public int NoeEjareId { get; set; }
        public decimal? PriceBazareab { get; set; }
        public decimal? PriceChap { get; set; }
        public decimal? PriceNasab { get; set; }
        public decimal? PriceTarah { get; set; }
        public string DisplayTarikhShorou { get; set; }
        public string DisplayTarikhPayan { get; set; }
        public ZhivarEnums.Status Status { get; set; }
        public SazeVM Saze { get; set; }
        public List<Contract_Saze_ChapkhaneVM> Contract_Saze_Chapkhanes { get; set; }
        public List<Contract_Saze_NasabVM> Contract_Saze_Nasabs { get; set; }
        public List<Contract_Saze_BazareabVM> Contarct_Saze_Bazareabs { get; set; }
        public List<Contract_Saze_TarahVM> Contract_Saze_Tarahs { get; set; }
        public List<ContractSazeImagesVM> ContractSazeImages { get; set; }
        public string NoeChap { get; set; }

    }
}
