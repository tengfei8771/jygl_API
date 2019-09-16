using RoadFlow.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Data
{
    public class Vote
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Vote Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.Vote>(id);
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="vote"></param>
        /// <returns></returns>
        public int Add(Model.Vote vote)
        {
            using (var db = new DataContext())
            {
                db.Add(vote);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="vote">实体</param>
        public int Update(Model.Vote vote)
        {
            using (var db = new DataContext())
            {
                db.Update(vote);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isDeleteAll">是否删除此项投票的选项以及结果所有数据</param>
        /// <returns></returns>
        public int Delete(Guid id, bool isDeleteAll = true)
        {
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_Vote WHERE Id={0}", id);
                if (isDeleteAll)
                {
                    db.Execute("DELETE FROM RF_VoteItem WHERE VoteId={0}", id);
                    db.Execute("DELETE FROM RF_VoteItemOption WHERE VoteId={0}", id);
                    db.Execute("DELETE FROM RF_VotePartakeUser WHERE VoteId={0}", id);
                    db.Execute("DELETE FROM RF_VoteResult WHERE VoteId={0}", id);
                    db.Execute("DELETE FROM RF_VoteResultUser WHERE VoteId={0}", id);
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
        /// <param name="currentUserId"></param>
        /// <param name="topic"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerList(out int count, int size, int number, Guid currentUserId, string topic, string date1, string date2, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetVoteSql(currentUserId, topic, date1, date2);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }

        /// <summary>
        /// 查询一页待提交数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="currentUserId"></param>
        /// <param name="topic"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetWaitSubmitPagerList(out int count, int size, int number, Guid currentUserId, string topic, string date1, string date2, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetWaitSubmitVoteSql(currentUserId, topic, date1, date2);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }

        /// <summary>
        /// 查询一页结果数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="currentUserId"></param>
        /// <param name="topic"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetResultPagerList(out int count, int size, int number, Guid currentUserId, string topic, string date1, string date2, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetVoteResultSql(currentUserId, topic, date1, date2);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }

        /// <summary>
        /// 查询一页参与人员数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="name"></param>
        /// <param name="org"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPartakeUserPagerList(out int count, int size, int number, Guid voteId, string name, string org, string order)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetPartakeUserSql(voteId, name, org);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }

    }
}
