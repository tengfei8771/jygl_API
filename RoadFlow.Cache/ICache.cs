using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Cache
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    internal interface ICache
    {
        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        object Insert(string key, object obj);
        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        object Insert(string key, object obj, DateTime expiry);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);
        /// <summary>
        /// 移出缓存
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
    }
}
