using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.DataLayer.MapConfiguration.Accounting
{
    public partial class TransferMoneyConfig : BaseEntityTypeConfig<TransferMoney>
    {
        public TransferMoneyConfig()
        {
            ToTable("TransferMoneys");

            MapViewKey(ZhivarResourceIds.Zhivar_Accounting_TransferMoney_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Accounting_TransferMoney_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Accounting_TransferMoney_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Accounting_TransferMoney_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

