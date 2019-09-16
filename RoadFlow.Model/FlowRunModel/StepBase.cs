using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 步骤基本设置
    /// </summary>
    public class StepBase
    {
        /// <summary>
        /// 流转类型 0系统控制 1单选一个分支流转 2多选几个分支流转(默认选中第一个) 3多选几个分支流转(默认全部选中)
        /// </summary>
        public int FlowType { get; set; }
        /// <summary>
        /// 运行时选择 0不允许 1允许
        /// </summary>
        public int RunSelect { get; set; }
        /// <summary>
        /// 处理者类型 0所有成员 1部门 2岗位 3工作组 4人员 5发起者 6前一步骤处理者 7某一步骤处理者 8字段值 9发起者主管 10发起者分管领导 11当前处理者主管 12当前处理者分管领导
        /// </summary>
        public string HandlerType { get; set; }
        /// <summary>
        /// 选择范围
        /// </summary>
        public string SelectRange { get; set; }
        /// <summary>
        /// 当处理者类型为 7某一步骤处理者 时的处理者步骤
        /// </summary>
        public Guid? HandlerStepId { get; set; }
        /// <summary>
        /// 当处理者类型为 8字段值 时的字段
        /// </summary>
        public string ValueField { get; set; }
        /// <summary>
        /// 默认处理者
        /// </summary>
        public string DefaultHandler { get; set; }
        /// <summary>
        /// 退回策略 0不能退回 1根据处理策略退回 2一人退回全部退回 3所有人退回才退回 4独立退回
        /// </summary>
        public int BackModel { get; set; }
        /// <summary>
        /// 处理策略 0所有人必须处理 1一人同意即可 2依据人数比例 3独立处理  4 按选择人员顺序处理
        /// </summary>
        public int HanlderModel { get; set; }
        /// <summary>
        /// 退回类型 0退回前一步 1退回第一步 2退回某一步
        /// </summary>
        public int BackType { get; set; }
        /// <summary>
        /// 是否可以在退回时选择接收人（默认是退回给上一步的发送人）
        /// </summary>
        public int BackSelectUser { get; set; }
        /// <summary>
        /// 策略百分比
        /// </summary>
        public decimal Percentage { get; set; }
        /// <summary>
        /// 退回步骤ID 当退回类型为 2退回某一步时
        /// </summary>
        public Guid? BackStepId { get; set; }
        /// <summary>
        /// 会签策略 0 不会签 1 所有步骤同意 2 一个步骤同意即可 3 依据比例
        /// </summary>
        public int Countersignature { get; set; }
        /// <summary>
        /// 步骤会签的起点步骤Id
        /// </summary>
        public Guid? CountersignatureStartStepId { get; set; }
        /// <summary>
        /// 会签策略是依据比例时设置的百分比
        /// </summary>
        public decimal CountersignaturePercentage { get; set; }
        /// <summary>
        /// 子流程处理策略 0 子流程完成后才能提交 1 子流程发起即可提交
        /// </summary>
        public int SubFlowStrategy { get; set; }
        /// <summary>
        /// 并发控制 0不控制 1控制
        /// </summary>
        public int ConcurrentModel { get; set; }
        /// <summary>
        /// 默认处理者SQL或方法
        /// </summary>
        public string DefaultHandlerSqlOrMethod { get; set; }
        /// <summary>
        /// 后续步骤有默认处理人直接发送(不需要点确定)
        /// </summary>
        public int AutoConfirm { get; set; }
        /// <summary>
        /// 发送人和接收人是同一人时跳过
        /// </summary>
        public int SkipIdenticalUser { get; set; }
        /// <summary>
        /// 根据方法跳过（方法返回1或true时跳过）
        /// </summary>
        public string SkipMethod { get; set; }
        /// <summary>
        /// 发送到退回步骤
        /// </summary>
        public int SendToBackStep { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
