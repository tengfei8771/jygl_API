using RoadFlow.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Data
{
    public class MailOutBox
    {
        /// <summary>
        /// 根据ID查询实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.MailOutBox Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.MailOutBox>(id);
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="mailOutBox"></param>
        /// <returns></returns>
        public int Add(Model.MailOutBox mailOutBox)
        {
            using (var db = new DataContext())
            {
                db.Add(mailOutBox);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="mailOutBox"></param>
        /// <returns></returns>
        public int Update(Model.MailOutBox mailOutBox)
        {
            using (var db = new DataContext())
            {
                db.Update(mailOutBox);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="mailOutBox"></param>
        /// <returns></returns>
        public int Delete(Model.MailOutBox mailOutBox)
        {
            using (var db = new DataContext())
            {
                db.Remove(mailOutBox);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="mailOutBox"></param>
        /// <returns></returns>
        public int Delete(Guid id)
        {
            using (var db = new DataContext())
            {
                db.Execute("DELETE RF_MailOutBox WHERE Id={0}", id);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailOutBox"></param>
        /// <param name="mailContent"></param>
        /// <param name="receiveUsers">接收人员</param>
        /// <param name="carbonCopyUsers">抄送人员</param>
        /// <param name="secretCopyUsers">密送人员</param>
        /// <param name="isAdd">是否新增</param>
        /// <returns></returns>
        public int Send(Model.MailOutBox mailOutBox, Model.MailContent mailContent, List<Model.User> receiveUsers, List<Model.User> carbonCopyUsers, List<Model.User> secretCopyUsers, bool isAdd)
        {
            using (var db = new DataContext())
            {
                int i = 0;
                if (isAdd)
                {
                    i += db.Add(mailOutBox);
                }
                else
                {
                    i += db.Update(mailOutBox);
                    db.Remove(mailContent);
                }
                db.Add(mailContent);
                if (mailOutBox.Status == 1)//如果是发送，则添加收件箱数据，存草稿不添加
                {
                    List<Model.MailInBox> mailInBoxes = new List<Model.MailInBox>();
                    //发送
                    foreach (var user in receiveUsers)
                    {
                        Model.MailInBox mailInBox = new Model.MailInBox
                        {
                            ContentsId = mailContent.Id,
                            Id = Guid.NewGuid(),
                            IsRead = 0,
                            SendDateTime = mailOutBox.SendDateTime,
                            SendUserId = mailOutBox.UserId,
                            Subject = mailOutBox.Subject,
                            SubjectColor = mailOutBox.SubjectColor,
                            UserId = user.Id,
                            OutBoxId = mailOutBox.Id,
                            MailType = 1
                        };
                        mailInBoxes.Add(mailInBox);
                    }
                    //抄送
                    foreach(var user in carbonCopyUsers)
                    {
                        Model.MailInBox mailInBox = new Model.MailInBox
                        {
                            ContentsId = mailContent.Id,
                            Id = Guid.NewGuid(),
                            IsRead = 0,
                            SendDateTime = mailOutBox.SendDateTime,
                            SendUserId = mailOutBox.UserId,
                            Subject = mailOutBox.Subject,
                            SubjectColor = mailOutBox.SubjectColor,
                            UserId = user.Id,
                            OutBoxId = mailOutBox.Id,
                            MailType = 2
                        };
                        mailInBoxes.Add(mailInBox);
                    }
                    //密送
                    foreach (var user in secretCopyUsers)
                    {
                        Model.MailInBox mailInBox = new Model.MailInBox
                        {
                            ContentsId = mailContent.Id,
                            Id = Guid.NewGuid(),
                            IsRead = 0,
                            SendDateTime = mailOutBox.SendDateTime,
                            SendUserId = mailOutBox.UserId,
                            Subject = mailOutBox.Subject,
                            SubjectColor = mailOutBox.SubjectColor,
                            UserId = user.Id,
                            OutBoxId = mailOutBox.Id,
                            MailType = 3
                        };
                        mailInBoxes.Add(mailInBox);
                    }
                    db.AddRange(mailInBoxes);
                }
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 撤回邮件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Withdraw(Guid id)
        {
            using (var db = new DataContext())
            {
                Model.MailOutBox mailOutBox = db.Find<Model.MailOutBox>(id);
                if (null == mailOutBox)
                {
                    return false;
                }
                db.Execute("DELETE RF_MailInBox WHERE OutBoxId={0}", id);
                db.Execute("UPDATE RF_MailOutBox SET Status=0 WHERE Id={0}", id);
                db.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="subject"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerList(out int count, int size, int number, Guid currentUserId, string subject, string date1, string date2, string order, int status = -1)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetMailOutBoxSql(currentUserId, subject, date1, date2, status);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }
    }
}
