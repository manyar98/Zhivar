using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Contract;

namespace Zhivar.DataLayer.MapConfiguration.Contract
{
    public partial class Contract_PayReceviesConfig : BaseEntityTypeConfig<Contract_PayRecevies>
    {
        public Contract_PayReceviesConfig()
        {
            ToTable("Contract_PayRecevies");

            HasRequired(x => x.Account).WithMany().HasForeignKey(x => x.AccountId);
            HasRequired(x => x.Contact).WithMany().HasForeignKey(x => x.ContactId);
            HasRequired(x => x.Document).WithMany().HasForeignKey(x => x.DocumentId);
            HasRequired(x => x.Contact).WithMany().HasForeignKey(x => x.ContractId);
            HasMany(x => x.Contract_DetailPayRecevies).WithOptional().HasForeignKey(x => x.Contract_PayRecevieId);

            MapViewKey(ZhivarResourceIds.Zhivar_Contract_Contracts_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Delete);


        }
    }
}
