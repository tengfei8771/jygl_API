using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class Program
    {
        /// <summary>
        /// 查询一个程序设计
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Program Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.Program>(id);
            }
        }
        /// <summary>
        /// 添加一个程序设计
        /// </summary>
        /// <param name="program">程序设计实体</param>
        /// <returns></returns>
        public int Add(Model.Program program)
        {
            using (var db = new DataContext())
            {
                db.Add(program);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新程序设计
        /// </summary>
        /// <param name="program">程序设计实体</param>
        public int Update(Model.Program program)
        {
            using (var db = new DataContext())
            {
                db.Update(program);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 查询一页程序设计
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="name"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerData(out int count, int size, int number, string name, string types, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetProgramSql(name, types, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }
    }
}
