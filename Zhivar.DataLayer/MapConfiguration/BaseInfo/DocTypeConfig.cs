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
    public class DocTypeConfig : BaseEntityTypeConfig<DocType>
    {
        public DocTypeConfig()
        {
            ToTable("Doc_Type");


            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_Payvast_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_Payvast_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_Payvast_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_Payvast_Delete);

            // MapEntityValidator(new DocTypeValidator());
        }
    }
}
