using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ViewModel.BaseInfo
{
    public class ItemGroupVM 
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int OrganID { get; set; }
        public bool? IsGroupSaze { get; set; }
        public List<ItemVM> Items { get; set; }
        
    }
}
