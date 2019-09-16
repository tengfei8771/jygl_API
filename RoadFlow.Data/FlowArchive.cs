using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class FlowArchive
    {
        /// <summary>
        /// 查询一个归档
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.FlowArchive Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.FlowArchive>(id);
            }
        }
        /// <summary>
        /// 添加一个归档
        /// </summary>
        /// <param name="flowArchive"></param>
        /// <returns></returns>
        public int Add(Model.FlowArchive flowArchive)
        {
            using (var db = new DataContext())
            {
                db.Add(flowArchive);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 查询归档
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="flowId"></param>
        /// <param name="stepName"></param>
        /// <param name="title"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerData(out int count, int size, int number, string flowId, string stepName, string title, string date1, string date2, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetFlowArchiveSql(flowId, stepName, title, date1, date2, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }
    }
}
