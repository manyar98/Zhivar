using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.ViewModel.Accunting
{
    public class NewObjectKala
    {
        public ItemVM item { get; set; }
        public bool? showItemUnit { get; set; }
        public List<ItemUnit> itemUnits { get; set; }
    }
}
