using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;

namespace RoadFlow.Business.WeiXin
{
    public class Common
    {
        private static string Access_Token = string.Empty;
        /// <summary>
        /// 过期时间
        /// </summary>
        private static DateTime LastTime = DateExtensions.Now;
        public static string GetAccessToken()
        {
            if (LastTime > DateExtensions.Now && !Access_Token.IsNullOrWhiteSpace())
            {
                return Access_Token;
            }
            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + Config.WeiXin_AppId + "&secret=" + Config.WeiXin_AppSecret;
            string json = HttpHelper.HttpGet(url);
            JObject jObject = JObject.Parse(json);
            if (jObject.ContainsKey("errcode"))
            {
                Log.Add("get access_token err", json);
                return string.Empty;
            }
            string access_token = jObject.Value<string>("access_token");
            string expires_in = jObject.Value<string>("expires_in");
            if (access_token.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            Access_Token = access_token;
            LastTime = DateExtensions.Now.AddSeconds(expires_in.ToInt(0));
            return Access_Token;
        }
    }
}
