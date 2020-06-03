using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class DocumentData
    {
        public List<AccountVM> accounts { get; set; }
        public List<DefaultDescriptions> defaultDescriptions { get; set; }
        public List<DetailAccount> detailAccounts { get; set; }
        public DocumentVM document { get; set; }

    }
}
