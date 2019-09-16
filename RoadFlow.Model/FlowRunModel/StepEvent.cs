using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 步骤事件实体
    /// </summary>
    public class StepEvent
    {
        /// <summary>
        /// 步骤提交前事件
        /// </summary>
        public string SubmitBefore { get; set; }
        /// <summary>
        /// 步骤提交后事件
        /// </summary>
        public string SubmitAfter { get; set; }
        /// <summary>
        /// 步骤退回前事件
        /// </summary>
        public string BackBefore { get; set; }
        /// <summary>
        /// 步骤退回后事件
        /// </summary>
        public string BackAfter { get; set; }
        /// <summary>
        /// 子流程激活前事件
        /// </summary>
        public string SubFlowActivationBefore { get; set; }
        /// <summary>
        /// 子流程完成后事件
        /// </summary>
        public string SubFlowCompletedBefore { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
