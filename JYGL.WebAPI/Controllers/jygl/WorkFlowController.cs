using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using UIDP.UTILITY;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WZGX.WebAPI.Controllers.jygl
{
    [Produces("application/json")]
    [Route("WorkFlow")]
    public class WorkFlowController : Controller
    {
        #region 测试调用流程API方法（无用）
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
        #endregion

        #region 执行流程(未启用)

        [HttpPost("executeFlow")]
        public IActionResult executeFlow(string systemcode, string stepid, string flowid, string taskid, string instanceid, string senderid, string tasktitle, string comment, string type, bool isFreeSend)
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
                    //string stepName = step.Value<string>("name");
                    //string beforeStepId = step.Value<string>("beforestepid");//原步骤ID(动态步骤的原步骤ID)
                    //string parallelorserial = step.Value<string>("parallelorserial");//0并且 1串行
                    //List<RoadFlow.Model.User> users = organize.GetAllUsers(step.Value<string>("users"));
                    string stepName = step["name"].ToString();
                    string beforeStepId = step["beforestepid"] == null ? null : step["beforestepid"].ToString();//原步骤ID(动态步骤的原步骤ID)
                    string parallelorserial = step["parallelorserial"] == null ? null : step["parallelorserial"].ToString();//0并且 1串行
                    List<RoadFlow.Model.User> users = organize.GetAllUsers(step["users"].ToString());
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
            r["code"] = errCode;
            r["message"] = errMsg;
            r["data"] = JObject.FromObject(executeResult);

            return Json(r);
            #endregion
        }


        #endregion

        #region 发起/执行流程
        /// <summary>
        /// 发起流程
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
        /// <param name="formtype">0：成本计划 1：费用报销模块</param>
        /// <returns></returns>
        [HttpPost("sendFlow")]
        public IActionResult sendFlow(string systemcode, string stepid, string flowid, string taskid, string instanceid, string senderid, string tasktitle, string comment, string type, bool isFreeSend, int formtype)
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
                    //string stepName = step.Value<string>("name");
                    //string beforeStepId = step.Value<string>("beforestepid");//原步骤ID(动态步骤的原步骤ID)
                    //string parallelorserial = step.Value<string>("parallelorserial");//0并且 1串行
                    //List<RoadFlow.Model.User> users = organize.GetAllUsers(step.Value<string>("users"));
                    string stepName = step["name"].ToString();
                    string beforeStepId = step["beforestepid"] == null ? null : step["beforestepid"].ToString();//原步骤ID(动态步骤的原步骤ID)
                    string parallelorserial = step["parallelorserial"] == null ? null : step["parallelorserial"].ToString();//0并且 1串行
                    List<RoadFlow.Model.User> users = organize.GetAllUsers(step["users"].ToString());
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
                else if (formtype == 1)
                {
                    string sql = "UPDATE jy_fybx set PROCESS_STATE={0} where BXDH='" + instanceid + "'";
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
                    string sql = "UPDATE jy_clbx set PROCESS_STATE={0} where CLBH='" + instanceid + "'";
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
        #endregion

        #region 回退流程

        [HttpPost("backFlow")]
        public IActionResult backFlow(string systemcode, string flowid,string groupid, string taskid, string instanceid, string senderid, string tasktitle, string comment, int formtype)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            DataTable firsttesp = new DataTable();
            RoadFlow.Business.FlowTask flowTask = new RoadFlow.Business.FlowTask();
            //var taskid = string.Empty;
            #region 获取流程第一步步骤
            using (RoadFlow.Mapper.DataContext context = new RoadFlow.Mapper.DataContext())
            {
                firsttesp = context.GetDataTable("select top 1 * from RF_FlowTask where groupId='"+ groupid + "' and InstanceId = '" + instanceid + "' ORDER BY ReceiveTime desc");
            }
            //if (firsttesp!=null&& firsttesp.Rows.Count==1)
            //{
            //    taskid = firsttesp.Rows[0]["Id"].ToString();
            //}
            var flowRunModel = new RoadFlow.Business.Flow().GetFlowRunModel(new Guid(flowid));
            if (null == flowRunModel)
            {
                r["code"] = -1;
                r["message"] = "未找到流程运行时";
                return Json(r);
            }
            JArray jArray = new JArray();
            Guid groupId = Guid.Empty;
            string instanceId = string.Empty;

            foreach (DataRow step in firsttesp.Rows)
            {
                JObject jObject1 = new JObject
                {
                    { "id", step["StepId"].ToString() },
                    { "name", step["StepName"].ToString() },
                    { "users", "u_"+step["SenderId"].ToString() },
                    { "note", step["Note"].ToString()  }
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
            executeModel.ExecuteType = flowTask.GetExecuteType("back");
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
                    //string stepName = step.Value<string>("name");
                    //string beforeStepId = step.Value<string>("beforestepid");//原步骤ID(动态步骤的原步骤ID)
                    //string parallelorserial = step.Value<string>("parallelorserial");//0并且 1串行
                    //List<RoadFlow.Model.User> users = organize.GetAllUsers(step.Value<string>("users"));
                    string stepName = step["name"].ToString();
                    string beforeStepId = step["beforestepid"] == null ? null : step["beforestepid"].ToString();//原步骤ID(动态步骤的原步骤ID)
                    string parallelorserial = step["parallelorserial"] == null ? null : step["parallelorserial"].ToString();//0并且 1串行
                    List<RoadFlow.Model.User> users = organize.GetAllUsers(step["users"].ToString());
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
                    int status = 3;

                    sql = string.Format(sql, status);
                    if (db.ExecutByStringResult(sql) == "")
                    {
                        r["code"] = 2000;
                        r["message"] = "审批已退回！";
                    }
                    else
                    {
                        r["code"] = -1;
                        r["message"] = "失败！";
                    }
                }
                else if (formtype == 1)
                {
                    string sql = "UPDATE jy_fybx set PROCESS_STATE={0} where BXDH='" + instanceid + "'";
                    int status = 3;
                    sql = string.Format(sql, status);
                    if (db.ExecutByStringResult(sql) == "")
                    {
                        r["code"] = 2000;
                        r["message"] = "审批已退回！";
                    }
                    else
                    {
                        r["code"] = -1;
                        r["message"] = "失败！";
                    }
                }
                else if (formtype == 2)
                {
                    string sql = "UPDATE jy_clbx set PROCESS_STATE={0} where CLBH='" + instanceid + "'";
                    int status = 3;
                    sql = string.Format(sql, status);
                    if (db.ExecutByStringResult(sql) == "")
                    {
                        r["code"] = 2000;
                        r["message"] = "审批已退回！";
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

        #endregion

        #region 撤销流程
        //后期可以考虑递归撤回
        //public string getTaskId(DataTable dt,string taskid)
        //{

        //    string s=string.Empty;

        //    DataRow[] dr=dt.Select("previd ='" + taskid + "'");
        //    if (dr.Length>0)
        //    {
        //        s = "'" + dr[0]["Id"].ToString() + "',";
        //    }
        //    return s;
        //}
        [HttpPost("revokeFlow")]
        public IActionResult revokeFlow(string instanceid, string senderid, int formtype)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            DataTable firsttesp = new DataTable();
            RoadFlow.Business.FlowTask flowTask = new RoadFlow.Business.FlowTask();
            var taskid = string.Empty;
            using (RoadFlow.Mapper.DataContext context = new RoadFlow.Mapper.DataContext())
            {
                firsttesp = context.GetDataTable("select top 1 * from RF_FlowTask where Status =2 and InstanceId = '" + instanceid + "' and ReceiveId='" + senderid + "' ORDER BY ReceiveTime desc");
            }
            if (firsttesp != null && firsttesp.Rows.Count == 1)
            {
                taskid = firsttesp.Rows[0]["Id"].ToString();
            }

            if (!string.IsNullOrEmpty(taskid) && !string.IsNullOrEmpty(instanceid))
            {
                DBTool db = new DBTool("");
                string flag = db.GetString("select  count(1) from RF_FlowTask where previd = '" + taskid + "' and Status in(0,1) ");//判断是否可以撤回
                if (!string.IsNullOrEmpty(flag)&&flag!="0")//只允许发起人撤回下一步未审批的流程
                //if (!string.IsNullOrEmpty(flag))//发起人可撤回任意状态的流程
                {
                    string updateSql = string.Empty;
                    string revokeSql = "delete from  RF_FlowTask where InstanceId = '" + instanceid + "' and (previd ='" + taskid + "' or id='" + taskid + "')";//只允许发起人撤回下一步未审批的流程
                    //string revokeSql = "delete from  RF_FlowTask where InstanceId = '" + instanceid + "'";//发起人可撤回任意状态的流程
                    if (formtype == 0)
                    {
                        updateSql = "UPDATE jy_cbjh set PROCESS_STATE=0 where XMBH='" + instanceid + "'";
                    }
                    else if(formtype==1)
                    {
                        updateSql = "UPDATE jy_fybx set PROCESS_STATE=0 where BXDH='" + instanceid + "'";
                    }
                    else
                    {
                        updateSql = "UPDATE jy_clbx set PROCESS_STATE=0 where CLBH='" + instanceid + "'";                      
                    }
                    List<string> list = new List<string>();
                    list.Add(revokeSql);
                    list.Add(updateSql);
                    string result = db.Executs(list);
                    if (result == "")
                    {
                        r["code"] = 2000;
                        r["message"] = "审批已撤回！";
                    }
                }
                else
                {
                    r["code"] = -1;
                    r["message"] = "撤销失败！流程已被审批！";
                }

            }
            else
            {
                r["code"] = -1;
                r["message"] = "撤销失败！参数异常！";
            }

            return Json(r);
        }
        #endregion

        #region 查看流程（根据表单主键查询流程groupId）
        [HttpPost("flowProcess")]
        public IActionResult flowProcess(string instanceid)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(instanceid))
            {
                DBTool db = new DBTool("");
                JObject jObject = null;
                DataTable flowTask = db.GetDataTable("select  top 1 FlowId,GroupId from RF_FlowTask where InstanceId='" + instanceid + "'  order by ReceiveTime desc");//判断是否可以撤回
                if (flowTask != null && flowTask.Rows.Count == 1)
                {
                    jObject = new JObject
                     {
                        { "flowId",  flowTask.Rows[0]["FlowId"].ToString() },
                        { "groupId", flowTask.Rows[0]["GroupId"].ToString() }
                    };

                    r["code"] = 2000;
                    r["message"] = "成功！";
                    r["data"] = JObject.FromObject(jObject);
                }
                else
                {
                    r["code"] = -1;
                    r["message"] = "未找到当前流程！";
                }

            }
            else
            {
                r["code"] = -1;
                r["message"] = "参数异常！";
            }

            return Json(r);
        }
        #endregion

    }
}
