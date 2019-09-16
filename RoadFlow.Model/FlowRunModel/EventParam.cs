using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 流程事件参数实体
    /// </summary>
    public class EventParam
    {
        /// <summary>
        /// 流程ID
        /// </summary>
        public Guid FlowId { get; set; }
        /// <summary>
        /// 步骤ID
        /// </summary>
        public Guid StepId { get; set; }
        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 分组ID
        /// </summary>
        public Guid GroupId { get; set; }
        /// <summary>
        /// 业务表ID值
        /// </summary>
        public string InstanceId { get; set; }
        /// <summary>
        /// 任务标题
        /// </summary>
        public string TaskTitle { get; set; }
        /// <summary>
        /// 流程运行时实体
        /// </summary>
        public FlowRun FlowRunModel { get; set; }
        /// <summary>
        /// 其它参数
        /// </summary>
        public object Other { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
