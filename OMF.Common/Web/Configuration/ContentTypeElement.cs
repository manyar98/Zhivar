using System.Configuration;

namespace OMF.Common.Web.Configuration
{
    public class ContentTypeElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsKey = true, IsRequired = true)]
        public string Key
        {
            get
            {
                return this["key"].ToString();
            }
            set
            {
                this["key"] = (object)value;
            }
        }

        [ConfigurationProperty("matchValue")]
        public string MatchValue
        {
            get
            {
                return this["matchValue"].ToString();
            }
            set
            {
                this["matchValue"] = (object)value;
            }
        }
    }
}
