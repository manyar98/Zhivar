using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.DataLayer.MapConfiguration.Accounting
{
    public partial class PayRecevieConfig : BaseEntityTypeConfig<PayRecevie>
    {
        public PayRecevieConfig()
        {
            ToTable("PayRecevies");

            HasRequired(x => x.Account).WithMany().HasForeignKey(x => x.AccountId);
            HasRequired(x => x.Contact).WithMany().HasForeignKey(x => x.ContactId);
            HasRequired(x => x.Cost).WithMany().HasForeignKey(x => x.CostId);
            HasRequired(x => x.Document).WithMany().HasForeignKey(x => x.DocumentId);
           // HasRequired(x => x.FinanYear).WithMany().HasForeignKey(x => x.Fin);
            HasRequired(x => x.Invoice).WithMany().HasForeignKey(x => x.InvoiceId);

            MapViewKey(ZhivarResourceIds.Zhivar_Accounting_PayRecevie_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Accounting_PayRecevie_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Accounting_PayRecevie_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Accounting_PayRecevie_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

