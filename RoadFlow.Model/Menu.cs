using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_Menu")]
    [Serializable]
    public class Menu
    {
        /// <summary>
        /// Id
        /// </summary>
        [DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 上级Id
        /// </summary>
        [DisplayName("上级Id")]
        [Required(ErrorMessage = "上级Id不能为空")]
        public Guid ParentId { get; set; }

        /// <summary>
        /// 应用程序库Id
        /// </summary>
        [DisplayName("应用程序库Id")]
        public Guid? AppLibraryId { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [DisplayName("菜单名称")]
        [Required(ErrorMessage = "菜单名称不能为空")]
        public string Title { get; set; }

        /// <summary>
        /// URL参数
        /// </summary>
        [DisplayName("URL参数")]
        public string Params { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [DisplayName("图标")]
        public string Ico { get; set; }

        /// <summary>
        /// 图标颜色
        /// </summary>
        [DisplayName("图标颜色")]
        public string IcoColor { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        public int Sort { get; set; }

        /// <summary>
        /// 英文标题
        /// </summary>
        public string Title_en { get; set; }

        /// <summary>
        /// 繁体中文标题
        /// </summary>
        public string Title_zh { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
