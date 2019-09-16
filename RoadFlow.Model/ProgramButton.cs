using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_ProgramButton")]
    public class ProgramButton
    {
        /// <summary>
		/// Id
		/// </summary>
		[Required]
        [DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 程序ID
        /// </summary>
        [Required]
        [DisplayName("程序ID")]
        public Guid ProgramId { get; set; }

        /// <summary>
        /// 系统按钮库ID
        /// </summary>
        [DisplayName("系统按钮库ID")]
        public Guid? ButtonId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [DisplayName("名称")]
        public string ButtonName { get; set; }

        /// <summary>
        /// 脚本
        /// </summary>
        [DisplayName("脚本")]
        public string ClientScript { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [DisplayName("图标")]
        public string Ico { get; set; }

        /// <summary>
        /// 显示类型 0工具栏按钮 1普通按钮 2列表按键
        /// </summary>
        [Required]
        [DisplayName("显示类型 0工具栏按钮 1普通按钮 2列表按键")]
        public int ShowType { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Required]
        [DisplayName("序号")]
        public int Sort { get; set; }

        /// <summary>
        /// 是否验证权限
        /// </summary>
        [Required]
        [DisplayName("是否验证权限")]
        public int IsValidateShow { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
