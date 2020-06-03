using System;
using System.Configuration;
using static OMF.Common.Enums;

namespace OMF.Common.Web.Configuration
{
    public class HttpCompressionSettings : ConfigurationElement
    {
        [ConfigurationProperty("algorithm", IsRequired = true)]
        public CompressionType Algorithm
        {
            get
            {
                return (CompressionType)Enum.Parse(typeof(CompressionType), this["algorithm"].ToString(), true);
            }
            set
            {
                this["algorithm"] = (object)value;
            }
        }

        [ConfigurationProperty("compressionLevel")]
        public CompressionLevel CompressionLevel
        {
            get
            {
                return (CompressionLevel)Enum.Parse(typeof(CompressionLevel), this["compressionLevel"].ToString(), true);
            }
            set
            {
                this["compressionLevel"] = (object)value;
            }
        }

        [ConfigurationProperty("excludeContentType")]
        public string ExcludeContentType
        {
            get
            {
                return this["excludeContentType"].ToString();
            }
            set
            {
                this["excludeContentType"] = (object)value;
            }
        }

        [ConfigurationProperty("ExcludeUserAgents")]
        public UserAgentCollection ExcludeUserAgents
        {
            get
            {
                return (UserAgentCollection)this[nameof(ExcludeUserAgents)];
            }
            set
            {
                this[nameof(ExcludeUserAgents)] = (object)value;
            }
        }

        [ConfigurationProperty("TextContentTypes")]
        public ContentTypeCollection TextContentTypes
        {
            get
            {
                return (ContentTypeCollection)this[nameof(TextContentTypes)];
            }
            set
            {
                this[nameof(TextContentTypes)] = (object)value;
            }
        }

        [ConfigurationProperty("ExcludeRequestPaths")]
        public RequestPathCollection ExcludeRequestPaths
        {
            get
            {
                return (RequestPathCollection)this[nameof(ExcludeRequestPaths)];
            }
            set
            {
                this[nameof(ExcludeRequestPaths)] = (object)value;
            }
        }
    }
}
