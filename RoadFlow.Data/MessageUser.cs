using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadFlow.Utility;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class MessageUser
    {
        /// <summary>
        /// 更新一个消息为已读
        /// </summary>
        /// <param name="messageUsers"></param>
        /// <returns></returns>
        public int UpdateIsRead(Guid messageId, Guid userId)
        {
            using (var db = new DataContext())
            {
                db.Execute("UPDATE RF_MessageUser SET IsRead=1,ReadTime={0} WHERE MessageId={1} AND UserId={2}",
                    DateExtensions.Now, messageId, userId);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 查询一页已发送消息阅读人员
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="messageId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetReadUserList(out int count, int size, int number, string messageId, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetMessageReadUserListSql(messageId, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }
    }
}
