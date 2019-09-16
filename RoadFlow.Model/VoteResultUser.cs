using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    /// <summary>
    /// 可以查看调查结果的用户
    /// </summary>
    [Serializable]
    [Table("RF_VoteResultUser")]
    public class VoteResultUser
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

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
