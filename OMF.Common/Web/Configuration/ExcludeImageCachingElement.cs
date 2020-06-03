using System.Configuration;

namespace OMF.Common.Web.Configuration
{
    internal class ExcludeImageCachingElement : ConfigurationElement
    {
        [ConfigurationProperty("relativeFileName", IsKey = true, IsRequired = true)]
        public string RelativeFileName
        {
            get
            {
                return (string)this["relativeFileName"];
            }
            set
            {
                this["relativeFileName"] = (object)value;
            }
        }
    }
}
