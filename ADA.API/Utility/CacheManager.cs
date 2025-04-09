using ADA.API.IServices;
using ADAClassLibrary;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.Utility
{
    public class CacheManager<T> where T : class
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IService<T> _service;
        private readonly double cacheExpireTime = 30;
        private readonly double slidingExpiration = 10;
        public CacheManager(IMemoryCache memoryCache, IService<T> service)
        {
            _memoryCache = memoryCache;
            _service = service;
        }
        public void CreateEntry(string key, List<T> values)
        {
            var cacheExpirationTime = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(cacheExpireTime),
                Priority = CacheItemPriority.Normal,
                SlidingExpiration = TimeSpan.FromMinutes(slidingExpiration)
            };
            _memoryCache.Set<List<T>>(key, values, cacheExpirationTime);
        }
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public List<T> TryGetValue(string key)
        {
            //object value;
            if (!_memoryCache.TryGetValue<List<T>>(key, out List<T> value))
            {
                var cacheExpirationTime = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(cacheExpireTime),
                    Priority = CacheItemPriority.Normal,
                    SlidingExpiration = TimeSpan.FromMinutes(slidingExpiration)
                };
                value = _service.GetAll().ToList();
                if (value != null && value.Count > 0)
                {
                    _memoryCache.Set<List<T>>(key, value, cacheExpirationTime);
                }
            }
            return value;
        }

        
    }
}
