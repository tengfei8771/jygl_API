using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 标识字段
    /// </summary>
    public class TitleField
    {
        /// <summary>
        /// 连接ID
        /// </summary>
        public Guid ConnectionId { get; set; }
        /// <summary>
        /// 表
        /// </summary>
        public string Table { get; set; }
        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 标识值
        /// </summary>
        public string Value { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
