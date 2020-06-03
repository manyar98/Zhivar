using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.DataLayer.MapConfiguration.Accounting
{
    public partial class AccountConfig : BaseEntityTypeConfig<Account>
    {
        public AccountConfig()
        {
            ToTable("Accounts");

            MapViewKey(ZhivarResourceIds.Zhivar_Accounting_Account_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Accounting_Account_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Accounting_Account_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Accounting_Account_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

