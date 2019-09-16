using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_Log")]
    [Serializable]
    public class Log
    {
        /// <summary>
		/// Id
		/// </summary>
		[DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [DisplayName("标题")]
        [Required(ErrorMessage = "标题不能为空")]
        public string Title { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [DisplayName("类型")]
        [Required(ErrorMessage = "类型不能为空")]
        public string Type { get; set; }

        /// <summary>
        /// 写入时间
        /// </summary>
        [DisplayName("写入时间")]
        [Required(ErrorMessage = "写入时间不能为空")]
        [DataType(DataType.DateTime)]
        public DateTime WriteTime { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DisplayName("用户ID")]
        public Guid? UserId { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [DisplayName("用户姓名")]
        public string UserName { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        [DisplayName("IP")]
        public string IPAddress { get; set; }
        /// <summary>
        /// 来源URL
        /// </summary>
        public string Referer { get; set; }
        /// <summary>
        /// 发生URL
        /// </summary>
        [DisplayName("发生URL")]
        public string URL { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [DisplayName("内容")]
        public string Contents { get; set; }

        /// <summary>
        /// 其它
        /// </summary>
        [DisplayName("其它")]
        public string Others { get; set; }

        /// <summary>
        /// 更改后
        /// </summary>
        [DisplayName("更改后")]
        public string NewContents { get; set; }

        /// <summary>
        /// 更改前
        /// </summary>
        [DisplayName("更改前")]
        public string OldContents { get; set; }

        /// <summary>
        /// 浏览器信息
        /// </summary>
        [DisplayName("浏览器信息")]
        public string BrowseAgent { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
