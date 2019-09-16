using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_Message")]
    public class Message
    {
        /// <summary>
		/// Id
		/// </summary>
		[Required(ErrorMessage = "Id不能为空")]
        [DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [Required(ErrorMessage = "消息内容不能为空")]
        [DisplayName("消息内容")]
        public string Contents { get; set; }

        /// <summary>
        /// 发送方式 0站内消息 1手机短信 2微信 
        /// </summary>
        [Required(ErrorMessage = "发送方式不能为空")]
        [DisplayName("发送方式 0站内消息 1手机短信 2微信 ")]
        public string SendType { get; set; }

        /// <summary>
        /// 是否是站内短信（把发送类型分开是为了提高查询效率）
        /// </summary>
        [Required(ErrorMessage = "是否是站内短信不能为空")]
        [DisplayName("是否是站内短信（把发送类型分开是为了提高查询效率）")]
        public int SiteMessage { get; set; }

        /// <summary>
        /// 发送人
        /// </summary>
        [DisplayName("发送人")]
        public Guid? SenderId { get; set; }

        /// <summary>
        /// 发送人姓名
        /// </summary>
        [DisplayName("发送人姓名")]
        public string SenderName { get; set; }

        /// <summary>
        /// 接收人组织机构字符串
        /// </summary>
        [Required(ErrorMessage = "接收人组织机构字符串不能为空")]
        [DisplayName("接收人组织机构字符串")]
        public string ReceiverIdString { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        [Required(ErrorMessage = "发送时间不能为空")]
        [DisplayName("发送时间")]
        public DateTime SendTime { get; set; }

        /// <summary>
        /// 0:用户发送消息 1：系统消息
        /// </summary>
        [Required(ErrorMessage = "消息类型不能为空")]
        [DisplayName("0:用户发送消息 1：系统消息")]
        public int Type { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        [DisplayName("附件")]
        public string Files { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
