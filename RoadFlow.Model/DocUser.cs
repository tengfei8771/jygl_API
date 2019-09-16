using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_DocUser")]
    public class DocUser
    {

        /// <summary>
        /// 文档id
        /// </summary>
        [DisplayName("文档id")]
        [Key]
        public Guid DocId { get; set; }

        /// <summary>
        /// 人员id
        /// </summary>
        [DisplayName("人员id")]
        [Key]
        public Guid UserId { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        [DisplayName("是否已读")]
        public int IsRead { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
