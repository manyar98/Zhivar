using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Contract
{
    public class ReservationTransObj
    {
        public ReservationVM Reservation { get; set; }
        public List<Reservation_DetailVM> ReservationDetails { get; set; }

    }
}
