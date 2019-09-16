using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RoadFlow.Utility;
using System;

namespace RoadFlow.Mvc
{
    /// <summary>
    /// 自定义验证属性类
    /// </summary>
    public class ValidateAttribute : Attribute , IActionFilter
    {
        /// <summary>
        /// 是否验证登录
        /// </summary>
        public bool CheckLogin { get; set; } = true;

        /// <summary>
        /// 是否验证权限
        /// </summary>
        public bool CheckApp { get; set; } = true;

        /// <summary>
        /// 是否验证URL
        /// </summary>
        public bool CheckUrl { get; set; } = true;

        /// <summary>
        /// 是否验证企业微信登录
        /// </summary>
        public bool CheckEnterPriseWeiXinLogin { get; set; } = false;

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Guid userId = Guid.Empty;
            bool isLogin = true;//PC端是否登录
            string loginIp = string.Empty;
            int loginType = Current.LoginType;
            //验证登录
            if (CheckLogin)
            {
                userId = Business.User.CurrentUserId;
                if (userId.IsEmptyGuid())
                {
                    isLogin = false;
                }
                else if(Config.SingleLogin && !Config.IsDebug && 0 == loginType)
                {
                    var ouModel = Business.OnlineUser.Get(userId);
                    string cookieUniqueId = context.HttpContext.Request.Cookies.TryGetValue("rf_login_uniqueid", out string v) ? v : "";
                    if (null == ouModel || !ouModel.UniqueId.Equals(cookieUniqueId))
                    {
                        isLogin = false;
                        loginIp = null == ouModel ? string.Empty : ouModel.IP;
                    }
                }
            }
            //验证企业微信登录
            if (userId.IsEmptyGuid() && CheckEnterPriseWeiXinLogin && !Business.EnterpriseWeiXin.Common.CheckLogin() && Current.WeiXinId.IsEmptyGuid())
            {
                context.Result = new ContentResult() { Content = "登录验证错误" };
                return;
            }
            if (!isLogin && 0 == loginType)
            {
                if (Tools.IsAjax(context.HttpContext.Request))//是ajax请求
                {
                    context.Result = new ContentResult() { Content = "{\"loginstatus\":-1, \"url\":\"\"}" };
                }
                else if (context.Controller.ToString().Contains("Controllers.HomeController"))//如果访问的是首页，则要转到登录页面
                {
                    context.Result = new ContentResult() { Content = "<script>" + (loginIp.IsNullOrWhiteSpace() ? "" : "alert('您的帐号已经在" + loginIp + "登录!');") + "top.location='" + Config.LoginUrl + "';</script>", ContentType = "text/html" };
                }
                else
                {
                    string lastURL = Tools.GetURL(context.HttpContext.Request);
                    context.Result = new ContentResult() { Content = "<script>" + (loginIp.IsNullOrWhiteSpace() ? "" : "alert('您的帐号已经在" + loginIp + "登录!');") + "top.lastURL='" + lastURL + "';top.roadflowCurrentWindow=window;top.login();</script>", ContentType = "text/html" };
                }
                return;
            }
            else
            {
                Business.OnlineUser.UpdateLast(userId);
            }

            //验证菜单权限
            if (CheckApp)
            {
                bool isApp = ValidateApp(context.HttpContext.Request.Querys("appid"), userId);
                if (!isApp)
                {
                    //context.Result = new ContentResult() { Content = "您没有权限使用该功能" };
                    return;
                }
            }
            //验证URL
            if (CheckUrl)
            {
                bool isUrl = ValidateURL(context.HttpContext.Request);
                if (!isUrl && !Config.IsDebug)
                {
                    context.Result = new ContentResult() { Content = "URL检查错误" };
                    return;
                }
            }
            
        }
        /// <summary>
        /// 验证是否有菜单使用权限
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool ValidateApp(string appId, Guid userId)
        {
            if (!appId.IsGuid(out Guid guid) || userId.IsEmptyGuid())
            {
                return false;
            }
            return new Business.MenuUser().GetListByMenuId(guid).Exists(p => p.Users.ContainsIgnoreCase(userId.ToString()));
        }
        /// <summary>
        /// 验证URL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool ValidateURL(HttpRequest request)
        {
            //return true;
            var referer = request.Headers["Referer"].ToString();
            if (referer.IsNullOrWhiteSpace())
            {
                return false;
            }
            //检查主机是否一至
            var host = request.Host.Host;
            Uri uri = new Uri(referer);
            bool check = host.EqualsIgnoreCase(uri.Host);
            if (!check)
            {
                Business.Log.Add("URL检查错误-" + host + "--" + uri.Host);
            }
            return check;
        }
        
    }
}
