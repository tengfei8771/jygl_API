using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Business
{
    /// <summary>
    /// 流程委托类
    /// </summary>
    public class FlowEntrust
    {
        private Data.FlowEntrust flowEntrustData;
        public FlowEntrust()
        {
            flowEntrustData = new Data.FlowEntrust();
        }
        /// <summary>
        /// 得到所有委托
        /// </summary>
        /// <returns></returns>
        public List<Model.FlowEntrust> GetAll()
        {
            return flowEntrustData.GetAll();
        }
        /// <summary>
        /// 查询一个委托
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.FlowEntrust Get(Guid id)
        {
            return flowEntrustData.Get(id);
        }
        /// <summary>
        /// 添加一个委托
        /// </summary>
        /// <param name="appLibrary"></param>
        /// <returns></returns>
        public int Add(Model.FlowEntrust flowEntrust)
        {
            return flowEntrustData.Add(flowEntrust);
        }
        /// <summary>
        /// 更新委托
        /// </summary>
        /// <param name="flowEntrust">委托实体</param>
        public int Update(Model.FlowEntrust flowEntrust)
        {
            return flowEntrustData.Update(flowEntrust);
        }
        /// <summary>
        /// 删除一批委托
        /// </summary>
        /// <param name="flowEntrusts">委托实体</param>
        /// <returns></returns>
        public int Delete(Model.FlowEntrust[] flowEntrusts)
        {
            return flowEntrustData.Delete(flowEntrusts);
        }
        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable GetPagerList(out int count, int size, int number, string userId, string date1, string date2, string order)
        {
            return flowEntrustData.GetPagerList(out count, size, number, userId, date1, date2, order);
        }
        /// <summary>
        /// 得到一个人员一个流程的委托人员（如果没有则返回string.Empty）
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <param name="user">人员实体</param>
        /// <returns>如果没有委托则返回string.Empty</returns>
        public string GetEntrustUserId(Guid flowId, Model.User user)
        {
            var all = GetAll();
            DateTime now = Utility.DateExtensions.Now;
            var entrust = all.Find(p => p.UserId == user.Id && (!p.FlowId.HasValue || p.FlowId == flowId) &&
            p.StartTime <= now && p.EndTime >= now);
            return null == entrust ? string.Empty : entrust.ToUserId;
        }
    }
}
