using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses;

namespace Zhivar.DataLayer.MapConfiguration.Contract
{
    public partial class Contract_Saze_NasabConfig : BaseEntityTypeConfig<DomainClasses.Contract.Contract_Saze_Nasab>
    {
        public Contract_Saze_NasabConfig()
        {
            ToTable("Contract_Saze_Nasab");


            MapViewKey(ZhivarResourceIds.Zhivar_Contract_Contracts_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

