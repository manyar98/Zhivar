using System.Configuration;

namespace OMF.Common.Web.Configuration
{
    internal class ExcludeImageCachingCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new ExcludeImageCachingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            ExcludeImageCachingElement imageCachingElement = element as ExcludeImageCachingElement;
            if (imageCachingElement != null)
                return (object)imageCachingElement.RelativeFileName;
            return (object)null;
        }
    }
}
