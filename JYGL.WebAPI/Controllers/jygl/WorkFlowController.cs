using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UIDP.ODS.jyglDB;
using UIDP.UTILITY;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WZGX.WebAPI.Controllers.jygl
{
    [Produces("application/json")]
    [Route("WorkFlow")]
    public class WorkFlowController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取流程步骤
        /// </summary>
        /// <returns></returns>
        [HttpPost("getStep")]
        public IActionResult getStep()
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            string s = "";
            try
            {
                //string sendUrl = "http://114.115.142.34:8080/RoadFlowCoreApi/FlowTask/ExecuteTask";
                string getStep = "http://114.115.142.34:8080/RoadFlowCoreApi/Flow/GetSendSteps";
                WebRequest req = WebRequest.Create(getStep);
                Dictionary<string, string> postData = new Dictionary<string, string>();
                postData["systemcode"] = "localhost";
                postData["flowid"] = "4447d595-3a2a-4641-8447-c4f012791bae";
                postData["stepid"] = "";
                postData["taskid"] = "";
                postData["userid"] = "EB03262C-AB60-4BC6-A4C0-96E66A4229FE";
                postData["freesend"] = "true";
                string jsonString = JsonConvert.SerializeObject(postData);
                byte[] objectContent = Encoding.UTF8.GetBytes(jsonString);
                req.ContentLength = objectContent.Length;
                req.ContentType = "application/json";
                req.Method = "POST";
                using (var stream = req.GetRequestStream())
                {
                    stream.Write(objectContent, 0, objectContent.Length);
                    stream.Close();
                }
                var resp = req.GetResponse();
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    s = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                r["code"] = 1000;
                r["message"] = ex.Message.ToString();
                return Json(r);
            }
            return Json(s);
        }


        [HttpPost("sendTask")]
        //public IActionResult sendTask([FromBody]JObject value)
        public IActionResult sendTask(string systemcode, string flowid, string taskid, string instanceid, string senderid, string tasktitle, string comment, string type, string steps)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            //Dictionary<string, object> postData = new Dictionary<string, object>();
            Dictionary<string, object> postData = new Dictionary<string, object>();
            //Dictionary<string, object> postData = value.ToObject<Dictionary<string, object>>();
            string s = "";
            try
            {
                string sendUrl = "http://114.115.142.34:8080/RoadFlowCoreApi/FlowTask/ExecuteTask";
                WebRequest req = WebRequest.Create(sendUrl);
                postData["systemcode"] = systemcode;
                postData["flowid"] = flowid;
                postData["taskid"] = taskid;
                postData["instanceid"] = instanceid;
                postData["senderid"] = senderid;
                postData["tasktitle"] = tasktitle;
                postData["comment"] = comment;
                postData["type"] = type;
                postData["steps"] = JsonConvert.DeserializeObject(steps);
                string jsonString = JsonConvert.SerializeObject(postData);
                byte[] objectContent = Encoding.UTF8.GetBytes(jsonString);
                req.ContentLength = objectContent.Length;
                req.ContentType = "application/json";
                req.Method = "POST";
                using (var stream = req.GetRequestStream())
                {
                    stream.Write(objectContent, 0, objectContent.Length);
                    stream.Close();
                }
                var resp = req.GetResponse();
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    s = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                r["code"] = 1000;
                r["message"] = ex.Message.ToString();
                return Json(r);
            }
            return Json(s);
        }
        /// <summary>
        /// 执行流程，发送或退回流程
        /// </summary>
        /// <param name="systemcode">系统标识</param>
        /// <param name="stepid">步骤ID</param>
        /// <param name="flowid">流程ID</param>
        /// <param name="taskid">当前任务ID</param>
        /// <param name="instanceid">实例业务ID</param>
        /// <param name="senderid">发送人ID</param>
        /// <param name="tasktitle">任务标题</param>
        /// <param name="comment">处理意见</param>
        /// <param name="type">处理类型(freesubmit,submit,save,back,completed,redirect,addwrite,copyforcompleted,taskend)</param>
        /// <param name="isFreeSend">是否是自由发送(false)</param>
        /// <returns></returns>
        [HttpPost("sendFlow")]
        public IActionResult sendFlow(string systemcode, string stepid, string flowid, string taskid, string instanceid, string senderid, string tasktitle, string comment, string type, bool isFreeSend,int formtype)
        {

            Dictionary<string, object> r = new Dictionary<string, object>();
            #region 获取流程步骤
            if (systemcode == null || systemcode.Length == 0)
            {
                r["code"] = -1;
                r["message"] = "参数systemcode为空";
                return Json(r);
            }
            var flowRunModel = new RoadFlow.Business.Flow().GetFlowRunModel(new Guid(flowid));
            if (null == flowRunModel)
            {
                r["code"] = -1;
                r["message"] = "未找到流程运行时";
                return Json(r);
            }
            if (!stepid.IsGuid(out Guid stepGuid))
            {
                stepGuid = flowRunModel.FirstStepId;
            }
            RoadFlow.Business.FlowTask flowTask = new RoadFlow.Business.FlowTask();
            Guid groupId = Guid.Empty;
            string instanceId = string.Empty;
            bool isMobile = false;
            if (taskid.IsGuid(out Guid taskGuid))
            {
                var task = flowTask.Get(taskGuid);
                if (null != task)
                {
                    groupId = task.GroupId;
                    instanceId = task.InstanceId;
                }
            }

            var (html, message, sendSteps) = flowTask.GetNextSteps(flowRunModel, stepGuid, groupId, taskGuid, instanceId, senderid.ToGuid(), isFreeSend, isMobile);
            JObject jObject = RoadFlow.Mvc.ApiTools.GetJObject();
            JArray jArray = new JArray();
            foreach (var step in sendSteps)
            {
                JObject jObject1 = new JObject
                {
                    { "id", step.Id },
                    { "name", step.Name },
                    { "users", step.RunDefaultMembers },
                    { "note", step.Note }
                };
                jArray.Add(jObject1);
            }
            #endregion
            #region 发送流程
            JArray steps = jArray;
            if (!flowid.IsGuid(out Guid flowGuid))
            {
                r["code"] = -1;
                r["message"] = "参数flowid不是Guid";
                return Json(r);

            }
            if (!senderid.IsGuid(out Guid senderGuid))
            {
                r["code"] = -1;
                r["message"] = "参数senderid不是Guid";
                return Json(r);
            }
            RoadFlow.Business.User user = new RoadFlow.Business.User();
            RoadFlow.Business.Organize organize = new RoadFlow.Business.Organize();
            var sender = user.Get(senderGuid);
            if (null == sender)
            {
                r["code"] = -1;
                r["message"] = "未找到发送人";
                return Json(r);
            }
            //RoadFlow.Business.FlowTask flowTask = new RoadFlow.Business.FlowTask();
            RoadFlow.Model.FlowTask currentTask = null;
            if (taskid.IsGuid(out Guid taskGuid2))
            {
                currentTask = flowTask.Get(taskGuid2);
            }

            RoadFlow.Model.FlowRunModel.Execute executeModel = new RoadFlow.Model.FlowRunModel.Execute();
            executeModel.Comment = comment;
            executeModel.ExecuteType = flowTask.GetExecuteType(type);
            executeModel.FlowId = flowGuid;
            executeModel.InstanceId = instanceid;
            executeModel.Sender = sender;
            executeModel.Title = tasktitle;
            if (null != currentTask)
            {
                executeModel.GroupId = currentTask.GroupId;
                executeModel.StepId = currentTask.StepId;
                executeModel.TaskId = currentTask.Id;
                if (instanceId.IsNullOrWhiteSpace())
                {
                    instanceId = currentTask.InstanceId;
                }
                if (tasktitle.IsNullOrWhiteSpace())
                {
                    tasktitle = currentTask.Title;
                }
            }
            else
            {
                var flowRunModel2 = new RoadFlow.Business.Flow().GetFlowRunModel(flowGuid);
                if (null == flowRunModel2)
                {
                    r["code"] = -1;
                    r["message"] = "未找到流程运行时";
                    return Json(r);
                }
                executeModel.StepId = flowRunModel.FirstStepId;
            }
            if (null != steps)
            {
                List<(Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?)> nextSteps = new List<(Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?)>();
                foreach (JObject step in steps)
                {
                    string id = step["id"].ToString();
                    Guid stepId = id.ToGuid();
                    string stepName = step.Value<string>("name");
                    string beforeStepId = step.Value<string>("beforestepid");//原步骤ID(动态步骤的原步骤ID)
                    string parallelorserial = step.Value<string>("parallelorserial");//0并且 1串行
                    List<RoadFlow.Model.User> users = organize.GetAllUsers(step.Value<string>("users"));
                    string datetTime = step.Value<string>("completedtime");
                    nextSteps.Add((stepId, stepName,
                        beforeStepId.IsGuid(out Guid beforeStepGuid) ? new Guid?(beforeStepGuid) : new Guid?(),
                                            parallelorserial.IsInt(out int parallelorserialInt) ? new int?(parallelorserialInt) : new int?(),
                                            users, datetTime.IsDateTime(out DateTime dateTime) ? dateTime : new DateTime?()));
                }
                executeModel.Steps = nextSteps;
            }
            RoadFlow.Model.FlowRunModel.ExecuteResult executeResult = flowTask.Execute(executeModel);
            int errCode = executeResult.IsSuccess ? 0 : 2001;
            string errMsg = executeResult.Messages;
            //r["code"] = errCode;
            //r["message"] = errMsg;
            //r["data"] = JObject.FromObject(executeResult);
            if (errCode == 0)
            {
                DBTool db = new DBTool("");
                if (formtype == 0)
                {
                    string sql = "UPDATE jy_cbjh set PROCESS_STATE={0} where XMBH='" + instanceid + "'";
                    int status = 0;
                    if (steps != null)
                    {
                        status = 1;
                    }
                    else
                    {
                        status = 2;
                    }
                    sql = string.Format(sql, status);
                    if (db.ExecutByStringResult(sql) == "")
                    {
                        r["code"] = 2000;
                        r["message"] = "流程流转成功！";
                    }
                    else
                    {
                        r["code"] = -1;
                        r["message"] = "失败！";
                    }
                }
                else
                {
                    string sql = "UPDATE jy_fybx set PROCESS_STATE={0} where S_ID='" + instanceid + "'";
                    int status = 0;
                    if (steps != null)
                    {
                        status = 1;
                    }
                    else
                    {
                        status = 2;
                    }
                    sql = string.Format(sql, status);
                    if (db.ExecutByStringResult(sql) == "")
                    {
                        r["code"] = 2000;
                        r["message"] = "流程流转成功！";
                    }
                    else
                    {
                        r["code"] = -1;
                        r["message"] = "失败！";
                    }
                }
            }
            else
            {
                r["code"] = errCode;
                r["message"] = errMsg;
            }
            return Json(r);
            #endregion
        }
    }
}
