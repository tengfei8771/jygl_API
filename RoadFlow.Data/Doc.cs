using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class Doc
    {
        /// <summary>
        /// 查询文档
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Doc Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.Doc>(id);
            }
        }
        /// <summary>
        /// 添加一个文档
        /// </summary>
        /// <param name="doc">文档实体</param>
        /// <param name="users">阅读人员</param>
        /// <returns></returns>
        public int Add(Model.Doc doc, List<Model.User> users = null)
        {
            using (var db = new DataContext())
            {
                db.Add(doc);
                //添加阅读人员
                if (null != users)
                {
                    foreach (var user in users)
                    {
                        db.Execute("INSERT INTO RF_DocUser VALUES({0},{1},{2},{3})", doc.Id, user.Id, 0, DBNull.Value);
                    }
                }
                return db.SaveChanges();
            }
        } 
        /// <summary>
        /// 更新文档
        /// </summary>
        /// <param name="doc">文档实体</param>
        /// <param name="users">阅读人员</param>
        public int Update(Model.Doc doc, List<Model.User> users = null)
        {
            using (var db = new DataContext())
            {
                db.Update(doc);
                //更新阅读人员
                if (null != users)
                {
                    db.Execute("DELETE FROM RF_DocUser WHERE DocId={0}", doc.Id);
                    foreach (var user in users)
                    {
                        db.Execute("INSERT INTO RF_DocUser VALUES({0},{1},{2},{3})", doc.Id, user.Id, 0, null);
                    }
                }
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新阅读次数
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public int UpdateReadCount(Model.Doc doc)
        {
            using (var db = new DataContext())
            {
                db.Execute("UPDATE RF_Doc SET ReadCount=ReadCount+1 WHERE Id={0}", doc.Id);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一个文档
        /// </summary>
        /// <param name="doc">文档实体</param>
        /// <returns></returns>
        public int Delete(Guid id)
        {
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_Doc WHERE Id={0}", id);
                db.Execute("DELETE FROM RF_DocUser WHERE DocId={0}", id);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="userId"></param>
        /// <param name="title"></param>
        /// <param name="dirId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerList(out int count, int size, int number, Guid userId, string title, string dirId, string date1, string date2, string order, int read)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetDocSql(userId, title, dirId, date1, date2, order, read);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }
    }
}
