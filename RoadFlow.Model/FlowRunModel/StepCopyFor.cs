using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 步骤抄送实体
    /// </summary>
    public class StepCopyFor
    {
        /// <summary>
        /// 抄送组织机构人员
        /// </summary>
        public string MemberId { get; set; }
        /// <summary>
        /// 处理者类型
        /// </summary>
        public string HandlerType { get; set; }
        /// <summary>
        /// 处理者步骤
        /// </summary>
        public string Steps { get; set; }
        /// <summary>
        /// 方法或SQL
        /// </summary>
        public string MethodOrSql { get; set; }
        /// <summary>
        /// 抄送时间 0步骤接收时 1步骤完成时
        /// </summary>
        public int CopyforTime { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
