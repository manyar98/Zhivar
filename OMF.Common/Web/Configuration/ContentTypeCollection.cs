using System.Configuration;

namespace OMF.Common.Web.Configuration
{
    public class ContentTypeCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new ContentTypeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (object)((ContentTypeElement)element).Key;
        }

        public void Add(ContentTypeElement element)
        {
            this.BaseAdd((ConfigurationElement)element);
        }
    }
}
