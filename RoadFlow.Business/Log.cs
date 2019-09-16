using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RoadFlow.Utility;

namespace RoadFlow.Business
{
    public class Log
    {
        public enum Type
        {
            用户登录,
            系统管理,
            流程管理,
            表单管理,
            流程运行,
            系统异常,
            其它
        }
        private static readonly Dictionary<string, string> ZH_Type = new Dictionary<string, string>
        {
            {"用户登录" ,"用戶登錄"},
            {"系统管理" ,"系統管理"},
            {"流程管理" ,"流程管理"},
            {"表单管理" ,"表單管理"},
            {"流程运行" ,"流程運行"},
            {"系统异常" ,"系統異常"},
            {"其它" ,"其它"},
        };
        private static readonly Dictionary<string, string> EN_US_Type = new Dictionary<string, string>
        {
            {"用户登录" ,"User login"},
            {"系统管理" ,"System management"},
            {"流程管理" ,"Workflow management"},
            {"表单管理" ,"Form management"},
            {"流程运行" ,"Workflow run"},
            {"系统异常" ,"Exception"},
            {"其它" ,"Other"},
        };

        /// <summary>
        /// 得到日志类型
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="language">语言</param>
        /// <returns></returns>

        public static string GetLogType(string type, string language)
        {
            switch (language)
            {
                case "zh-CN":
                    return type;
                case "zh":
                    return ZH_Type.TryGetValue(type, out string t) ? t : type;
                case "en-US":
                    return EN_US_Type.TryGetValue(type, out string t2) ? t2 : type;
            }
            return type;
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="log"></param>
        private static void AddLog(Model.Log log)
        {
            log.Type = GetLogType(log.Type, Config.Language_Default);
            new Data.Log().Add(log);
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="log"></param>
        public static void Add(Model.Log log)
        {
            Task task = Task.Run(() => AddLog(log));
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="title"></param>
        /// <param name="contents"></param>
        /// <param name="type"></param>
        /// <param name="oldContents"></param>
        /// <param name="newContents"></param>
        /// <param name="others"></param>
        /// <param name="browseAgent"></param>
        /// <param name="ipAddress"></param>
        /// <param name="url"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        public static void Add(string title, string contents = "", Type type = Type.其它, string oldContents = "", string newContents = "", string others = "", string browseAgent = "", string ipAddress = "", string url = "", string userId = "", string userName = "")
        {
            Model.Log logModel = new Model.Log
            {
                Id = Guid.NewGuid(),
                Title = title,
                Type = type.ToString(),
                IPAddress = ipAddress.IsNullOrWhiteSpace() ? Tools.GetIP() : ipAddress,
                URL = url.IsNullOrWhiteSpace() ? Tools.GetAbsoluteURL() : url,
                WriteTime = DateExtensions.Now,
                Referer = Tools.GetReferer()
            };
            if (userId.IsGuid(out Guid userGuid))
            {
                logModel.UserId = userGuid;
            }
            else
            {
                try
                {
                    Guid uid = User.CurrentUserId;
                    if (uid.IsEmptyGuid())
                    {
                        uid = EnterpriseWeiXin.Common.GetUserId();
                    }
                    if (uid.IsNotEmptyGuid())
                    {
                        logModel.UserId = uid;
                    }
                }
                catch { }
            }
            if (!userName.IsNullOrWhiteSpace())
            {
                logModel.UserName = userName;
            }
            else
            {
                try
                {
                    var userModel = User.CurrentUser;
                    if (null == userModel)
                    {
                        userModel = EnterpriseWeiXin.Common.GetUser();
                    }
                    if (null != userModel)
                    {
                        logModel.UserName = userModel.Name;
                    }
                }
                catch { }
            }

            if (!contents.IsNullOrWhiteSpace())
            {
                logModel.Contents = contents;
            }
            if (!others.IsNullOrWhiteSpace())
            {
                logModel.Others = others;
            }
            if (!oldContents.IsNullOrWhiteSpace())
            {
                logModel.OldContents = oldContents;
            }
            if (!newContents.IsNullOrWhiteSpace())
            {
                logModel.NewContents = newContents;
            }
            logModel.BrowseAgent = browseAgent.IsNullOrWhiteSpace() ? Tools.GetBrowseAgent() : browseAgent;
            AddLog(logModel);
        }

        /// <summary>
        /// 添加异常日志
        /// </summary>
        /// <param name="err">异常类</param>
        /// <param name="title">标题 如果为空用err.Message</param>
        public static void Add(Exception err, string title = "")
        {
            Model.Log logModel = new Model.Log
            {
                Id = Guid.NewGuid(),
                Title = title.IsNullOrWhiteSpace() ? err.Message : title,
                Type = GetLogType("系统异常", Tools.GetCurrentLanguage()),
                IPAddress = Tools.GetIP(),
                URL = Tools.GetAbsoluteURL(),
                WriteTime = DateExtensions.Now,
                Referer = Tools.GetReferer()
            };
            Guid uid = User.CurrentUserId;
            if (uid.IsEmptyGuid())
            {
                uid = EnterpriseWeiXin.Common.GetUserId();
            }
            if (uid.IsNotEmptyGuid())
            {
                logModel.UserId = uid;
            }
            var userModel = User.CurrentUser;
            if (null == userModel)
            {
                userModel = EnterpriseWeiXin.Common.GetUser();
            }
            if (null != userModel)
            {
                logModel.UserName = userModel.Name;
            }
            logModel.Contents = err.StackTrace;
            logModel.Others = err.Source + "（" + err.Message + "）";
            logModel.BrowseAgent = Tools.GetBrowseAgent();
            AddLog(logModel);
        }

        /// <summary>
        /// 查询一页日志
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerList(out int count, int size, int number, string title, string type, string userId, string date1, string date2, string order)
        {
            return new Data.Log().GetPagerList(out count, size, number, title, type, userId, date1, date2, order);
        }

        /// <summary>
        /// 查询一条日志
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Log Get(Guid id)
        {
            return new Data.Log().Get(id);
        }

        /// <summary>
        /// 得到类别下拉项
        /// </summary>
        /// <returns></returns>
        public string GetTypeOptions(string value = "")
        {
            StringBuilder options = new StringBuilder();
            List<string> types = new List<string>();
            switch (Config.Language_Default)
            {
                case "zh":
                    foreach (var dict in ZH_Type)
                    {
                        types.Add(dict.Value);
                    }
                    break;
                case "en-US":
                    foreach (var dict in EN_US_Type)
                    {
                        types.Add(dict.Value);
                    }
                    break;
                default:
                    var array = Enum.GetValues(typeof(Type));
                    foreach (var arr in array)
                    {
                        types.Add(arr.ToString());
                    }
                    break;
            }
            foreach (var arr in types)
            {
                options.AppendFormat("<option value=\"{0}\" {1}>{0}</option>", arr, arr.ToString().Equals(value) ? "selected=\"selected\"" : "");
            }
            return options.ToString();
        }
    }
} 
