using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class Dictionary
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_dictionary";
        /// <summary>
        /// 得到所有字典
        /// </summary>
        /// <returns></returns>
        public List<Model.Dictionary> GetAll()
        {
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var dictionaries = db.QueryAll<Model.Dictionary>();
                    Cache.IO.Insert(CACHEKEY, dictionaries);
                    return dictionaries;
                }
            }
            else
            {
                return (List<Model.Dictionary>)obj;
            }
        }
        /// <summary>
        /// 添加一个字典
        /// </summary>
        /// <param name="dictionary">字典实体</param>
        /// <returns></returns>
        public int Add(Model.Dictionary dictionary)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(dictionary);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新字典
        /// </summary>
        /// <param name="dictionary">字典实体</param>
        public int Update(Model.Dictionary dictionary)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(dictionary);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新一批字典
        /// </summary>
        /// <param name="dictionarys">字典实体数组</param>
        public int Update(Model.Dictionary[] dictionarys)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.UpdateRange(dictionarys);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一个字典
        /// </summary>
        /// <param name="dictionary">字典实体</param>
        /// <returns></returns>
        public int Delete(Model.Dictionary dictionary)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Remove(dictionary);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一批字典
        /// </summary>
        /// <param name="dictionarys">字典实体数组</param>
        /// <returns></returns>
        public int Delete(Model.Dictionary[] dictionarys)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.RemoveRange(dictionarys);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            Cache.IO.Remove(CACHEKEY);
        }
    }
}
