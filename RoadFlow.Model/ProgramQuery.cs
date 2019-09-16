using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_ProgramQuery")]
    public class ProgramQuery
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Id不能为空")]
        [DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// ProgramId
        /// </summary>
        [Required(ErrorMessage = "ProgramId不能为空")]
        [DisplayName("ProgramId")]
        public Guid ProgramId { get; set; }

        /// <summary>
        /// 字段
        /// </summary>
        [Required(ErrorMessage = "字段不能为空")]
        [DisplayName("字段")]
        public string Field { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [DisplayName("显示名称")]
        public string ShowTitle { get; set; }

        /// <summary>
        /// 操作符
        /// </summary>
        [Required(ErrorMessage = "操作符不能为空")]
        [DisplayName("操作符")]
        public string Operators { get; set; }

        /// <summary>
        /// 控件名称
        /// </summary>
        [DisplayName("控件名称")]
        public string ControlName { get; set; }

        /// <summary>
        /// 输入类型 0文本 1日期 2日期范围 3日期时间 4日期时间范围 5下拉选择 6组织机构 7数据字典
        /// </summary>
        [Required(ErrorMessage = "输入类型不能为空")]
        [DisplayName("输入类型 0文本 1日期 2日期范围 3日期时间 4日期时间范围 5下拉选择 6组织机构 7数据字典")]
        public int InputType { get; set; }

        /// <summary>
        /// 显示样式
        /// </summary>
        [DisplayName("显示样式")]
        public string ShowStyle { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        [Required(ErrorMessage = "显示顺序不能为空")]
        [DisplayName("显示顺序")]
        public int Sort { get; set; }

        /// <summary>
        /// 数据来源 0.字符串表达式 1.数据字典 2.SQL
        /// </summary>
        [DisplayName("数据来源 0.字符串表达式 1.数据字典 2.SQL")]
        public int? DataSource { get; set; }

        /// <summary>
        /// DataSourceString
        /// </summary>
        [DisplayName("DataSourceString")]
        public string DataSourceString { get; set; }

        /// <summary>
        /// 数据来源为数据字典时的值
        /// </summary>
        public string DictValue { get; set; }

        /// <summary>
        /// 数据来源为SQL时的数据连接ID
        /// </summary>
        [DisplayName("数据来源为SQL时的数据连接ID")]
        public string ConnId { get; set; }

        /// <summary>
        /// 组织机构查询时是否将选择转换为人员
        /// </summary>
        [DisplayName("组织机构查询时是否将选择转换为人员")]
        public int? IsQueryUsers { get; set; }

        /// <summary>
        /// 当输入类型为组织机构时的属性
        /// </summary>
        public string OrgAttribute { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
