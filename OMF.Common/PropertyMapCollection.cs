using OMF.Common.Cache;
using OMF.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OMF.Common
{
    public class PropertyMapCollection
    {
        private const string pattern = "__PropertyMapCollection_{0}__";

        public static void AddMap<TEntity>(
          Expression<Func<TEntity, object>> propFromSelector,
          Expression<Func<TEntity, object>> propToSelector)
          where TEntity : class, IEntity
        {
            PropertyMapCollection.AddMap<TEntity, TEntity>(propFromSelector, propToSelector, false);
        }

        public static void AddMap<TFromEntity, TToEntity>(
          Expression<Func<TFromEntity, object>> propFromSelector,
          Expression<Func<TToEntity, object>> propToSelector,
          bool isTwoWay = true)
          where TFromEntity : class
          where TToEntity : class
        {
            propFromSelector.Guard("'propFromSelector' cannot be null");
            propToSelector.Guard("'propToSelector' cannot be null");
            string key1 = string.Format("__PropertyMapCollection_{0}__", (object)typeof(TFromEntity).FullName);
            Dictionary<string, string> dictionary1 = CacheManager.GetData(key1) as Dictionary<string, string>;
            if (dictionary1 == null)
            {
                dictionary1 = new Dictionary<string, string>();
                CacheManager.Add(key1, (object)dictionary1);
            }
            string name1 = propFromSelector.GetPropertyInfo<TFromEntity>().Name;
            string name2 = propToSelector.GetPropertyInfo<TToEntity>().Name;
            dictionary1[name1] = name2;
            if (!isTwoWay)
                return;
            string key2 = string.Format("__PropertyMapCollection_{0}__", (object)typeof(TToEntity).FullName);
            Dictionary<string, string> dictionary2 = CacheManager.GetData(key1) as Dictionary<string, string>;
            if (dictionary2 == null)
            {
                dictionary2 = new Dictionary<string, string>();
                CacheManager.Add(key2, (object)dictionary2);
            }
            dictionary2[name2] = name1;
        }

        public static string GetMapPropertyName<TEntity>(Expression<Func<TEntity, object>> propSelector) where TEntity : class, IEntity
        {
            propSelector.Guard("'propSelector' cannot be null");
            return PropertyMapCollection.GetMapPropertyName<TEntity>(propSelector.GetPropertyInfo<TEntity>().Name);
        }

        public static string GetMapPropertyName<TEntity>(string propName) where TEntity : class
        {
            return PropertyMapCollection.GetMapPropertyName(typeof(TEntity), propName);
        }

        public static string GetMapPropertyName(Type entityType, string propName)
        {
            Dictionary<string, string> data = CacheManager.GetData(string.Format("__PropertyMapCollection_{0}__", (object)entityType.FullName)) as Dictionary<string, string>;
            if (data == null || !data.ContainsKey(propName))
                return propName;
            return data[propName];
        }
    }
}
