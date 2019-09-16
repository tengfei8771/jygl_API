using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Business
{
    public class Doc
    {
        private readonly Data.Doc docData;
        public Doc()
        {
            docData = new Data.Doc();
        }
        /// <summary>
        /// 添加一个文档
        /// </summary>
        /// <param name="doc">文档实体</param>
        /// <param name="users">阅读人员</param>
        /// <returns></returns>
        public int Add(Model.Doc doc, List<Model.User> users = null)
        {
            return docData.Add(doc, users);
        }
        /// <summary>
        /// 查询一个文档
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Doc Get(Guid id)
        {
            return docData.Get(id);
        }
        /// <summary>
        /// 更新文档
        /// </summary>
        /// <param name="doc">文档实体</param>
        /// <param name="users">阅读人员</param>
        public int Update(Model.Doc doc, List<Model.User> users = null)
        {
            return docData.Update(doc, users);
        }
        /// <summary>
        /// 删除一个文档
        /// </summary>
        /// <param name="doc">文档实体</param>
        /// <returns></returns>
        public int Delete(Guid id)
        {
            return docData.Delete(id);
        }
        /// <summary>
        /// 更新阅读次数
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public int UpdateReadCount(Model.Doc doc)
        {
            return docData.UpdateReadCount(doc);
        }
        /// <summary>
        /// 得到文档等级下拉选择
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        public string GetRankOptions(int rank)
        {
            return new Dictionary().GetOptionsByCode("system_documentrank", Dictionary.ValueField.Value, rank.ToString(), false);
        }

        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="userId"></param>
        /// <param name="title"></param>
        /// <param name="dirId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerList(out int count, int size, int number, Guid userId, string title, string dirId, string date1, string date2, string order, int read)
        {
            return docData.GetPagerList(out count, size, number, userId, title, dirId, date1, date2, order, read);
        }
    }
}
