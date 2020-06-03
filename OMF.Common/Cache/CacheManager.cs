using System.Collections.Generic;

namespace OMF.Common.Cache
{
    public static class CacheManager
    {
        private static ICacheProvider cache;

        public static ICacheProvider CacheProvider
        {
            get
            {
                return CacheManager.cache;
            }
        }

        public static void InitiateCacheProvider(ICacheProvider cacheProvider)
        {
            CacheManager.cache = cacheProvider;
        }

        static CacheManager()
        {
            CacheManager.InitiateCacheProvider((ICacheProvider)new MemoryCacheProvider(nameof(CacheManager)));
        }

        public static int Count
        {
            get
            {
                return CacheManager.CacheProvider.Count;
            }
        }

        public static void Add(string key, object value)
        {
            CacheManager.CacheProvider.Add(key, value);
        }

        public static bool Contains(string key)
        {
            return CacheManager.CacheProvider.Contains(key);
        }

        public static object GetData(string key)
        {
            return CacheManager.CacheProvider.GetData(key);
        }

        public static void Remove(string key)
        {
            if (!CacheManager.CacheProvider.Contains(key))
                return;
            CacheManager.CacheProvider.Remove(key);
        }

        public static void ClearCache()
        {
            CacheManager.CacheProvider.Clear();
        }

        public static void Clear(string keyLike)
        {
            List<string> stringList = new List<string>();
            foreach (string key in CacheManager.CacheProvider.Keys)
            {
                if (key.ToString().Contains(keyLike))
                    stringList.Add(key.ToString());
            }
            foreach (string key in stringList)
                CacheManager.CacheProvider.Remove(key);
        }
    }
}
