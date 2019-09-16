using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 步骤字段状态实体
    /// </summary>
    public class StepFieldStatus
    {
        /// <summary>
        /// 字段 连接ID.表名.字段名
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 状态 0编辑 1只读 2隐藏
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 数据检查 0不检查 1允许为空,非空时检查 2不允许为空,并检查
        /// </summary>
        public int Check { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
