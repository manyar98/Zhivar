using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses;

namespace Zhivar.DataLayer.MapConfiguration.Contract
{
    public partial class ContractConfig : BaseEntityTypeConfig<DomainClasses.Contract.Contract>
    {
        public ContractConfig()
        {
            ToTable("Contracts");

            //HasRequired(x => x.Contact).WithMany().HasForeignKey(x => x.ContactId);

            HasMany(x => x.Contract_Sazes).WithOptional().HasForeignKey(x => x.ContractID);
            HasMany(x => x.Contract_PayRecevies).WithOptional().HasForeignKey(x => x.ContractId);

            MapViewKey(ZhivarResourceIds.Zhivar_Contract_Contracts_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

