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
    public partial class VahedTolConfig : BaseEntityTypeConfig<VahedTol>
    {
        public VahedTolConfig()
        {
            ToTable("VahedTols");

            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_VahedTol_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_VahedTol_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_VahedTol_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_VahedTol_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

