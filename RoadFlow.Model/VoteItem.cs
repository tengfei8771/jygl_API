using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_VoteItem")]
    public class VoteItem
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
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "标题不能为空")]
        [Column("ItemTitle")]
        [DisplayName("标题")]
        public string ItemTitle { get; set; }

        /// <summary>
        /// 选择方式 0 不是选择 1 单选 2多选
        /// </summary>
        [Required(ErrorMessage = "选择方式 0 不是选择 1 单选 2多选不能为空")]
        [Column("SelectModel")]
        [DisplayName("选择方式 0 不是选择 1 单选 2多选")]
        public int SelectModel { get; set; }

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
