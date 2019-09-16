using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RoadFlow.Business
{
    public class FlowComment
    {
        private readonly Data.FlowComment flowCommentData;
        public FlowComment()
        {
            flowCommentData = new Data.FlowComment();
        }
        /// <summary>
        /// 得到所有意见
        /// </summary>
        /// <returns></returns>
        public List<Model.FlowComment> GetAll()
        {
            return flowCommentData.GetAll();
        }
        /// <summary>
        /// 查询一个意见
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.FlowComment Get(Guid id)
        {
            return flowCommentData.GetAll().Find(p => p.Id == id);
        }
        /// <summary>
        /// 添加一个意见
        /// </summary>
        /// <param name="flowComment"></param>
        /// <returns></returns>
        public int Add(Model.FlowComment flowComment)
        {
            return flowCommentData.Add(flowComment);
        }
        /// <summary>
        /// 更新意见
        /// </summary>
        /// <param name="flowComment">意见实体</param>
        public int Update(Model.FlowComment flowComment)
        {
            return flowCommentData.Update(flowComment);
        }
        /// <summary>
        /// 删除一批意见
        /// </summary>
        /// <param name="flowComments">意见实体</param>
        /// <returns></returns>
        public int Delete(Model.FlowComment[] flowComments)
        {
            return flowCommentData.Delete(flowComments);
        }
        /// <summary>
        /// 得到最大序号
        /// </summary>
        /// <returns></returns>
        public int GetMaxSort()
        {
            var all = GetAll();
            return all.Count == 0 ? 5 : all.Max(p => p.Sort) + 5;
        }
        /// <summary>
        /// 得到一个用户可以使用的意见
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Model.FlowComment> GetListByUserId(Guid userId)
        {
            var all = GetAll();
            return all.FindAll(p => p.UserId == Guid.Empty || p.UserId == userId).Distinct(new Model.FlowComment()).ToList();
        }
        /// <summary>
        /// 得到一个用户的意见选项
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetOptionsByUserId(Guid userId)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var comments = GetListByUserId(userId);
            foreach (var comment in comments)
            {
                stringBuilder.Append("<option value=\"" + comment.Comments + "\">" + comment.Comments + "</option>");
            }
            return stringBuilder.ToString();
        }

    }
}
