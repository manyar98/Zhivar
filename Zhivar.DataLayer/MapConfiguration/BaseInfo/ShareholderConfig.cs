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
    public partial class ShareholderConfig : BaseEntityTypeConfig<Shareholder>
    {
        public ShareholderConfig()
        {
            ToTable("Shareholders");

            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_Shareholder_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_Shareholder_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_Shareholder_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_Shareholder_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

