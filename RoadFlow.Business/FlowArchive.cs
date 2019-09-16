using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Business
{
    public class FlowArchive
    {
        private readonly Data.FlowArchive flowArchiveData;
        public FlowArchive()
        {
            flowArchiveData = new Data.FlowArchive();
        }
        /// <summary>
        /// 查询一个归档
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.FlowArchive Get(Guid id)
        {
            return flowArchiveData.Get(id);
        }
        /// <summary>
        /// 添加一个归档
        /// </summary>
        /// <param name="flowArchive"></param>
        /// <returns></returns>
        public int Add(Model.FlowArchive flowArchive)
        {
            return flowArchiveData.Add(flowArchive);
        }
        /// <summary>
        /// 得到归档数据JSON
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetArchiveData(Guid id)
        {
            var model = Get(id);
            return null == model ? string.Empty : model.DataJson;
        }
        /// <summary>
        /// 得到处理意见HTML
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetArchiveComments(Guid id)
        {
            var model = Get(id);
            return null == model ? string.Empty : model.Comments;
        }
        /// <summary>
        /// 查询归档
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="flowId"></param>
        /// <param name="stepName"></param>
        /// <param name="title"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerData(out int count, int size, int number, string flowId, string stepName, string title, string date1, string date2, string order)
        {
            return flowArchiveData.GetPagerData(out count, size, number, flowId, stepName, title, date1, date2, order);
        }
    }
}
