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
    public partial class NoeSazeConfig : BaseEntityTypeConfig<NoeSaze>
    {
        public NoeSazeConfig()
        {
            ToTable("NoeSazes");

            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_NoeSaze_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_NoeSaze_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_NoeSaze_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_NoeSaze_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

