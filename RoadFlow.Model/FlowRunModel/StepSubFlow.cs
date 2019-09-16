using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 步骤子流程实体
    /// </summary>
    public class StepSubFlow
    {
        /// <summary>
        /// 子流程ID
        /// </summary>
        public Guid SubFlowId { get; set; }
        /// <summary>
        /// 子流程策略，0子流程完成才能提交，1子流程发起即可提交，2子流程完成自动提交
        /// </summary>
        public int SubFlowStrategy { get; set; }
        /// <summary>
        /// 实例类型 0所有人同一实例 1每个人单独实例
        /// </summary>
        public int TaskType { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
