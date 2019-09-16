using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace RoadFlow.Business.EnterpriseWeiXin
{
    public class Common
    {
        /// <summary>
        /// 企业微信ID
        /// </summary>
        public static readonly string AppId = Config.Enterprise_WeiXin_AppId;
        /// <summary>
        /// 保存微信登录人员ID的SESSION KEY
        /// </summary>
        public static readonly string SessionKey = "roadflow_enterprise_userid";
        /// <summary>
        /// 得到访问access_token
        /// </summary>
        /// <param name="corpsecret">应用的凭证密钥</param>
        /// <returns></returns>
        public static string GetAccessToken(string corpsecret)
        {
            string cacheKey = "EnterpriseWeiXin_AccessToken_" + corpsecret;
            object obj = Cache.IO.Get(cacheKey);
            if (null != obj)
            {
                return obj.ToString();
            }
            string apiUrl = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=" + AppId + "&corpsecret=" + corpsecret;
            //返回JOSN样例{"errcode":0，"errmsg":""，"access_token": "accesstoken000001","expires_in": 7200}
            string returnJson = HttpHelper.HttpGet(apiUrl);
            JObject jObject = JObject.Parse(returnJson);
            if (jObject.Value<int>("errcode") == 0)
            {
                string access_token = jObject.Value<string>("access_token");
                int expires_in = jObject.Value<int>("expires_in");
                Cache.IO.Insert(cacheKey, access_token, DateExtensions.Now.AddSeconds(expires_in - 300));
                return access_token;
            }
            else
            {
                Log.Add("获取企业微信access_token错误", returnJson);
                return string.Empty;
            }
        }
        /// <summary>
        /// 向微信发送消息
        /// </summary>
        /// <param name="userAccounts">接收人员帐号，|分开</param>
        /// <param name="contentJson">消息内容JSON</param>
        /// <param name="msgType">消息类型(text,image,voice,video,file,textcard,news,mpnews)</param>
        /// <param name="agentId">接收消息的应用Id（如果为0则从数据字典应用中找标题为包含消息的应用）</param>
        /// <returns>返回空字符串表示成功，其它为错误信息</returns>
        public static string SendMessage(string userAccounts, JObject contentJson, string msgType = "text", int agentId = 0)
        {
            var dicts = new Dictionary().GetChilds("EnterpriseWeiXin");
            if (!dicts.Any())
            {
                return "未在数据字典中设置微信应用";
            }
            string corpsecret = string.Empty;
            if (agentId == 0)
            {
                var msgDict = dicts.Find(p => p.Title.Contains("消息"));
                if (null != msgDict)
                {
                    agentId = msgDict.Value.ToInt();
                    corpsecret = msgDict.Note.Trim1();
                }
                else
                {
                    return "未找到接收消息的应用";
                }
            }
            else
            {
                var dict = dicts.Find(p => p.Value.Equals(agentId.ToString()));
                if (null == dict)
                {
                    return "未找到" + agentId + "对应的应用";
                }
                corpsecret = dict.Note.Trim1();
            }
            string access_token = GetAccessToken(corpsecret);
            string url = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + access_token;
            JObject jObject = new JObject
            {
                { "msgtype", msgType },
                { "agentid", agentId },
                { msgType, contentJson},
                { "safe", 0 },
                { "touser", userAccounts }
            };
            string postJson = jObject.ToString(Newtonsoft.Json.Formatting.None);
            string returnJson = HttpHelper.HttpPost(url, postJson);
            JObject returnObject = JObject.Parse(returnJson);
            int errcode = returnObject.Value<int>("errcode");
            if (0 != errcode)
            {
                Log.Add("发送企业微信消息发生了错误", postJson, others: "返回:" + returnJson + "  URL:" + url);
                return returnObject.Value<string>("errmsg");
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 向微信发送消息
        /// </summary>
        /// <param name="receiveUsers">接收人员</param>
        /// <param name="contentJson">消息内容JSON</param>
        /// <param name="msgType">消息类型(text,image,voice,video,file,textcard,news,mpnews)</param>
        /// <param name="agentId">接收消息的应用Id（如果为0则从数据字典应用中找标题为包含消息的应用）</param>
        /// <returns>返回空字符串表示成功，其它为错误信息</returns>
        public static string SendMessage(List<Model.User> receiveUsers, JObject contentJson, string msgType = "text", int agentId = 0)
        {
            //每次最大只能发1000人，所以这里要分段
            List<List<Model.User>> toUsers = new List<List<Model.User>>();
            int skip = 1000;
            int number = 1;
            while (skip == 1000)
            {
                var userList = receiveUsers.Skip(number++ * 1000 - 1000).Take(1000).ToList();
                toUsers.Add(userList);
                skip = userList.Count;
            }
            bool success = true;
            foreach (var toUser in toUsers)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var user in toUser)
                {
                    stringBuilder.Append(user.Account);
                    stringBuilder.Append("|");
                }
                string msg = SendMessage(stringBuilder.ToString().TrimEnd('|'), contentJson, msgType, agentId);
                if (!msg.IsNullOrWhiteSpace() && success)
                {
                    success = false;
                }
            }
            return success ? string.Empty : "发送错误";
        }

        /// <summary>
        /// 向微信发送消息
        /// </summary>
        /// <param name="receiveUser">接收人员</param>
        /// <param name="contentJson">消息内容JSON</param>
        /// <param name="msgType">消息类型(text,image,voice,video,file,textcard,news,mpnews)</param>
        /// <param name="agentId">接收消息的应用Id（如果为0则从数据字典应用中找标题为包含消息的应用）</param>
        /// <returns>返回空字符串表示成功，其它为错误信息</returns>
        public static string SendMessage(Model.User receiveUser, JObject contentJson, string msgType = "text", int agentId = 0)
        {
            if (receiveUser.Mobile.IsNullOrWhiteSpace() && receiveUser.Email.IsNullOrWhiteSpace())
            {
                return receiveUser.Name + "未绑定微信!";
            }
            return SendMessage(new List<Model.User>() { receiveUser }, contentJson, msgType, agentId);
        }
        /// <summary>
        /// 得到当前登录用户ID
        /// </summary>
        /// <returns></returns>
        public static Guid GetUserId()
        {
            var httpContext = Tools.HttpContext;
            if (null == httpContext)
            {
                return Guid.Empty;
            }
            string id = httpContext.Session.GetString(SessionKey);
            if (id.IsGuid(out Guid uid))
            {
                return uid;
            }
            if (Config.IsDebug && Config.DebugUserId.IsGuid(out Guid debugUserId))
            {
                return debugUserId;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 得到当前登录用户实体
        /// </summary>
        /// <returns></returns>
        public static Model.User GetUser()
        {
            Guid userId = GetUserId();
            return userId.IsEmptyGuid() ? null : new User().Get(userId);
        }

        /// <summary>
        /// 验证登录
        /// </summary>
        /// <param name="isRedirect">如果没有登录是否跳转到微信登录</param>
        /// <returns></returns>
        public static bool CheckLogin(bool isRedirect = true)
        {
            Guid userId = GetUserId();
            if (userId.IsNotEmptyGuid())
            {
                return true;
            }
            if (!isRedirect)//如果不跳转到微信验证登录，直接返回false
            {
                return false;
            }
            var dicts = new Dictionary().GetChilds("EnterpriseWeiXin");
            if (!dicts.Any())
            {
                return false;
            }
            var httpContext = Tools.HttpContext;
            if (httpContext.Request.Path.HasValue)
            {
                httpContext.Session.SetString("EnterpriseWeiXin_LastURL", httpContext.Request.Url());
            }
            string agentId = dicts.First().Value.Trim1();
            string redirectUrl = (Config.Enterprise_WeiXin_WebUrl + "/RoadFlowCore/Mobile/GetUserAccount").UrlEncode();
            string url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + AppId + "&redirect_uri=" + redirectUrl + "&response_type=code&scope=snsapi_base&agentid=" + agentId + "&state=STATE#wechat_redirect";
            Tools.HttpContext.Response.Redirect(url);
            return false;
        }

        /// <summary>
        /// 根据微信返回code得到帐号
        /// </summary>
        /// <returns></returns>
        public static string GetUserAccountByCode(string code)
        {
            var dicts = new Dictionary().GetChilds("EnterpriseWeiXin");
            if (!dicts.Any())
            {
                return string.Empty;
            }
            string secret = dicts.First().Note.Trim1();//这里要写应用中心第一个应用的secret
            string access_token = GetAccessToken(secret);
            string url = "https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token=" + access_token + "&code=" + code;
            string returnJson = HttpHelper.HttpGet(url);
            JObject jObject = JObject.Parse(returnJson);
            int errcode = jObject.Value<int>("errcode");
            if (0 != errcode)
            {
                Log.Add("根据微信返回code得到帐号发生了错误", returnJson, others: "Code:" + code);
                return string.Empty;
            }
            else
            {
                return jObject.Value<string>("UserId");
            }
        }
    }
}
