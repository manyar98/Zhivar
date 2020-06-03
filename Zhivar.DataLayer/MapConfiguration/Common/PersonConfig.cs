using OMF.Common;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;
using Zhivar.DomainClasses;

namespace Zhivar.DataLayer.MapConfiguration.Common
{
    public partial class PersonConfig : BaseEntityTypeConfig<Person>
    {
        public PersonConfig()
        {
            ToTable("People");

            MapViewKey(ZhivarResourceIds.Zhivar_Common_Person_View);
            MapInsertKey(ZhivarResourceIds.Zhivar_Common_Person_Insert);
            MapUpdateKey(ZhivarResourceIds.Zhivar_Common_Person_Update);
            MapDeleteKey(ZhivarResourceIds.Zhivar_Common_Person_Delete);

           // MapEntityValidator(new KhedmatMarkazValidator());

   
        }
    }
}

