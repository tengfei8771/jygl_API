using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_MailContent")]
    public class MailContent
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
        /// 邮件内容
        /// </summary>
        [Required(ErrorMessage = "邮件内容不能为空")]
        [Column("Contents")]
        [DisplayName("邮件内容")]
        public string Contents { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        [Column("Files")]
        [DisplayName("附件")]
        public string Files { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
