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
    public class MadarekPayvastConfig : BaseEntityTypeConfig<MadarekPayvast>
    {
        public MadarekPayvastConfig()
        {
            ToTable("Madarek_Payvast");

            Ignore(tasvir => tasvir.TasvirBlobBase64);

            //HasRequired(Doc => Doc.TasvirBlob).WithRequiredPrincipal();

            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_Payvast_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_Payvast_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_Payvast_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_Payvast_Delete);

            // MapEntityValidator(new DocTypeValidator());
        }

     
    }

    //class TasvirBlobConfig : BaseEntityTypeConfig<TasvirBlob>
    //{
    //    public TasvirBlobConfig()
    //    {
    //        ToTable("Madarek_Payvast");
    //        Property(Doc => Doc.Blob).HasColumnName("BLOB").HasColumnType("BLOB");
    //    }
    //}
}
