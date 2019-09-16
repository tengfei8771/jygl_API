using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RoadFlow.Model
{
    /// <summary>
    /// 流程运行时实体
    /// </summary>
    public class FlowRun
    {
        /// <summary>
        /// 流程ID
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 流程分类
        /// </summary>
        public Guid Type { get; set; }
        /// <summary>
        /// 流程管理人员
        /// </summary>
        public string Manager { get; set; }
        /// <summary>
        /// 流程实例管理人员
        /// </summary>
        public string InstanceManager { get; set; }
        /// <summary>
        /// 第一步ID
        /// </summary>
        public Guid FirstStepId { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 创建人员
        /// </summary>
        public Guid CreateUserId { get; set; }
        /// <summary>
        /// 设计时JSON
        /// </summary>
        public string DesignerJSON { get; set; }
        /// <summary>
        /// 安装日期
        /// </summary>
        public DateTime? InstallDate { get; set; }
        /// <summary>
        /// 安装人员ID
        /// </summary>
        public Guid? InstallUserId { get; set; }
        /// <summary>
        /// 运行时JSON
        /// </summary>
        public string RunJSON { get; set; }
        /// <summary>
        /// 流程图标(发起流程列表时用)
        /// </summary>
        public string Ico { get; set; }
        /// <summary>
        /// 图标颜色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 状态 0:设计中 1:已安装 2:已卸载 3:已删除
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 所属系统Id
        /// </summary>
        public Guid? SystemId { get; set; }
        /// <summary>
        /// 调试模式 0关闭 1开启(有调试窗口) 2开启(无调试窗口)
        /// </summary>
        public int Debug { get; set; }
        /// <summary>
        /// 调试人员ID
        /// </summary>
        public string DebugUserIds { get; set; }
        /// <summary>
        /// 流程数据连接信息
        /// </summary>
        public List<FlowRunModel.Database> Databases { get; set; }
        /// <summary>
        /// 标识字段
        /// </summary>
        public FlowRunModel.TitleField TitleField { get; set; }
        /// <summary>
        /// 步骤信息
        /// </summary>
        public List<FlowRunModel.Step> Steps { get; set; }
        /// <summary>
        /// 连线信息
        /// </summary>
        public List<FlowRunModel.Line> Lines { get; set; }


        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
