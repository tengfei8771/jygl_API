using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_MailInBox")]
    public class MailInBox
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
        /// 接收人
        /// </summary>
        [Required(ErrorMessage = "接收人不能为空")]
        [Column("UserId")]
        [DisplayName("接收人")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 发送人
        /// </summary>
        [Required(ErrorMessage = "发送人不能为空")]
        [Column("SendUserId")]
        [DisplayName("发送人")]
        public Guid SendUserId { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        [Required(ErrorMessage = "发送时间不能为空")]
        [Column("SendDateTime")]
        [DisplayName("发送时间")]
        public DateTime SendDateTime { get; set; }

        /// <summary>
        /// 邮件内容Id
        /// </summary>
        [Required(ErrorMessage = "邮件内容Id不能为空")]
        [Column("ContentsId")]
        [DisplayName("邮件内容Id")]
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

        /// <summary>
        /// 邮件类型 1发送 2抄送 3密送
        /// </summary>
        [Required(ErrorMessage = "邮件类型不能为空")]
        [Column("MailType")]
        [DisplayName("邮件类型")]
        public int MailType { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
