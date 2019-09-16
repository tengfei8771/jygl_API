using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Utility;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class Message
    {
        /// <summary>
        /// 添加一个消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public int Add(Model.Message message)
        {
            using (var db = new DataContext())
            {
                db.Add(message);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 查询一个消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Message Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.Message>(id);
            }
        }

        /// <summary>
        /// 添加一批消息
        /// </summary>
        /// <param name="message">消息实体</param>
        /// <param name="messageUsers">阅读人员</param>
        /// <returns></returns>
        public int Add(Model.Message message, Model.MessageUser[] messageUsers)
        {
            using (var db = new DataContext())
            {
                db.Add(message);
                db.AddRange(messageUsers);
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
        /// <param name="status">0发送记录 1未读 2已读</param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetSendList(out int count, int size, int number, string userId, string contents, string date1, string date2, string status, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetMessageSendListSql(userId, contents, date1, date2, status, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }

        /// <summary>
        /// 得到一个用户的未读消息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>(消息实体，未读消息条数)</returns>
        public (Model.Message, int count) GetNoRead(Guid userId)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetMessageSendListSql(userId.ToString(), "", "", "", "1", "SendTime DESC");
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, 1, 1, out int count, parameter, "SendTime DESC");
                var dt = db.GetDataTable(pagerSql, parameter);
                Model.Message messageModel = null;
                if (dt.Rows.Count > 0)
                {
                    messageModel = Get(dt.Rows[0]["Id"].ToString().ToGuid());
                }
                return (messageModel, count);
            }
        }
    }
}
