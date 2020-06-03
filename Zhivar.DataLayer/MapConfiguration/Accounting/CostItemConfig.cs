using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.DataLayer.MapConfiguration.Accounting
{
    public partial class CostItemConfig : BaseEntityTypeConfig<CostItem>
    {
        public CostItemConfig()
        {
            ToTable("CostItems");

           // HasRequired(x => x.Item).WithMany().HasForeignKey(x => x.ItemId);

            MapViewKey(ZhivarResourceIds.Zhivar_Accounting_CostItem_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Accounting_CostItem_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Accounting_CostItem_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Accounting_CostItem_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

