using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_FlowComment")]
    public class FlowComment : IEqualityComparer<FlowComment>
    {
        /// <summary>
		/// Id
		/// </summary>
		[DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 意见使用人
        /// </summary>
        [DisplayName("意见使用人")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 类型 0用户添加 1管理员添加
        /// </summary>
        [DisplayName("类型 0用户添加 1管理员添加")]
        public int AddType { get; set; }

        /// <summary>
        /// 意见
        /// </summary>
        [DisplayName("意见")]
        [Required(ErrorMessage = "意见不能为空")]
        public string Comments { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        public int Sort { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public bool Equals(FlowComment u1, FlowComment u2)
        {
            return u1.Comments == u2.Comments;
        }

        public int GetHashCode(FlowComment u)
        {
            return u.Comments.GetHashCode();
        }
    }
}
