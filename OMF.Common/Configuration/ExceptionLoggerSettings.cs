using System.Configuration;

namespace OMF.Common.Configuration
{
    internal class ExceptionLoggerSettings : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new ExceptionLoggerSetting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (object)((ExceptionLoggerSetting)element).Name;
        }
    }
}
