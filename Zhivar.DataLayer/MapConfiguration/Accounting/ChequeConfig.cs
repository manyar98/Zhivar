using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.DataLayer.MapConfiguration.Accounting
{
    public partial class ChequeConfig : BaseEntityTypeConfig<Cheque>
    {
        public ChequeConfig()
        {
            ToTable("Cheques");

            MapViewKey(ZhivarResourceIds.Zhivar_Accounting_Cheque_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Accounting_Cheque_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Accounting_Cheque_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Accounting_Cheque_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

