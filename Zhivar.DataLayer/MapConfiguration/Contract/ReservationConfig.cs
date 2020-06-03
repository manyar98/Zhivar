using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Contract;

namespace Zhivar.DataLayer.MapConfiguration.Contract
{
    public partial class ReservationConfig : BaseEntityTypeConfig<Reservation>
    {
        public ReservationConfig()
        {
            ToTable("Reservations");

            //HasRequired(x => x.Contact).WithMany().HasForeignKey(x => x.ContactId);
            HasMany(x => x.ReservationDetails).WithOptional().HasForeignKey(x => x.ReservationID);


            MapViewKey(ZhivarResourceIds.Zhivar_Contract_Contracts_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}
