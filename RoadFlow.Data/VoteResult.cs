using RoadFlow.Mapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Utility;

namespace RoadFlow.Data
{
    public class VoteResult
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="voteResult"></param>
        /// <returns></returns>
        public int Add(Model.VoteResult voteResult)
        {
            using (var db = new DataContext())
            {
                db.Add(voteResult);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="voteResults"></param>
        /// <returns></returns>
        public int AddRange(List<Model.VoteResult> voteResults)
        {
            if(voteResults == null || voteResults.Count == 0)
            {
                return 0;
            }
            Guid voteId = voteResults.First().VoteId;
            Guid userId = voteResults.First().UserId;
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_VoteResult WHERE VoteId={0} AND UserId={1}", voteId, userId);
                db.AddRange(voteResults);
                //修改状态为已提交
                db.Execute("UPDATE RF_VotePartakeUser SET Status=1,SubmitTime={0} WHERE VoteId={1} AND UserId={2}", DateExtensions.Now, voteId, userId);
                //修改问卷状态
                if (db.GetDataTable("SELECT Id FROM RF_VotePartakeUser WHERE Status=0 And VoteId={0} AND UserId!={1}", voteId, userId).Rows.Count == 0)
                {
                    db.Execute("UPDATE RF_Vote SET Status=3 WHERE Id={0}", voteId);
                }
                else
                {
                    db.Execute("UPDATE RF_Vote SET Status=2 WHERE Id={0}", voteId);
                }
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="voteResult">实体</param>
        public int Update(Model.VoteResult voteResult)
        {
            using (var db = new DataContext())
            {
                db.Update(voteResult);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(Guid id)
        {
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_VoteResult WHERE Id={0}", id);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 得到一个问卷的结果
        /// </summary>
        /// <param name="voteId"></param>
        /// <returns></returns>
        public List<Model.VoteResult> GetVoteResults(Guid voteId)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.VoteResult>("SELECT * FROM RF_VoteResult WHERE VoteId={0}", voteId);
            }
        }
    }
}
