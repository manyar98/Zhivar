using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses;


namespace Zhivar.DataLayer.MapConfiguration.BaseInfo
{
    public partial class CashConfig : BaseEntityTypeConfig<Cash>
    {
        public CashConfig()
        {
            ToTable("Cashes");

            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_Cash_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_Cash_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_Cash_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_Cash_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

