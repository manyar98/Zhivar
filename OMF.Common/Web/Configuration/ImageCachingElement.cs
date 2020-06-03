using System.Configuration;

namespace OMF.Common.Web.Configuration
{
    internal class ImageCachingElement : ConfigurationElement
    {
        [ConfigurationProperty("extension", IsKey = true, IsRequired = true)]
        public string Extension
        {
            get
            {
                return (string)this["extension"];
            }
            set
            {
                this["extension"] = (object)value.Replace(".", "");
            }
        }

        [ConfigurationProperty("contentType", IsRequired = true)]
        public string ContentType
        {
            get
            {
                return (string)this["contentType"];
            }
            set
            {
                this["contentType"] = (object)value;
            }
        }
    }
}
