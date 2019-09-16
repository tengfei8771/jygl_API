using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_OrganizeUser")]
    [Serializable]
    public class OrganizeUser
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
		/// 组织机构Id
		/// </summary>
		[DisplayName("组织机构Id")]
        [Required]
        public Guid OrganizeId { get; set; }

        /// <summary>
        /// 人员Id
        /// </summary>
        [DisplayName("人员Id")]
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// 是否主要
        /// </summary>
        [DisplayName("是否主要")]
        public int IsMain { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        public int Sort { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

    }
}
