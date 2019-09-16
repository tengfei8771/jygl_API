using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_Form")]
    public class Form
    {
        /// <summary>
		/// ID
		/// </summary>
		[DisplayName("ID")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        [DisplayName("表单名称")]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 表单分类
        /// </summary>
        [DisplayName("表单分类")]
        [Required]
        public Guid FormType { get; set; }

        /// <summary>
		/// 创建人员ID
		/// </summary>
		[DisplayName("创建人员ID")]
        public Guid CreateUserId { get; set; }

        /// <summary>
        /// 创建人员姓名
        /// </summary>
        [DisplayName("创建人员姓名")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DisplayName("修改时间")]
        public DateTime EditDate { get; set; }

        /// <summary>
        /// 表单HTML
        /// </summary>
        [DisplayName("表单HTML")]
        public string Html { get; set; }

        /// <summary>
        /// 子表json
        /// </summary>
        [DisplayName("子表json")]
        public string SubtableJSON { get; set; }

        /// <summary>
        /// 事件json
        /// </summary>
        [DisplayName("事件json")]
        public string EventJSON { get; set; }

        /// <summary>
        /// 属性json
        /// </summary>
        [DisplayName("属性json")]
        public string attribute { get; set; }

        /// <summary>
        /// 状态：0 保存 1 编译 2作废
        /// </summary>
        [DisplayName("状态：0 保存 1 编译 2作废")]
        public int Status { get; set; }

        /// <summary>
        ///备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 生成后的HTML
        /// </summary>
        public string RunHtml { get; set; }

        /// <summary>
        /// 管理人员
        /// </summary>
        public string ManageUser { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
