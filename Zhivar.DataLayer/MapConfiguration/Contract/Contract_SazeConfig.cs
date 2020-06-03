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
    public partial class Contract_SazeConfig : BaseEntityTypeConfig<DomainClasses.Contract.Contract_Saze>
    {
        public Contract_SazeConfig()
        {
            ToTable("Contract_Saze");
            HasMany(x => x.Contarct_Saze_Bazareabs).WithOptional().HasForeignKey(x => x.ContarctSazeID);
            HasMany(x => x.Contract_Saze_Tarahs).WithOptional().HasForeignKey(x => x.ContarctSazeID);
            HasMany(x => x.Contract_Saze_Chapkhanes).WithOptional().HasForeignKey(x => x.ContarctSazeID);
            HasMany(x => x.Contract_Saze_Nasabs).WithOptional().HasForeignKey(x => x.ContarctSazeID);
            HasMany(x => x.ContractSazeImages).WithOptional().HasForeignKey(x => x.ContractSazeId);

            MapViewKey(ZhivarResourceIds.Zhivar_Contract_Contracts_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Contract_Contracts_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

