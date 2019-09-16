using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using RoadFlow.Utility;
using Microsoft.AspNetCore.Hosting;

namespace RoadFlow.Mvc
{
    public class Current
    {
        /// <summary>
        /// 当前http请求
        /// </summary>
        public static HttpContext HttpContext
        {
            get
            {
                return Tools.HttpContext;
            }
        }

        /// <summary>
        /// 当前登录用户ID
        /// </summary>
        public static Guid UserId
        {
            get
            {
                return UserIdOrWeiXinId;
            }
        }

        /// <summary>
        /// 当前登录用户ID,如果PC端没有则从微信端取
        /// </summary>
        public static Guid UserIdOrWeiXinId
        {
            get
            {
                Guid userId = Business.User.CurrentUserId;
                return userId.IsEmptyGuid() ? EnterpriseWeiXinUserId : userId;
            }
        }

        /// <summary>
        /// 当前登录用户实体
        /// </summary>
        public static Model.User User
        {
            get
            {
                var userModel = Business.User.CurrentUser;
                if (null != userModel)
                {
                    return userModel;
                }
                return Business.EnterpriseWeiXin.Common.GetUser();//如果pc为空则从微信登录信息中获取
            }
        }

        /// <summary>
        /// 当前登录用户姓名
        /// </summary>
        public static string UserName
        {
            get
            {
                var userModel = User;
                return null == userModel ? string.Empty : userModel.Name;
            }
        }

        /// <summary>
        /// web目录绝对路径(包含wwwroot)
        /// </summary>
        public static string WebRootPath
        {
            get
            {
                return Tools.GetWebRootPath();
            }
        }

        /// <summary>
        /// 得到站点目录绝对路径
        /// </summary>
        public static string ContentRootPath
        {
            get
            {
                return Tools.GetContentRootPath();
            }
        }

        /// <summary>
        /// 当前日期时间
        /// </summary>
        public static DateTime DateTime
        {
            get
            {
                return DateExtensions.Now;
            }
        }

        /// <summary>
        /// 当前企业微信登录用户Id
        /// </summary>
        public static Guid EnterpriseWeiXinUserId
        {
            get
            {
                return Business.EnterpriseWeiXin.Common.GetUserId();
            }
        }

        /// <summary>
        /// 微信公众号ID
        /// </summary>
        public static Guid WeiXinId
        {
            get
            {
                string userId = HttpContext.Session.GetString("roadflow_weixin_userid");
                if(userId.IsGuid(out Guid uid))
                {
                    return uid;
                }
                if(!HttpContext.Request.Cookies.TryGetValue("roadflow_weixin_openid", out string openId) || openId.IsNullOrWhiteSpace())
                {
                    return Guid.Empty;
                }
                var userModel = new Business.User().GetByWeiXinOpenId(openId);
                if(null == userModel)
                {
                    return Guid.Empty;
                }
                HttpContext.Session.SetString("roadflow_weixin_userid", userModel.Id.ToString());
                return userModel.Id;
            }
        }

        /// <summary>
        /// 登录方式 0 PC 1 移动端
        /// </summary>
        public static int LoginType
        {
            get
            {
                return 0;
                int? type = HttpContext.Session.GetInt32("rf_login_type");
                if (type.HasValue)
                {
                    return type.Value;
                }
                else
                {
                    int type1 = Tools.IsPhoneAccess(HttpContext.Request) ? 1 : 0;
                    HttpContext.Session.SetInt32("rf_login_type", type1);
                    return type1;
                }
            }
        }

        /// <summary>
        /// 当前主机地址（如：http://localhost:80158）
        /// </summary>
        public static string BaseUrl
        {
            get
            {
                return Tools.GetHttpHost();
            }
        }

        /// <summary>
        /// 当前语言
        /// </summary>
        public static string Language
        {
            get
            {
                return "zh-CN";
            }
        }
    }
}
