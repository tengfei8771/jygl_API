using RoadFlow.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Data
{
    public class MailDeletedBox
    {
        /// <summary>
        /// 根据ID查询实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.MailDeletedBox Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.MailDeletedBox>(id);
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="mailInBox"></param>
        /// <returns></returns>
        public int Add(Model.MailDeletedBox mailDeletedBox)
        {
            using (var db = new DataContext())
            {
                db.Add(mailDeletedBox);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="mailDeletedBox"></param>
        /// <returns></returns>
        public int Update(Model.MailDeletedBox mailDeletedBox)
        {
            using (var db = new DataContext())
            {
                db.Update(mailDeletedBox);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="mailDeletedBox"></param>
        /// <returns></returns>
        public int Delete(Model.MailDeletedBox mailDeletedBox)
        {
            using (var db = new DataContext())
            {
                db.Remove(mailDeletedBox);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 还原一个已删除邮件
        /// </summary>
        /// <param name="mailDeletedBox"></param>
        /// <returns></returns>
        public int Recovery(Model.MailDeletedBox mailDeletedBox)
        {
            using (var db = new DataContext())
            {
                Model.MailInBox mailInBox = new Model.MailInBox();
                mailInBox.ContentsId = mailDeletedBox.ContentsId;
                mailInBox.Id = mailDeletedBox.Id;
                mailInBox.IsRead = mailDeletedBox.IsRead;
                mailInBox.OutBoxId = mailDeletedBox.OutBoxId;
                mailInBox.ReadDateTime = mailDeletedBox.ReadDateTime;
                mailInBox.SendDateTime = mailDeletedBox.SendDateTime;
                mailInBox.SendUserId = mailDeletedBox.SendUserId;
                mailInBox.Subject = mailDeletedBox.Subject;
                mailInBox.SubjectColor = mailDeletedBox.SubjectColor;
                mailInBox.UserId = mailDeletedBox.UserId;
                db.Add(mailInBox);
                db.Remove(mailDeletedBox);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerList(out int count, int size, int number, Guid currentUserId, string subject, string userId, string date1, string date2, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetMailDeletedBoxSql(currentUserId, subject, userId, date1, date2);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }
    }
}
