using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_FlowApiSystem")]
    public class FlowApiSystem
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
        /// 系统名称
        /// </summary>
        [Required(ErrorMessage = "系统名称不能为空")]
        [Column("Name")]
        [DisplayName("系统名称")]
        public string Name { get; set; }

        /// <summary>
		/// 系统标识(不能重复)
		/// </summary>
		[Required(ErrorMessage = "系统标识不能为空")]
        [Column("SystemCode")]
        [DisplayName("系统标识")]
        public string SystemCode { get; set; }

        /// <summary>
		/// 系统标识(不能重复)
		/// </summary>
		[Required(ErrorMessage = "调用IP不能为空")]
        [Column("SystemIP")]
        [DisplayName("调用IP")]
        public string SystemIP { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("Note")]
        [DisplayName("备注")]
        public string Note { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Required(ErrorMessage = "排序不能为空")]
        [Column("Sort")]
        [DisplayName("排序")]
        public int Sort { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
