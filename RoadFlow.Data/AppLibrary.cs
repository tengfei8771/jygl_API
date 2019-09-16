using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class AppLibrary
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_applibrary";
        private static List<Model.AppLibrary> AppLibrarieList = null;
        /// <summary>
        /// 得到所有应用
        /// </summary>
        /// <returns></returns>
        public List<Model.AppLibrary> GetAll()
        {
            if(AppLibrarieList != null)
            {
                return AppLibrarieList;
            }
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var appLibraries = db.QueryAll<Model.AppLibrary>();
                    Cache.IO.Insert(CACHEKEY, appLibraries);
                    AppLibrarieList = appLibraries;
                    return appLibraries;
                }
            }
            else
            {
                var appLibraries = (List<Model.AppLibrary>)obj;
                AppLibrarieList = appLibraries;
                return appLibraries;
            }
        }
        /// <summary>
        /// 添加一个应用
        /// </summary>
        /// <param name="appLibrary"></param>
        /// <returns></returns>
        public int Add(Model.AppLibrary appLibrary)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(appLibrary);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新应用
        /// </summary>
        /// <param name="appLibrary">应用实体</param>
        public int Update(Model.AppLibrary appLibrary)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(appLibrary);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一个应用
        /// </summary>
        /// <param name="appLibrary">应用实体</param>
        /// <returns></returns>
        public int Delete(Model.AppLibrary appLibrary)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Remove(appLibrary);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一批应用
        /// </summary>
        /// <param name="appLibrary">应用实体</param>
        /// <returns></returns>
        public int Delete(Model.AppLibrary[] appLibrarys)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.RemoveRange(appLibrarys);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            AppLibrarieList = null;
            Cache.IO.Remove(CACHEKEY);
            //应用程序库改变要清除菜单相关缓存
            new Menu().ClearCache();
        }

        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerList(out int count, int size, int number, string title, string address, string typeId, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetApplibrarySql(title, address, typeId, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }
    }
}
