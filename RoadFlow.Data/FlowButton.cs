using System;
using System.Collections.Generic;
using System.Linq;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class FlowButton
    {
        private const string CACHEKEY = "roadflow_cache_flowbutton";
        /// <summary>
        /// 得到所有按钮
        /// </summary>
        /// <returns></returns>
        public List<Model.FlowButton> GetAll()
        {
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var flowButtons = db.QueryAll<Model.FlowButton>().OrderBy(p=>p.Sort).ToList();
                    Cache.IO.Insert(CACHEKEY, flowButtons);
                    return flowButtons;
                }
            }
            else
            {
                return (List<Model.FlowButton>)obj;
            }
        }
        
        /// <summary>
        /// 添加一个按钮
        /// </summary>
        /// <param name="flowButton">按钮实体</param>
        /// <returns></returns>
        public int Add(Model.FlowButton flowButton)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(flowButton);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="flowButton">按钮实体</param>
        public int Update(Model.FlowButton flowButton)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(flowButton);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="flowButtons">按钮实体</param>
        /// <returns></returns>
        public int Delete(Model.FlowButton[] flowButtons)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.RemoveRange(flowButtons);
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
