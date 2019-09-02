using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using Microsoft.AspNetCore.Http.Internal;

namespace RoadFlow.Mvc
{
    public class ApiValidateAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// 是否验证
        /// </summary>
        public bool Check { get; set; } = true;

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Request.EnableRewind();
            var reader = new StreamReader(context.HttpContext.Request.Body);
            string json = reader.ReadToEnd();
            context.HttpContext.Request.Body.Position = 0;
            JObject jObject = null;
            try
            {
                jObject = JObject.Parse(json);
            }
            catch { }
            if (null == jObject)
            {
                context.Result = new ContentResult() { Content = ApiTools.GetErrorJson("json格式错误") };
                return;
            }
            string systemCode = jObject.Value<string>("systemcode");
            if (systemCode.IsNullOrEmpty())
            {
                context.Result = new ContentResult() { Content = ApiTools.GetErrorJson("系统标识为空") };
                return;
            }
            var systemModel = new Business.FlowApiSystem().Get(systemCode.Trim());
            if (null == systemModel)
            {
                context.Result = new ContentResult() { Content = ApiTools.GetErrorJson("未找到对应的系统") };
                return;
            }
            string systemIP = ',' + systemModel.SystemIP + ",";
            //如果设定的本地IP地址，则直接返回不比较
            if (systemIP.ContainsIgnoreCase(",127.0.0.1,") || systemIP.ContainsIgnoreCase(",::1,") || systemIP.ContainsIgnoreCase(",localhost,"))
            {
                return;
            }
            string ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
            if (!systemIP.ContainsIgnoreCase("," + ip + ","))
            {
                context.Result = new ContentResult() { Content = ApiTools.GetErrorJson("未授权的访问") };
                return;
            }
        }
    }
}
