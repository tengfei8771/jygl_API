using RoadFlow.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Data
{
    public class VoteItemOption
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.VoteItemOption Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.VoteItemOption>(id);
            }
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="voteItemOption"></param>
        /// <returns></returns>
        public int Add(Model.VoteItemOption voteItemOption)
        {
            using (var db = new DataContext())
            {
                db.Add(voteItemOption);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="voteItemOption">实体</param>
        public int Update(Model.VoteItemOption voteItemOption)
        {
            using (var db = new DataContext())
            {
                db.Update(voteItemOption);
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
                db.Execute("DELETE FROM RF_VoteItemOption WHERE Id={0}", id);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 得到选题的选项
        /// </summary>
        /// <param name="itemId">选题ID</param>
        /// <returns></returns>
        public List<Model.VoteItemOption> GetItemOptions(Guid itemId)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.VoteItemOption>("SELECT * FROM RF_VoteItemOption WHERE ItemId={0}", itemId);
            }
        }

        /// <summary>
        /// 得到整个问卷的选项
        /// </summary>
        /// <param name="voteId">问卷ID</param>
        /// <returns></returns>
        public List<Model.VoteItemOption> GetVoteItemOptions(Guid voteId)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.VoteItemOption>("SELECT * FROM RF_VoteItemOption WHERE VoteId={0}", voteId);
            }
        }
    }
}
