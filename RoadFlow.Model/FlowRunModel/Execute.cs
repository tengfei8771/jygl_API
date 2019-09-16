using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 流程执行参数实体
    /// </summary>
    public class Execute
    {
        public Execute()
        {

        }
        /// <summary>
        /// 操作类型
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// 提交
            /// </summary>
            Submit,
            /// <summary>
            /// 自由发送
            /// </summary>
            FreeSubmit,
            /// <summary>
            /// 保存
            /// </summary>
            Save,
            /// <summary>
            /// 退回
            /// </summary>
            Back,
            /// <summary>
            /// 完成
            /// </summary>
            Completed,
            /// <summary>
            /// 转交
            /// </summary>
            Redirect,
            /// <summary>
            /// 加签
            /// </summary>
            AddWrite,
            /// <summary>
            /// 抄送完成
            /// </summary>
            CopyforCompleted,
            /// <summary>
            /// 终止
            /// </summary>
            TaskEnd
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Guid FlowId { get; set; }
        /// <summary>
        /// 步骤ID
        /// </summary>
        public Guid StepId { get; set; }
        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 实例ID
        /// </summary>
        public string InstanceId { get; set; }
        /// <summary>
        /// 分组ID
        /// </summary>
        public Guid GroupId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public Type ExecuteType  { get; set; }
        /// <summary>
        /// 发送人员
        /// </summary>
        public Model.User Sender { get; set; }
        /// <summary>
        /// 接收的步骤和人员和要求完成时间
        /// <para>stepId：步骤ID</para> 
        /// <para>stepName：步骤名称(如果为空根据步骤ID去流程实体中查询,主要是保存动态步骤中自定义的步骤名称)</para>  
        /// <para>beforeStepId：原步骤ID(动态步骤的原步骤ID)</para>  
        /// <para>parallelOrSerial：0并行 1串行</para>
        /// <para>completedTime：要求完成时间</para> 
        /// </summary>
        public List<(Guid stepId, string stepName, Guid? beforeStepId, int? parallelOrSerial, List<User> receiveUsers, DateTime? completedTime)> Steps { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 是否签章
        /// </summary>
        public int IsSign { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 其他类型
        /// </summary>
        public int OtherType { get; set; }
        /// <summary>
        /// 执行的参数json字符串
        /// </summary>
        public string ParamsJSON { get; set; }
        /// <summary>
        /// 是否是自动提交
        /// </summary>
        public bool IsAutoSubmit { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public string Attachment { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
