using OMF.Common;
using System;
using System.Collections.Generic;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Contract
{
    public class Contract_Saze : LoggableEntity,IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

        public int SazeId { get; set; }
        public bool HasBazareab { get; set; }
        public bool HasChap { get; set; }
        public bool HasNasab { get; set; }
        public bool HasTarah { get; set; }
        public decimal? PriceBazareab { get; set; }
        public decimal? PriceChap { get; set; }
        public decimal? PriceNasab { get; set; }
        public decimal? PriceTarah { get; set; }
        public DateTime TarikhShorou { get; set; }
        public DateTime TarikhPayan { get; set; }
        public string DisplayTarikhShorou { get; set; }
        public string DisplayTarikhPayan { get; set; }
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
        public int NoeEjareId { get; set; }
        public ZhivarEnums.Status Status { get; set; }
        public List<Contract_Saze_Chapkhane> Contract_Saze_Chapkhanes { get; set; }
        public List<Contract_Saze_Nasab> Contract_Saze_Nasabs { get; set; }
        public List<Contract_Saze_Bazareab> Contarct_Saze_Bazareabs { get; set; }
        public List<Contract_Saze_Tarah> Contract_Saze_Tarahs { get; set; }
        public List<ContractSazeImages> ContractSazeImages { get; set; }

    }
}
