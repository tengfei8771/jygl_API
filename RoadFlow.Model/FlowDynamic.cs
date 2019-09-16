using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_FlowDynamic")]
    public class FlowDynamic
    {
        /// <summary>
		/// 动态步骤ID
		/// </summary>
		[Key]
        [Required(ErrorMessage = "动态步骤ID不能为空")]
        [Column("StepId")]
        [DisplayName("动态步骤ID")]
        public Guid StepId { get; set; }

        /// <summary>
		/// 组ID
		/// </summary>
		[Key]
        [Required(ErrorMessage = "组ID不能为空")]
        [Column("GroupId")]
        [DisplayName("组ID")]
        public Guid GroupId { get; set; }

        /// <summary>
        /// 流程JSON
        /// </summary>
        [Required(ErrorMessage = "流程JSON不能为空")]
        [Column("FlowJSON")]
        [DisplayName("流程JSON")]
        public string FlowJSON { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
