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
    public partial class CostConfig : BaseEntityTypeConfig<Cost>
    {
        public CostConfig()
        {
            ToTable("Costs");

            //HasRequired(x => x.Contact).WithMany().HasForeignKey(x => x.ContactId);

            HasMany(x => x.CostItems).WithOptional().HasForeignKey(x => x.CostId);

            MapViewKey(ZhivarResourceIds.Zhivar_Accounting_Cost_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Accounting_Cost_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Accounting_Cost_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Accounting_Cost_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

