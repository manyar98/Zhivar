using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Common;

namespace Zhivar.ViewModel.Contract
{
    public class ReservationVM
    {
        public int ID { get; set; }
        public int OrganID { get; set; }
        public int ContactID { get; set; }
        public DateTime RegisterDate { get; set; }
        public string DisplayRegisterDate { get; set; }
        public ContactVM Contact { get; set; }
        public List<Reservation_DetailVM> ReservationDetails { get; set; }
        public Contact ContactCommon { get; set; }
        public int ValiditDuration { get; set; }
        public string ContactTitle { get; set; }
        public string Number { get; set; }
        public int Color { get; set; }
        
            
    }
}
