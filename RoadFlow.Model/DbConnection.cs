using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_DbConnection")]
    public class DbConnection
    {
        /// <summary>
		/// Id
		/// </summary>
		[DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 连接名称
        /// </summary>
        [DisplayName("连接名称")]
        [Required(ErrorMessage = "连接名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 连接类型
        /// </summary>
        [DisplayName("连接类型")]
        [Required(ErrorMessage = "连接类型不能为空")]
        public string ConnType { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        [DisplayName("连接字符串")]
        [Required(ErrorMessage = "连接字符串不能为空")]
        public string ConnString { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        public string Note { get; set; }

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
