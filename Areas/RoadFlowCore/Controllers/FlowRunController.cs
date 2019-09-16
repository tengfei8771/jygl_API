using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoadFlow.Utility;
using Newtonsoft.Json.Linq;

namespace RoadFlow.Mvc.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class FlowRunController : Controller
    {
        [Validate(CheckApp = false, CheckUrl = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult Index()
        {
            string flowid = Request.Querys("flowid");
            string stepid = Request.Querys("stepid");
            string taskid = Request.Querys("taskid");
            string groupid = Request.Querys("groupid");
            string instanceid = Request.Querys("instanceid");
            string appid = Request.Querys("appid");
            string tabid = Request.Querys("tabid");
            string display = Request.Querys("display");
            string showtoolbar = Request.Querys("showtoolbar");//是否显示工具栏，查看表单的时候，有时不需要显示。
            string ismobile = Request.Querys("ismobile");
            int rf_appopenmodel = Request.Querys("rf_appopenmodel").ToInt(0);//窗口打开方式 0tab, 1,2弹出层 3,4,5弹出窗口

            if (display.IsNullOrEmpty())
            {
                display = "0";
            }
            if (!flowid.IsGuid(out Guid flowId))
            {
                return new ContentResult() { Content = "流程ID错误!" };
            }
            Business.Flow flow = new Business.Flow();
            Business.FlowTask flowTask = new Business.FlowTask();
            var (dynamicTaskModel, flowTaskModel, groupTasks) = flowTask.GetDynamicTask(groupid.ToGuid(), taskid.ToGuid());
            Model.FlowRun flowRunModel = flow.GetFlowRunModel(flowId, true, dynamicTaskModel);
            if (null == flowRunModel)
            {
                return new ContentResult() { Content = "未找到流程运行时实体!" };
            }
            Guid stepId = stepid.IsGuid(out Guid stepGuid) ? stepGuid : flowRunModel.FirstStepId;
            if (null == flowTaskModel && flowRunModel.Status != 1)
            {
                string statusTitle = flow.GetStatusTitle(flowRunModel.Status);
                return new ContentResult() { Content = "该流程" + statusTitle + ",不能发起新实例!" };
            }

            if(null != flowTaskModel && flowTaskModel.ReceiveId != Current.UserId)
            {
                return new ContentResult() { Content = "您不能处理当前任务!" };
            }

            if (null != flowTaskModel && !"1".Equals(display) && flowTaskModel.Status.NotIn(0, 1))
            {
                return new ContentResult() { Content = "该任务" + (flowTaskModel.Status == -1 ? "等待中" : "已完成") + ",不能处理!" };
            }

            //抄送任务只读
            if(null != flowTaskModel && flowTaskModel.TaskType == 5)
            {
                display = "1";
            }

            bool isAddWrite = null != flowTaskModel && flowTaskModel.TaskType.In(6, 7, 8);//是否是加签
            Model.FlowRunModel.Step stepModel = flowRunModel.Steps.Find(p => p.Id == stepId);
            if (null == stepModel)
            {
                return new ContentResult() { Content = "未找到当前步骤运行时实体!" };
            }
            //检查并发处理
            if (1 == stepModel.StepBase.ConcurrentModel && null != flowTaskModel && flowTaskModel.TaskType != 5)
            {
                var executeTask = flowTask.GetListByGroupId(flowTaskModel.GroupId).Find(p => p.TaskType != 5 && p.Status == 1 && p.ReceiveId != flowTaskModel.ReceiveId);
                if (null != executeTask)
                {
                    string closeScript = string.Empty;
                    if (0 == rf_appopenmodel)
                    {
                        closeScript = "top.mainTab.closeTab();";
                    }
                    else if (rf_appopenmodel.In(1, 2))
                    {
                        closeScript = "top.mainDialog.close();";
                    }
                    else if (rf_appopenmodel.In(3, 4, 5))
                    {
                        closeScript = "window.close();";
                    }
                    return new ContentResult() { Content = "<script>alert('当前任务正由" + executeTask.ReceiveName + "处理中,请等待!');try{"+ closeScript + "}catch(e){}</script>", ContentType = "text/html" };
                }
            }
            //更新任务状态为处理中
            if (null != flowTaskModel && flowTaskModel.Status == 0)
            {
                flowTaskModel.Status = 1;
                flowTaskModel.ExecuteType = 1;
                if (!flowTaskModel.OpenTime.HasValue)//更新打开时间
                {
                    flowTaskModel.OpenTime = DateExtensions.Now;
                }
                flowTask.Update(flowTaskModel);
            }

            string query = "flowid=" + flowid + "&taskid=" + taskid + "&groupid=" + groupid + "&stepid=" + stepId + "&instanceid="
                + instanceid + "&appid=" + appid + "&tabid=" + tabid + "&dilplay=" + display + "&showtoolbar=" + showtoolbar
                + "&rf_appopenmodel=" + rf_appopenmodel.ToString() + "&ismobile=" + ismobile;

            #region 组织流程按钮
            List<Model.FlowRunModel.StepButton> stepButtons = new List<Model.FlowRunModel.StepButton>();
            List<Model.FlowButton> flowButtons = new List<Model.FlowButton>();
            Business.FlowButton flowButton = new Business.FlowButton();
            if ("1".Equals(display))//如果是查看则显示打印和过程查看按钮
            {
                if (!"0".Equals(showtoolbar))
                {
                    stepButtons.Add(new Model.FlowRunModel.StepButton()
                    {
                        Id = "cadd9f81-2b8c-479b-a7f1-3cec775768fa".ToGuid(),
                        Sort = 0
                    });
                    stepButtons.Add(new Model.FlowRunModel.StepButton()
                    {
                        Id = "d9511329-a03e-4af2-84e5-73beda0d3f42".ToGuid(),
                        Sort = 1
                    });
                    if (null != flowTaskModel && flowTaskModel.TaskType == 5 && flowTaskModel.Status.In(0,1))//如果是抄送任务，只显示已阅知按钮
                    {
                        stepButtons.Add(new Model.FlowRunModel.StepButton()
                        {
                            Id = "aa27eab5-66f0-4cde-958f-1f28da4b4392".ToGuid(),
                            Sort = 1
                        });
                    }
                }
            }
            else
            {
                stepButtons.AddRange(stepModel.StepButtons);
            }
            //如果是子流程要显示查看主流程表单按钮
            if (null != flowTaskModel && flowTaskModel.OtherType == 1)
            {
                stepButtons.Add(new Model.FlowRunModel.StepButton() {
                    Id = "1673ff03-48c6-465a-9caa-46b776c932a9".ToGuid(),
                    Sort = stepButtons.Count + 5
                });
            }
            foreach (var stepButton in stepButtons)
            {
                if (stepButton.Id.IsEmptyGuid())
                {
                    var flowButtonModel = new Model.FlowButton() { Id = Guid.Empty, Title = "" };
                    flowButtons.Add(flowButtonModel);
                }
                else
                {
                    var flowButtonModel = flowButton.Get(stepButton.Id).Clone();
                    if (null != flowButtonModel)
                    {
                        flowButtonModel.Title = stepButton.ShowTitle.IsNullOrWhiteSpace() ? flowButtonModel.Title : stepButton.ShowTitle;
                        flowButtonModel.Sort = stepButton.Sort;
                        flowButtons.Add(flowButtonModel);
                    }
                }
            }
            #endregion

            #region 得到表单地址
            Model.FlowRunModel.StepForm stepFormModel = stepModel.StepForm;
            string formUrl = string.Empty;
            bool isCustomeForm = false;//是否是自定义表单
            if (null != stepFormModel)
            {
                Guid formId = stepFormModel.Id;
                if ("1".Equals(ismobile) && stepFormModel.MobileId.IsNotEmptyGuid())//如果是移动端并且移动端表单不为空，则用移动端表单
                {
                    formId = stepFormModel.MobileId;
                }
                var applibraryModel = new Business.AppLibrary().Get(formId);
                if (null != applibraryModel)
                {
                    isCustomeForm = applibraryModel.Code.IsNullOrWhiteSpace();
                    if (isCustomeForm)
                    {
                        formUrl = new Business.Menu().GetAddress(applibraryModel.Address, query);
                    }
                    else
                    {
                        formUrl = Url.Content("~/wwwroot/RoadFlowResources/scripts/formDesigner/form/" + applibraryModel.Address);
                    }
                }
            }
            #endregion

            #region 签章图片
            var user = Current.User;
            if (null == user)
            {
                return new ContentResult() { Content = "未找到当前登录用户!" };
            }
            string signPath = Current.WebRootPath + "/RoadFlowResources/images/userSigns/" + user.Id.ToUpperString();
            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(signPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            if (!System.IO.File.Exists(signPath+ "/default.png"))
            {
                var img = new Business.User().CreateSignImage(user.Name);
                img.Save(signPath+ "/default.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            string signSrc = Url.Content("~/RoadFlowResources/images/userSigns/" + user.Id.ToUpperString() + "/default.png");
            #endregion

            ViewData["tabId"] = tabid;
            ViewData["appId"] = appid;
            ViewData["instanceId"] = instanceid;
            ViewData["display"] = display;
            ViewData["formUrl"] = formUrl;
            ViewData["query"] = query;
            ViewData["isCustomeForm"] = isCustomeForm ? "1" : "0";//是否是自定义表单
            ViewData["isDebug"] = flowRunModel.Debug;
            ViewData["signType"] = flowTaskModel != null && flowTaskModel.TaskType == 5 ? 0 : stepModel.SignatureType;//如果是抄送任务不显示意见栏
            ViewData["attachment"] = stepModel.Attachment;//是否可以传附件
            ViewData["flowType"] = stepModel.StepBase.FlowType;
            ViewData["isArchives"] = stepModel.Archives;
            ViewData["flowButtons"] = flowButtons;
            ViewData["signSrc"] = signSrc;//签章图片地址
            ViewData["request"] = Request;
            ViewData["showComment"] = stepModel.CommentDisplay;//是否显示历史意见
            ViewData["commentOptions"] = new Business.FlowComment().GetOptionsByUserId(user.Id);//默认处理意见选项
            ViewData["ismobile"] = ismobile;
            ViewData["Title"] = flowTaskModel != null ? flowTaskModel.Title : string.Empty;
            ViewData["comments"] = flowTaskModel != null ? flowTaskModel.Comments : string.Empty;
            ViewData["uploadAttachment"] = flowTaskModel != null ? flowTaskModel.Attachment : string.Empty;
            return View();
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult Execute()
        {
            string paramsJSON = Request.Forms("params");
            string issign = Request.Forms("issign");
            string comment = Request.Forms("comment");
            string attachment = Request.Forms("attachment");
            string flowid = Request.Querys("flowid");
            string instanceid = Request.Querys("instanceid");
            string taskid = Request.Querys("taskid");
            string stepid = Request.Querys("stepid");
            string groupid = Request.Querys("groupid");
            string ismobile = Request.Querys("ismobile");
            bool isCustomeForm = "1".Equals(Request.Forms("form_iscustomeform"));//是否是自定义表单
            if (instanceid.IsNullOrWhiteSpace())
            {
                instanceid = Request.Forms("instanceid");
            }
            if (instanceid.IsNullOrWhiteSpace())
            {
                instanceid = Request.Forms("form_instanceid");
            }
            ViewData["rf_appopenmodel"] = Request.Querys("rf_appopenmodel");
            #region 参数检查
            if (paramsJSON.IsNullOrWhiteSpace())
            {
                ViewData["alertMsg"] = "执行参数为空";
                ViewData["success"] = "0";
                return View();
            }
            if (!flowid.IsGuid(out Guid flowId))
            {
                ViewData["alertMsg"] = "流程ID错误";
                ViewData["success"] = "0";
                return View();
            }
            Business.Flow flow = new Business.Flow();
            Business.FlowTask flowTask = new Business.FlowTask();

            //查找当前任务是否有动态步骤，有则从动态步骤中取流程运行时
            var (dynamicTaskModel, flowTaskModel, groupTasks) = flowTask.GetDynamicTask(groupid.ToGuid(), taskid.ToGuid());
            var flowRunModel = flow.GetFlowRunModel(flowId, true, dynamicTaskModel);
            //================================================

            if (null == flowRunModel)
            {
                ViewData["alertMsg"] = "未找到流程运行实体";
                ViewData["success"] = "0";
                return View();
            }
            JObject paramsObject = JObject.Parse(paramsJSON);
            string opation = paramsObject.Value<string>("type");
            if (opation.IsNullOrWhiteSpace())
            {
                ViewData["alertMsg"] = "未找到要处理的类型";
                ViewData["success"] = "0";
                return View();
            }
            #endregion

            #region 组织执行参数实体
            Model.FlowRunModel.Execute executeModel = new Model.FlowRunModel.Execute();
            switch (opation)
            {
                case "freesubmit": //自由流程发送
                    executeModel.ExecuteType = Model.FlowRunModel.Execute.Type.FreeSubmit;
                    break;
                case "submit":
                    executeModel.ExecuteType = Model.FlowRunModel.Execute.Type.Submit;
                    break;
                case "save":
                    executeModel.ExecuteType = Model.FlowRunModel.Execute.Type.Save;
                    break;
                case "back":
                    executeModel.ExecuteType = Model.FlowRunModel.Execute.Type.Back;
                    break;
                case "completed":
                    executeModel.ExecuteType = Model.FlowRunModel.Execute.Type.Completed;
                    break;
                case "redirect":
                    executeModel.ExecuteType = Model.FlowRunModel.Execute.Type.Redirect;
                    break;
                case "addwrite":
                    executeModel.ExecuteType = Model.FlowRunModel.Execute.Type.AddWrite;
                    break;
                case "copyforcompleted": //抄送完成(已阅知)
                    executeModel.ExecuteType = Model.FlowRunModel.Execute.Type.CopyforCompleted;
                    break;
                case "taskend":
                    executeModel.ExecuteType = Model.FlowRunModel.Execute.Type.TaskEnd;
                    break;
            }
            executeModel.IsAutoSubmit = false;
            executeModel.Comment = comment.Trim();
            executeModel.FlowId = flowId;
            executeModel.GroupId = groupid.ToGuid();
            executeModel.InstanceId = instanceid;
            executeModel.IsSign = issign.ToInt(0);
            executeModel.Note = string.Empty;
            executeModel.OtherType = 0;
            executeModel.Sender = Current.User;
            executeModel.StepId = stepid.IsGuid(out Guid stepId) ? stepId : flowRunModel.FirstStepId;
            executeModel.TaskId = taskid.ToGuid();
            executeModel.ParamsJSON = paramsJSON;
            executeModel.Attachment = attachment;

            List<(Guid, string, Guid?, int?, List<Model.User>, DateTime?)> nextSteps = new List<(Guid, string, Guid?, int?, List<Model.User>, DateTime?)>();
            var currentStepModel = flowRunModel.Steps.Find(p => p.Id == executeModel.StepId);
            if (null == currentStepModel)
            {
                ViewData["alertMsg"] = "未找到当前步骤!";
                ViewData["success"] = "0";
                return View();
            }
            JArray stepsArray = paramsObject.Value<JArray>("steps");
            if (null != stepsArray)
            {
                Business.Organize organize = new Business.Organize();
                Business.FlowEntrust flowEntrust = new Business.FlowEntrust();
                Business.User user = new Business.User();
                foreach (JObject stepObject in stepsArray)
                {
                    string id = stepObject.Value<string>("id");
                    string name = stepObject.Value<string>("name");//步骤名称
                    string beforeStepId = stepObject.Value<string>("beforestepid");//原步骤ID(动态步骤的原步骤ID)
                    string parallelorserial = stepObject.Value<string>("parallelorserial");//0并且 1串行
                    string member = stepObject.Value<string>("member");
                    string completedTime = stepObject.Value<string>("completedtime");
                    if (!id.IsGuid(out Guid nextStepId))
                    {
                        continue;
                    }
                    DateTime? completedDateTime = new DateTime?();
                    if (completedTime.IsDateTime(out DateTime cTime))
                    {
                        completedDateTime = cTime;
                    }
                    if (executeModel.ExecuteType == Model.FlowRunModel.Execute.Type.Submit ||
                        executeModel.ExecuteType == Model.FlowRunModel.Execute.Type.Back ||
                        executeModel.ExecuteType == Model.FlowRunModel.Execute.Type.Redirect ||
                        executeModel.ExecuteType == Model.FlowRunModel.Execute.Type.FreeSubmit)
                    {
                        var userList = organize.GetAllUsers(member);
                        #region 判断委托
                        for (int i = 0; i < userList.Count; i++)
                        {
                            var userModel = userList[i].Clone();
                            userModel.Note = "";
                            string entrustId = flowEntrust.GetEntrustUserId(flowId, userModel);
                            if (!entrustId.IsNullOrWhiteSpace())
                            {
                                var entrustUser = user.Get(entrustId);
                                if (null != entrustUser)
                                {
                                    userModel = entrustUser.Clone();
                                    userModel.Note = userList[i].Id.ToString();//用这个字段来保存委托人ID,在加入任务表的时候要用到
                                }
                            }
                            userList[i] = userModel;
                        }
                        #endregion

                        nextSteps.Add((nextStepId, name,
                            beforeStepId.IsGuid(out Guid beforeStepGuid1) ? new Guid?(beforeStepGuid1) : new Guid?(),
                            parallelorserial.IsInt(out int parallelorserialInt1) ? new int?(parallelorserialInt1) : new int?(),
                            userList, completedDateTime));
                    }
                }
            }
            executeModel.Steps = nextSteps;
            #endregion

            #region 保存业务数据
            //判断抄送完成和终止任务不保存数据，如果是系统判断，在点发送时已经保存过了，这里不保存
            if ((executeModel.ExecuteType != Model.FlowRunModel.Execute.Type.CopyforCompleted
                && executeModel.ExecuteType != Model.FlowRunModel.Execute.Type.TaskEnd
                && currentStepModel.StepBase.FlowType != 0) 
                || executeModel.ExecuteType == Model.FlowRunModel.Execute.Type.Save)
            {
                try
                {
                    var (newInstanceId, errMsg) = new Business.Form().SaveData(Request);
                    if (executeModel.InstanceId.IsNullOrWhiteSpace())
                    {
                        executeModel.InstanceId = newInstanceId;
                    }
                }
                catch(Exception err)
                {
                    Business.Log.Add(err, "处理流程（"+ flowRunModel.Name + "）保存数据时发生了错误");
                    ViewData["debutMsg"] = flowRunModel.Debug == 1 ? "执行参数：<br/>" + paramsJSON + "<br/><br/>参数实体：<br/>" + executeModel.ToString() + "<br/><br/>执行结果：<br/>" + err.Message + "<br/>" + err.StackTrace : "";
                    ViewData["alertMsg"] = "操作发生了错误，请联系管理员！";
                    ViewData["success"] ="0";
                    return View();
                }
            }
            #endregion

            #region 设置任务标题
            string taskTitle = string.Empty;
            if (isCustomeForm)
            {
                taskTitle = Request.Forms("customformtitle");
            }
            else
            {
                string form_dbtabletitleexpression = Request.Forms("form_dbtabletitleexpression");//标题表达式
                if (form_dbtabletitleexpression.IsNullOrWhiteSpace())
                {
                    string form_dbtabletitle = Request.Forms("form_dbtabletitle");
                    taskTitle = Request.Forms(Request.Forms("form_dbtable") + "-" + form_dbtabletitle);
                }
                else
                {
                    string tableName = Request.Forms("Form_DBTable");
                    taskTitle = new Business.Form().ReplaceTitleExpression(form_dbtabletitleexpression, tableName, executeModel.InstanceId, Request);
                }
            }
            executeModel.Title = taskTitle;
            #endregion

            //添加动态流程步骤JSON(不是第一步时，第一步在流程提交时添加)
            if (Config.EnableDynamicStep 
                && executeModel.Steps.Exists(p => p.beforeStepId.IsNotEmptyGuid()) 
                && executeModel.TaskId.IsNotEmptyGuid() 
                && executeModel.GroupId.IsNotEmptyGuid())
            {
                var newFlowRunModel = new Business.FlowDynamic().Add(executeModel, groupTasks);
                if (null != newFlowRunModel)
                {
                    flowRunModel = newFlowRunModel;
                }
                executeModel.Steps.RemoveAll(p => p.parallelOrSerial.HasValue && p.parallelOrSerial == 1 && p.stepId != p.beforeStepId);
            }
            //===================

            //执行流程
            var executeResult = flowTask.Execute(executeModel, flowRunModel);

            #region 提交执行后要自动提交的任务(一般是指子流程完成要自动提交主流程步骤的情况)
            if (executeResult != null && executeResult.AutoSubmitTasks.Any())
            {
                foreach (var autoSubmitTask in executeResult.AutoSubmitTasks)
                {
                    var autoSubmitResult = flowTask.AutoSubmit(autoSubmitTask.Id);
                    Business.Log.Add("流程完成后自动提交了任务 - " + (autoSubmitResult.IsSuccess ? "成功" : "失败") + " - " + autoSubmitTask.FlowName
                        + " - " + autoSubmitTask.StepName + " - " + autoSubmitTask.Title, autoSubmitTask.ToString(), Business.Log.Type.流程运行,
                        others: "执行结果：" + autoSubmitResult.ToString());
                }
            }
            #endregion

            #region 归档
            if (currentStepModel.Archives == 1
                && (executeModel.ExecuteType == Model.FlowRunModel.Execute.Type.Submit
                || executeModel.ExecuteType == Model.FlowRunModel.Execute.Type.FreeSubmit
                || executeModel.ExecuteType == Model.FlowRunModel.Execute.Type.Completed))
            {
                var dbs = flowRunModel.Databases;
                if (dbs.Count > 0)
                {
                    string comments = Request.Forms("form_commentlist_div_textarea");
                    var db = dbs.First();
                    string formDataJson = new Business.Form().GetFormData(db.ConnectionId.ToString(),
                        db.Table, db.PrimaryKey, executeModel.InstanceId, executeModel.StepId.ToString(), executeModel.FlowId.ToString(),
                        Request.Forms("form_dataformatjson"), out string fieldStatusJSON);
                    string executeUserId = string.Empty;
                    string executeUserName = string.Empty;
                    if (executeResult.CurrentTask != null)
                    {
                        executeUserId = executeResult.CurrentTask.ReceiveOrganizeId.HasValue
                            ? Business.Organize.PREFIX_RELATION + executeResult.CurrentTask.ReceiveOrganizeId.Value.ToString()
                            : Business.Organize.PREFIX_USER + executeResult.CurrentTask.ReceiveId.ToString();
                        executeUserName = executeResult.CurrentTask.ReceiveName;
                    }
                    else
                    {
                        executeUserId = Business.Organize.PREFIX_USER + executeModel.Sender.Id.ToString();
                        executeUserName = executeModel.Sender.Name;
                    }
                    string formHtml = Request.Forms("form_body_div_textarea");//取表单HTML
                    Model.FlowArchive flowArchiveModel = new Model.FlowArchive
                    {
                        Comments = comments.Trim(),
                        DataJson = formDataJson,
                        FlowId = flowRunModel.Id,
                        FlowName = flowRunModel.Name,
                        Id = Guid.NewGuid(),
                        InstanceId = executeModel.InstanceId,
                        StepId = currentStepModel.Id,
                        StepName = currentStepModel.Name,
                        TaskId = executeResult.CurrentTask == null ? executeModel.TaskId : executeResult.CurrentTask.Id,
                        Title = executeResult.CurrentTask == null ? executeModel.Title : executeResult.CurrentTask.Title,
                        UserId = executeUserId,
                        UserName = executeUserName,
                        WriteTime = DateExtensions.Now,
                        FormHtml = formHtml
                    };
                    new Business.FlowArchive().Add(flowArchiveModel);
                }
            }
            #endregion

            Business.Log.Add("处理了流程 " + flowRunModel.Name + "] - 步骤 " + flowTask.GetStepName(flowRunModel, executeModel.StepId) + " - 标题 " + executeModel.Title, paramsJSON, Business.Log.Type.流程运行, oldContents: "执行参数：" + executeModel.ToString(), others: "返回：" + executeResult.ToString());
            string url = string.Empty;
            //如果下一步接收者中有当前人员则直接打开
            var nextTask = executeResult.NextTasks?.Find(p => p.ReceiveId == executeModel.Sender.Id && p.Status.In(0, 1));
            if (nextTask != null)
            {
                url = string.Format(Url.Content("~/RoadFlowCore/FlowRun/{0}?flowid={1}&stepid={2}&taskid={3}&groupid={4}&instanceid={5}&appid={6}&tabid={7}&rf_appopenmodel={8}&ismobile={9}"),
                "Index", nextTask.FlowId, nextTask.StepId, nextTask.Id, nextTask.GroupId, nextTask.InstanceId,
                Request.Querys("appid"), Request.Querys("tabid"), Request.Querys("rf_appopenmodel"), Request.Querys("ismobile")
                );
            }
            ViewData["debutMsg"] = flowRunModel.Debug == 1 ? "执行参数：<br/>" + paramsJSON + "<br/><br/>参数实体：<br/>" + executeModel.ToString() + "<br/><br/>执行结果：<br/>" + executeResult.ToString() : "";
            ViewData["alertMsg"] = executeResult.Messages;
            ViewData["success"] = executeResult.IsSuccess ? "1" : "0";
            ViewData["ismobile"] = ismobile;
            ViewData["url"] = url;
            return View();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult SaveData()
        {
            var (newInstanceId, errMsg) = new Business.Form().SaveData(Request);
            //如果是第一步要保存一个待办事项(为了解决点了发送又取消的时候业务表会有一条多余的数据，有待办事项可以在待办事项中删除)
            //if (Request.Querys("taskid").IsNullOrWhiteSpace())
            //{
            //    new Business.FlowTask().Execute(new Model.FlowRunModel.Execute() {
            //        FlowId = Request.Querys("flowid").ToGuid(),
            //        StepId = Request.Querys("stepid").ToGuid(),
            //        GroupId = Request.Querys("groupid").ToGuid(),
            //        TaskId = Guid.Empty,
            //        InstanceId = newInstanceId,
            //        ExecuteType = Model.FlowRunModel.Execute.Type.Save,
            //        Sender = Current.User
            //    });
            //}
            ViewData["instanceId"]= newInstanceId;
            ViewData["errMsg"] = errMsg;
            ViewData["opation"] = Request.Querys("opation");
            return View();
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult FlowSend()
        {
            string flowid = Request.Querys("flowid");
            string taskid = Request.Querys("taskid");
            string stepid = Request.Querys("stepid");
            string groupid = Request.Querys("groupid");
            string instanceid = Request.Querys("instanceid");
            string freedomsend = Request.Querys("freedomsend");
            string ismobile = Request.Querys("ismobile");
            if (instanceid.IsNullOrWhiteSpace())
            {
                instanceid = Request.Querys("instanceid1");
            }
            if (!flowid.IsGuid(out Guid flowId))
            {
                return new ContentResult() { Content = "流程ID错误!" };
            }
            if (!stepid.IsGuid(out Guid stepId))
            {
                return new ContentResult() { Content = "步骤ID错误!" };
            }
            Guid groupGuid = groupid.ToGuid();
            Guid taskGuid = taskid.ToGuid();
            var (dynamicTask, currentTask, groupTasks) = new Business.FlowTask().GetDynamicTask(groupGuid, null);
            var flowRunModel = new Business.Flow().GetFlowRunModel(flowId, true, dynamicTask);
            if (null == flowRunModel)
            {
                return new ContentResult() { Content = "未找到流程运行时!" };
            }
            var (html, message, sendSteps) = new Business.FlowTask().GetNextSteps(flowRunModel, stepId, groupGuid, taskGuid, instanceid, Current.UserId, "1".Equals(freedomsend), "1".Equals(ismobile), groupTasks);
            var stepModel = flowRunModel.Steps.Find(p => p.Id == stepId);
            ViewData["nextSteps"] = html;
            ViewData["openerId"] = Request.Querys("openerid");
            ViewData["freedomSend"] = freedomsend;
            ViewData["tabId"] = Request.Querys("tabid");
            ViewData["iframeId"] = Request.Querys("iframeid");
            ViewData["nextStepCount"] = sendSteps.Count;
            ViewData["autoConfirm"] = null != stepModel ? stepModel.StepBase.AutoConfirm : 0;//是否自动确认
            ViewData["isAddWrite"] = "0";
            ViewData["isMobile"] = Request.Querys("ismobile");
            return View();
        }

        /// <summary>
        /// 退回
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult FlowBack()
        {
            string flowid = Request.Querys("flowid");
            string taskid = Request.Querys("taskid");
            string stepid = Request.Querys("stepid");
            string groupid = Request.Querys("groupid");
            string instanceid = Request.Querys("instanceid");
            if (instanceid.IsNullOrWhiteSpace())
            {
                instanceid = Request.Querys("instanceid1");
            }
            if (!flowid.IsGuid(out Guid flowId))
            {
                return new ContentResult() { Content = "流程ID错误" };
            }
            if (!stepid.IsGuid(out Guid stepId))
            {
                return new ContentResult() { Content = "步骤ID错误" };
            }
            var (html, message, sendSteps) = new Business.FlowTask().GetBackSteps(flowId, stepId, groupid.ToGuid(), taskid.ToGuid(), instanceid, Current.UserId);
            ViewData["backSteps"] = html;
            ViewData["message"] = message;
            ViewData["tabId"] = Request.Querys("tabid");
            ViewData["iframeId"] = Request.Querys("iframeid");
            ViewData["openerId"] = Request.Querys("openerid");
            return View();
        }

        /// <summary>
        /// 转交
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult FlowRedirect()
        {
            ViewData["stepId"] = Request.Querys("stepid");
            ViewData["openerId"] = Request.Querys("openerid");
            return View();
        }

        /// <summary>
        /// 抄送
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult FlowCopyFor()
        {
            ViewData["taskId"] = Request.Querys("taskid");
            return View();
        }

        /// <summary>
        /// 保存抄送
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        [ValidateAntiForgeryToken]
        public string FlowCopyForSave()
        {
            string taskid = Request.Forms("taskid");
            string user = Request.Forms("user");
            var users = new Business.Organize().GetAllUsers(user);
            if (users.Count == 0)
            {
                return "没有接收人!";
            }
            Business.FlowTask flowTask = new Business.FlowTask();
            var taskModel = flowTask.Get(taskid.ToGuid());
            string msg = new Business.FlowTask().CopyFor(taskModel, users);
            return "1".Equals(msg) ? "抄送成功!" : msg;
        }

        /// <summary>
        /// 签章
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult Sign()
        {
            ViewData["openerId"] = Request.Querys("openerid");
            ViewData["queryString"] = Request.UrlQuery();
            ViewData["buttonId"] = Request.Querys("buttonid");
            return View();
        }

        /// <summary>
        /// 验证签章
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public string SignCheck()
        {
            string pass = Request.Forms("pass");
            if (pass.IsNullOrWhiteSpace())
            {
                return "密码不能为空!";
            }
            var user = Current.User;
            if (null == user)
            {
                return "未找到您的用户信息!";
            }
            if (!user.Password.Equals(new Business.User().GetMD5Password(user.Id, pass.Trim())))
            {
                return "密码错误!";
            }
            return "1";
        }

        /// <summary>
        /// 显示流程图
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult ShowDesign()
        {
            //判断如果有动态步骤，流程图要从rf_flowdynamic表中取
            string groupId = Request.Querys("groupid");
            string dynamicStepId = string.Empty;
            if (Config.EnableDynamicStep && groupId.IsGuid(out Guid groupGuid))
            {
                var groupTasks = new Business.FlowTask().GetListByGroupId(groupGuid);
                var dynamicTasks = groupTasks.Where(p => p.BeforeStepId.IsNotEmptyGuid()).OrderByDescending(p => p.Sort);
                if (dynamicTasks.Any())
                {
                    dynamicStepId = dynamicTasks.First().BeforeStepId.Value.ToString();
                }
            }
            ViewData["stepid"] = dynamicStepId;
            ViewData["groupid"] = dynamicStepId.IsNullOrWhiteSpace() ? "" : groupId;
            ViewData["flowid"] = Request.Querys("flowid");
            ViewData["tabid"] = Request.Querys("tabid");
            ViewData["appid"] = Request.Querys("appid");
            return View();
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckUrl = false,  CheckEnterPriseWeiXinLogin = true)]
        public IActionResult Print()
        {
            string flowid = Request.Querys("flowid");
            string stepid = Request.Querys("stepid");
            string taskid = Request.Querys("taskid");
            string groupid = Request.Querys("groupid");
            string instanceid = Request.Querys("instanceid");
            string appid = Request.Querys("appid");
            string tabid = Request.Querys("tabid");
            string showarchive = Request.Querys("showarchive");
            string ismobile = Request.Querys("ismobile");

            if (instanceid.IsNullOrWhiteSpace())
            {
                instanceid = Request.Querys("instanceid1");
            }

            if (!flowid.IsGuid(out Guid flowId))
            {
                return new ContentResult() { Content = "流程ID错误!" };
            }
            if (!taskid.IsGuid(out Guid taskId))
            {
                return new ContentResult() { Content = "未找到当前任务，请先保存再打印!" };
            }
            Business.Flow flow = new Business.Flow();
            var flowRunModel = flow.GetFlowRunModel(flowId);
            if (null == flowRunModel)
            {
                return new ContentResult() { Content = "未找到当前流程运行时实体!" };
            }
            if (!stepid.IsGuid(out Guid stepId))
            {
                return new ContentResult() { Content = "未找到当前步骤，请先保存再打印!" };
            }
            var stepModel = flowRunModel.Steps.Find(p => p.Id == stepId);
            if (null == stepModel)
            {
                return new ContentResult() { Content = "未找到当前步骤运行时实体!" };
            }

            string query = "flowid=" + flowid + "&taskid=" + taskid + "&groupid=" + groupid + "&stepid=" + stepId + "&instanceid="
                + instanceid + "&appid=" + appid + "&tabid=" + tabid + "&dilplay=1";

            Model.FlowRunModel.StepForm stepFormModel = stepModel.StepForm;
            string formUrl = string.Empty;
            bool isCustomeForm = false;//是否是自定义表单
            if (null != stepFormModel)
            {
                if (null != stepFormModel)
                {
                    Guid formId = stepFormModel.Id;
                    if ("1".Equals(ismobile) && stepFormModel.MobileId.IsNotEmptyGuid())//如果是移动端并且移动端表单不为空，则用移动端表单
                    {
                        formId = stepFormModel.MobileId;
                    }
                    var applibraryModel = new Business.AppLibrary().Get(formId);
                    if (null != applibraryModel)
                    {
                        isCustomeForm = applibraryModel.Code.IsNullOrWhiteSpace();
                        if (isCustomeForm)
                        {
                            formUrl = new Business.Menu().GetAddress(applibraryModel.Address, query);
                        }
                        else
                        {

                            formUrl = Url.Content("~/wwwroot/RoadFlowResources/scripts/formDesigner/form/" + applibraryModel.Address);
                        }
                    }
                }
            }
            string comments = string.Empty;//归档的意见
            string formHtml = string.Empty;//归档的HTML
            string formData = string.Empty;//归档的数据
            if ("1".Equals(showarchive))//如果是查看归档，要查询处理意见HTML
            {
                string archiveId = Request.Querys("archiveid");
                if (archiveId.IsGuid(out Guid aid))
                {
                    var archiveModel = new Business.FlowArchive().Get(aid);
                    if (null != archiveModel)
                    {
                        comments = archiveModel.Comments;
                        formHtml = archiveModel.FormHtml;
                        formData = archiveModel.DataJson;
                    }
                }
            }
            ViewData["comments"] = comments;
            ViewData["formUrl"] = formUrl;
            ViewData["isCustomeForm"] = isCustomeForm ? "1" : "0";
            ViewData["request"] = Request;
            ViewData["showarchive"] = showarchive;
            ViewData["formHtml"] = formHtml;
            ViewData["formData"] = formData.IsNullOrWhiteSpace() ? "[]" : formData;
            return View();
        }

        /// <summary>
        /// 查看主流程表单
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult ShowMainForm()
        {
            Business.FlowTask flowTask = new Business.FlowTask();
            string taskid = Request.Querys("taskid");
            var currentTask = flowTask.Get(taskid.ToGuid());
            if (null == currentTask)
            {
                return new ContentResult() { Content = "未找到当前任务!" };
            }
            var tasks = flowTask.GetListBySubFlowGroupId(currentTask.GroupId);
            if (tasks.Count == 0)
            {
                return new ContentResult() { Content = "未找到当前任务的主流程任务!" };
            }
            var task = tasks.First();
            string url = "Index?flowid=" + task.FlowId + "&stepid=" + task.StepId + "&instanceid=" + task.InstanceId
                + "&taskid=" + task.Id + "&groupid=" + task.GroupId + "&appid=" + Request.Querys("appid")
                + "&display=1&showtoolbar=0&tabid=" + Request.Querys("tabid") + "&ismobile=" + Request.Querys("ismobile");
            return Redirect(url);
        }

        /// <summary>
        /// 加签
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult AddWrite()
        {
            ViewData["queryString"] = Request.UrlQuery();
            ViewData["rf_appopenmodel"] = Request.Querys("rf_appopenmodel");
            ViewData["openerId"] = Request.Querys("openerid");
            return View();
        }

        /// <summary>
        /// 指定后续步骤处理人
        /// </summary>
        /// <returns></returns>
        public IActionResult SetNextStepHandler()
        {
            string stepId = Request.Querys("stepid");
            string flowId = Request.Querys("flowid");
            string taskId = Request.Querys("taskid");
            string groupId = Request.Querys("groupid");
            if(!flowId.IsGuid(out Guid flowGuid))
            {
                return new ContentResult() { Content = "流程Id错误!" };
            }
            if (!taskId.IsGuid())
            {
                return new ContentResult() { Content = "请先保存当前任务再指定!" };
            }
            Business.Flow flow = new Business.Flow();
            Business.FlowTask flowTask = new Business.FlowTask();
            var flowRunModel = flow.GetFlowRunModel(flowGuid);
            if(null == flowRunModel)
            {
                return new ContentResult() { Content = "未找到流程运行时!" };
            }
            if (!stepId.IsGuid(out Guid stepGuid))
            {
                stepGuid = flowRunModel.FirstStepId;
            }
            var nextSteps = flow.GetAllNextSteps(flowRunModel, stepGuid);
            ViewData["queryString"] = Request.UrlQuery();
            ViewData["rf_appopenmodel"] = Request.Querys("rf_appopenmodel");
            ViewData["openerId"] = Request.Querys("openerid");
            ViewData["nextSteps"] = nextSteps;
            ViewData["nextStepsHandle"] = flowTask.GetNextStepsHandle(flowTask.GetListByGroupId(groupId.ToGuid()));
            return View();
        }
        /// <summary>
        /// 保存指定后续步骤处理人
        /// </summary>
        /// <returns></returns>
        public string SaveSetNextStepHandler()
        {
            string steps = Request.Forms("steps");
            string flowId = Request.Querys("flowid");
            string taskId = Request.Querys("taskid");
            if (!taskId.IsGuid(out Guid taskGuid))
            {
                return "未找到当前任务,不能保存!";
            }
            Business.FlowTask flowTask = new Business.FlowTask();
            var taskModel = flowTask.Get(taskGuid);
            if (null == taskModel)
            {
                return "未找到当前任务,不能保存!";
            }
            JArray jArray = new JArray();
            foreach (string step in steps.Split(','))
            {
                string handle = Request.Forms("handler_" + step);
                JObject jObject = new JObject
                {
                    { "stepId", step },
                    { "handle", handle }
                };
                jArray.Add(jObject);
            }
            taskModel.NextStepsHandle = jArray.ToString(Newtonsoft.Json.Formatting.None);
            flowTask.Update(taskModel);
            return "保存成功!";
        }

        /// <summary>
        /// 发起流程页面
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        public IActionResult Starts()
        {
            var flows = new Business.Flow().GetStartFlows(Current.UserId);
            return View(flows);
        }

        /// <summary>
        /// 不通过流程，直接编辑表单
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true, CheckUrl = false)]
        public IActionResult FormEdit()
        {
            string applibraryid = Request.Querys("applibraryid");
            string instanceid = Request.Querys("instanceid");
            string appid = Request.Querys("appid");
            string tabid = Request.Querys("tabid");
            string display = Request.Querys("display");
            string returnUrl = Request.Querys("returnurl");
            string showtoolbar = Request.Querys("showtoolbar");//是否显示工具栏，查看表单的时候，有时不需要显示。
            string secondtable = Request.Querys("secondtable");//从表表名(弹出式子表用)
            string primarytableid = Request.Querys("primarytableid");//主表实例ID(弹出式子表用)
            string secondtablerelationfield = Request.Querys("secondtablerelationfield");//从表与主表关系字段(弹出式子表用)

            if (display.IsNullOrEmpty())
            {
                display = "0";
            }
            if (!applibraryid.IsGuid(out Guid applibraryId))
            {
                return new ContentResult() { Content = "应用程序库ID错误!" };
            }

            #region 得到表单地址
            bool isCustomeForm = false;//是否是自定义表单
            var appModel = new Business.AppLibrary().Get(applibraryId);
            string formUrl = string.Empty;
            if (null == appModel)
            {
                return new ContentResult() { Content = "未找到应用!" };
            }
            isCustomeForm = appModel.Code.IsNullOrWhiteSpace();
            if (isCustomeForm)
            {
                formUrl = appModel.Address;
            }
            else
            {
                formUrl = Url.Content("~/wwwroot/RoadFlowResources/scripts/formDesigner/form/" + appModel.Address);
            }
            #endregion

            string query = "applibraryid=" + applibraryid + "&instanceid=" + instanceid + "&appid=" + appid + "&programid=" + Request.Querys("programid")
                + "&tabid=" + tabid + "&dilplay=" + display + "&iframeid=" + Request.Querys("iframeid")
                + "&showtoolbar=" + showtoolbar + "&rf_appopenmodel=" + Request.Querys("rf_appopenmodel")
                + "&isrefresparent=" + Request.Querys("isrefresparent") + "&primarytableid=" + primarytableid 
                + "&secondtablerelationfield=" + secondtablerelationfield + "&secondtable="+ secondtable
                + "&returnurl=" + Request.Querys("returnurl").UrlEncode() + "&openerid=" + Request.Querys("openerid");

            #region 组织按钮
            List<Model.FlowButton> flowButtons = new List<Model.FlowButton>();
            if (!isCustomeForm)//如果是自定义表单这里不添加按钮，自己在表单中设置按钮
            {
                List<Model.FlowRunModel.StepButton> stepButtons = new List<Model.FlowRunModel.StepButton>();
                if (!"1".Equals(display))
                {
                    flowButtons.Add(new Model.FlowButton()
                    {
                        Id = Guid.NewGuid(),
                        Title = "确认保存",
                        Ico = "fa-save",
                        Script = "saveEditFormData(a);",
                        Sort = 8
                    });
                }
                flowButtons.Add(new Model.FlowButton()
                {
                    Id = Guid.NewGuid(),
                    Title = "关闭窗口",
                    Ico = "fa-close",
                    Script = "closeWindw(" + Request.Querys("rf_appopenmodel") + ");",
                    Sort = 9
                });
                if (!returnUrl.IsNullOrWhiteSpace())
                {
                    flowButtons.Add(new Model.FlowButton()
                    {
                        Id = Guid.NewGuid(),
                        Title = "返回",
                        Ico = "fa-mail-reply",
                        Script = "window.location='" + returnUrl + "';",
                        Sort = 0
                    });
                }
            }
            #endregion

            ViewData["tabId"] = tabid;
            ViewData["appId"] = appid;
            ViewData["instanceId"] = instanceid;
            ViewData["display"] = display;
            ViewData["formUrl"] = formUrl;
            ViewData["query"] = query;
            ViewData["isCustomeForm"] = isCustomeForm ? "1" : "0";//是否是自定义表单
            ViewData["flowButtons"] = flowButtons.OrderBy(p => p.Sort).ToList();
            ViewData["request"] = Request;
            ViewData["title"] = appModel.Title;
            ViewData["secondtablerelationfield"] = secondtablerelationfield;
            ViewData["primarytableid"] = primarytableid;
            ViewData["secondtable"] = secondtable;
            return View();
        }

        /// <summary>
        /// 保存不通过流程，直接编辑表单
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult FormEditSave()
        {
            var (newInstanceId, errMsg) = new Business.Form().SaveData(Request);
            if (errMsg.IsNullOrWhiteSpace() && !newInstanceId.IsNullOrWhiteSpace())
            {
                string url = "FormEdit?instanceid=" + newInstanceId + "&applibraryid=" + Request.Querys("applibraryid") + "&programid=" + Request.Querys("programid")
                    + "&appid=" + Request.Querys("appid").Split(',')[0] + "&tabid=" + Request.Querys("tabid") + "&dilplay="
                    + Request.Querys("display") + "&isrefresparent=" + Request.Querys("isrefresparent")
                    + "&showtoolbar=" + Request.Querys("showtoolbar") + "&primarytableid=" + Request.Querys("primarytableid")
                    + "&secondtablerelationfield=" + Request.Querys("secondtablerelationfield") + "&secondtable=" + Request.Querys("secondtable")
                    + "&iframeid=" + Request.Querys("iframeid") + "&openerid=" + Request.Querys("openerid")
                    + "&rf_appopenmodel=" + Request.Querys("rf_appopenmodel") + "&returnurl=" + Request.Querys("returnurl").UrlEncode();
                //return new ContentResult() { Content = "<script>alert('保存成功!');parent.location='" + url + "';</script>", ContentType = "text/html" };
                ViewData["issuccess"] = "1";
                ViewData["msg"] = "保存成功!";
                ViewData["url"] = url;
            }
            else
            {
                ViewData["issuccess"] = "0";
                ViewData["msg"] = errMsg;
                ViewData["url"] = "";
                //return new ContentResult() { Content = "<script>alert('" + errMsg + "!')</script>", ContentType = "text/html" };
            }
            ViewData["isrefreshflow"] = !Request.Querys("secondtablerelationfield").IsNullOrWhiteSpace() 
                && !Request.Querys("secondtable").IsNullOrWhiteSpace() ? "1" : "0";//是否要刷新流程中的主表单
            ViewData["rf_appopenmodel"] = Request.Querys("rf_appopenmodel");
            ViewData["isrefresparent"] = Request.Querys("isrefresparent");
            return View();
        }

        /// <summary>
        /// 删除表单数据
        /// </summary>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public string FormDelete()
        {
            string connId = Request.Querys("connId");
            string secondtable = Request.Querys("secondtable");
            string secondtablePrimaryKey = Request.Querys("secondtablePrimaryKey");
            string secondtableId = Request.Querys("secondtableId");
            if (!connId.IsGuid(out Guid connid) || secondtable.IsNullOrWhiteSpace() || secondtablePrimaryKey.IsNullOrWhiteSpace() || secondtableId.IsNullOrWhiteSpace())
            {
                return "连接ID、表名、表主键、主键值不能为空!";
            }
            Business.DbConnection dbConnection = new Business.DbConnection();
            string msg = dbConnection.ExecuteSQL(connid, "DELETE FROM " + secondtable + " WHERE " + secondtablePrimaryKey + "={0}", secondtableId);
            Business.Log.Add("删除了表单数据-" + secondtable, "连接ID：" + connId + ", 主键值：" + secondtablePrimaryKey, Business.Log.Type.流程运行);
            return msg.IsInt() ? "删除成功!" : msg;
        }
    }
}