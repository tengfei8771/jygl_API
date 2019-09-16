using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    /// <summary>
    /// 可以参与调查的用户
    /// </summary>
    [Serializable]
    [Table("RF_VotePartakeUser")]
    public class VotePartakeUser
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
        /// UserId
        /// </summary>
        [Required(ErrorMessage = "UserId不能为空")]
        [Column("UserId")]
        [DisplayName("UserId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [Required(ErrorMessage = "用户姓名不能为空")]
        [Column("UserName")]
        [DisplayName("用户姓名")]
        public string UserName { get; set; }

        /// <summary>
        /// 用户所在组织
        /// </summary>
        [Required(ErrorMessage = "用户所在组织不能为空")]
        [Column("UserOrganize")]
        [DisplayName("用户所在组织")]
        public string UserOrganize { get; set; }

        /// <summary>
        /// 状态 0未提交 1已提交
        /// </summary>
        [Required(ErrorMessage = "状态不能为空")]
        [Column("Status")]
        [DisplayName("状态")]
        public int Status { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Column("SubmitTime")]
        [DisplayName("提交时间")]
        public DateTime? SubmitTime { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
