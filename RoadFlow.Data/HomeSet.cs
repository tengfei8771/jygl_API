using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class HomeSet
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_homeset";
        /// <summary>
        /// 得到所有设置
        /// </summary>
        /// <returns></returns>
        public List<Model.HomeSet> GetAll()
        {
            using (var db = new DataContext())
            {
                var homeSets = db.QueryAll<Model.HomeSet>();
                return homeSets;
            }
        }
        /// <summary>
        /// 添加一个设置
        /// </summary>
        /// <param name="homeSet"></param>
        /// <returns></returns>
        public int Add(Model.HomeSet homeSet)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(homeSet);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新设置
        /// </summary>
        /// <param name="homeSet">设置实体</param>
        public int Update(Model.HomeSet homeSet)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(homeSet);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一个设置
        /// </summary>
        /// <param name="homeSet">设置实体</param>
        /// <returns></returns>
        public int Delete(Model.HomeSet homeSet)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Remove(homeSet);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一批设置
        /// </summary>
        /// <param name="homeSets">设置实体</param>
        /// <returns></returns>
        public int Delete(Model.HomeSet[] homeSets)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.RemoveRange(homeSets);
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
        /// 查询一页设置
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerData(out int count, int size, int number, string name, string title, string type, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetHomeSetSql(name, title, type, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }
    }
}
