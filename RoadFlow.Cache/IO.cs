using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace RoadFlow.Cache
{
    /// <summary>
    /// 缓存操作类
    /// </summary>
    public class IO
    {
        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="obj">缓存对象</param>
        /// <returns></returns>
        public static object Insert(string key, object obj)
        {
            return Factory.GetInstance().Insert(key, obj);
        }
        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public static object Insert(string key, object obj, DateTime expiry)
        {
            return Factory.GetInstance().Insert(key, obj, expiry);
        }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <returns></returns>
        public static object Get(string key)
        {
            return Factory.GetInstance().Get(key);
        }
        /// <summary>
        /// 移出缓存对象
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <returns></returns>
        public static void Remove(string key)
        {
            Factory.GetInstance().Remove(key);
        }
    }
}
