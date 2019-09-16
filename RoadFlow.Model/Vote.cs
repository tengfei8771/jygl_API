using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_Vote")]
    public class Vote
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
        /// 主题
        /// </summary>
        [Required(ErrorMessage = "主题不能为空")]
        [Column("Topic")]
        [DisplayName("主题")]
        public string Topic { get; set; }

        /// <summary>
        /// 发起人
        /// </summary>
        [Required(ErrorMessage = "发起人不能为空")]
        [Column("CreateUserId")]
        [DisplayName("发起人")]
        public Guid CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required(ErrorMessage = "创建时间不能为空")]
        [Column("CreateTime")]
        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 参与人员
        /// </summary>
        [Required(ErrorMessage = "参与人员不能为空")]
        [Column("PartakeUsers")]
        [DisplayName("参与人员")]
        public string PartakeUsers { get; set; }

        /// <summary>
        /// 结果查看人员
        /// </summary>
        [Column("ResultViewUsers")]
        [DisplayName("结果查看人员")]
        public string ResultViewUsers { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Column("EndTime")]
        [DisplayName("结束时间")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("Note")]
        [DisplayName("备注")]
        public string Note { get; set; }

        /// <summary>
        /// 状态 0设计中 1已发布 2已有结果 3已完成(所有人提交)
        /// </summary>
        [Required(ErrorMessage = "状态不能为空")]
        [Column("Status")]
        [DisplayName("状态")]
        public int Status { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        [Column("PublishTime")]
        [DisplayName("发布时间")]
        public DateTime? PublishTime { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        [Column("Anonymous")]
        [DisplayName("是否匿名")]
        public int Anonymous { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
