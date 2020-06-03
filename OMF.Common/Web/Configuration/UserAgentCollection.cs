using System;
using System.Configuration;

namespace OMF.Common.Web.Configuration
{
    public class UserAgentCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new UserAgentElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (object)((UserAgentElement)element).Name;
        }

        public void Add(UserAgentElement element)
        {
            this.BaseAdd((ConfigurationElement)element);
        }

        [ConfigurationProperty("required")]
        public bool Required
        {
            get
            {
                return Convert.ToBoolean(this["required"]);
            }
            set
            {
                this["required"] = (object)value;
            }
        }
    }
}
