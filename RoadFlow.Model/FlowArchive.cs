using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_FlowArchive")]
    public class FlowArchive
    {
        /// <summary>
		/// Id
		/// </summary>
		[Required]
        [DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 流程ID
        /// </summary>
        [Required]
        [DisplayName("流程ID")]
        public Guid FlowId { get; set; }

        /// <summary>
        /// 步骤
        /// </summary>
        [Required]
        [DisplayName("步骤")]
        public Guid StepId { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [Required]
        [DisplayName("流程名称")]
        public string FlowName { get; set; }

        /// <summary>
        /// 步骤名称
        /// </summary>
        [Required]
        [DisplayName("步骤名称")]
        public string StepName { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        [Required]
        [DisplayName("任务ID")]
        public Guid TaskId { get; set; }

        /// <summary>
        /// 组
        /// </summary>
        [Required]
        [DisplayName("组")]
        public Guid GroupId { get; set; }

        /// <summary>
        /// 实例ID
        /// </summary>
        [Required]
        [DisplayName("实例ID")]
        public string InstanceId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        [DisplayName("标题")]
        public string Title { get; set; }

        /// <summary>
        /// 处理人ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 处理人姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [Required]
        [DisplayName("数据")]
        public string DataJson { get; set; }

        /// <summary>
        /// 处理意见HTML
        /// </summary>
        [DisplayName("处理意见HTML")]
        public string Comments { get; set; }

        /// <summary>
        /// 写入时间
        /// </summary>
        [Required]
        [DisplayName("写入时间")]
        public DateTime WriteTime { get; set; }

        /// <summary>
        /// 表单HTML
        /// </summary>
        public string FormHtml { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
