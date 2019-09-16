using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RoadFlow.Model
{
    [Table("RF_MenuUser")]
    [Serializable]
    public class MenuUser
    {
        /// <summary>
		/// Id
		/// </summary>
		[DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>
        [DisplayName("菜单ID")]
        [Required(ErrorMessage = "菜单ID不能为空")]
        public Guid MenuId { get; set; }

        /// <summary>
        /// 使用对象（组织机构ID）
        /// </summary>
        [DisplayName("使用对象（组织机构ID）")]
        public string Organizes { get; set; }

        /// <summary>
        /// 使用人员，人员ID
        /// </summary>
        [DisplayName("使用人员，人员ID")]
        public string Users { get; set; }

        /// <summary>
        /// 可使用的按钮
        /// </summary>
        [DisplayName("可使用的按钮")]
        public string Buttons { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        [DisplayName("参数")]
        public string Params { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
