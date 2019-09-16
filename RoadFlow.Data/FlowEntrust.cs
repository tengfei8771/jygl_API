using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadFlow.Utility;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class FlowEntrust
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_flowentrust";
        /// <summary>
        /// 得到所有委托
        /// </summary>
        /// <returns></returns>
        public List<Model.FlowEntrust> GetAll()
        {
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var flowEntrust = db.QueryAll<Model.FlowEntrust>();
                    Cache.IO.Insert(CACHEKEY, flowEntrust);
                    return flowEntrust;
                }
            }
            else
            {
                return (List<Model.FlowEntrust>)obj;
            }
        }
        /// <summary>
        /// 查询一个委托
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.FlowEntrust Get(Guid id)
        {
            var all = GetAll();
            return all.Find(p => p.Id == id);
        }
        /// <summary>
        /// 添加一个委托
        /// </summary>
        /// <param name="appLibrary"></param>
        /// <returns></returns>
        public int Add(Model.FlowEntrust flowEntrust)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(flowEntrust);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新委托
        /// </summary>
        /// <param name="flowEntrust">委托实体</param>
        public int Update(Model.FlowEntrust flowEntrust)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(flowEntrust);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一批委托
        /// </summary>
        /// <param name="flowEntrusts">委托实体</param>
        /// <returns></returns>
        public int Delete(Model.FlowEntrust[] flowEntrusts)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.RemoveRange(flowEntrusts);
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

        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable GetPagerList(out int count, int size, int number, string userId, string date1, string date2, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetFlowEntrustSql(userId, date1, date2, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }
    }
}
