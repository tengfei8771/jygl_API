using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_VoteItemOption")]
    public class VoteItemOption
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Id不能为空")]
        [Column("Id")]
        [DisplayName("Id")]
        public Guid Id { get; set; }

        /// <summary>
		/// VoteId
		/// </summary>
		[Required(ErrorMessage = "VoteId不能为空")]
        [Column("VoteId")]
        [DisplayName("VoteId")]
        public Guid VoteId { get; set; }

        /// <summary>
        /// ItemId
        /// </summary>
        [Required(ErrorMessage = "ItemId不能为空")]
        [Column("ItemId")]
        [DisplayName("ItemId")]
        public Guid ItemId { get; set; }

        /// <summary>
        /// 选项标题
        /// </summary>
        [Required(ErrorMessage = "选项标题不能为空")]
        [Column("OptionTitle")]
        [DisplayName("选项标题")]
        public string OptionTitle { get; set; }

        /// <summary>
        /// 输入 1文本框 2文本域 0无
        /// </summary>
        [Required(ErrorMessage = "输入类型不能为空")]
        [Column("IsInput")]
        [DisplayName("输入类型")]
        public int IsInput { get; set; }

        /// <summary>
        /// 输入框样式
        /// </summary>
        [Column("InputStyle")]
        [DisplayName("输入框样式")]
        public string InputStyle { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Required(ErrorMessage = "排序不能为空")]
        [Column("Sort")]
        [DisplayName("排序")]
        public int Sort { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
