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

namespace Zhivar.DataLayer.MapConfiguration.Accounting
{
    public partial class TransactionConfig : BaseEntityTypeConfig<Transaction>
    {
        public TransactionConfig()
        {
            ToTable("Transactions");

            MapViewKey(ZhivarResourceIds.Zhivar_Accounting_Transaction_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Accounting_Transaction_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Accounting_Transaction_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Accounting_Transaction_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

