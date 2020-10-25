using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart
{
    public class Cache : ICache
    {
        private IMemoryCache _memoryCache;
        public Cache()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions
            {
                
            });
        }

        public void Add(string key, object value, TimeSpan ttl)
        {
            _memoryCache.Set(key, value, ttl);
        }

        public object Get(string key)
        {
            return _memoryCache.Get(key);
        }
    }
}
