using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses;


namespace Zhivar.DataLayer.MapConfiguration.Accounting
{
    public partial class InvoiceItemConfig : BaseEntityTypeConfig<InvoiceItem>
    {
        public InvoiceItemConfig()
        {
            ToTable("InvoiceItems");

            //HasRequired(x => x.Item).WithMany().HasForeignKey(x => x.ItemId);

            MapViewKey(ZhivarResourceIds.Zhivar_Accounting_InvoiceItem_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Accounting_InvoiceItem_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Accounting_InvoiceItem_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Accounting_InvoiceItem_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

