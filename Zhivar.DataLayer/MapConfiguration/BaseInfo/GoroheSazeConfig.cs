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
    public partial class GoroheSazeConfig : BaseEntityTypeConfig<GoroheSaze>
    {
        public GoroheSazeConfig()
        {
            ToTable("GoroheSazes");

            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_GoroheSaze_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_GoroheSaze_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_GoroheSaze_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_GoroheSaze_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

