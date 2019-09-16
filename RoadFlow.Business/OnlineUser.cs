using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;
using System.Linq;

namespace RoadFlow.Business
{
    /// <summary>
    /// 在线用户管理类
    /// </summary>
    public class OnlineUser
    {
        /// <summary>
        /// 登录用户字典(key为userid+"_"+登录方式)
        /// </summary>
        public static Dictionary<string, Model.OnlineUser> OnlineUsers;
        /// <summary>
        /// 添加一个在线用户
        /// </summary>
        /// <param name="onlineUser"></param>
        public static void Add(Model.OnlineUser onlineUser)
        {
            string key = onlineUser.UserId.ToUpperString() + "_" + onlineUser.LoginType.ToString();
            if (null == OnlineUsers)
            {
                OnlineUsers = new Dictionary<string, Model.OnlineUser>
                {
                    { key, onlineUser }
                };
                return;
            }
            if (OnlineUsers.ContainsKey(key))
            {
                OnlineUsers[key] = onlineUser;
            }
            else
            {
                OnlineUsers.Add(key, onlineUser);
            }
        }
        /// <summary>
        /// 移出一个在线用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginType">登录方式,默认为-1,PC和移动端都移出</param>
        public static void Remove(Guid userId, int loginType = -1)
        {
            if (null == OnlineUsers)
            {
                return;
            }
            if (loginType == -1)
            {
                OnlineUsers.Remove(userId.ToUpperString() + "_0");
                OnlineUsers.Remove(userId.ToUpperString() + "_1");
            }
            else
            {
                string key = userId.ToUpperString() + "_" + loginType.ToString();
                OnlineUsers.Remove(key);
            }
        }
        /// <summary>
        /// 更新一个用户的最后活动时间和URL
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginType">登录方式</param>
        /// <param name="url"></param>
        public static void UpdateLast(Guid userId, int loginType = 0, string url = "")
        {
            if (null == OnlineUsers)
            {
                return;
            }
            string key = userId.ToUpperString() + "_" + loginType.ToString();
            if (OnlineUsers.ContainsKey(key))
            {
                OnlineUsers[key].LastTime = DateExtensions.Now;
                OnlineUsers[key].LastUrl = url.IsNullOrWhiteSpace() ? Tools.HttpContext.Request.Url() : url;
            }
        }
        /// <summary>
        /// 查询一个在线用户实体
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Model.OnlineUser Get(Guid userId, int loginType = 0)
        {
            string key = userId.ToUpperString() + "_" + loginType.ToString();
            return null == OnlineUsers || !OnlineUsers.ContainsKey(key) ? null : OnlineUsers[key];
        }
        /// <summary>
        /// 得到所有在线用户
        /// </summary>
        /// <returns></returns>
        public static List<Model.OnlineUser> GetAll()
        {
            return null == OnlineUsers ? new List<Model.OnlineUser>() : OnlineUsers.Values.ToList();
        }
        /// <summary>
        /// 在线用户数量(只统计PC)
        /// </summary>
        public static int Count()
        {
            if (null == OnlineUsers)
            {
                return 0;
            }
            return OnlineUsers.Values.Count(p => p.LoginType == 0);
        }
    }
}
