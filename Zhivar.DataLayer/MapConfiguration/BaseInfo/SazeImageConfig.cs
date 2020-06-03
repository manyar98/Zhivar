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
    public partial class SazeImageConfig : BaseEntityTypeConfig<SazeImage>
    {
        public SazeImageConfig()
        {
            ToTable("SazeImages");

            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_Saze_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_Saze_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_Saze_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_Saze_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

