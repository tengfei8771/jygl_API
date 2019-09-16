using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_AppLibraryButton")]
    [Serializable]
    public class AppLibraryButton
    {
        /// <summary>
		/// Id
		/// </summary>
		[DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 应用程序库ID
        /// </summary>
        [DisplayName("AppLibraryId")]
        [Required(ErrorMessage = "应用程序库ID不能为空")]
        public Guid AppLibraryId { get; set; }

        [DisplayName("按钮库按钮ID")]
        public Guid? ButtonId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DisplayName("名称")]
        [Required(ErrorMessage = "名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 执行脚本
        /// </summary>
        [DisplayName("执行脚本")]
        public string Events { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [DisplayName("图标")]
        public string Ico { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        public int Sort { get; set; }

        /// <summary>
        /// 显示类型 0工具栏按钮 1普通按钮 2列表按键
        /// </summary>
        [DisplayName("显示类型 0工具栏按钮 1普通按钮 2列表按键")]
        public int ShowType { get; set; }

        /// <summary>
        /// 是否验证权限
        /// </summary>
        [DisplayName("是否验证权限")]
        public int IsValidateShow { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
