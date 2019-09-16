using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model
{
    /// <summary>
    /// 在线用户实体
    /// </summary>
    public class OnlineUser : IEqualityComparer<OnlineUser>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 唯一ID（验证是否是一个地方登录,用sessionid）
        /// </summary>
        public string UniqueId { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户所在组织
        /// </summary>
        public string UserOrganize { get; set; }
        /// <summary>
        /// 登录IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }
        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime LastTime { get; set; }
        /// <summary>
        /// 最后访问URL
        /// </summary>
        public string LastUrl { get; set; }
        /// <summary>
        /// 登录方式 0 PC 1 移动端
        /// </summary>
        public int LoginType { get; set; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string BrowseAgent { get; set;}

        public bool Equals(OnlineUser u1, OnlineUser u2)
        {
            return u1.UserId == u2.UserId;
        }

        public int GetHashCode(OnlineUser u)
        {
            return u.UserId.GetHashCode();
        }
    }
}
