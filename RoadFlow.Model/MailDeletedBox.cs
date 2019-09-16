using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_MailDeletedBox")]
    public class MailDeletedBox
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
        [Column("Subject")]
        [DisplayName("主题")]
        public string Subject { get; set; }

        /// <summary>
        /// 主题颜色
        /// </summary>
        [Column("SubjectColor")]
        [DisplayName("主题颜色")]
        public string SubjectColor { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Required(ErrorMessage = "用户ID不能为空")]
        [Column("UserId")]
        [DisplayName("用户ID")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 发送人ID
        /// </summary>
        [Required(ErrorMessage = "发送人ID不能为空")]
        [Column("SendUserId")]
        [DisplayName("发送人ID")]
        public Guid SendUserId { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        [Required(ErrorMessage = "发送时间不能为空")]
        [Column("SendDateTime")]
        [DisplayName("发送时间")]
        public DateTime SendDateTime { get; set; }

        /// <summary>
        /// 内容ID
        /// </summary>
        [Required(ErrorMessage = "内容ID不能为空")]
        [Column("ContentsId")]
        [DisplayName("内容ID")]
        public Guid ContentsId { get; set; }

        /// <summary>
        /// 是否查看
        /// </summary>
        [Required(ErrorMessage = "是否查看不能为空")]
        [Column("IsRead")]
        [DisplayName("是否查看")]
        public int IsRead { get; set; }

        /// <summary>
        /// 查看时间
        /// </summary>
        [Column("ReadDateTime")]
        [DisplayName("查看时间")]
        public DateTime? ReadDateTime { get; set; }

        /// <summary>
        /// 发件ID
        /// </summary>
        [Column("OutBoxId")]
        [DisplayName("发件ID")]
        public Guid OutBoxId { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
