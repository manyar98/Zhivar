using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;
using Zhivar.DomainClasses;

namespace Zhivar.DataLayer.MapConfiguration.Common
{
    public partial class BussinessConfig : BaseEntityTypeConfig<Bussiness>
    {
        public BussinessConfig()
        {
            ToTable("Bussinesses");

            MapViewKey(ZhivarResourceIds.Zhivar_Common_Bussiness_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Common_Bussiness_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Common_Bussiness_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Common_Bussiness_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

