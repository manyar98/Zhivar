using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.DomainClasses.Attributes
{
    public class PersianTitleAttribute:Attribute
    {
        private string title;

        public PersianTitleAttribute(string title)
        {
            this.title = title;
        }

        public string Title
        {
            get
            {
                return this.title;
            }
        }

    }
}
