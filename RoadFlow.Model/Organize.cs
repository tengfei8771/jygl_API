using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_Organize")]
    [Serializable]
    public class Organize
    {
        /// <summary>
		/// Id
		/// </summary>
		[DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        [DisplayName("父ID")]
        [Required(ErrorMessage = "父ID不能为空")]
        public Guid ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DisplayName("名称")]
        [Required(ErrorMessage = "名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 类型:1 单位 2 部门 3 岗位
        /// </summary>
        [DisplayName("类型:1 单位 2 部门 3 岗位")]
        public int Type { get; set; }

        /// <summary>
        /// 部门或岗位领导
        /// </summary>
        [DisplayName("部门或岗位领导")]
        public string Leader { get; set; }

        /// <summary>
        /// 分管领导
        /// </summary>
        [DisplayName("分管领导")]
        public string ChargeLeader { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        public string Note { get; set; }

        /// <summary>
        /// 状态  0 正常 1 冻结
        /// </summary>
        [DisplayName("状态  0 正常 1 冻结")]
        public int Status { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        public int Sort { get; set; }

        /// <summary>
        /// 这里为了其他系统调用，比如微信
        /// </summary>
        [DisplayName("这里为了其他系统调用，比如微信")]
        public int IntId { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
