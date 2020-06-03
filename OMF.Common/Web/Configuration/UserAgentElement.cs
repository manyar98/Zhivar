using System.Configuration;

namespace OMF.Common.Web.Configuration
{
    public class UserAgentElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"].ToString();
            }
            set
            {
                this["name"] = (object)value;
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
