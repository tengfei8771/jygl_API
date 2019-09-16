using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 流程处理结果类
    /// </summary>
    public class ExecuteResult
    {
        public ExecuteResult()
        {
            NextTasks = new List<FlowTask>();
            AutoSubmitTasks = new List<FlowTask>();
        }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Messages { get; set; }
        /// <summary>
        /// 调试信息
        /// </summary>
        public string DebugMessages { get; set; }
        /// <summary>
        /// 其它信息
        /// </summary>
        public object Other { get; set; }
        /// <summary>
        /// 当前任务
        /// </summary>
        public FlowTask CurrentTask { get; set; }
        /// <summary>
        /// 后续任务
        /// </summary>
        public List<FlowTask> NextTasks { get; set; }
        /// <summary>
        /// 完成后要自动提交的任务
        /// </summary>
        public List<FlowTask> AutoSubmitTasks { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
