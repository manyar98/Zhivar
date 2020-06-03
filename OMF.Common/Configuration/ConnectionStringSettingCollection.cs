using System.Configuration;

namespace OMF.Common.Configuration
{
    public class ConnectionStringSettingCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new  ConnectionStringSettingElement();
        }

        public ConnectionStringSettingElement GetConnectionStringSetting(
          string name)
        {
            foreach (ConnectionStringSettingElement stringSettingElement in (ConfigurationElementCollection)this)
            {
                if (stringSettingElement.Name == name)
                    return stringSettingElement;
            }
            return (ConnectionStringSettingElement)null;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (object)((ConnectionStringSettingElement)element).Name;
        }
    }
}
