using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RoadFlow.Business.EnterpriseWeiXin
{
    /// <summary>
    /// 与微信通讯录交互类
    /// </summary>
    public class Organize
    {
        public Organize()
        {
            var dicts = new Dictionary().GetChilds("EnterpriseWeiXin");
            if (dicts.Any())
            {
                var dict = dicts.Find(p => p.Title.Equals("通讯录同步"));
                if (null != dict)
                {
                    Secret = dict.Note.Trim1();
                    RootDeptId = dict.Value.ToInt(0);
                }
            }
            if (Secret.IsNullOrWhiteSpace())
            {
                throw new Exception("通讯录同步Secret为空");
            }
        }
        /// <summary>
        /// 通讯录管理Secret
        /// </summary>
        public string Secret;
        /// <summary>
        /// 根部门ID
        /// </summary>
        public int RootDeptId;

        #region 人员相关操作
        /// <summary>
        /// 接口URL
        /// </summary>
        private readonly string Url = "https://qyapi.weixin.qq.com/cgi-bin/user/";
        /// <summary>
        /// 添加人员
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string AddUser(Model.User user)
        {
            if (!Config.Enterprise_WeiXin_IsSyncOrg)
            {
                return string.Empty;
            }
            if (null == user)
            {
                return "要添加的用户为空";
            }
            if (user.Mobile.IsNullOrWhiteSpace() && user.Email.IsNullOrWhiteSpace())
            {
                return "手机号和邮箱不能同时为空";
            }
            string url = Url + "create?access_token=" + Common.GetAccessToken(Secret);
            JObject jObject = new JObject
            {
                { "userid", user.Account },
                { "name", user.Name },
                { "position", new User().GetOrganizeMainShowHtml(user.Id, false).TrimAll() },
                { "mobile", user.Mobile },
                { "department", new JArray{ RootDeptId } },
                { "order", 0 },
                { "enable", user.Status == 0 ? 1 : 0 },
                { "email", user.Email }
            };
            if (user.Tel.IsTelNumber())
            {
                jObject.Add("telephone", user.Tel);
            }
            if (user.Sex.HasValue)
            {
                jObject.Add("gender", user.Sex.Value + 1);
            }
            string json = jObject.ToString(Newtonsoft.Json.Formatting.None);
            string msg = HttpHelper.HttpPost(url, json);
            JObject returnJson = JObject.Parse(msg);
            if (0 != returnJson.Value<int>("errcode"))
            {
                Log.Add("企业微信添加人员发生了错误", json, others: "返回：" + msg + " url:" + url);
                return returnJson.Value<string>("errmsg");
            }
            return string.Empty;
        }
        /// <summary>
        /// 修改人员
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string UpdateUser(Model.User user)
        {
            if (!Config.Enterprise_WeiXin_IsSyncOrg)
            {
                return string.Empty;
            }
            if (null == user)
            {
                return "要添加或修改的用户为空";
            }
            if (user.Mobile.IsNullOrWhiteSpace() && user.Email.IsNullOrWhiteSpace())
            {
                return "手机号和邮箱不能同时为空";
            }
            string url = Url + "update?access_token=" + Common.GetAccessToken(Secret);
            JObject jObject = new JObject
            {
                { "userid", user.Account },
                { "name", user.Name },
                { "position", new User().GetOrganizeMainShowHtml(user.Id, false).TrimAll() },
                { "mobile", user.Mobile },
                { "department", new JArray{ RootDeptId } },
                { "order", 0 },
                { "enable", user.Status == 0 ? 1 : 0 },
                { "email", user.Email }
            };
            if (user.Tel.IsTelNumber())
            {
                jObject.Add("telephone", user.Tel);
            }
            if (user.Sex.HasValue)
            {
                jObject.Add("gender", user.Sex.Value + 1);
            }
            string json = jObject.ToString(Newtonsoft.Json.Formatting.None);
            string msg = HttpHelper.HttpPost(url, json);
            JObject returnJson = JObject.Parse(msg);
            int errCode = returnJson.Value<int>("errcode");
            if (0 != errCode)
            {
                //如果是返回人员不存在，则添加人员
                if (60111 == errCode)
                {
                    return AddUser(user);
                }
                Log.Add("企业微信修改人员发生了错误", json, others: "返回：" + msg + " url:" + url);
                return returnJson.Value<string>("errmsg");
            }
            return string.Empty;
        }
        /// <summary>
        /// 删除人员
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string DeleteUser(string account)
        {
            if (!Config.Enterprise_WeiXin_IsSyncOrg)
            {
                return string.Empty;
            }
            string url = Url + "delete?access_token=" + Common.GetAccessToken(Secret) + "&userid=" + account;
            string msg = HttpHelper.HttpGet(url);
            JObject returnJson = JObject.Parse(msg);
            if (0 != returnJson.Value<int>("errcode"))
            {
                Log.Add("企业微信删除人员发生了错误", url, others: "返回：" + msg);
                return returnJson.Value<string>("errmsg");
            }
            return string.Empty;
        }
        /// <summary>
        /// 查询人员
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public JObject GetUser(string account)
        {
            string url = Url + "get?access_token=" + Common.GetAccessToken(Secret) + "&userid=" + account;
            string msg = HttpHelper.HttpGet(url);
            JObject returnJson = JObject.Parse(msg);
            if (0 != returnJson.Value<int>("errcode"))
            {
                Log.Add("企业微信获取人员发生了错误", url, others: "返回：" + msg);
                return null;
            }
            return returnJson;
        }
        #endregion

        #region 部门相关操作
        /// <summary>
        /// 部门接口URL
        /// </summary>
        private readonly string DeptUrl = "https://qyapi.weixin.qq.com/cgi-bin/department/";
        public string AddDept(Model.Organize organize)
        {
            if (!Config.Enterprise_WeiXin_IsSyncOrg)
            {
                return string.Empty;
            }
            if (null == organize)
            {
                return "要添加的部门为空";
            }
            string url = DeptUrl + "create?access_token=" + Common.GetAccessToken(Secret);
            JObject jObject = new JObject
            {
                { "name", organize.Name },
                { "parentid", organize.ParentId.ToInt() },
                { "order", organize.Sort },
                { "id", organize.IntId }
            };
            string json = jObject.ToString(Newtonsoft.Json.Formatting.None);
            string msg = HttpHelper.HttpPost(url, json);
            JObject returnJson = JObject.Parse(msg);
            if (0 != returnJson.Value<int>("errcode"))
            {
                Log.Add("企业微信添加部门发生了错误", json, others: "返回：" + msg + " url:" + url);
                return returnJson.Value<string>("errmsg");
            }
            return string.Empty;
        }

        /// <summary>
        /// 更新部门
        /// </summary>
        /// <param name="organize"></param>
        /// <returns></returns>
        public string UpdateDept(Model.Organize organize)
        {
            if (!Config.Enterprise_WeiXin_IsSyncOrg)
            {
                return string.Empty;
            }
            if (null == organize)
            {
                return "要更新的部门为空";
            }
            string url = DeptUrl + "update?access_token=" + Common.GetAccessToken(Secret);
            JObject jObject = new JObject
            {
                { "name", organize.Name },
                { "parentid", organize.ParentId.ToInt() },
                { "order", organize.Sort },
                { "id", organize.IntId }
            };
            string json = jObject.ToString(Newtonsoft.Json.Formatting.None);
            string msg = HttpHelper.HttpPost(url, json);
            JObject returnJson = JObject.Parse(msg);
            if (0 != returnJson.Value<int>("errcode"))
            {
                Log.Add("企业微信更新部门发生了错误", json, others: "返回：" + msg + " url:" + url);
                return returnJson.Value<string>("errmsg");
            }
            return string.Empty;
        }
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string DeleteDept(Model.Organize organize)
        {
            if (!Config.Enterprise_WeiXin_IsSyncOrg)
            {
                return string.Empty;
            }
            if (null == organize)
            {
                return "要删除的部门为空";
            }
            string url = DeptUrl + "delete?access_token=" + Common.GetAccessToken(Secret) + "&id=" + organize.IntId;
            string msg = HttpHelper.HttpGet(url);
            JObject returnJson = JObject.Parse(msg);
            if (0 != returnJson.Value<int>("errcode"))
            {
                Log.Add("企业微信删除部门发生了错误", url, others: "返回：" + msg);
                return returnJson.Value<string>("errmsg");
            }
            return string.Empty;
        }
        #endregion
    }
}
