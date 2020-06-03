using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Accounting;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.BaseInfo;
using Zhivar.ViewModel.Common;
using Zhivar.ViewModel.Security;

namespace Zhivar.ViewModel.Contract
{
    public class RockData
    {
        public List<GoroheSazeVM> listGoroheSaze { get; set; }
        public List<SazeVM> Sazes { get; set; }
        public List<ContactVM> contacts { get; set; }
        public List<TemplateDate> listTemplateDate { get; set; }
    }
}
