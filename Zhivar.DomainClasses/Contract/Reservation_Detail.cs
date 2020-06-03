using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Contract
{
    public class Reservation_Detail : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

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
        public int RowNumber { get; set; }
    }
}
