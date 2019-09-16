using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Mapper;


namespace RoadFlow.Data
{
    public class Log
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="log">日志实体</param>
        /// <returns></returns>
        public int Add(Model.Log log)
        {
            using (var db = new DataContext())
            {
                db.Add(log);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 查询一条日志
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Log Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.Log>(id);
            }
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
        public System.Data.DataTable GetPagerList(out int count, int size, int number, string title, string type, string userId, string date1, string date2, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetLogSql(title, type, userId, date1, date2, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }

    }
    
}
