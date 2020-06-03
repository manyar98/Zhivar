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

        public partial class ItemGroupConfig : BaseEntityTypeConfig<ItemGroup>
        {
            public ItemGroupConfig()
            {

            ToTable("ItemGroups");

            HasMany(x => x.Items).WithOptional().HasForeignKey(x => x.ItemGroupId);

            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_ItemGroup_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_ItemGroup_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_ItemGroup_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_ItemGroup_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

