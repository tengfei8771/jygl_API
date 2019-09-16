using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_Dictionary")]
    [Serializable]
    public class Dictionary
    {
        /// <summary>
		/// Id
		/// </summary>
		[DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        [DisplayName("上级ID")]
        [Required(ErrorMessage = "上级Id不能为空")]
        public Guid ParentId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [DisplayName("标题")]
        [Required(ErrorMessage = "标题不能为空")]
        public string Title { get; set; }

        /// <summary>
        /// 唯一代码
        /// </summary>
        [DisplayName("唯一代码")]
        public string Code { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [DisplayName("值")]
        public string Value { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        public string Note { get; set; }

        /// <summary>
        /// 其它信息
        /// </summary>
        [DisplayName("其它信息")]
        public string Other { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        public int Sort { get; set; }

        /// <summary>
        /// 0 正常 1 删除
        /// </summary>
        [DisplayName("0 正常 1 删除")]
        public int Status { get; set; }

        /// <summary>
        /// 标题_英语
        /// </summary>
        public string Title_en { get; set; }

        /// <summary>
        /// 标题_繁体中文
        /// </summary>
        public string Title_zh { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
