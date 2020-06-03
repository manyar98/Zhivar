using System;

namespace OMF.Common.Attributes
{
    public class PersianTitleAttribute : Attribute
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
