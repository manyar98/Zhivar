using System;
using System.Configuration;

namespace OMF.Common.Web.Configuration
{
    internal class ImageCachingCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new ImageCachingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            ImageCachingElement imageCachingElement = element as ImageCachingElement;
            if (imageCachingElement != null)
                return (object)imageCachingElement.Extension;
            return (object)null;
        }

        [ConfigurationProperty("cachingTime", IsRequired = true)]
        public TimeSpan CachingTime
        {
            get
            {
                TimeSpan result;
                if (TimeSpan.TryParse(this["cachingTime"].ToString(), out result))
                    return result;
                return new TimeSpan(30, 0, 0, 0);
            }
        }

        [ConfigurationProperty("ExcludeImages")]
        public ExcludeImageCachingCollection ExcludeImages
        {
            get
            {
                return (ExcludeImageCachingCollection)this[nameof(ExcludeImages)];
            }
        }
    }
}
