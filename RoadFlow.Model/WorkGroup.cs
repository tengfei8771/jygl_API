using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_WorkGroup")]
    public class WorkGroup
    {
        /// <summary>
		/// Id
		/// </summary>
		[DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 工作组名称
        /// </summary>
        [DisplayName("工作组名称")]
        [Required(ErrorMessage = "工作组名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 工作组成员
        /// </summary>
        [DisplayName("工作组成员")]
        public string Members { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        public string Note { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 数字ID，用于微信等其它系统关联
        /// </summary>
        [DisplayName("数字ID，用于微信等其它系统关联")]
        public int IntId { get; set; }
    }
}
