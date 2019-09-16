using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_Flow")]
    public class Flow
    {
        /// <summary>
		/// Id
		/// </summary>
		[DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DisplayName("名称")]
        [Required(ErrorMessage = "名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [DisplayName("分类")]
        [Required(ErrorMessage = "分类不能为空")]
        public Guid FlowType { get; set; }

        /// <summary>
        /// 管理人员
        /// </summary>
        [DisplayName("管理人员")]
        public string Manager { get; set; }

        /// <summary>
        /// 实例管理人员
        /// </summary>
        [DisplayName("实例管理人员")]
        public string InstanceManager { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [DisplayName("创建日期")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建人员
        /// </summary>
        [DisplayName("创建人员")]
        public Guid CreateUser { get; set; }

        /// <summary>
        /// 设计时JSON
        /// </summary>
        [DisplayName("设计时JSON")]
        public string DesignerJSON { get; set; }

        /// <summary>
        /// 运行时JSON
        /// </summary>
        [DisplayName("运行时JSON")]
        public string RunJSON { get; set; }

        /// <summary>
        /// 安装日期
        /// </summary>
        [DisplayName("安装日期")]
        public DateTime? InstallDate { get; set; }

        /// <summary>
        /// 安装人员
        /// </summary>
        [DisplayName("安装人员")]
        public Guid? InstallUser { get; set; }

        /// <summary>
        /// 状态 0设计中 1已安装 2已卸载 3已删除
        /// </summary>
        [DisplayName("状态 0设计中 1已安装 2已卸载 3已删除")]
        public int Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        public string Note { get; set; }

        /// <summary>
        /// 所属系统Id
        /// </summary>
        [DisplayName("所属系统Id")]
        public Guid? SystemId { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
