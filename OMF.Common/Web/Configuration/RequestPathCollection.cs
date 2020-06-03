using System.Configuration;

namespace OMF.Common.Web.Configuration
{
    public class RequestPathCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new RequestPathElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (object)((RequestPathElement)element).Key;
        }

        public void Add(RequestPathElement element)
        {
            this.BaseAdd((ConfigurationElement)element);
        }
    }
}
