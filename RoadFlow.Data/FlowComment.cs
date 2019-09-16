using System;
using System.Collections.Generic;
using System.Linq;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class FlowComment
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_flowcomment";
        /// <summary>
        /// 得到所有意见
        /// </summary>
        /// <returns></returns>
        public List<Model.FlowComment> GetAll()
        {
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var flowComments = db.QueryAll<Model.FlowComment>().OrderBy(p => p.Sort).ToList();
                    Cache.IO.Insert(CACHEKEY, flowComments);
                    return flowComments;
                }
            }
            else
            {
                return (List<Model.FlowComment>)obj;
            }
        }
        /// <summary>
        /// 添加一个意见
        /// </summary>
        /// <param name="appLibrary"></param>
        /// <returns></returns>
        public int Add(Model.FlowComment flowComments)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(flowComments);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新意见
        /// </summary>
        /// <param name="appLibrary">意见实体</param>
        public int Update(Model.FlowComment flowComments)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(flowComments);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一批意见
        /// </summary>
        /// <param name="flowComments">意见实体</param>
        /// <returns></returns>
        public int Delete(Model.FlowComment[] flowComments)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.RemoveRange(flowComments);
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
