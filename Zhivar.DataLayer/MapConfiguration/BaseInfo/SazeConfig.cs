using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses;
using Zhivar.DataLayer.Validation;
using Zhivar.DataLayer.Validation.BaseInfo;

namespace Zhivar.DataLayer.MapConfiguration.BaseInfo
{
    public partial class SazeConfig : BaseEntityTypeConfig<Saze>
    {
        public SazeConfig()
        {
            ToTable("Sazes");

            HasMany(x => x.Images).WithOptional().HasForeignKey(x => x.SazeId);

            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_Saze_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_Saze_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_Saze_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_Saze_Delete);

             //MapEntityValidator(new SazeValidator());


        }
    }
}

