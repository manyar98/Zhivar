using OMF.Common.Cache;
using OMF.Common.ExceptionManagement;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace OMF.Common.Web
{
    public class HttpMemoryCacheProvider : ICacheProvider, IEnumerable
    {
        private System.Web.Caching.Cache cm = (System.Web.Caching.Cache)null;

        public HttpMemoryCacheProvider()
        {
            this.cm = HttpContext.Current.Cache;
        }

        public object this[string key]
        {
            get
            {
                return this.cm[key];
            }
            set
            {
                this.cm[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return this.cm.Count;
            }
        }

        public void Add(string key, object value)
        {
            try
            {
                if (value == null)
                    return;
                if (this.Contains(key))
                {
                    object obj = this.GetData(key);
                    obj = value;
                }
                else
                    this.cm.Insert(key, value, (CacheDependency)null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, (CacheItemRemovedCallback)((cacheKey, expensiveObject, reason) =>
                    {
                        if (!key.Contains("_EntityValidator__") && !key.Contains("_OperationKeys__"))
                            return;
                        EventLogHelper.Write(string.Format("{0}Cache: Key:{1}", (object)reason.ToString(), (object)key), EventLogEntryType.Warning);
                    }));
            }
            catch (Exception ex)
            {
                ExceptionManager.SaveException((Exception)new OMFException(string.Format("InsertCacheException: Key:{0}, message:{1}", (object)key, (object)ex.Message), ex));
            }
        }

        public bool Contains(string key)
        {
            return this.cm.Get(key) != null;
        }

        public object GetData(string key)
        {
            return this.cm.Get(key);
        }

        public void Remove(string key)
        {
            this.cm.Remove(key);
        }

        public void Clear()
        {
            foreach (string key in this.Keys)
                this.Remove(key);
        }

        public void Clear(string keyLike)
        {
            foreach (string key in this.Keys)
            {
                if (key.Contains(keyLike))
                    this.Remove(key);
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (object obj in this.cm.OfType<DictionaryEntry>().Select<DictionaryEntry, object>((Func<DictionaryEntry, object>)(keyValue => keyValue.Value)))
            {
                object item = obj;
                yield return item;
                item = (object)null;
            }
        }

        public List<string> Keys
        {
            get
            {
                return this.cm.OfType<DictionaryEntry>().Select<DictionaryEntry, string>((Func<DictionaryEntry, string>)(keyValue => keyValue.Key.ToString())).ToList<string>();
            }
        }
    }
}
