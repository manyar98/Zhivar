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
    public partial class InvoiceConfig : BaseEntityTypeConfig<Invoice>
    {
        public InvoiceConfig()
        {
            ToTable("Invoices");

            //HasRequired(x => x.Contact).WithMany().HasForeignKey(x => x.ContactId);

            HasMany(x => x.InvoiceItems).WithOptional().HasForeignKey(x => x.InvoiceId);

            MapViewKey(ZhivarResourceIds.Zhivar_Accounting_Invoice_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Accounting_Invoice_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Accounting_Invoice_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Accounting_Invoice_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

