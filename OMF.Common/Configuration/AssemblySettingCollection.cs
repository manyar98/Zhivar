using System;
using System.Configuration;

namespace OMF.Common.Configuration
{
    internal class AssemblySettingCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new AssemblySettingElement();
        }

        public AssemblySettingElement this[string name]
        {
            get
            {
                foreach (AssemblySettingElement assemblySettingElement in (ConfigurationElementCollection)this)
                {
                    if (assemblySettingElement.Name == name)
                        return assemblySettingElement;
                }
                throw new Exception("Can't Find Assembly Setting '" + name + "' !");
            }
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (object)((AssemblySettingElement)element).Name;
        }

        public void Add(AssemblySettingElement element)
        {
            this.BaseAdd((ConfigurationElement)element);
        }
    }
}
