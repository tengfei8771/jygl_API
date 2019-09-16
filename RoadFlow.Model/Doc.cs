using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_Doc")]
    public class Doc
    {
        /// <summary>
		/// Id
		/// </summary>
		[DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 栏目Id
        /// </summary>
        [DisplayName("栏目Id")]
        public Guid DirId { get; set; }

        /// <summary>
        /// 栏目名称
        /// </summary>
        [DisplayName("栏目名称")]
        public string DirName { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [DisplayName("标题")]
        [Required(ErrorMessage = "标题不能为空")]
        public string Title { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [DisplayName("来源")]
        public string Source { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [DisplayName("内容")]
        [Required(ErrorMessage = "内容不能为空")]
        public string Contents { get; set; }

        /// <summary>
        /// 相关附件
        /// </summary>
        [DisplayName("相关附件")]
        public string Files { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        [DisplayName("添加时间")]
        public DateTime WriteTime { get; set; }

        /// <summary>
        /// 添加人员Id
        /// </summary>
        [DisplayName("添加人员Id")]
        public Guid WriteUserID { get; set; }

        /// <summary>
        /// 添加人员姓名
        /// </summary>
        [DisplayName("添加人员姓名")]
        public string WriteUserName { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [DisplayName("最后修改时间")]
        public DateTime? EditTime { get; set; }

        /// <summary>
        /// 修改用户ID
        /// </summary>
        [DisplayName("修改用户ID")]
        public Guid? EditUserID { get; set; }

        /// <summary>
        /// 修改人姓名
        /// </summary>
        [DisplayName("修改人姓名")]
        public string EditUserName { get; set; }

        /// <summary>
        /// 阅读人员
        /// </summary>
        [DisplayName("阅读人员")]
        public string ReadUsers { get; set; }

        /// <summary>
        /// 阅读次数
        /// </summary>
        [DisplayName("阅读次数")]
        public int ReadCount { get; set; }

        /// <summary>
        /// 文档等级 0普通 1重要 2非常重要
        /// </summary>
        [DisplayName("文档等级 0普通 1重要 2非常重要")]
        public int DocRank { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
