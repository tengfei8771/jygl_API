using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class UserFileShare
    {
        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="userFileShare"></param>
        /// <returns></returns>
        public int Add(Model.UserFileShare userFileShare)
        {
            using (var db = new DataContext())
            {
                db.Add(userFileShare);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 添加多条
        /// </summary>
        /// <param name="userFileShares"></param>
        /// <returns></returns>
        public int Add(IEnumerable<Model.UserFileShare> userFileShares)
        {
            using (var db = new DataContext())
            {
                db.AddRange(userFileShares);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Model.UserFileShare Get(string fileId, Guid userId)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.UserFileShare>(fileId, userId);
            }
        }

        /// <summary>
        /// 删除一个文件的分享记录
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public int DeleteByFileId(string fileId)
        {
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_UserFileShare WHERE FileId={0}", fileId);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 查询一个文件的分享列表
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public List<Model.UserFileShare> GetListByFileId(string fileId)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.UserFileShare>("SELECT * FROM RF_UserFileShare WHERE FileId={0}", fileId);
            }
        }

        /// <summary>
        /// 分享文件（先删除再添加）
        /// </summary>
        /// <param name="userFileShares"></param>
        /// <param name="fileId">文件id</param>
        /// <returns></returns>
        public int Share(IEnumerable<Model.UserFileShare> userFileShares, string fileId)
        {
            using (var db = new DataContext())
            {
                List<Model.UserFileShare> adds = new List<Model.UserFileShare>();
                var shares = GetListByFileId(fileId);//这里检查，如果已经有的记录跳过
                foreach (var share in userFileShares)
                {
                    if (shares.Exists(p => p.UserId == share.UserId))
                    {
                        continue;
                    }
                    adds.Add(share);
                }
                if (adds.Any())
                {
                    db.AddRange(adds);
                }
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 查询一页我分享的数据
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable GetMySharePagerList(out int count, int size, int number, Guid userId, string fileName, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetUserFileShareSql(userId, fileName, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }

        /// <summary>
        /// 查询我分享的文件的接收人员
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable GetMyShareUsers(string fileId, Guid userId)
        {
            using (var db = new DataContext())
            {
                return db.GetDataTable("SELECT UserId FROM RF_UserFileShare WHERE FileId={0} AND ShareUserId={1}", fileId, userId);
            }
        }

        /// <summary>
        /// 查询一页我收到的数据
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable GetShareMyPagerList(out int count, int size, int number, Guid userId, string fileName, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetUserFileShareMySql(userId, fileName, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }
    }
}
