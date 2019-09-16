using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadFlow.Mapper;


namespace RoadFlow.Data
{
    public class WorkGroup
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_workgroup";
        private static List<Model.WorkGroup> WorkGroupList = null;
        /// <summary>
        /// 得到所有工作组
        /// </summary>
        /// <returns></returns>
        public List<Model.WorkGroup> GetAll()
        {
            if (WorkGroupList != null)
            {
                return WorkGroupList;
            }
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var workGroups = db.QueryAll<Model.WorkGroup>();
                    Cache.IO.Insert(CACHEKEY, workGroups);
                    WorkGroupList = workGroups;
                    return workGroups;
                }
            }
            else
            {
                var workGroups = (List<Model.WorkGroup>)obj;
                WorkGroupList = workGroups;
                return workGroups;
            }
        }
        /// <summary>
        /// 添加一个工作组
        /// </summary>
        /// <param name="workGroup">工作组实体</param>
        /// <returns></returns>
        public int Add(Model.WorkGroup workGroup)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(workGroup);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新工作组
        /// </summary>
        /// <param name="workGroup">工作组实体</param>
        public int Update(Model.WorkGroup workGroup)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(workGroup);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新工作组
        /// </summary>
        /// <param name="workGroups">工作组实体数组</param>
        public int Update(Model.WorkGroup[] workGroups)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.UpdateRange(workGroups);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一个工作组
        /// </summary>
        /// <param name="workGroup">工作组实体</param>
        /// <returns></returns>
        public int Delete(Model.WorkGroup workGroup)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Remove(workGroup);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            WorkGroupList = null;
            Cache.IO.Remove(CACHEKEY);
            new HomeSet().ClearCache();//清除首页设置缓存
        }
    }
}
