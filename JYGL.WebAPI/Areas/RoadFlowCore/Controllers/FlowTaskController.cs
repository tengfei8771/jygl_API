using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoadFlow.Utility;
using System.Data;
using System.Text;

namespace RoadFlow.Mvc.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class FlowTaskController : Controller
    {
        /// <summary>
        /// 待办事项
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        public IActionResult Wait()
        {
            ViewData["flowOptions"] = new Business.Flow().GetOptions();
            ViewData["query"] = "appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            return View();
        }
        /// <summary>
        /// 查询待办事项
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        public string QueryWait()
        {
            string appid = Request.Querys("appid");
            string tabid = Request.Querys("tabid");
            string flowid = Request.Forms("flowid");
            string title = Request.Forms("title");
            string date1 = Request.Forms("date1");
            string date2 = Request.Forms("date2");
            string sidx = Request.Forms("sidx");
            string sord = Request.Forms("sord");
            string order = (sidx.IsNullOrEmpty() ? "ReceiveTime" : sidx) + " " + (sord.IsNullOrEmpty() ? "ASC" : sord);
            int size = Tools.GetPageSize();
            int number = Tools.GetPageNumber();

            Business.FlowTask flowTask = new Business.FlowTask();
            DataTable dataTable = flowTask.GetWaitTask(size, number, Current.UserId, flowid, title, date1, date2, order, out int count);
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (DataRow dr in dataTable.Rows)
            {
                int openModel = 0, width = 0, height = 0;
                StringBuilder opation = new StringBuilder("<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('" + Url.Content("~/RoadFlowCore/FlowRun/Index") + "?" + string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}",
                        dr["FlowId"], dr["StepId"], dr["InstanceId"], dr["Id"], dr["GroupId"], appid
                        ) + "','" + dr["Title"].ToString().RemoveHTML().UrlEncode() + "','" + dr["Id"] + "'," + openModel + "," + width + "," + height + ");return false;\"><i class=\"fa fa-pencil-square-o\"></i>处理</a>"
                        + "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"detail('" + dr["FlowId"] + "','" + dr["GroupId"] + "','" + dr["Id"] + "');return false;\"><i class=\"fa fa-navicon\"></i>过程</a>");

                if (flowTask.IsDelete(dr["Id"].ToString().ToGuid()))//第一步发起者可以删除
                {
                    opation.Append("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"delTask('" + dr["FlowId"] + "','" + dr["GroupId"] + "','" + dr["Id"] + "');return false;\"><i class=\"fa fa-remove\"></i>作废</a>");
                }
                string taskTitle = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('" + Url.Content("~/RoadFlowCore/FlowRun/Index") + "?" + string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}",
                        dr["FlowId"], dr["StepId"], dr["InstanceId"], dr["Id"], dr["GroupId"], appid
                        ) + "','" + dr["Title"].ToString().RemoveHTML().UrlEncode() + "','" + dr["Id"] + "'," + openModel + "," + width + "," + height + ");return false;\">" + dr["Title"] + "</a>";
                string status = "<div>正常</div>";
                if (dr["CompletedTime"].ToString().IsDateTime(out DateTime ctime))
                {
                    DateTime nowTime = DateExtensions.Now;
                    if (nowTime > ctime)
                    {
                        status = "<div title='要求完成时间：" + ctime.ToString("yyyy-MM-dd HH:mm") +
                            "' style='color:red;font-weight:bold;'>已超期</div>";
                    }
                    else if ((ctime - nowTime).Days < 3)
                    {
                        double day = Math.Ceiling((ctime - nowTime).TotalDays);
                        string dsystring = day == 0 ? (ctime - nowTime).Hours + "小时" : day.ToString() + "天";
                        status = "<div title='要求完成时间：" + ctime.ToString("yyyy-MM-dd HH:mm") +
                            "' style='color:#ff7302;font-weight:bold;'>" + dsystring + "内到期</div>";
                    }
                    else
                    {
                        status = "<div title='要求完成时间：" + ctime.ToString("yyyy-MM-dd HH:mm") +
                            "' style=''>正常</div>";
                    }
                }
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", dr["Id"].ToString() },
                    { "Title", taskTitle },
                    { "FlowName", dr["FlowName"].ToString() },
                    { "StepName", dr["StepName"].ToString() },
                    { "SenderName", dr["SenderName"].ToString() },
                    { "ReceiveTime", dr["ReceiveTime"].ToString().ToDateTime().ToDateTimeString() },
                    { "StatusTitle", status },
                    { "Note", dr["Note"].ToString() },
                    { "Opation", opation.ToString() }
                };
                jArray.Add(jObject);
            }
            return "{\"userdata\":{\"total\":" + count + ",\"pagesize\":" + size + ",\"pagenumber\":" + number + "},\"rows\":" + jArray.ToString() + "}";
        }

        /// <summary>
        /// 已办事项
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        public IActionResult Completed()
        {
            ViewData["flowOptions"] = new Business.Flow().GetOptions();
            ViewData["query"] = "appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            ViewData["tabId"] = Request.Querys("tabid");
            return View();
        }

        /// <summary>
        /// 查询已办事项
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        public string QueryCompleted()
        {
            string appid = Request.Querys("appid");
            string tabid = Request.Querys("tabid");
            string flowid = Request.Forms("flowid");
            string title = Request.Forms("title");
            string date1 = Request.Forms("date1");
            string date2 = Request.Forms("date2");
            string sidx = Request.Forms("sidx");
            string sord = Request.Forms("sord");
            string order = (sidx.IsNullOrEmpty() ? "CompletedTime1" : sidx) + " " + (sord.IsNullOrEmpty() ? "ASC" : sord);
            int size = Tools.GetPageSize();
            int number = Tools.GetPageNumber();

            Business.FlowTask flowTask = new Business.FlowTask();
            DataTable dataTable = flowTask.GetCompletedTask(size, number, Current.UserId, flowid, title, date1, date2, order, out int count);
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (DataRow dr in dataTable.Rows)
            {
                int openModel = 0, width = 0, height = 0;
                StringBuilder opation = new StringBuilder(
                    "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('" + Url.Content("~/RoadFlowCore/FlowRun/Index") + "?" + string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}&display=1",
                        dr["FlowId"], dr["StepId"], dr["InstanceId"], dr["Id"], dr["GroupId"], appid
                        ) + "','" + dr["Title"].ToString().RemoveHTML().UrlEncode() + "','" + dr["Id"] + "'," + openModel + "," + width + "," + height + ");return false;\"><i class=\"fa fa-file-text-o\"></i>表单</a>"+
                        "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"detail('" + dr["FlowId"] + "','" + dr["GroupId"] + "','" + dr["Id"] + "');return false;\"><i class=\"fa fa-navicon\"></i>过程</a>");
                bool isHasten = flowTask.IsHasten(dr["Id"].ToString().ToGuid(), out bool isWithdraw);
                if (isWithdraw)
                {
                    opation.Append("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"withdraw('" + dr["Id"] + "','" + dr["GroupId"] + "');return false;\"><i class=\"fa fa-mail-reply\"></i>收回</a>");
                }
                if (isHasten)
                {
                    opation.Append("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"hasten('" + dr["Id"] + "','" + dr["GroupId"] + "');return false;\"><i class=\"fa fa-bullhorn\"></i>催办</a>");
                }
                string taskTitle = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('" + Url.Content("~/RoadFlowCore/FlowRun/Index") + "?" + string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}&display=1",
                        dr["FlowId"], dr["StepId"], dr["InstanceId"], dr["Id"], dr["GroupId"], appid
                        ) + "','" + dr["Title"].ToString().RemoveHTML().UrlEncode() + "','" + dr["Id"] + "'," + openModel + "," + width + "," + height + ");return false;\">" + dr["Title"] + "</a>";
                string note = dr["Note"].ToString();
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", dr["Id"].ToString() },
                    { "Title", taskTitle },
                    { "FlowName", dr["FlowName"].ToString() },
                    { "StepName", dr["StepName"].ToString() },
                    { "SenderName", dr["SenderName"].ToString() },
                    { "ReceiveTime", dr["ReceiveTime"].ToString().ToDateTime().ToDateTimeString() },
                    { "CompletedTime", dr["CompletedTime1"].ToString().ToDateTime().ToDateTimeString() },
                    { "Note", note },
                    { "Opation", opation.ToString() }
                };
                jArray.Add(jObject);
            }
            return "{\"userdata\":{\"total\":" + count + ",\"pagesize\":" + size + ",\"pagenumber\":" + number + "},\"rows\":" + jArray.ToString() + "}";
        }

        /// <summary>
        /// 我的流程
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        public IActionResult MyStarts()
        {
            ViewData["flowOptions"] = new Business.Flow().GetOptions();
            ViewData["query"] = "appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            ViewData["tabId"] = Request.Querys("tabid");
            return View();
        }

        /// <summary>
        /// 查询我的流程
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        public string QueryMyStarts()
        {
            string appid = Request.Querys("appid");
            string tabid = Request.Querys("tabid");
            string flowid = Request.Forms("flowid");
            string title = Request.Forms("title");
            string date1 = Request.Forms("date1");
            string date2 = Request.Forms("date2");
            string sidx = Request.Forms("sidx");
            string sord = Request.Forms("sord");
            string order = (sidx.IsNullOrEmpty() ? "ReceiveTime" : sidx) + " " + (sord.IsNullOrEmpty() ? "ASC" : sord);
            int size = Tools.GetPageSize();
            int number = Tools.GetPageNumber();

            Business.FlowTask flowTask = new Business.FlowTask();
            DataTable dataTable = flowTask.GetMyStartList(size, number, Current.UserId, flowid, title, date1, date2, order, out int count);
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (DataRow dr in dataTable.Rows)
            {
                int openModel = 0, width = 0, height = 0;
                StringBuilder opation = new StringBuilder(
                    "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('" + Url.Content("~/RoadFlowCore/FlowRun/Index") + "?" + string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}&display=1",
                        dr["FlowId"], dr["StepId"], dr["InstanceId"], dr["Id"], dr["GroupId"], appid
                        ) + "','" + dr["Title"].ToString().RemoveHTML().UrlEncode() + "','" + dr["Id"] + "'," + openModel + "," + width + "," + height + ");return false;\"><i class=\"fa fa-file-text-o\"></i>表单</a>" +
                        "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"detail('" + dr["FlowId"] + "','" + dr["GroupId"] + "','" + dr["Id"] + "');return false;\"><i class=\"fa fa-navicon\"></i>过程</a>");
                //bool isHasten = flowTask.IsHasten(dr["Id"].ToString().ToGuid(), out bool isWithdraw);
                //if (isWithdraw)
                //{
                //    opation.Append("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"withdraw('" + dr["Id"] + "','" + dr["GroupId"] + "');return false;\"><i class=\"fa fa-mail-reply\"></i>收回</a>");
                //}
                //if (isHasten)
                //{
                //    opation.Append("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"hasten('" + dr["Id"] + "','" + dr["GroupId"] + "');return false;\"><i class=\"fa fa-bullhorn\"></i>催办</a>");
                //}
                string taskTitle = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('" + Url.Content("~/RoadFlowCore/FlowRun/Index") + "?" + string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}&display=1",
                        dr["FlowId"], dr["StepId"], dr["InstanceId"], dr["Id"], dr["GroupId"], appid
                        ) + "','" + dr["Title"].ToString().RemoveHTML().UrlEncode() + "','" + dr["Id"] + "'," + openModel + "," + width + "," + height + ");return false;\">" + dr["Title"] + "</a>";
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", dr["Id"].ToString() },
                    { "Title", taskTitle },
                    { "FlowName", dr["FlowName"].ToString() },
                    { "ReceiveTime", dr["ReceiveTime"].ToString().ToDateTime().ToDateTimeString() },
                    { "Opation", opation.ToString() }
                };
                jArray.Add(jObject);
            }
            return "{\"userdata\":{\"total\":" + count + ",\"pagesize\":" + size + ",\"pagenumber\":" + number + "},\"rows\":" + jArray.ToString() + "}";
        }

        /// <summary>
        /// 我的已委托事项
        /// </summary>
        /// <returns></returns>
        public IActionResult MyEntrust()
        {
            ViewData["flowOptions"] = new Business.Flow().GetOptions();
            ViewData["query"] = "appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            return View();
        }

        /// <summary>
        /// 查询我的已委托事项
        /// </summary>
        /// <returns></returns>
        public string QueryMyEntrust()
        {
            string appid = Request.Querys("appid");
            string tabid = Request.Querys("tabid");
            string flowid = Request.Forms("flowid");
            string title = Request.Forms("title");
            string date1 = Request.Forms("date1");
            string date2 = Request.Forms("date2");
            string sidx = Request.Forms("sidx");
            string sord = Request.Forms("sord");
            string order = (sidx.IsNullOrEmpty() ? "ReceiveTime" : sidx) + " " + (sord.IsNullOrEmpty() ? "ASC" : sord);
            int size = Tools.GetPageSize();
            int number = Tools.GetPageNumber();

            Business.FlowTask flowTask = new Business.FlowTask();
            DataTable dataTable = flowTask.GetEntrustTask(size, number, Current.UserId, flowid, title, date1, date2, order, out int count);
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (DataRow dr in dataTable.Rows)
            {
                int openModel = 0, width = 0, height = 0;
                StringBuilder opation = new StringBuilder();
                bool isEntrustWithdraw = flowTask.IsEntrustWithdraw(dr["Id"].ToString().ToGuid());
                if (isEntrustWithdraw)
                {
                    opation.Append("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"withdraw('" + dr["Id"] + "');return false;\"><i class=\"fa fa-mail-reply\"></i>收回</a>");
                }
                string taskTitle = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('" + Url.Content("~/RoadFlowCore/FlowRun/Index") + "?" + string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}&display=1",
                        dr["FlowId"], dr["StepId"], dr["InstanceId"], dr["Id"], dr["GroupId"], appid
                        ) + "','" + dr["Title"].ToString().RemoveHTML().UrlEncode() + "','" + dr["Id"] + "'," + openModel + "," + width + "," + height + ");return false;\">" + dr["Title"] + "</a>";
                string status = flowTask.GetExecuteTypeTitle(dr["ExecuteType"].ToString().ToInt());
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", dr["Id"].ToString() },
                    { "Title", taskTitle },
                    { "FlowName", dr["FlowName"].ToString() },
                    { "StepName", dr["StepName"].ToString() },
                    { "SenderName", dr["SenderName"].ToString() },
                    { "ReceiveTime", dr["ReceiveTime"].ToString().ToDateTime().ToDateTimeString() },
                    { "ReceiveName", dr["ReceiveName"].ToString() },
                    { "Status", status },
                    { "Opation", opation.ToString() }
                };
                jArray.Add(jObject);
            }
            return "{\"userdata\":{\"total\":" + count + ",\"pagesize\":" + size + ",\"pagenumber\":" + number + "},\"rows\":" + jArray.ToString() + "}";
        }

        /// <summary>
        /// 收回委托事项
        /// </summary>
        /// <returns></returns>
        public string EntrustWithdrawTask()
        {
            string taskid = Request.Querys("taskid");
            if (!taskid.IsGuid(out Guid tid))
            {
                return "任务ID错误!";
            }
            Business.FlowTask flowTask = new Business.FlowTask();
            var taskModel = flowTask.Get(tid);
            if (null == taskModel)
            {
                return "未找到任务!";
            }
            if (!taskModel.EntrustUserId.HasValue)
            {
                return "该任务不是委托任务!";
            }
            taskModel.ReceiveId = taskModel.EntrustUserId.Value;
            taskModel.ReceiveName = new Business.User().GetName(taskModel.EntrustUserId.Value);
            taskModel.EntrustUserId = new Guid?();
            taskModel.Note = "";
            flowTask.Update(taskModel);
            return "收回成功，请到待办事项中查看!";
        }

        /// <summary>
        /// 任务催办
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        public IActionResult Hasten()
        {
            string hastentaskid = Request.Querys("hastentaskid");
            string hastengroupid = Request.Querys("hastengroupid");
            if (!hastengroupid.IsGuid(out Guid groupId) || !hastentaskid.IsGuid(out Guid taskId))
            {
                return new ContentResult() { Content = "任务Id错误!" };
            }
            Business.FlowTask flowTask = new Business.FlowTask();
            //得到可以催办的人员
            var groupTasks = flowTask.GetListByGroupId(groupId);
            var hastenTasks = groupTasks.FindAll(p => p.PrevId == taskId && p.Status.In(0, 1));
            List<Guid> userIds = new List<Guid>();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var task in hastenTasks)
            {
                if (task.Status.In(0, 1) && !userIds.Contains(task.ReceiveId))
                {
                    userIds.Add(task.ReceiveId);
                    stringBuilder.Append("<input checked=\"checked\" type=\"checkbox\" style=\"vertical-align:middle;\" value=\"" + Business.Organize.PREFIX_USER + task.ReceiveId.ToString() + "\" name=\"Users\" id=\"Users_" + task.ReceiveId.ToString("N") + "\" /><label style=\"vertical-align:middle;\" for=\"Users_" + task.ReceiveId.ToString("N") + "\">" + task.ReceiveName + "</label>");
                }
            }
            ViewData["Users"] = stringBuilder.ToString();
            ViewData["Contents"] = "您有一个待办事项“" + (hastenTasks.Any() ? hastenTasks.First().Title : "") + "”，请尽快处理！";
            ViewData["queryString"] = Request.UrlQuery();
            return View();
        }

        /// <summary>
        /// 催办
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        [ValidateAntiForgeryToken]
        public string HastenSave()
        {
            string hastentaskid = Request.Querys("hastentaskid");
            string hastengroupid = Request.Querys("hastengroupid");
            if (!hastengroupid.IsGuid(out Guid groupId) || !hastentaskid.IsGuid(out Guid taskId))
            {
                return "Id错误!";
            }
            string Users = Request.Forms("Users");
            string Model = Request.Forms("Model");
            string Contents = Request.Forms("Contents");
            Business.FlowTask flowTask = new Business.FlowTask();
            //得到可以催办的人员
            var groupTasks = flowTask.GetListByGroupId(groupId);
            var hastenTasks = groupTasks.FindAll(p => p.PrevId == taskId && p.Status.In(0, 1));
            List<Model.FlowTask> hastenTasks1 = new List<Model.FlowTask>();
            foreach (var task in hastenTasks)
            {
                if (task.Status.In(0, 1) && Users.ContainsIgnoreCase(task.ReceiveId.ToString()))
                {
                    hastenTasks1.Add(task);
                }
            }
            
            flowTask.SendMessage(hastenTasks1, Current.User, Model, Contents);
            return "催办成功!";
        }

        /// <summary>
        /// 收回
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        [ValidateAntiForgeryToken]
        public string Withdraw()
        {
            string withdrawtaskid = Request.Querys("withdrawtaskid");
            string withdrawgroupid = Request.Querys("withdrawgroupid");
            if (!withdrawgroupid.IsGuid(out Guid groupId) || !withdrawtaskid.IsGuid(out Guid taskId))
            {
                return "任务Id错误!";
            }
            Business.FlowTask flowTask = new Business.FlowTask();
            var groupTasks = flowTask.GetListByGroupId(groupId);
            var tasks = groupTasks.FindAll(p => p.PrevId == taskId && p.Status == 0);
            List<Model.FlowTask> removeTasks = new List<Model.FlowTask>();
            foreach (var task in tasks)
            {
                removeTasks.Add(task);
            }
            List<Model.FlowTask> updateTasks = new List<Model.FlowTask>();
            var currentTask = groupTasks.Find(p => p.Id == taskId);
            if (null == currentTask)
            {
                return "未找到当前任务!";
            }
            currentTask.Status = 0;
            currentTask.ExecuteType = 0;
            currentTask.Comments = "";
            currentTask.IsSign = 0;
            currentTask.CompletedTime1 = new DateTime?();
            updateTasks.Add(currentTask);
            flowTask.Update(removeTasks, updateTasks, null, null);
            return "收回成功!";
        }

        /// <summary>
        /// 作废任务
        /// </summary>
        /// <returns></returns>
        public string DeleteTask()
        {
            string deltaskid = Request.Querys("deltaskid");
            if (!deltaskid.IsGuid(out Guid taskId))
            {
                return "Id错误!";
            }
            Business.FlowTask flowTask = new Business.FlowTask();
            var taskModel = flowTask.Get(taskId);
            if (null == taskModel)
            {
                return "作废成功!";
            }
            flowTask.DeleteByGroupId(taskModel.GroupId);
            var flowrunModel = new Business.Flow().GetFlowRunModel(taskModel.FlowId);
            if (null == flowrunModel)
            {
                return "作废成功!";
            }
            var stepModel = flowrunModel.Steps.Find(p => p.Id == taskModel.StepId);
            if (null == stepModel)
            {
                return "作废成功!";
            }
            var applibrary = new Business.AppLibrary().Get(stepModel.StepForm.Id);
            if(null !=applibrary && applibrary.Code.IsGuid(out Guid formId))
            {
                new Business.Form().DeleteFormData(formId, taskModel.InstanceId);
            }
            return "作废成功!";
        }

        /// <summary>
        /// 实例管理列表
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult Instance()
        {
            ViewData["appid"] = Request.Querys("appid");
            ViewData["query"] = "&appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            ViewData["flowOptions"] = new Business.Flow().GetManageInstanceOptions(Current.UserId);
            return View();
        }

        /// <summary>
        /// 查询实例列表
        /// </summary>
        /// <returns></returns>
        [Validate]
        public string QueryInstance()
        {
            string appid = Request.Querys("appid");
            string tabid = Request.Querys("tabid");
            string FlowID = Request.Forms("FlowID");
            string Title = Request.Forms("Title");
            string ReceiveID = Request.Forms("ReceiveID");
            string Date1 = Request.Forms("Date1");
            string Date2 = Request.Forms("Date2");
            string sidx = Request.Forms("sidx");
            string sord = Request.Forms("sord");
            string order = (sidx.IsNullOrEmpty() ? "ReceiveTime" : sidx) + " " + (sord.IsNullOrEmpty() ? "DESC" : sord);
            int size = Tools.GetPageSize();
            int number = Tools.GetPageNumber();
            Business.FlowTask flowTask = new Business.FlowTask();
            Guid userId = new Business.User().GetUserId(ReceiveID);
            if (FlowID.IsNullOrWhiteSpace())
            {
                FlowID = new Business.Flow().GetManageInstanceFlowIds(Current.UserId).JoinSqlIn();
            }
            DataTable dataTable = flowTask.GetInstanceList(size, number, FlowID, Title, userId.IsEmptyGuid() ? "" : userId.ToString(), Date1, Date2, order, out int count);
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (DataRow dr in dataTable.Rows)
            {
                var groupTasks = flowTask.GetListByGroupId(dr["GroupId"].ToString().ToGuid());
                Model.FlowTask taskModel = null;
                if (!Title.IsNullOrWhiteSpace())
                {
                    taskModel = groupTasks.Where(p=>p.Title.ContainsIgnoreCase(Title)).OrderByDescending(p => p.Sort).ThenBy(p => p.Status).First();
                }
                if (null == taskModel)
                {
                    taskModel = groupTasks.OrderByDescending(p => p.Sort).ThenBy(p => p.Status).First();
                }
                int openModel = 0, width = 0, height = 0;
                string opation = "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"manage('" + taskModel.Id.ToString() + "', '" + taskModel.GroupId.ToString() + "');\"><i class=\"fa fa-check-square-o\"></i>管理</a>";
                //if (taskModel.ExecuteType.In(-1, 0, 1))
                //{
                    opation += "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"delete1('" + taskModel.Id.ToString() + "', '" + taskModel.GroupId.ToString() + "');\"><i class=\"fa fa-trash-o\"></i>删除</a>";
                //}
               
                string status = flowTask.GetExecuteTypeTitle(taskModel.ExecuteType);
                string taskTitle = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('" + Url.Content("~/RoadFlowCore/FlowRun/Index") + "?" + string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}&display=1",
                        taskModel.FlowId.ToString(), taskModel.StepId.ToString(), taskModel.InstanceId, taskModel.Id.ToString(), taskModel.GroupId.ToString(), appid
                        ) + "','" + taskModel.Title.TrimAll() + "','" + taskModel.Id.ToString() + "'," + openModel + "," + width + "," + height + ");return false;\">" + taskModel.Title + "</a>";
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", taskModel.Id.ToString() },
                    { "Title", taskTitle },
                    { "FlowName", taskModel.FlowName },
                    { "StepName", taskModel.StepName },
                    { "ReceiveName", taskModel.ReceiveName },
                    { "ReceiveTime", taskModel.ReceiveTime.ToDateTimeString() },
                    { "StatusTitle", status },
                    { "Opation", opation }
                };
                jArray.Add(jObject);
            }
            return "{\"userdata\":{\"total\":" + count + ",\"pagesize\":" + size + ",\"pagenumber\":" + number + "},\"rows\":" + jArray.ToString() + "}";
        }

        /// <summary>
        /// 删除实例
        /// </summary>
        /// <returns></returns>
        [Validate]
        public string DeleteInstance()
        {
            string groupId = Request.Querys("groupid");
            if (!groupId.IsGuid(out Guid guid))
            {
                return "组ID错误!";
            }
            Business.FlowTask flowTask = new Business.FlowTask();
            var groupTasks = flowTask.GetListByGroupId(guid);
            if (groupTasks.Count > 0)
            {
                flowTask.DeleteByGroupId(groupTasks.ToArray());
                
                Business.Log.Add("删除了流程实例-" + groupTasks.First().Title, Newtonsoft.Json.JsonConvert.SerializeObject(groupTasks), Business.Log.Type.流程运行);
            }
            return "删除成功!";
        }

        /// <summary>
        /// 实例管理
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult InstanceManage()
        {
            string groupId = Request.Querys("groupid");
            Business.FlowTask flowTask = new Business.FlowTask();
            var groupTasks = flowTask.GetListByGroupId(groupId.ToGuid());
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (var groupTask in groupTasks)
            {
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject();
                string opation = "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"cngStatus('" + groupTask.Id.ToString() + "');\"><i class=\"fa fa-exclamation\"></i>状态</a>";
                if (groupTask.ExecuteType.In(0, 1) && 5 != groupTask.TaskType)
                {
                    opation += "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"designate('" + groupTask.Id.ToString() + "');\"><i class=\"fa fa-hand-o-right\"></i>指派</a>"
                            + "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"goTo('" + groupTask.Id.ToString() + "');\"><i class=\"fa fa-level-up\"></i>跳转</a>";
                }
                jObject.Add("id", groupTask.Id);
                jObject.Add("StepID", groupTask.StepName);
                jObject.Add("SenderName", groupTask.SenderName);
                jObject.Add("ReceiveTime", groupTask.ReceiveTime.ToDateTimeString());
                jObject.Add("ReceiveName", groupTask.ReceiveName);
                jObject.Add("CompletedTime1", groupTask.CompletedTime1.HasValue ? groupTask.CompletedTime1.Value.ToDateTimeString() : "");
                jObject.Add("Status", flowTask.GetExecuteTypeTitle(groupTask.ExecuteType));
                jObject.Add("Comment", groupTask.Comments);
                jObject.Add("Note", groupTask.Note);
                jObject.Add("Opation", opation);
                jArray.Add(jObject);
            }
            ViewData["json"] = jArray.ToString();
            ViewData["appid"] = Request.Querys("appid");
            ViewData["iframeid"] = Request.Querys("iframeid");
            return View();
        }

        /// <summary>
        /// 改变任务状态
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult ChangeStatus()
        {
            string taskId = Request.Querys("taskid");
            Business.FlowTask flowTask = new Business.FlowTask();
            var taskModel = flowTask.Get(taskId.ToGuid());
            if (null == taskModel)
            {
                return new ContentResult() { Content = "未找到当前任务!" };
            }
            ViewData["queryString"] = Request.UrlQuery();
            ViewData["statusOptions"] = new Business.FlowTask().GetExecuteTypeOptions(taskModel.ExecuteType);
            return View();
        }

        /// <summary>
        /// 保存任务状态
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string SaveStatus()
        {
            string status = Request.Forms("status");
            string taskId = Request.Querys("taskid");
            if (!taskId.IsGuid(out Guid guid))
            {
                return "任务ID错误!";
            }
            if (!status.IsInt(out int i))
            {
                return "状态错误!";
            }
            Business.FlowTask flowTask = new Business.FlowTask();
            var taskModel = flowTask.Get(guid);
            string oldModel = taskModel.ToString();
            if (null == taskModel)
            {
                return "未找到当前任务!";
            }
            taskModel.ExecuteType = i;
            if (i < 2)
            {
                taskModel.Status = i;
            }
            else
            {
                taskModel.Status = 2;
            }
            flowTask.Update(taskModel);
            Business.Log.Add("改变了任务状态-" + taskModel.Title, "", Business.Log.Type.流程运行, oldModel, taskModel.ToString());
            return "保存成功!";
        }

        /// <summary>
        /// 指派任务
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult Designate()
        {
            ViewData["queryString"] = Request.UrlQuery();
            return View();
        }

        /// <summary>
        /// 保存指派任务
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string SaveDesignate()
        {
            string taskid = Request.Querys("taskid");
            string user = Request.Forms("user");
            if (!taskid.IsGuid(out Guid taskId))
            {
                return "任务ID错误!";
            }
            if (user.IsNullOrWhiteSpace())
            {
                return "没有选择要指派的人员!";
            }
            Business.FlowTask flowTask = new Business.FlowTask();
            var taskModel = flowTask.Get(taskId);
            if (null == taskModel)
            {
                return "没有找到当前任务!";
            }
            string msg = flowTask.Designate(taskModel, new Business.Organize().GetAllUsers(user));
            Business.Log.Add("指派了任务-" + taskModel.Title, taskModel.ToString() + "-" + msg, Business.Log.Type.流程运行, others: user);
            return "1".Equals(msg) ? "指派成功!" : msg;
        }

        /// <summary>
        /// 跳转
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult GoTo()
        {
            string taskid = Request.Querys("taskid");
            if (!taskid.IsGuid(out Guid taskId))
            {
                return new ContentResult() { Content= "任务ID错误!" };
            }
            var taskModel = new Business.FlowTask().Get(taskId);
            if (null == taskModel)
            {
                return new ContentResult() { Content = "未找到当前任务!" };
            }
            var flowRunModel = new Business.Flow().GetFlowRunModel(taskModel.FlowId);
            if (null == flowRunModel)
            {
                return new ContentResult() { Content = "未找到当前流程运行时!" };
            }
            ViewData["steps"] = flowRunModel.Steps;
            ViewData["queryString"] = Request.UrlQuery();
            return View();
        }

        /// <summary>
        /// 保存跳转
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string SaveGoTo()
        {
            string taskid = Request.Querys("taskid");
            string steps = Request.Forms("step");
            if (!taskid.IsGuid(out Guid taskId))
            {
                return "任务ID错误!";
            }
            Business.FlowTask flowTask = new Business.FlowTask();
            var taskModel = flowTask.Get(taskId);
            if (null == taskModel)
            {
                return "未找到当前任务!";
            }
            Dictionary<Guid, List<Model.User>> dicts = new Dictionary<Guid, List<Model.User>>();
            Business.Organize organize = new Business.Organize();
            foreach (string step in steps.Split(','))
            {
                if (!step.IsGuid(out Guid stepId))
                {
                    continue;
                }
                string member = Request.Forms("member_" + step);
                if (member.IsNullOrWhiteSpace())
                {
                    continue;
                }
                dicts.Add(stepId, organize.GetAllUsers(member));
            }
            string msg = new Business.FlowTask().GoTo(taskModel, dicts);
            Business.Log.Add("跳转了任务-" + taskModel.Title, taskModel.ToString() + "-" + msg, Business.Log.Type.流程运行, others: Newtonsoft.Json.JsonConvert.SerializeObject(dicts));
            return "1".Equals(msg) ? "跳转成功!" : msg;
        }

        /// <summary>
        /// 查看处理过程
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        public IActionResult Detail()
        {
            string flowid = Request.Querys("flowid");
            string groupid = Request.Querys("groupid");
            string taskid = Request.Querys("taskie");
            string appid = Request.Querys("appid");
            string tabid = Request.Querys("tabid");
            string displaymodel = Request.Querys("displaymodel");
            string ismobile = Request.Querys("ismobile");

            Business.FlowTask bflowTask = new Business.FlowTask();
            var flowTasks = bflowTask.GetListByGroupId(groupid.ToGuid());

            //判断不是流程参与者不能查看
            if (!flowTasks.Exists(p => p.ReceiveId == Current.UserId))
            {
                return new ContentResult() { Content = "您不是流程参与者，不能查看!" };
            }

            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            Newtonsoft.Json.Linq.JArray jArray1 = new Newtonsoft.Json.Linq.JArray();
            foreach (var flowTask in flowTasks)
            {
                string stepName = flowTask.StepName;
                if (!flowTask.SubFlowGroupId.IsNullOrWhiteSpace())
                {
                    stepName = "<a href=\"javascript:;\" class=\"blue1\" onclick=\"showSubFlow('" + flowTask.Id + "')\">" + stepName + "</a>";
                }
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "StepName", stepName },
                    { "SenderName", flowTask.SenderName },
                    { "SenderTime", flowTask.ReceiveTime.ToDateTimeString() },
                    { "ReceiveName", flowTask.ReceiveName },
                    { "CompletedTime1", flowTask.CompletedTime1.ToDateTimeString() },
                    { "StatusTitle", bflowTask.GetExecuteTypeTitle(flowTask.ExecuteType) },
                    { "Comment", flowTask.Comments },
                    { "Note", flowTask.Note }
                };
                jArray.Add(jObject);
                if (true)
                {
                    Newtonsoft.Json.Linq.JObject jObject1 = new Newtonsoft.Json.Linq.JObject
                    {
                        { "stepid", flowTask.StepId },
                        { "prevstepid", flowTask.PrevStepId },
                        { "status", flowTask.Status },
                        { "sender", flowTask.SenderName },
                        { "sendtime", flowTask.ReceiveTime.ToDateTimeString() },
                        { "receiver", flowTask.ReceiveName },
                        { "completedtime1", flowTask.CompletedTime1.ToDateTimeString() },
                        { "comment", flowTask.Comments },
                        { "sort", flowTask.Sort },
                        { "tasktype", flowTask.TaskType },
                        { "statustitle", bflowTask.GetExecuteTypeTitle(flowTask.ExecuteType)}
                    };
                    jArray1.Add(jObject1);
                }
            }
            ViewData["json"] = jArray.ToString();
            ViewData["json1"] = jArray1.ToString();
            ViewData["displaymodel"] = displaymodel.IsNullOrWhiteSpace() ? "0" : displaymodel;
            ViewData["tabid"] = tabid;
            ViewData["query"] = "flowid=" + flowid + "&stepid=" + Request.Querys("stepid") + "&groupid=" + groupid + "&taskid=" + taskid + "&appid=" + appid + "&tabid=" + tabid + "&iframeid=" + Request.Querys("iframeid") + "&ismobile=" + ismobile;
            ViewData["flowid"] = flowid;
            ViewData["ismobile"] = ismobile;
            //判断如果有动态步骤，流程图要从rf_flowdynamic表中取
            var dynamicSteps = Config.EnableDynamicStep ? flowTasks.Where(p => p.BeforeStepId.IsNotEmptyGuid()).OrderByDescending(p => p.Sort).ToList() : new List<Model.FlowTask>();
            if (dynamicSteps.Count > 0)
            {
                ViewData["stepid"] = dynamicSteps.First().BeforeStepId.ToString();
                ViewData["groupid"] = dynamicSteps.First().GroupId.ToString();
            }
            else
            {
                ViewData["stepid"] = string.Empty;
                ViewData["groupid"] = string.Empty;
            }
            return View();
        }

        /// <summary>
        /// 查看子流程处理过程
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        public IActionResult DetailSubFlow()
        {
            string taskid = Request.Querys("taskid");
            string ismobile = Request.Querys("ismobile");
            if (!taskid.IsGuid(out Guid taskId))
            {
                return new ContentResult() { Content = "任务ID错误!" };
            }
            Business.FlowTask flowTask = new Business.FlowTask();
            var currentTask = flowTask.Get(taskId);
            if (null == currentTask)
            {
                return new ContentResult() { Content = "未找到当前任务!" };
            }
            if (currentTask.SubFlowGroupId.IsNullOrWhiteSpace())
            {
                return new ContentResult() { Content = "未找到当前任务的子流程任务!" };
            }
            string[] subFlowGroupIds = currentTask.SubFlowGroupId.Split(',');

            if (subFlowGroupIds.Length == 1)
            {
                var groupTasks = flowTask.GetListByGroupId(subFlowGroupIds[0].ToGuid());
                if (groupTasks.Count > 0)
                {
                    return Redirect("Detail?flowid=" + groupTasks.First().FlowId + "&stepid=" + groupTasks.First().StepId +
                        "&groupid=" + subFlowGroupIds[0] + "&taskid=" + groupTasks.First().Id + "&appid=" + Request.Querys("appid")
                        + "&tabid=" + Request.Querys("tabid") + "&iframeid=" + Request.Querys("iframeid") + "&ismobile=" + ismobile);
                }
            }
            else
            {
                Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
                foreach (var id in subFlowGroupIds)
                {
                    var groupTasks = flowTask.GetListByGroupId(id.ToGuid());
                    if (groupTasks.Count == 0)
                    {
                        continue;
                    }
                    var groupTask = groupTasks.First();
                    Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                    {
                        { "FlowName", groupTask.FlowName },
                        { "StepName", groupTask.StepName },
                        { "SenderName", groupTask.SenderName },
                        { "SenderTime", groupTask.ReceiveTime.ToDateTimeString() },
                        { "ReceiveName", groupTask.ReceiveName },
                        { "CompletedTime1", groupTask.CompletedTime1.HasValue ? groupTask.CompletedTime1.Value.ToShortDateString() : "" },
                        { "StatusTitle", flowTask.GetExecuteTypeTitle(groupTask.ExecuteType) },
                        { "Show", "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"detail('" + groupTask.FlowId + "', '" + groupTask.GroupId + "');return false;\"><i class=\"fa fa-search\"></i>查看</a>" }
                    };
                    jArray.Add(jObject);
                }
                ViewData["json"] = jArray.ToString();
                ViewData["ismobile"] = ismobile;
            }
            return View();
        }
    }
}