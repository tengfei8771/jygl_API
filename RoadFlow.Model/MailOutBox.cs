using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_MailOutBox")]
    public class MailOutBox
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
        /// Subject
        /// </summary>
        [Required(ErrorMessage = "Subject不能为空")]
        [Column("Subject")]
        [DisplayName("Subject")]
        public string Subject { get; set; }

        /// <summary>
        /// 主题颜色
        /// </summary>
        [Column("SubjectColor")]
        [DisplayName("主题颜色")]
        public string SubjectColor { get; set; }

        /// <summary>
        /// 发送人ID
        /// </summary>
        [Required(ErrorMessage = "发送人ID不能为空")]
        [Column("UserId")]
        [DisplayName("发送人ID")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 接收人员(组织机构ID字符串)
        /// </summary>
        [Required(ErrorMessage = "接收人员(组织机构ID字符串)不能为空")]
        [Column("ReceiveUsers")]
        [DisplayName("接收人员(组织机构ID字符串)")]
        public string ReceiveUsers { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        [Required(ErrorMessage = "发送时间不能为空")]
        [Column("SendDateTime")]
        [DisplayName("发送时间")]
        public DateTime SendDateTime { get; set; }

        /// <summary>
        /// 邮件内容ID
        /// </summary>
        [Required(ErrorMessage = "邮件内容ID不能为空")]
        [Column("ContentsId")]
        [DisplayName("邮件内容ID")]
        public Guid ContentsId { get; set; }

        /// <summary>
        /// 0 草稿 1已发送
        /// </summary>
        [Required(ErrorMessage = "0 草稿 1已发送不能为空")]
        [Column("Status")]
        [DisplayName("0 草稿 1已发送")]
        public int Status { get; set; }

        /// <summary>
        /// 抄送
        /// </summary>
        [Column("CarbonCopy")]
        [DisplayName("抄送")]
        public string CarbonCopy { get; set; }

        /// <summary>
        /// 密送
        /// </summary>
        [Column("SecretCopy")]
        [DisplayName("密送")]
        public string SecretCopy { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
