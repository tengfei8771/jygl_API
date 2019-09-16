using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable]
    [Table("RF_ProgramValidate")]
    public class ProgramValidate
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Id不能为空")]
        [DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// ProgramId
        /// </summary>
        [Required(ErrorMessage = "ProgramId不能为空")]
        [DisplayName("ProgramId")]
        public Guid ProgramId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [Required(ErrorMessage = "表名不能为空")]
        [DisplayName("表名")]
        public string TableName { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        [Required(ErrorMessage = "字段名不能为空")]
        [DisplayName("字段名")]
        public string FieldName { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        [DisplayName("字段说明")]
        public string FieldNote { get; set; }

        /// <summary>
        /// 验证类型 0不检查 1允许为空,非空时检查 2不允许为空,并检查
        /// </summary>
        [Required(ErrorMessage = "验证类型 0不检查 1允许为空,非空时检查 2不允许为空,并检查不能为空")]
        [DisplayName("验证类型 0不检查 1允许为空,非空时检查 2不允许为空,并检查")]
        public int Validate { get; set; }

        /// <summary>
        /// 状态 0编辑 1只读 2隐藏
        /// </summary>
        public int Status { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
