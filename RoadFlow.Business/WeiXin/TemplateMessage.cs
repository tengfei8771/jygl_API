using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility;

namespace RoadFlow.Business.WeiXin
{
    /// <summary>
    /// 模板消息类
    /// </summary>
    public class TemplateMessage
    {
        /// <summary>
        /// 发送待办消息
        /// </summary>
        /// <param name="flowTask"></param>
        /// <returns></returns>
        public static string SendWaitTaskMessage(Model.FlowTask flowTask)
        {
            if (!Config.WeiXin_IsUse)
            {
                return string.Empty;
            }
            if(null == flowTask || flowTask.ReceiveId.IsEmptyGuid())
            {
                return string.Empty;
            }
            var userModel = new User().Get(flowTask.ReceiveId);
            if(null == userModel || userModel.WeiXinOpenId.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            var httpContext = Tools.HttpContext;
            string appopenmodel = null == httpContext ? "0" : httpContext.Request.Querys("rf_appopenmodel");
            string linkUrl = Config.WeiXin_WebUrl + string.Format("/RoadFlowCore/FlowRun/{0}?flowid={1}&stepid={2}&taskid={3}&groupid={4}&instanceid={5}&appid={6}&tabid={7}&rf_appopenmodel={8}&ismobile=1",
                       "Index", flowTask.FlowId, flowTask.StepId, flowTask.Id, flowTask.GroupId, flowTask.InstanceId,
                       null == httpContext ? "" : httpContext.Request.Querys("appid"), null == httpContext ? "" : httpContext.Request.Querys("tabid"),
                       appopenmodel);

            string token = Common.GetAccessToken();
            string url = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + token;
            JObject jObject = new JObject
            {
                ["touser"] = userModel.WeiXinOpenId,
                ["template_id"] = "1wFmd0moKTb8HIw1ZIqUGIbo7h253R5IjOQAW5_UkcQ",
                ["url"] = linkUrl
            };
            JObject data = new JObject();
            JObject first = new JObject
            {
                ["value"] = flowTask.Title,
                ["color"] = ""
            };
            data["first"] = first;

            JObject key1 = new JObject
            {
                ["value"] = flowTask.SenderName
            };
            data["keyword1"] = key1;

            JObject key2 = new JObject
            {
                ["value"] = flowTask.ReceiveTime.ToShortDateTimeString()
            };
            data["keyword2"] = key2;

            JObject remark = new JObject
            {
                ["value"] = flowTask.CompletedTime.HasValue ? "要求完成时间：" + flowTask.CompletedTime.Value.ToShortDateTimeString() : ""
            };
            data["remark"] = remark;

            jObject["data"] = data;
            string msg = HttpHelper.HttpPost(url, jObject.ToString(Newtonsoft.Json.Formatting.None));
            JObject returnJson = JObject.Parse(msg);
            if (returnJson.Value<string>("errcode").Equals("0"))
            {
                //Log.Add("Send template message", msg);
                return "1";
            }
            else
            {
                Log.Add("发送公众号模板消息错误", msg);
                return string.Empty;
            }
        }
    }
}
