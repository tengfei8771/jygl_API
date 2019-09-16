using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_MessageUser")]
    public class MessageUser
    {
        /// <summary>
        /// MessageId
        /// </summary>
        [Required(ErrorMessage = "MessageId不能为空")]
        [DisplayName("MessageId")]
        [Key]
        [Column(Order = 1)]
        public Guid MessageId { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        [Required(ErrorMessage = "UserId不能为空")]
        [DisplayName("UserId")]
        [Key]
        [Column(Order = 2)]
        public Guid UserId { get; set; }

        /// <summary>
        /// IsRead
        /// </summary>
        [Required(ErrorMessage = "IsRead不能为空")]
        [DisplayName("IsRead")]
        public int IsRead { get; set; }

        /// <summary>
        /// ReadTime
        /// </summary>
        [DisplayName("ReadTime")]
        public DateTime? ReadTime { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
