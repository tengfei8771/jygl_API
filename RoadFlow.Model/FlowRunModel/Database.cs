using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 流程数据连接实体
    /// </summary>
    public class Database
    {
        /// <summary>
        /// 连接ID
        /// </summary>
        public Guid ConnectionId { get; set; }
        /// <summary>
        /// 连接名称
        /// </summary>
        public string ConnectionName { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string Table { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string PrimaryKey { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
