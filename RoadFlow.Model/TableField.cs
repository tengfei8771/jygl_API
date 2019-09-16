using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model
{
    /// <summary>
    /// 表字段实体
    /// </summary>
    public class TableField
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool IsNull { get; set; }

        /// <summary>
        /// 是否有默认值
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 是否是自增
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        public string Comment { get; set; }
    }

}
