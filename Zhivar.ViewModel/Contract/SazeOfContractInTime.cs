using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;

namespace Zhivar.ViewModel.Contract
{
    public class SazeOfContractInTime
    {
        public int ID { get; set; }
        public int SazeID { get; set; }
        public int GoroheSazeID { get; set; }
        public int? ContractID { get; set; }
        public int? ReservationID { get; set; }
        public int? ReservationDetailID { get; set; }
        public double Amount { get; set; }
        public double Distance { get; set; }
        public ZhivarEnums.ContractType Type { get; set; }
        public bool CheckedIn { get; set; }
        public bool CheckedOut { get; set; }
        public bool Reservation { get; set; }
        public Dictionary<string, string> Styles { get; set; }
        public string Title { get; set; }
        public int ContactID { get; set; }
        public string ContactTitle { get; set; }
        public string ContractTitle { get; set; }
        public string ContractNumber { get; set; }
        public string ContractAmount { get; set; }
        public string SazeAmount { get; set; }
        public string UnitPrice { get; set; }
        public string StartDisplayDate { get; set; }
        public string EndDisplayDate { get; set; }
        public double Occupy { get; set; }
        public decimal CostRentTo { get; set; }
        public decimal CostRentFrom { get; set; }

    }
}
