using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace RoadFlow.Cache
{
    /// <summary>
    /// Core自带缓存实现
    /// </summary>
    internal class CoreCache : ICache
    {
        private static MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions() {});
        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="obj">缓存对象</param>
        /// <returns></returns>
        public object Insert(string key, object obj)
        {
            return memoryCache.Set(key, obj);
        }
        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="expiry">过期时间(绝对时间)</param>
        /// <returns></returns>
        public object Insert(string key, object obj, DateTime expiry)
        {

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = new DateTimeOffset(expiry)
            };
            return memoryCache.Set(key, obj, cacheEntryOptions);
        }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (memoryCache.TryGetValue(key, out object obj))
            {
                return obj;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 移出缓存对象
        /// </summary>
        /// <param name="key">缓存key</param>
        public void Remove(string key)
        {
            memoryCache.Remove(key);
        }
    }
}
