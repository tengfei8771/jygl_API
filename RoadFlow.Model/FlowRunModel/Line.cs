using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 流程连线实体
    /// </summary>
    public class Line
    {
        /// <summary>
        /// 连线ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 来原步骤ID
        /// </summary>
        public Guid FromId { get; set; }
        /// <summary>
        /// 到步骤ID
        /// </summary>
        public Guid ToId { get; set; }
        /// <summary>
        /// 条件判断的方法
        /// </summary>
        public string CustomMethod { get; set; }
        /// <summary>
        /// 条件判断的SQL条件
        /// </summary>
        public string SqlWhere { get; set; }
        /// <summary>
        /// 条件判断的组织机构表达式
        /// </summary>
        public string OrganizeExpression { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
