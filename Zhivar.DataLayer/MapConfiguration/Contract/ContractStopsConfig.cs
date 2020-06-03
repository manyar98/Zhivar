using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Contract;

namespace Zhivar.DataLayer.MapConfiguration.Contract
{
    public partial class ContractStopsConfig : BaseEntityTypeConfig<ContractStops>
    {
        public ContractStopsConfig()
        {
            ToTable("ContractStops");

            HasMany(x => x.ContractStopDetails).WithOptional().HasForeignKey(x => x.ContractStopID);


            MapViewKey(ZhivarResourceIds.Zhivar_Contract_Contracts_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}
