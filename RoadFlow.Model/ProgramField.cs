using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_ProgramField")]
    public class ProgramField
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
        [DisplayName("字段")]
        public string Field { get; set; }

        /// <summary>
        /// 显示标题
        /// </summary>
        [DisplayName("显示标题")]
        public string ShowTitle { get; set; }

        /// <summary>
        /// 对齐方式
        /// </summary>
        [Required(ErrorMessage = "对齐方式不能为空")]
        [DisplayName("对齐方式")]
        public string Align { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        [DisplayName("宽度")]
        public string Width { get; set; }

        /// <summary>
        /// 0直接输出 1序号 2日期时间 3数字 4数据字典ID显示标题  5组织机构id显示为名称 6自定义 100按钮列
        /// </summary>
        [Required(ErrorMessage = "显示类型不能为空")]
        [DisplayName("0直接输出 1序号 2日期时间 3数字 4数据字典ID显示标题  5组织机构id显示为名称 6自定义 100按钮列")]
        public int ShowType { get; set; }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        [DisplayName("格式化字符串")]
        public string ShowFormat { get; set; }

        /// <summary>
        /// 自定义字符串
        /// </summary>
        [DisplayName("自定义字符串")]
        public string CustomString { get; set; }

        /// <summary>
        /// 是否可以排序(jqgrid点击列排序)
        /// </summary>
        [DisplayName("是否可以排序(jqgrid点击列排序)")]
        public string IsSort { get; set; }

        /// <summary>
        /// 是否默认排序列
        /// </summary>
        [Required(ErrorMessage = "是否默认排序列不能为空")]
        [DisplayName("是否默认排序列")]
        public int IsDefaultSort { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Required(ErrorMessage = "排序不能为空")]
        [DisplayName("排序")]
        public int Sort { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
