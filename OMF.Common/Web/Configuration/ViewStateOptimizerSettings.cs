using OMF.Common.Configuration;
using System;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Web.UI;
using static OMF.Common.Enums;

namespace OMF.Common.Web.Configuration
{
    public class ViewStateOptimizerSettings : ConfigurationElement
    {
        [ConfigurationProperty("assemblyName")]
        public string AssemblyName
        {
            get
            {
                return this["assemblyName"].ToString();
            }
            set
            {
                this["assemblyName"] = (object)value;
            }
        }

        [ConfigurationProperty("persisterClassName")]
        public string PersisterClassName
        {
            get
            {
                return this["persisterClassName"].ToString();
            }
            set
            {
                this["persisterClassName"] = (object)value;
            }
        }

        [ConfigurationProperty("storageMethod")]
        public ViewStateStorageMethod StorageMethod
        {
            get
            {
                return (ViewStateStorageMethod)Enum.Parse(typeof(ViewStateStorageMethod), this["storageMethod"].ToString(), true);
            }
            set
            {
                this["storageMethod"] = (object)value;
            }
        }

        [ConfigurationProperty("storagePath")]
        public string StoragePath
        {
            get
            {
                return this["storagePath"].ToString();
            }
            set
            {
                this["storagePath"] = (object)value;
            }
        }

        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get
            {
                return this["connectionStringName"].ToString();
            }
            set
            {
                this["connectionStringName"] = (object)value;
            }
        }

        [ConfigurationProperty("tableName")]
        public string TableName
        {
            get
            {
                return this["tableName"].ToString();
            }
            set
            {
                this["tableName"] = (object)value;
            }
        }

        [ConfigurationProperty("compressed")]
        public bool Compressed
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["compressed"]);
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                this["compressed"] = (object)value;
            }
        }

        [ConfigurationProperty("requestBehavior")]
        public ViewStateStorageBehavior RequestBehavior
        {
            get
            {
                return (ViewStateStorageBehavior)Enum.Parse(typeof(ViewStateStorageBehavior), this["requestBehavior"].ToString(), true);
            }
            set
            {
                this["requestBehavior"] = (object)value;
            }
        }

        [ConfigurationProperty("viewStateCleanupInterval")]
        public TimeSpan ViewStateCleanupInterval
        {
            get
            {
                try
                {
                    return TimeSpan.Parse(this["viewStateCleanupInterval"].ToString());
                }
                catch
                {
                    return TimeSpan.FromHours(1.0);
                }
            }
            set
            {
                this["viewStateCleanupInterval"] = (object)value;
            }
        }

        public object CreatePersister(Page page)
        {
            try
            {
                return ConfigurationController.GetAssembly(this.AssemblyName).CreateInstance(this.PersisterClassName, true, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, (Binder)null, new object[1]
                {
          (object) page
                }, (CultureInfo)null, (object[])null);
            }
            catch
            {
                return (object)null;
            }
        }
    }
}
