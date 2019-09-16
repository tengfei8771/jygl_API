using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 步骤表单实体
    /// </summary>
    public class StepForm
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 表单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 移动端表单ID
        /// </summary>
        public Guid MobileId { get; set; }
        /// <summary>
        /// 移动端表单名称
        /// </summary>
        public string MobileName { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
