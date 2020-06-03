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
    public partial class NoeChapConfig : BaseEntityTypeConfig<NoeChap>
    {
        public NoeChapConfig()
        {
            ToTable("NoeChaps");

            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_NoeChap_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_NoeChap_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_NoeChap_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_NoeChap_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

