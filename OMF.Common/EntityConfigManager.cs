using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OMF.Common.Cache;
using OMF.Common.Extensions;

namespace OMF.Common
{
    public class EntityConfigManager
    {
        private const string pattern = "__{0}_EntityConfig__";

        public static void SetPersianTitle<T>(string persianTitle) where T : class
        {
            EntityConfigManager.SetPersianTitle(typeof(T), persianTitle);
        }

        public static void SetPersianTitle(Type entityType, string persianTitle)
        {
            string key = string.Format("__{0}_EntityConfig__", (object)entityType.FullName);
            EntityConfigManager.EntityConfig entityConfig = CacheManager.GetData(key) as EntityConfigManager.EntityConfig;
            if (entityConfig == null)
            {
                entityConfig = new EntityConfigManager.EntityConfig();
                CacheManager.Add(key, (object)entityConfig);
            }
            entityConfig.EntityPersianTitle = persianTitle;
        }

        public static string GetPersianTitle<T>() where T : class
        {
            return EntityConfigManager.GetPersianTitle(typeof(T));
        }

        public static string GetPersianTitle(Type entityType)
        {
            string str = entityType.Name;
            EntityConfigManager.EntityConfig data = CacheManager.GetData(string.Format("__{0}_EntityConfig__", (object)entityType.FullName)) as EntityConfigManager.EntityConfig;
            if (data != null)
                str = data.EntityPersianTitle;
            return str;
        }

        public static void SetPropertyPersianTitle<T>(
          Expression<Func<T, object>> propertySelector,
          string title)
          where T : class
        {
            propertySelector.Guard("propertySelector cannot be null");
            EntityConfigManager.SetPropertyPersianTitle(typeof(T), propertySelector.GetPropertyInfo<T>().Name, title);
        }

        public static void SetPropertyPersianTitle(Type entityType, string propertyName, string title)
        {
            string key = string.Format("__{0}_EntityConfig__", (object)entityType.FullName);
            EntityConfigManager.EntityConfig entityConfig = CacheManager.GetData(key) as EntityConfigManager.EntityConfig;
            if (entityConfig == null)
            {
                entityConfig = new EntityConfigManager.EntityConfig();
                CacheManager.Add(key, (object)entityConfig);
            }
            entityConfig.SetPropertyTitle(propertyName, title);
        }

        public static string GetPropertyPersianTitle<T>(Expression<Func<T, object>> propertySelector) where T : class
        {
            propertySelector.Guard("propertySelector cannot be null");
            return EntityConfigManager.GetPropertyPersianTitle(typeof(T), propertySelector.GetPropertyInfo<T>().Name);
        }

        public static string GetPropertyPersianTitle(Type entityType, string propertyName)
        {
            string str = "";
            EntityConfigManager.EntityConfig data = CacheManager.GetData(string.Format("__{0}_EntityConfig__", (object)entityType.FullName)) as EntityConfigManager.EntityConfig;
            if (data != null)
                str = data.GetPropertyTitle(propertyName);
            if (string.IsNullOrWhiteSpace(str))
                str = propertyName;
            return str;
        }

        private class EntityConfig
        {
            private Dictionary<string, string> internalDictionary = new Dictionary<string, string>();

            public string EntityPersianTitle { get; set; }

            public void SetPropertyTitle(string propName, string title)
            {
                if (this.internalDictionary.ContainsKey(propName))
                    this.internalDictionary[propName] = title;
                else
                    this.internalDictionary.Add(propName, title);
            }

            public string GetPropertyTitle(string propName)
            {
                if (this.internalDictionary.ContainsKey(propName))
                    return this.internalDictionary[propName];
                return "";
            }
        }
    }
}
