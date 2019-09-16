using RoadFlow.Mapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Utility;

namespace RoadFlow.Data
{
    public class VotePartakeUser
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="votePartakeUser"></param>
        /// <returns></returns>
        public int Add(Model.VotePartakeUser votePartakeUser)
        {
            using (var db = new DataContext())
            {
                db.Add(votePartakeUser);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="votePartakeUser">实体</param>
        public int Update(Model.VotePartakeUser votePartakeUser)
        {
            using (var db = new DataContext())
            {
                db.Update(votePartakeUser);
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
                db.Execute("DELETE FROM RF_VotePartakeUser WHERE Id={0}", id);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除一个问卷的所有参与人员和结果查询人员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteByVoteId(Guid voteId)
        {
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_VotePartakeUser WHERE VoteId={0}", voteId);
                db.Execute("DELETE FROM RF_VoteResultUser WHERE VoteId={0}", voteId);
                //更新问卷状态为未发布
                db.Execute("UPDATE RF_Vote SET Status=0,PublishTime=NULL WHERE Id={0}", voteId);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 添加一个问卷的参与人员和结果查询人员（先删除再添加）
        /// </summary>
        /// <param name="votePartakeUsers">参与人员</param>
        /// <param name="voteResultUsers">结果查询人员</param>
        /// <returns></returns>
        public int Add(List<Model.VotePartakeUser> votePartakeUsers, List<Model.VoteResultUser> voteResultUsers)
        {
            if(votePartakeUsers == null || votePartakeUsers.Count == 0)
            {
                return 0;
            }
            Guid voteId = votePartakeUsers.First().VoteId;
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_VotePartakeUser WHERE VoteId={0}", voteId);
                if (votePartakeUsers != null && votePartakeUsers.Count > 0)
                {
                    db.AddRange(votePartakeUsers);
                }
                db.Execute("DELETE FROM RF_VoteResultUser WHERE VoteId={0}", voteId);
                if (voteResultUsers != null && voteResultUsers.Count > 0)
                {
                    db.AddRange(voteResultUsers);
                }
                //更新问卷状态为已发布
                db.Execute("UPDATE RF_Vote SET Status=1,PublishTime={0} WHERE Id={1}", DateExtensions.Now, voteId);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 查询一个问卷的参与人员
        /// </summary>
        /// <param name="voteID"></param>
        /// <param name="status">-1查询所有 0未提交 1已提交</param>
        /// <returns></returns>
        public List<Model.VotePartakeUser> GetPartakeUsers(Guid voteID, int status = -1)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.VotePartakeUser>("SELECT * FROM RF_VotePartakeUser WHERE VoteId={0}"
                    + (status != -1 ? " AND Status=" + status.ToString() : ""), voteID);
            }
        }
    }
}
