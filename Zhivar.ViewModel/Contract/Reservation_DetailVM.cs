using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.ViewModel.Contract
{
    public class Reservation_DetailVM
    {
        public int ID { get; set; }
        public int ReservationID { get; set; }
        public int SazeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartDisplayDate { get; set; }
        public string EndDisplayDate { get; set; }
        public int NoeEjareID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? PriceBazareab { get; set; }
        public decimal? PriceTarah { get; set; }
        public decimal? PriceChap { get; set; }
        public decimal? PriceNasab { get; set; }
        public SazeVM Saze { get; set; }
        public NoeEjareVM NoeEjare { get; set; }
        public int RowNumber { get; set; }
    }
}
