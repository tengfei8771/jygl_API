using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_FlowTask")]
    public class FlowTask : IEqualityComparer<FlowTask>
    {
        /// <summary>
		/// 任务ID
		/// </summary>
		[DisplayName("任务ID")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 上一任务ID
        /// </summary>
        [DisplayName("上一任务ID")]
        public Guid PrevId { get; set; }

        /// <summary>
        /// 上一步骤ID
        /// </summary>
        [DisplayName("上一步骤ID")]
        public Guid PrevStepId { get; set; }

        /// <summary>
        /// 流程ID
        /// </summary>
        [DisplayName("流程ID")]
        public Guid FlowId { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [DisplayName("流程名称")]
        public string FlowName { get; set; }

        /// <summary>
        /// 步骤ID
        /// </summary>
        [DisplayName("步骤ID")]
        public Guid StepId { get; set; }

        /// <summary>
        /// 步骤名称
        /// </summary>
        [DisplayName("步骤名称")]
        public string StepName { get; set; }

        /// <summary>
        /// 对应业务表主键值
        /// </summary>
        [DisplayName("对应业务表主键值")]
        public string InstanceId { get; set; }

        /// <summary>
        /// 分组ID
        /// </summary>
        [DisplayName("分组ID")]
        public Guid GroupId { get; set; }

        /// <summary>
        /// 任务类型 0常规 1指派 2委托 3转交 4退回 5抄送 6前加签 7后加签 8并签 9跳转
        /// </summary>
        [DisplayName("任务类型 0常规 1指派 2委托 3转交 4退回 5抄送 6前加签 7后加签 8并签 9跳转")]
        public int TaskType { get; set; }

        /// <summary>
        /// 任务标题
        /// </summary>
        [DisplayName("任务标题")]
        public string Title { get; set; }

        /// <summary>
        /// 发送人ID(如果是兼职岗位R_关系表ID)
        /// </summary>
        [DisplayName("发送人ID(如果是兼职岗位R_关系表ID)")]
        public Guid SenderId { get; set; }

        /// <summary>
        /// 发送人姓名
        /// </summary>
        [DisplayName("发送人姓名")]
        public string SenderName { get; set; }

        /// <summary>
        /// 接收人ID
        /// </summary>
        [DisplayName("接收人ID")]
        public Guid ReceiveId { get; set; }

        /// <summary>
        /// 接收人姓名
        /// </summary>
        [DisplayName("接收人姓名")]
        public string ReceiveName { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        [DisplayName("接收时间")]
        public DateTime ReceiveTime { get; set; }

        /// <summary>
        /// 打开时间
        /// </summary>
        [DisplayName("打开时间")]
        public DateTime? OpenTime { get; set; }

        /// <summary>
        /// 要求完成时间
        /// </summary>
        [DisplayName("要求完成时间")]
        public DateTime? CompletedTime { get; set; }

        /// <summary>
        /// 实际完成时间
        /// </summary>
        [DisplayName("实际完成时间")]
        public DateTime? CompletedTime1 { get; set; }

        /// <summary>
        /// 处理意见
        /// </summary>
        [DisplayName("处理意见")]
        public string Comments { get; set; }

        /// <summary>
        /// 是否签章
        /// </summary>
        [DisplayName("是否签章")]
        public int IsSign { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        public string Note { get; set; }

        /// <summary>
        /// 子流程实例分组ID
        /// </summary>
        [DisplayName("子流程实例分组ID")]
        public string SubFlowGroupId { get; set; }

        /// <summary>
        /// 是否超时自动提交 0否 1是 2自动提交失败
        /// </summary>
        [DisplayName("是否超时自动提交 0否 1是 2自动提交失败")]
        public int IsAutoSubmit { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        [DisplayName("附件")]
        public string Attachment { get; set; }

        /// <summary>
        /// 任务状态 -1等待中 0未处理 1处理中 2已完成
        /// </summary>
        [DisplayName("任务状态 -1等待中 0未处理 1处理中 2已完成")]
        public int Status { get; set; }

        /// <summary>
        /// 任务顺序
        /// </summary>
        [DisplayName("任务顺序")]
        public int Sort { get; set; }

        /// <summary>
        /// 处理类型 处理类型 -1等待中 0未处理 1处理中 2已完成 3已退回 4他人已处理 5他人已退回 6已转交 7已委托 8已阅知 9已指派 10已跳转 11已终止 12他人已终止 13已加签
        /// </summary>
        [DisplayName("处理类型 处理类型 -1等待中 0未处理 1处理中 2已完成 3已退回 4他人已处理 5他人已退回 6已转交 7已委托 8已阅知 9已指派 10已跳转 11已终止 12他人已终止 13已加签")]
        public int ExecuteType { get; set; }

        /// <summary>
        /// 接收人所在机构ID（如果是兼职人员的情况下这里有值）
        /// </summary>
        [DisplayName("接收人所在机构ID（如果是兼职人员的情况下这里有值）")]
        public Guid? ReceiveOrganizeId { get; set; }

        /// <summary>
        /// 一个步骤内的处理顺序(选择人员顺序处理时的处理顺序)
        /// </summary>
        [DisplayName("一个步骤内的处理顺序(选择人员顺序处理时的处理顺序)")]
        public int StepSort { get; set; }

        /// <summary>
        /// 如果是委托任务，这里记录委托人员ID
        /// </summary>
        [DisplayName("如果是委托任务，这里记录委托人员ID")]
        public Guid? EntrustUserId { get; set; }

        /// <summary>
        /// 其它类型 1 子流程任务 
        /// <para>111前加签(所有人同意)</para> 
        /// <para>112前加签(一人同意)</para>
        /// <para>113前加签(顺序处理)</para>
        /// <para>121后加签(所有人同意)</para> 
        /// <para>122后加签(一人同意)</para> 
        /// <para>123后加签(顺序处理)</para> 
        /// <para>131并签(所有人同意)</para> 
        /// <para>132并签(一人同意)</para> 
        /// <para>133并签(顺序处理)</para>
        /// </summary>
        [DisplayName("其它类型")]
        public int OtherType { get; set; }

        /// <summary>
        /// 指定的后续步骤处理人
        /// </summary>
        [DisplayName("指定的后续步骤处理人")]
        public string NextStepsHandle { get; set; }

        /// <summary>
        /// 原步骤ID(动态步骤的原步骤ID)
        /// </summary>
        [DisplayName("原步骤ID(动态步骤的原步骤ID)")]
        public Guid? BeforeStepId { get; set; }
        /// <summary>
        /// 提醒时间(如果任务设置了超期提示)
        /// </summary>
        [DisplayName("提醒时间")]
        public DateTime? RemindTime { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public bool Equals(FlowTask u1, FlowTask u2)
        {
            return u1.Id == u2.Id;
        }

        public int GetHashCode(FlowTask u)
        {
            return u.GetHashCode();
        }

        /// <summary>
        /// 深度克隆
        /// </summary>
        /// <returns></returns>
        public FlowTask Clone()
        {
            return (FlowTask)MemberwiseClone();
        }
    }
}
