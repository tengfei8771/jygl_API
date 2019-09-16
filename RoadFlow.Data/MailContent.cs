using RoadFlow.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Data
{
    public class MailContent
    {
        /// <summary>
        /// 根据ID查询实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.MailContent Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.MailContent>(id);
            }
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="mailContent"></param>
        /// <returns></returns>
        public int Add(Model.MailContent mailContent)
        {
            using (var db = new DataContext())
            {
                db.Add(mailContent);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="mailContent"></param>
        /// <returns></returns>
        public int Update(Model.MailContent mailContent)
        {
            using (var db = new DataContext())
            {
                db.Update(mailContent);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="mailContent"></param>
        /// <returns></returns>
        public int Delete(Model.MailContent mailContent)
        {
            using (var db = new DataContext())
            {
                db.Remove(mailContent);
                return db.SaveChanges();
            }
        }
    }
}
