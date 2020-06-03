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
    public partial class BankConfig : BaseEntityTypeConfig<Bank>
    {
        public BankConfig()
        {
            ToTable("Banks");

            MapViewKey(ZhivarResourceIds.Zhivar_BaseInfo_Bank_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_BaseInfo_Bank_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_BaseInfo_Bank_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_BaseInfo_Bank_Delete);

            // MapEntityValidator(new KhedmatMarkazValidator());


        }
    }
}

