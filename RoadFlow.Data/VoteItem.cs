using RoadFlow.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Data
{
    public class VoteItem
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.VoteItem Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.VoteItem>(id);
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="voteItem"></param>
        /// <returns></returns>
        public int Add(Model.VoteItem voteItem)
        {
            using (var db = new DataContext())
            {
                db.Add(voteItem);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="voteItem">实体</param>
        public int Update(Model.VoteItem voteItem)
        {
            using (var db = new DataContext())
            {
                db.Update(voteItem);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isDeleteOptions">是否删除选项</param>
        /// <returns></returns>
        public int Delete(Guid id, bool isDeleteOptions = true)
        {
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_VoteItem WHERE Id={0}", id);
                if (isDeleteOptions)
                {
                    db.Execute("DELETE FROM RF_VoteItemOption WHERE VoteId={0}", id);
                }
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 根据问卷ID得到选题
        /// </summary>
        /// <param name="voteId"></param>
        /// <returns></returns>
        public List<Model.VoteItem> GetItems(Guid voteId)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.VoteItem>("SELECT * FROM RF_VoteItem WHERE VoteId={0}", voteId);
            }
        }
    }
}
