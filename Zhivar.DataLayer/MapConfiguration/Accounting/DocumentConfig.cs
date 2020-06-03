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
    public partial class DocumentConfig : BaseEntityTypeConfig<Document>
    {
        public DocumentConfig()
        {
            ToTable("Documents");

            HasMany(x => x.Transactions).WithOptional().HasForeignKey(x => x.DocumentId);

            MapViewKey(ZhivarResourceIds.Zhivar_Accounting_Document_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Accounting_Document_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Accounting_Document_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Accounting_Document_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

