using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.DataLayer.MapConfiguration.BaseInfo
{
    public class PersonelConfig : BaseEntityTypeConfig<Personel>
    {
        public PersonelConfig()
        {
            ToTable("Personel");


            // MapEntityValidator(new PersonelValidator());

            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_Personel_Delete);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_Personel_Update);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_Personel_Insert);
            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_Personel_View);
        }
    }
}
