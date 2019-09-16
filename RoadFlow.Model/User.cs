using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    /// <summary>
    /// 用户实体类
    /// </summary>
    [Table("RF_User")]
    [Serializable]
    public class User : IEqualityComparer<User>
    {
        /// <summary>
        /// Id
        /// </summary>
        [DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayName("姓名")]
        [Required(ErrorMessage = "姓名不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 帐号
        /// </summary>
        [DisplayName("帐号")]
        [Required(ErrorMessage = "帐号不能为空")]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [DisplayName("密码")]
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }

        /// <summary>
        /// 性别 0男 1女
        /// </summary>
        [DisplayName("性别 0男 1女")]
        public int? Sex { get; set; }

        /// <summary>
        /// 状态 0 正常 1 冻结
        /// </summary>
        [DisplayName("状态 0 正常 1 冻结")]
        public int Status { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        [DisplayName("职务")]
        public string Job { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        public string Note { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [DisplayName("手机")]
        public string Mobile { get; set; }

        /// <summary>
        /// 办公电话
        /// </summary>
        [DisplayName("办公电话")]
        public string Tel { get; set; }

        /// <summary>
        /// 其它联系方式
        /// </summary>
        [DisplayName("其它联系方式")]
        public string OtherTel { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        [DisplayName("传真")]
        public string Fax { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [DisplayName("邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [DisplayName("QQ")]
        public string QQ { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [DisplayName("头像")]
        public string HeadImg { get; set; }

        /// <summary>
        /// 微信号
        /// </summary>
        [DisplayName("微信号")]
        public string WeiXin { get; set; }

        /// <summary>
        /// 人员兼职的机构ID(兼职时有用)(organizeuser表ID)
        /// </summary>
        [DisplayName("人员兼职的机构ID")]
        public Guid? PartTimeId { get; set; }

        /// <summary>
        /// 微信openid
        /// </summary>
        [DisplayName("微信openid")]
        public string WeiXinOpenId { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public bool Equals(User u1, User u2)
        {
            return u1.Id == u2.Id;
        }

        public int GetHashCode(User u)
        {
            return u.Id.GetHashCode();
        }

        public User Clone()
        {
            return (User)MemberwiseClone();
        }
    }
}
