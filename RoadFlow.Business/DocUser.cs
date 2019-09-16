using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Business
{
    public class DocUser
    {
        private readonly Data.DocUser docUserData;
        public DocUser()
        {
            docUserData = new Data.DocUser();
        }
        /// <summary>
        /// 更新阅读人员状态
        /// </summary>
        /// <param name="docId">文档ID</param>
        /// <param name="userId">人员ID</param>
        /// <param name="isRead">是否阅读</param>
        public int UpdateIsRead(Guid docId, Guid userId, int isRead)
        {
            return docUserData.UpdateIsRead(docId, userId, isRead);
        }
        /// <summary>
        /// 判断一个人员一个文档是否已读
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsRead(Guid docId, Guid userId)
        {
            return docUserData.IsRead(docId, userId);
        }
        /// <summary>
        /// 删除一个人员记录
        /// </summary>
        /// <param name="userId">人员ID</param>
        /// <returns></returns>
            public int Delete(Guid userId)
        {
            return docUserData.Delete(userId);
        }
        /// <summary>
        /// 查询文档阅读情况
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="docId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetDocReadPagerList(out int count, int size, int number, string docId, string order)
        {
            return docUserData.GetDocReadPagerList(out count, size, number, docId, order);
        }
    }
}
