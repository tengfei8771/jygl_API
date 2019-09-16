using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 步骤按钮实体
    /// </summary>
    public class StepButton
    {
        /// <summary>
        /// 按钮ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 按钮说明
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 显示标题
        /// </summary>
        public string ShowTitle { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
