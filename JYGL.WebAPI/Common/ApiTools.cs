using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility;

namespace RoadFlow.Mvc
{
    public class ApiTools
    {
        /// <summary>
        /// 得到返回的JSON对象
        /// </summary>
        /// <param name="errcode"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public static JObject GetJObject(int errcode = 0, string errmsg = "ok")
        {
            JObject jObject = new JObject
            {
                { "errcode", errcode },
                { "errmsg", errmsg }
            };
            return jObject;
        }

        /// <summary>
        /// 得到返回错误的JSON字符串
        /// </summary>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public static string GetErrorJson(string errmsg, int errcode = 1001)
        {
            JObject jObject = GetJObject(errcode, errmsg);
            return jObject.ToString(Newtonsoft.Json.Formatting.None);
        }

        /// <summary>
        /// 得到token
        /// </summary>
        /// <param name="systemCode">系统标识</param>
        /// <returns></returns>
        public static string GetToken(string systemCode)
        {
            if (systemCode.IsNullOrWhiteSpace())
            {
                return "";
            }
            Business.FlowApiSystem flowApiSystem = new Business.FlowApiSystem();
            var model = flowApiSystem.Get(systemCode);
            if (null == model)
            {
                return "";
            }
            return "";
        }

        /// <summary>
        /// 将body转为jobject对象
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static JObject GetBodyJObject(Stream body)
        {
            var reader = new StreamReader(body);
            string json = reader.ReadToEnd();
            try
            {
                return JObject.Parse(json);
            }
            catch{
                return null;
            }
        }
    }
}
