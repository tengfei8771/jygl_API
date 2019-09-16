using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_FlowEntrust")]
    public class FlowEntrust
    {
        /// <summary>
		/// Id
		/// </summary>
		[DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 委托人
        /// </summary>
        [DisplayName("委托人")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DisplayName("开始时间")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DisplayName("结束时间")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 委托流程ID,为空表示所有流程
        /// </summary>
        [DisplayName("委托流程ID,为空表示所有流程")]
        public Guid? FlowId { get; set; }

        /// <summary>
        /// 被委托人
        /// </summary>
        [DisplayName("被委托人")]
        public string ToUserId { get; set; }

        /// <summary>
        /// 设置时间
        /// </summary>
        [DisplayName("设置时间")]
        public DateTime WriteTime { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [DisplayName("备注说明")]
        public string Note { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
