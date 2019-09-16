using RoadFlow.Mapper;
using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;

namespace RoadFlow.Data
{
    public class MailInBox
    {
        /// <summary>
        /// 根据ID查询实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.MailInBox Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.MailInBox>(id);
            }
        }

        /// <summary>
        /// 判断一个发件是否所有人都未读(如果所有人未读则可以撤回)
        /// </summary>
        /// <param name="outBoxId"></param>
        /// <returns></returns>
        public bool AllNoRead(Guid outBoxId)
        {
            using (var db = new DataContext())
            {
                System.Data.DataTable dataTable = db.GetDataTable("SELECT Id FROM RF_MailInBox WHERE OutBoxId={0} AND IsRead=1", outBoxId);
                if (dataTable.Rows.Count > 0)
                {
                    return false;
                }
                else
                {
                    System.Data.DataTable dataTable1 = db.GetDataTable("SELECT Id FROM RF_MailDeletedBox WHERE OutBoxId={0} AND IsRead=1", outBoxId);
                    return dataTable1.Rows.Count == 0;
                }
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="mailInBox"></param>
        /// <returns></returns>
        public int Add(Model.MailInBox mailInBox)
        {
            using (var db = new DataContext())
            {
                db.Add(mailInBox);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="mailInBox"></param>
        /// <returns></returns>
        public int Update(Model.MailInBox mailInBox)
        {
            using (var db = new DataContext())
            {
                db.Update(mailInBox);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id">收件箱ID</param>
        /// <param name="status">状态</param>
        /// <param name="isUpdateDate">是否更新阅读时间</param>
        /// <returns></returns>
        public int UpdateIsRead(Guid id, int status, bool isUpdateDate = false)
        {
            using (var db = new DataContext())
            {
                if (isUpdateDate)
                {
                    db.Execute("UPDATE RF_MailInBox SET IsRead={0},ReadDateTime={1} WHERE Id={2}", status, DateExtensions.Now, id);
                }
                else
                {
                    db.Execute("UPDATE RF_MailInBox SET IsRead={0} WHERE Id={1}", status, id);
                }
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="mailInBox"></param>
        /// <returns></returns>
        public int Delete(Model.MailInBox mailInBox)
        {
            using (var db = new DataContext())
            {
                db.Remove(mailInBox);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">收件箱ID</param>
        /// <param name="status">删除方式 0转到已删除 1彻底删除</param>
        /// <returns></returns>
        public int Delete(Guid id, int status)
        {
            using (var db = new DataContext())
            {
                var model = db.Find<Model.MailInBox>(id);
                if (null != model)
                {
                    if (0 == status)
                    {
                        Model.MailDeletedBox mailDeletedBox = new Model.MailDeletedBox();
                        mailDeletedBox.Id = model.Id;
                        mailDeletedBox.IsRead = model.IsRead;
                        mailDeletedBox.OutBoxId = model.OutBoxId;
                        mailDeletedBox.ReadDateTime = model.ReadDateTime;
                        mailDeletedBox.SendDateTime = model.SendDateTime;
                        mailDeletedBox.SendUserId = model.SendUserId;
                        mailDeletedBox.Subject = model.Subject;
                        mailDeletedBox.SubjectColor = model.SubjectColor;
                        mailDeletedBox.UserId = model.UserId;
                        db.Add(mailDeletedBox);
                    }
                    db.Remove(model);
                }
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
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetMailInBoxSql(currentUserId, subject, userId, date1, date2);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }
    }
}
