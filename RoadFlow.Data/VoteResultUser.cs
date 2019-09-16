using RoadFlow.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Data
{
    public class VoteResultUser
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="voteResultUser"></param>
        /// <returns></returns>
        public int Add(Model.VoteResultUser voteResultUser)
        {
            using (var db = new DataContext())
            {
                db.Add(voteResultUser);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="voteResultUser">实体</param>
        public int Update(Model.VoteResultUser voteResultUser)
        {
            using (var db = new DataContext())
            {
                db.Update(voteResultUser);
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
                db.Execute("DELETE FROM RF_VoteResultUser WHERE Id={0}", id);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 得到一个问卷的用户
        /// </summary>
        /// <param name="voteId"></param>
        /// <returns></returns>
        public List<Model.VoteResultUser> GetVoteResultUsers(Guid voteId)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.VoteResultUser>("SELECT * FROM RF_VoteResultUser WHERE VoteId={0}", voteId);
            }
        }
    }
}
