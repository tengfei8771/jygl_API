using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_FlowButton")]
    public class FlowButton
    {
        /// <summary>
		/// Id
		/// </summary>
		[DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 按钮标题
        /// </summary>
        [DisplayName("按钮标题")]
        [Required(ErrorMessage = "按钮标题不能为空")]
        public string Title { get; set; }

        /// <summary>
        /// 按钮图标
        /// </summary>
        [DisplayName("按钮图标")]
        public string Ico { get; set; }

        /// <summary>
        /// 按钮脚本
        /// </summary>
        [DisplayName("按钮脚本")]
        public string Script { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [DisplayName("备注说明")]
        public string Note { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        public int Sort { get; set; }

        /// <summary>
        /// 英文标题
        /// </summary>
        public string Title_en { get; set; }

        /// <summary>
        /// 繁体中文标题
        /// </summary>
        public string Title_zh { get; set; }

        /// <summary>
        /// 备注英语
        /// </summary>
        public string Note_en { get; set; }

        /// <summary>
        /// 备注繁体中文
        /// </summary>
        public string Note_zh { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public FlowButton Clone()
        {
            return (FlowButton)MemberwiseClone();
        }
    }
}
