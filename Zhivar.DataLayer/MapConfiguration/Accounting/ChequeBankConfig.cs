using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses;


namespace Zhivar.DataLayer.MapConfiguration.Accounting
{
    public partial class ChequeBankConfig : BaseEntityTypeConfig<ChequeBank>
    {
        public ChequeBankConfig()
        {
            ToTable("ChequeBanks");

            MapViewKey(ZhivarResourceIds.Zhivar_Accounting_ChequeBank_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Accounting_ChequeBank_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Accounting_ChequeBank_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Accounting_ChequeBank_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

