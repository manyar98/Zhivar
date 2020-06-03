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
    public partial class BaseColorConfig : BaseEntityTypeConfig<BaseColor>
    {
        public BaseColorConfig()
        {
            ToTable("BaseColor");

            MapViewKey(ZhivarResourceIds.Zhivar_Common_Contact_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Common_Contact_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Common_Contact_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Common_Contact_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

