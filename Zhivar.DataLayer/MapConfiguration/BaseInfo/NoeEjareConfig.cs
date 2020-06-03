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
    public partial class NoeEjareConfig : BaseEntityTypeConfig<NoeEjare>
    {
        public NoeEjareConfig()
        {
            ToTable("NoeEjares");

            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_NoeEjare_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_NoeEjare_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_NoeEjare_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_NoeEjare_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

