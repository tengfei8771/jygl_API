using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_DocDir")]
    public class DocDir
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
        public Guid ParentId { get; set; }

        /// <summary>
        /// 栏目名称
        /// </summary>
        [DisplayName("栏目名称")]
        public string Name { get; set; }

        /// <summary>
        /// 阅读人员
        /// </summary>
        [DisplayName("阅读人员")]
        public string ReadUsers { get; set; }

        /// <summary>
        /// 管理人员
        /// </summary>
        [DisplayName("管理人员")]
        public string ManageUsers { get; set; }

        /// <summary>
        /// 发布人员
        /// </summary>
        [DisplayName("发布人员")]
        public string PublishUsers { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        public int Sort { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
