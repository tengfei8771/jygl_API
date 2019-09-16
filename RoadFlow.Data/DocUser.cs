using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class DocUser
    {
        /// <summary>
        /// 更新阅读人员状态
        /// </summary>
        /// <param name="docId">文档ID</param>
        /// <param name="userId">人员ID</param>
        /// <param name="isRead">是否阅读</param>
        public int UpdateIsRead(Guid docId, Guid userId, int isRead)
        {
            using (var db = new DataContext())
            {
                db.Execute("UPDATE RF_DocUser SET IsRead={0},ReadTime={1} WHERE DocId={2} AND UserId={3}",
                    isRead, 1 == isRead ? DateExtensions.Now : new DateTime?(), docId, userId);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 判断一个人员一个文档是否已读
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsRead(Guid docId, Guid userId)
        {
            using (var db = new DataContext())
            {
                string isRead = db.ExecuteScalarString("SELECT IsRead FROM RF_DocUser WHERE DocId={0} AND UserId={1}", docId, userId);
                return "1".Equals(isRead);
            }
        }
        /// <summary>
        /// 删除一个人员记录
        /// </summary>
        /// <param name="userId">人员ID</param>
        /// <returns></returns>
        public int Delete(Guid userId)
        {
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_DocUser WHERE UserId={0}", userId);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 查询文档阅读情况
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="docId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetDocReadPagerList(out int count, int size, int number, string docId, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetDocReadUserListSql(docId, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }
    }
}
