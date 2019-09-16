using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 步骤实体
    /// </summary>
    public class Step
    {
        /// <summary>
        /// 步骤ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 步骤类型 0常规 1子流程
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 步骤名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否是动态步骤(0：不是(常规步骤)
        /// <para>1：动态步骤 </para>
        /// <para>2：动态步骤从字段值中获取，字段值中存:机构ID1|默认处理人ID1,机构ID2|默认处理人ID2)</para>
        /// <para>动态步骤是指步骤数量不固定，发送时可以自行添加。例如审批部门不确定的情况下，发送时自行添加要发送到几个部门审批。</para>
        /// </summary>
        public int Dynamic { get; set; }
        /// <summary>
        /// 动态步骤中设定的字段
        /// </summary>
        public string DynamicField { get; set; }
        /// <summary>
        /// 意见显示 0不显示 1显示
        /// </summary>
        public int CommentDisplay { get; set; }
        /// <summary>
        /// 签章意见时是否可传附件 0不可以 1可以
        /// </summary>
        public int Attachment { get; set; }
        /// <summary>
        /// 超期提醒0不提醒 1提前多少天提醒
        /// </summary>
        public int ExpiredPrompt { get; set; }
        /// <summary>
        /// 提前多少天提醒
        /// </summary>
        public decimal ExpiredPromptDays { get; set; } = 0;
        /// <summary>
        /// 审签类型 0无签批意见栏 1有签批意见(无须签章) 2有签批意见(须签章)
        /// </summary>
        public int SignatureType { get; set; }
        /// <summary>
        /// 工时（天）
        /// </summary>
        public decimal WorkTime { get; set; }
        /// <summary>
        /// 是否归档 0不归档 1要归档
        /// </summary>
        public int Archives { get; set; }
        /// <summary>
        /// 步骤说明
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 步骤发送后提示语
        /// </summary>
        public string SendShowMessage { get; set; }
        /// <summary>
        /// 步骤退回后提示语
        /// </summary>
        public string BackShowMessage { get; set; }
        /// <summary>
        /// X坐标
        /// </summary>
        public decimal Position_X { get; set; }
        /// <summary>
        /// Y坐标
        /// </summary>
        public decimal Position_Y { get; set; }
        /// <summary>
        /// 是否要在发送时指定接收人的完成时间
        /// </summary>
        public int SendSetWorkTime{ get; set; }
        /// <summary>
        /// 任务超时的处理方式 0不处理 1自动提交
        /// </summary>
        public int ExpiredExecuteModel { get; set; }
        /// <summary>
        /// 步骤运行时获取的默认处理人员
        /// </summary>
        public string RunDefaultMembers { get; set; }
        /// <summary>
        /// 数据编辑模式 0共同编辑 1独立编辑
        /// </summary>
        public int DataEditModel { get; set; } = 0;
        /// <summary>
        /// 步骤基本设置
        /// </summary>
        public StepBase StepBase { get; set; }
        /// <summary>
        /// 步骤表单
        /// </summary>
        public StepForm StepForm { get; set; }
        /// <summary>
        /// 步骤按钮
        /// </summary>
        public List<StepButton> StepButtons { get; set; }
        /// <summary>
        /// 步骤字段状态
        /// </summary>
        public List<StepFieldStatus> StepFieldStatuses { get; set; }
        /// <summary>
        /// 步骤事件
        /// </summary>
        public StepEvent StepEvent { get; set; }
        /// <summary>
        /// 步骤子流程设置
        /// </summary>
        public StepSubFlow StepSubFlow { get; set; }
        /// <summary>
        /// 步骤抄送设置
        /// </summary>
        public StepCopyFor StepCopyFor { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public Step Clone()
        {
            return (Step)MemberwiseClone();
        }
    }
}
