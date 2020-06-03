using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Accunting
{
    public class Node
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FamilyTree { get; set; }
        public Node Parent { get; set; }
        public string Parents { get; set; }
        public int SystemAccount { get; set; }

    }
}
