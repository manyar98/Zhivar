using System.Collections;
using System.Collections.Generic;

namespace OMF.Common.Cache
{
    public interface ICacheProvider : IEnumerable
    {
        object this[string key] { get; set; }

        int Count { get; }

        void Add(string key, object value);

        bool Contains(string key);

        object GetData(string key);

        void Remove(string key);

        void Clear();

        void Clear(string keyLike);

        List<string> Keys { get; }
    }
}
