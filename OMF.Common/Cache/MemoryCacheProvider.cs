using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;

namespace OMF.Common.Cache
{
    public class MemoryCacheProvider : ICacheProvider, IEnumerable
    {
        private MemoryCache cm = (MemoryCache)null;

        public MemoryCacheProvider(string name)
        {
            this.cm = new MemoryCache(name, (NameValueCollection)null);
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
                return this.cm.Count<KeyValuePair<string, object>>();
            }
        }

        public void Add(string key, object value)
        {
            if (value == null)
                return;
            this.Add(new CacheItem(key, value));
        }

        public void Add(string key, object value, CacheItemPolicy policy)
        {
            if (value == null)
                return;
            this.Add(new CacheItem(key, value), policy);
        }

        public void Add(CacheItem cacheItem)
        {
            this.Add(cacheItem, new CacheItemPolicy());
        }

        public void Add(CacheItem cacheItem, CacheItemPolicy policy)
        {
            if (cacheItem == null)
                return;
            if (this.Contains(cacheItem.Key))
                this.Remove(cacheItem.Key);
            this.cm.Add(cacheItem, policy);
        }

        public bool Contains(string key)
        {
            return this.cm.Contains(key, (string)null);
        }

        public object GetData(string key)
        {
            return this.cm.Get(key, (string)null);
        }

        public void Remove(string key)
        {
            this.cm.Remove(key, (string)null);
        }

        public void Clear()
        {
            this.cm.Dispose();
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
            foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>)this.cm)
            {
                KeyValuePair<string, object> item = keyValuePair;
                yield return item.Value;
                item = new KeyValuePair<string, object>();
            }
        }

        public List<string> Keys
        {
            get
            {
                return this.cm.Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>)(keyValue => keyValue.Key)).ToList<string>();
            }
        }
    }
}
