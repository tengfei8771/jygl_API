using RoadFlow.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Data
{
    public class FlowApiSystem
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_flowapisystem";
        /// <summary>
        /// 得到所有系统
        /// </summary>
        /// <returns></returns>
        public List<Model.FlowApiSystem> GetAll()
        {
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var flowApiSystems = db.QueryAll<Model.FlowApiSystem>();
                    Cache.IO.Insert(CACHEKEY, flowApiSystems);
                    return flowApiSystems;
                }
            }
            else
            {
                return (List<Model.FlowApiSystem>)obj;
            }
        }
        /// <summary>
        /// 添加一个系统
        /// </summary>
        /// <param name="flowApiSystem"></param>
        /// <returns></returns>
        public int Add(Model.FlowApiSystem flowApiSystem)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(flowApiSystem);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新系统
        /// </summary>
        /// <param name="flowApiSystem">系统实体</param>
        public int Update(Model.FlowApiSystem flowApiSystem)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(flowApiSystem);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一个系统
        /// </summary>
        /// <param name="flowApiSystem">系统实体</param>
        /// <returns></returns>
        public int Delete(Model.FlowApiSystem flowApiSystem)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Remove(flowApiSystem);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一批系统
        /// </summary>
        /// <param name="flowApiSystems">系统实体</param>
        /// <returns></returns>
        public int Delete(Model.FlowApiSystem[] flowApiSystems)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.RemoveRange(flowApiSystems);
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
