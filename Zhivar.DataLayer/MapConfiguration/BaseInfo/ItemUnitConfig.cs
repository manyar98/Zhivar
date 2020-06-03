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
    public partial class ItemUnitConfig : BaseEntityTypeConfig<ItemUnit>
    {
        public ItemUnitConfig()
        {
            ToTable("ItemUnits");

            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_Item_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_Item_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_Item_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_Item_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

