using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using RoadFlow.Utility;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Localization;

namespace RoadFlow.Business
{
    public class Flow
    {
        private readonly Data.Flow flowData;

        public Flow()
        {
            flowData = new Data.Flow();
        }
        /// <summary>
        /// 得到所有流程
        /// </summary>
        /// <returns></returns>
        public List<Model.Flow> GetAll()
        {
            return flowData.GetAll();
        }
        /// <summary>
        /// 查询一个流程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Flow Get(Guid id)
        {
            return flowData.Get(id);
        }
        /// <summary>
        /// 添加一个流程
        /// </summary>
        /// <param name="flow">流程实体</param>
        /// <returns></returns>
        public int Add(Model.Flow flow)
        {
            return flowData.Add(flow);
        }
        /// <summary>
        /// 更新流程
        /// </summary>
        /// <param name="flow">流程实体</param>
        public int Update(Model.Flow flow)
        {
            return flowData.Update(flow);
        }
        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="flow">流程实体</param>
        /// <returns></returns>
        public int Delete(Model.Flow flow)
        {
            return flowData.Delete(flow);
        }
        /// <summary>
        /// 得到所有流程下拉选项
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetOptions(string value = "")
        {
            return GetFlowOptions(GetAll(), value);
        }

        /// <summary>
        /// 得到可管理实例的流程下拉选项
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetManageInstanceOptions(Guid userId, string value = "")
        {
            return GetFlowOptions(GetManageInstanceFlow(userId), value);
        }

        private string GetFlowOptions(List<Model.Flow> flows, string value = "")
        {
            StringBuilder stringBuilder = new StringBuilder();
            Dictionary dictionary = new Dictionary();
            Guid dictRootID = dictionary.GetIdByCode("system_applibrarytype_flow");
            foreach (var flow in flows)
            {
                stringBuilder.Append("<option value=\"" + flow.Id + "\"");
                if (flow.Id.ToString().EqualsIgnoreCase(value))
                {
                    stringBuilder.Append(" selected=\"selected\"");
                }
                stringBuilder.Append(">" + flow.Name + " (" + dictionary.GetAllParentTitle(flow.FlowType, true, false, dictRootID.ToString()) + ")");
                stringBuilder.Append("</option>");
            }
            return stringBuilder.ToString();
        }


        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="flowIdList"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="order"></param>
        /// <param name="status">状态-1表示查询未删除的流程</param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerList(out int count, int size, int number, List<Guid> flowIdList, string name, string type, string order, int status = -1)
        {
            return flowData.GetPagerList(out count, size, number, flowIdList, name, type, order, status);
        }

        /// <summary>
        /// 得到状态显示
        /// </summary>
        /// <param name="status"></param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        public string GetStatusTitle(int status, IStringLocalizer localizer = null)
        {
            string title = string.Empty;
            switch (status)
            {
                case 0:
                    title = localizer == null ? "设计中" : localizer["State_Designing"];
                    break;
                case 1:
                    title = localizer == null ? "已安装" : localizer["State_Installed"];
                    break;
                case 2:
                    title = localizer == null ? "已卸载" : localizer["State_Uninstalled"];
                    break;
                case 3:
                    title = localizer == null ? "已删除" : localizer["State_Deleted"];
                    break;
            }
            return title;
        }

        /// <summary>
        /// 保存流程
        /// </summary>
        /// <param name="json"></param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        public string Save(string json, IStringLocalizer localizer = null)
        {
            JObject jObject = null;
            try
            {
                jObject = JObject.Parse(json);
            }
            catch
            {
                return localizer == null ? "JSON解析错误" : localizer["Save_JsonParseError"];
            }
            if (null == jObject)
            {
                return localizer == null ? "JSON解析错误" : localizer["Save_JsonParseError"];
            }
            string flowId = jObject.Value<string>("id");
            string name = jObject.Value<string>("name");
            string type = jObject.Value<string>("type");
            string systemId = jObject.Value<string>("systemId");
            if (!flowId.IsGuid(out Guid fid))
            {
                return localizer == null ? "流程ID错误" : localizer["Save_FlowIdError"];
            }
            if (name.IsNullOrWhiteSpace())
            {
                return localizer == null ? "流程名称为空" : localizer["Save_FlowNameEmpty"];
            }
            if (!type.IsGuid(out Guid typeId))
            {
                return localizer == null ? "流程分类错误" : localizer["Save_FlowTypeError"];
            }
            Flow flow = new Flow();
            Model.Flow flowModel = flow.Get(fid);
            bool isAdd = false;
            if (null == flowModel)
            {
                isAdd = true;
                flowModel = new Model.Flow
                {
                    Id = fid,
                    CreateDate = DateExtensions.Now,
                    CreateUser = User.CurrentUserId,
                    Status = 0
                };
            }
            flowModel.DesignerJSON = json;
            flowModel.InstanceManager = jObject.Value<string>("instanceManager");
            flowModel.Manager = jObject.Value<string>("manager");
            flowModel.Name = name;
            flowModel.FlowType = typeId;
            flowModel.Note = jObject.GetValue("note").ToString();
            flowModel.SystemId = systemId.IsGuid(out Guid guid) ? guid : new Guid?();
            if (isAdd)
            {
                flow.Add(flowModel);
            }
            else
            {
                flow.Update(flowModel);
            }
            Log.Add((localizer == null ? "保存了流程" : localizer["Save_FlowLog"]) + "-" + flowModel.Name, json, Log.Type.流程管理);
            return "1";
        }

        /// <summary>
        /// 安装流程
        /// </summary>
        /// <param name="json"></param>
        /// <param name="logTitle">日志标题</param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        public string Install(string json, IStringLocalizer localizer = null)
        {
            string saveMsg = Save(json, localizer);
            if (!"1".Equals(saveMsg))
            {
                return saveMsg;
            }
            var flowRunModel = GetFlowRunModel(json, out string errMsg, localizer: localizer);
            if (null == flowRunModel)
            {
                return errMsg;
            }
            var flowModel = Get(flowRunModel.Id);
            if (null == flowModel)
            {
                return localizer == null ? "未找到流程实体" : localizer["Install_NoFindFlowModel"];
            }
            flowModel.InstallDate = DateExtensions.Now;
            flowModel.InstallUser = User.CurrentUserId;
            flowModel.RunJSON = json;
            flowModel.Status = 1;
            flowData.Install(flowModel);
            //清除运行时缓存
            ClearCache(flowModel.Id);
            Log.Add((localizer == null ? "安装了流程" : localizer["Install_Log"]) + "-" + flowModel.Name, json, Log.Type.流程管理, others: errMsg);
            return "1";
        }

        /// <summary>
        /// 流程运行时实体缓存键
        /// </summary>
        public const string CACHEKEY = "roadflow_cache_flowrun_";

        /// <summary>
        /// 得到流程运行时实体
        /// </summary>
        /// <param name="id">流程ID</param>
        /// <param name="isCache">是否从缓存中取</param>
        /// <param name="currentTask">当前任务实体(动态步骤时要取动态的步骤流程运行时实体)</param>
        /// <returns></returns>
        public Model.FlowRun GetFlowRunModel(Guid id, bool isCache = true, Model.FlowTask currentTask = null)
        {
            //如果当前任务有之前步骤ID，则从动态流程中取JSON
            if (null != currentTask && currentTask.BeforeStepId.IsNotEmptyGuid())
            {
                string key = CACHEKEY + "_" + currentTask.BeforeStepId.Value.ToNString() + "_" + currentTask.GroupId.ToNString();
                if (isCache)
                {
                    object obj = Cache.IO.Get(key);
                    if(null != obj)
                    {
                        return (Model.FlowRun)obj;
                    }
                }
                var flowDynamicModel = new FlowDynamic().Get(currentTask.BeforeStepId.Value, currentTask.GroupId);
                var runModel = null == flowDynamicModel || flowDynamicModel.FlowJSON.IsNullOrWhiteSpace() ? null
                    : GetFlowRunModel(flowDynamicModel.FlowJSON, out string msg1);
                if(null != runModel)
                {
                    Cache.IO.Insert(key, runModel);
                }
                return runModel;
            }
            //=========================================

            string cacheKey = CACHEKEY + id.ToString("N");
            if (isCache)
            {
                object obj = Cache.IO.Get(cacheKey);
                if (null != obj)
                {
                    return (Model.FlowRun)obj;
                }
            }
            var flowModel = Get(id);
            if (null == flowModel)
            {
                return null;
            }
            var flowRunModel = GetFlowRunModel(flowModel.RunJSON.IsNullOrWhiteSpace() ? flowModel.DesignerJSON : flowModel.RunJSON, out string msg);
            if (null != flowRunModel)
            {
                Cache.IO.Insert(cacheKey, flowRunModel);
            }
            return flowRunModel;
        }

        /// <summary>
        /// 清除流程运行时缓存
        /// </summary>
        /// <param name="id"></param>
        public void ClearCache(Guid id)
        {
            Cache.IO.Remove(CACHEKEY + id.ToString("N"));
        }

        /// <summary>
        /// 得到流程运行时实体
        /// </summary>
        /// <param name="json">流程设置JSON</param>
        /// <param name="errMsg">加载错误时的错误信息</param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>

        public Model.FlowRun GetFlowRunModel(string json, out string errMsg, IStringLocalizer localizer = null)
        {
            errMsg = string.Empty;
            JObject jObject = null;
            try
            {
                jObject = JObject.Parse(json);
            }
            catch
            {
                errMsg = localizer == null ? "JSON解析错误" : localizer["Save_JsonParseError"];
                return null;
            }
            if (null == jObject)
            {
                errMsg = localizer == null ? "JSON解析错误" : localizer["Save_JsonParseError"];
                return null;
            }
            string id = jObject.Value<string>("id");
            string name = jObject.Value<string>("name");
            string type = jObject.Value<string>("type");
            string systemId = jObject.Value<string>("systemId");
            string mananger = jObject.Value<string>("manager");
            string instanceManager = jObject.Value<string>("instanceManager");
            if (!id.IsGuid(out Guid flowId))
            {
                errMsg = localizer == null ? "流程ID错误" : localizer["Save_FlowIdError"];
                return null;
            }
            if (name.IsNullOrWhiteSpace())
            {
                errMsg = localizer == null ? "流程名称为空" : localizer["Save_FlowNameEmpty"];
                return null;
            }
            if (!type.IsGuid(out Guid typeId))
            {
                errMsg = localizer == null ? "流程分类错误" : localizer["Save_FlowTypeError"];
                return null;
            }
            if (mananger.IsNullOrWhiteSpace())
            {
                errMsg = localizer == null ? "流程管理者为空" : localizer["Install_ManagerEmpty"];
                return null;
            }
            if (instanceManager.IsNullOrWhiteSpace())
            {
                errMsg = localizer == null ? "流程实例管理者为空" : localizer["Install_InstanceManagerEmpty"];
                return null;
            }
            var flowModel = Get(flowId);
            if (null == flowModel)
            {
                errMsg = localizer == null ? "未找到该流程" : localizer["Install_NoFindFlowModel"];
                return null;
            }
            var flowRunModel = new Model.FlowRun
            {
                Id = flowId,
                Name = name,
                Type = typeId,
                Manager = mananger,
                InstanceManager = instanceManager,
                FirstStepId = Guid.Empty,
                Note = jObject.Value<string>("note"),
                SystemId = systemId.IsGuid(out Guid guid) ? guid : new Guid?(),
                Debug = jObject.Value<string>("debug").IsInt(out int debug) ? debug : 0,
                DebugUserIds = new Organize().GetAllUsersId(jObject.Value<string>("debugUsers")),
                Status = flowModel.Status,
                CreateDate = flowModel.CreateDate,
                CreateUserId = flowModel.CreateUser,
                InstallDate = flowModel.InstallDate,
                InstallUserId = flowModel.InstallUser,
                Ico = jObject.Value<string>("ico"),
                Color = jObject.Value<string>("color")
            };

            #region 流程数据库连接信息
            JArray dbsArray = jObject.Value<JArray>("databases");
            List<Model.FlowRunModel.Database> databases = new List<Model.FlowRunModel.Database>();
            if (null != dbsArray)
            {
                foreach (JObject dbs in dbsArray)
                {
                    Model.FlowRunModel.Database database = new Model.FlowRunModel.Database
                    {
                        ConnectionId = dbs.Value<string>("link").IsGuid(out Guid cid) ? cid : Guid.Empty,
                        ConnectionName = dbs.Value<string>("linkName"),
                        Table = dbs.Value<string>("table"),
                        PrimaryKey = dbs.Value<string>("primaryKey")
                    };
                    databases.Add(database);
                }
            }
            flowRunModel.Databases = databases;
            #endregion

            #region 流程标识字段信息
            JObject titleFieldObject = jObject.Value<JObject>("titleField");
            Model.FlowRunModel.TitleField titleField = new Model.FlowRunModel.TitleField();
            if (null != titleFieldObject)
            {
                titleField.ConnectionId = titleFieldObject.Value<string>("link").IsGuid(out Guid cid) ? cid : Guid.Empty;
                titleField.Table = titleFieldObject.Value<string>("table");
                titleField.Field = titleFieldObject.Value<string>("field");
                titleField.Value = titleFieldObject.Value<string>("value");
            }
            flowRunModel.TitleField = titleField;
            #endregion

            #region 步骤基本信息
            JArray stepArray = jObject.Value<JArray>("steps");
            List<Model.FlowRunModel.Step> steps = new List<Model.FlowRunModel.Step>();
            if (null != stepArray)
            {
                foreach (JObject stepObject in stepArray)
                {
                    Model.FlowRunModel.Step stepModel = new Model.FlowRunModel.Step();
                    #region 坐标信息
                    JObject positionObject = stepObject.Value<JObject>("position");
                    if (null != positionObject)
                    {
                        stepModel.Position_X = positionObject.Value<string>("x").IsDecimal(out decimal x) ? x : 0;
                        stepModel.Position_Y = positionObject.Value<string>("y").IsDecimal(out decimal y) ? y : 0;
                    }
                    #endregion

                    #region 步骤信息
                    stepModel.Archives = stepObject.Value<string>("archives").IsInt(out int archives) ? archives : 0;
                    stepModel.ExpiredPrompt = stepObject.Value<string>("expiredPrompt").IsInt(out int expiredPrompt) ? expiredPrompt : 0;
                    stepModel.ExpiredPromptDays = stepObject.Value<string>("expiredPromptDays").IsDecimal(out decimal expiredPromptDays) ? expiredPromptDays : 0;
                    stepModel.Id = stepObject.Value<string>("id").IsGuid(out Guid sid) ? sid : Guid.Empty;
                    stepModel.Type = stepObject.Value<string>("type").EqualsIgnoreCase("normal") ? 0 : 1;
                    stepModel.Name = stepObject.Value<string>("name");
                    stepModel.Dynamic = stepObject.Value<string>("dynamic").ToInt(0);
                    stepModel.DynamicField = stepObject.Value<string>("dynamicField");
                    stepModel.Note = stepObject.Value<string>("note");
                    stepModel.CommentDisplay = stepObject.Value<string>("opinionDisplay").IsInt(out int opinionDisplay) ? opinionDisplay : 0;
                    stepModel.SignatureType = stepObject.Value<string>("signatureType").IsInt(out int signatureType) ? signatureType : 0;
                    stepModel.WorkTime = stepObject.Value<string>("workTime").IsDecimal(out decimal workTime) ? workTime : 0;
                    stepModel.SendShowMessage = stepObject.Value<string>("sendShowMsg");
                    stepModel.BackShowMessage = stepObject.Value<string>("backShowMsg");
                    stepModel.SendSetWorkTime = stepObject.Value<string>("sendSetWorkTime").IsInt(out int sendSetWorkTime) ? sendSetWorkTime : 0;
                    stepModel.ExpiredExecuteModel = stepObject.Value<string>("timeOutModel").IsInt(out int timeOutModel) ? timeOutModel : 0;
                    stepModel.DataEditModel = stepObject.Value<string>("dataEditModel").IsInt(out int dataEditModel) ? dataEditModel : 0;
                    stepModel.Attachment = stepObject.Value<string>("attachment").ToInt(0);

                    #region 基本信息
                    JObject baseObject = stepObject.Value<JObject>("behavior");
                    Model.FlowRunModel.StepBase stepBaseModel = new Model.FlowRunModel.StepBase();
                    if (null != baseObject)
                    {
                        stepBaseModel.BackModel = baseObject.Value<string>("backModel").IsInt(out int backModel) ? backModel : 0;
                        if (baseObject.Value<string>("backStep").IsGuid(out Guid backStepId))
                        {
                            stepBaseModel.BackStepId = backStepId;
                        }
                        stepBaseModel.BackType = baseObject.Value<string>("backType").IsInt(out int backType) ? backType : 0;
                        stepBaseModel.BackSelectUser = baseObject.Value<string>("backSelectUser").IsInt(out int backSelectUser) ? backSelectUser : 0;
                        stepBaseModel.DefaultHandler = baseObject.Value<string>("defaultHandler");
                        stepBaseModel.FlowType = baseObject.Value<string>("flowType").IsInt(out int flowType) ? flowType : 0;
                        if (baseObject.Value<string>("handlerStep").IsGuid(out Guid handlerStepId))
                        {
                            stepBaseModel.HandlerStepId = handlerStepId;
                        }
                        stepBaseModel.HandlerType = baseObject.Value<string>("handlerType");
                        stepBaseModel.HanlderModel = baseObject.Value<string>("hanlderModel").IsInt(out int hanlderModel) ? hanlderModel : 0;
                        stepBaseModel.Percentage = baseObject.Value<string>("percentage").IsDecimal(out decimal percentage) ? percentage : 0;
                        stepBaseModel.RunSelect = baseObject.Value<string>("runSelect").IsInt(out int runSelect) ? runSelect : 0;
                        stepBaseModel.SelectRange = baseObject.Value<string>("selectRange");
                        stepBaseModel.ValueField = baseObject.Value<string>("valueField");
                        stepBaseModel.Countersignature = baseObject.Value<string>("countersignature").IsInt(out int countersignature) ? countersignature : 0;
                        stepBaseModel.CountersignatureStartStepId = baseObject.Value<string>("countersignatureStartStep").IsGuid(out Guid countersignatureStartStepId) ? new Guid?(countersignatureStartStepId) : new Guid?();
                        stepBaseModel.CountersignaturePercentage = baseObject.Value<string>("countersignaturePercentage").IsDecimal(out decimal countersignaturePercentage) ? countersignaturePercentage : 0;
                        stepBaseModel.SubFlowStrategy = baseObject.Value<string>("subflowstrategy").IsInt(out int subflowstrategy) ? subflowstrategy : 0;
                        stepBaseModel.ConcurrentModel = baseObject.Value<string>("concurrentModel").IsInt(out int concurrentModel) ? concurrentModel : 0;
                        stepBaseModel.DefaultHandlerSqlOrMethod = baseObject.Value<string>("defaultHandlerSqlOrMethod");
                        stepBaseModel.AutoConfirm = baseObject.Value<string>("autoConfirm").ToInt(0);
                        stepBaseModel.SkipIdenticalUser = baseObject.Value<string>("skipIdenticalUser").ToInt(0);
                        stepBaseModel.SkipMethod = baseObject.Value<string>("skipMethod");
                        stepBaseModel.SendToBackStep = baseObject.Value<string>("sendToBackStep").ToInt(0);
                    }
                    stepModel.StepBase = stepBaseModel;
                    #endregion

                    #region 抄送
                    Model.FlowRunModel.StepCopyFor stepCopyForModel = new Model.FlowRunModel.StepCopyFor();
                    JObject copyForObject = stepObject.Value<JObject>("copyFor");
                    if (null != copyForObject)
                    {
                        stepCopyForModel.MemberId = copyForObject.Value<string>("memberId");
                        stepCopyForModel.HandlerType = copyForObject.Value<string>("handlerType");
                        stepCopyForModel.Steps = copyForObject.Value<string>("steps");
                        stepCopyForModel.MethodOrSql = copyForObject.Value<string>("methodOrSql");
                        stepCopyForModel.CopyforTime = copyForObject.Value<string>("time").ToInt(0);
                    }
                    stepModel.StepCopyFor = stepCopyForModel;
                    #endregion

                    #region 按钮信息
                    List<Model.FlowRunModel.StepButton> stepButtons = new List<Model.FlowRunModel.StepButton>();
                    JArray buttonArray = stepObject.Value<JArray>("buttons");
                    if (null != buttonArray)
                    {
                        foreach (JObject buttonObject in buttonArray)
                        {
                            Model.FlowRunModel.StepButton stepButtonModel = new Model.FlowRunModel.StepButton();
                            if (buttonObject.Value<string>("id").IsGuid(out Guid bid))
                            {
                                var flowButtonModel = new FlowButton().Get(bid);
                                stepButtonModel.Id = bid;
                                stepButtonModel.Note = "";
                                string showTitle = buttonObject.Value<string>("showTitle");
                                stepButtonModel.ShowTitle = showTitle;
                                stepButtonModel.Sort = buttonObject.Value<int>("sort");
                                if (null != flowButtonModel)
                                {
                                    stepButtonModel.Note = flowButtonModel.Note;
                                    stepButtonModel.ShowTitle = showTitle.IsNullOrWhiteSpace() ? flowButtonModel.Title : showTitle;
                                }
                            }
                            stepButtons.Add(stepButtonModel);
                        }
                    }
                    stepModel.StepButtons = stepButtons;
                    #endregion

                    #region 事件
                    JObject eventObject = stepObject.Value<JObject>("event");
                    Model.FlowRunModel.StepEvent stepEventModel = new Model.FlowRunModel.StepEvent();
                    if (null != eventObject)
                    {
                        stepEventModel.BackAfter = eventObject.Value<string>("backAfter");
                        stepEventModel.BackBefore = eventObject.Value<string>("backBefore");
                        stepEventModel.SubmitAfter = eventObject.Value<string>("submitAfter");
                        stepEventModel.SubmitBefore = eventObject.Value<string>("submitBefore");
                        stepEventModel.SubFlowActivationBefore = eventObject.Value<string>("subflowActivationBefore");
                        stepEventModel.SubFlowCompletedBefore = eventObject.Value<string>("subflowCompletedBefore");
                    }
                    stepModel.StepEvent = stepEventModel;
                    #endregion

                    #region 表单
                    JArray formArray = stepObject.Value<JArray>("forms");
                    Model.FlowRunModel.StepForm stepFormModel = new Model.FlowRunModel.StepForm();
                    if (null != formArray && formArray.Count > 0)
                    {
                        JObject formObject = (JObject)formArray.First;
                        if (formObject.Value<string>("id").IsGuid(out Guid formId))
                        {
                            stepFormModel.Id = formId;
                        }
                        stepFormModel.Name = formObject.Value<string>("name");
                        if (formObject.Value<string>("idApp").IsGuid(out Guid appId))
                        {
                            stepFormModel.MobileId = appId;
                        }
                        stepFormModel.MobileName = formObject.Value<string>("nameApp");
                    }
                    stepModel.StepForm = stepFormModel;
                    #endregion

                    #region 字段状态
                    JArray fieldArray = stepObject.Value<JArray>("fieldStatus");
                    List<Model.FlowRunModel.StepFieldStatus> stepFieldStatuses = new List<Model.FlowRunModel.StepFieldStatus>();
                    if (null != fieldArray)
                    {
                        foreach (JObject fieldObject in fieldArray)
                        {
                            Model.FlowRunModel.StepFieldStatus stepFieldStatusModel = new Model.FlowRunModel.StepFieldStatus
                            {
                                Check = fieldObject.Value<string>("check").IsInt(out int check) ? check : 0,
                                Field = fieldObject.Value<string>("field"),
                                Status = fieldObject.Value<string>("status").IsInt(out int status) ? status : 0
                            };
                            stepFieldStatuses.Add(stepFieldStatusModel);
                        }
                    }
                    stepModel.StepFieldStatuses = stepFieldStatuses;
                    #endregion

                    #region 子流程
                    JObject subflowObject = stepObject.Value<JObject>("subflow");
                    Model.FlowRunModel.StepSubFlow stepSubFlowModel = new Model.FlowRunModel.StepSubFlow();
                    if (null != subflowObject)
                    {
                        if (subflowObject.Value<string>("flowId").IsGuid(out Guid subId))
                        {
                            stepSubFlowModel.SubFlowId = subId;
                        }
                        stepSubFlowModel.SubFlowStrategy = subflowObject.Value<string>("flowStrategy").IsInt(out int flowStrategy) ? flowStrategy : 0;
                        stepSubFlowModel.TaskType = subflowObject.Value<string>("taskType").IsInt(out int taskType) ? taskType : 0;
                    }
                    stepModel.StepSubFlow = stepSubFlowModel;
                    #endregion

                    steps.Add(stepModel);
                    #endregion
                }
            }
            flowRunModel.Steps = steps;
            if (steps.Count == 0)
            {
                errMsg = localizer == null ? "流程至少需要一个步骤" : localizer["Install_OneStep"];
                return null;
            }
            #endregion

            #region 连线信息
            JArray lineArray = jObject.Value<JArray>("lines");
            List<Model.FlowRunModel.Line> lines = new List<Model.FlowRunModel.Line>();
            if (null != lineArray)
            {
                foreach (JObject lineObject in lineArray)
                {
                    Model.FlowRunModel.Line lineModel = new Model.FlowRunModel.Line
                    {
                        Id = lineObject.Value<string>("id").IsGuid(out Guid lid) ? lid : Guid.Empty,
                        FromId = lineObject.Value<string>("from").IsGuid(out Guid fid) ? fid : Guid.Empty,
                        ToId = lineObject.Value<string>("to").IsGuid(out Guid tid) ? tid : Guid.Empty,
                        CustomMethod = lineObject.Value<string>("customMethod"),
                        SqlWhere = lineObject.Value<string>("sql")
                    };
                    if (lineObject.Value<JArray>("organize") != null)
                    {
                        lineModel.OrganizeExpression = lineObject.Value<JArray>("organize").ToString(Newtonsoft.Json.Formatting.None);
                    }
                    lines.Add(lineModel);
                }
            }
            flowRunModel.Lines = lines;
            #endregion

            #region 设置开始步骤
            Model.FlowRunModel.Step firstStep = null;
            foreach (var step in flowRunModel.Steps)
            {
                if (flowRunModel.Lines.Find(p => p.ToId == step.Id) == null)
                {
                    firstStep = step;
                    break;
                }
            }
            if (null == firstStep)
            {
                errMsg = localizer == null ? "流程没有开始步骤" : localizer["Install_NotStartStep"];
                return null;
            }
            flowRunModel.FirstStepId = firstStep.Id;
            #endregion

            return flowRunModel;
        }

        /// <summary>
        /// 得到一个流程步骤的下一步骤集合
        /// </summary>
        /// <param name="flowRunModel">流程运行时实体</param>
        /// <param name="stepId">步骤ID</param>
        /// <returns></returns>
        public List<Model.FlowRunModel.Step> GetNextSteps(Model.FlowRun flowRunModel, Guid stepId)
        {
            List<Model.FlowRunModel.Step> steps = new List<Model.FlowRunModel.Step>();
            if (null == flowRunModel)
            {
                return steps;
            }
            var lines = flowRunModel.Lines.FindAll(p => p.FromId == stepId);
            foreach (var line in lines)
            {
                var step = flowRunModel.Steps.Find(p => p.Id == line.ToId);
                if (null != step)
                {
                    steps.Add(step);
                }
            }
            return steps.OrderBy(p => p.Position_Y).ThenBy(p => p.Position_X).ToList();
        }

        /// <summary>
        /// 得到一个步骤的所有后续步骤集合
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public List<Model.FlowRunModel.Step> GetAllNextSteps(Model.FlowRun flowRunModel, Guid stepId)
        {
            List<Model.FlowRunModel.Step> steps = new List<Model.FlowRunModel.Step>();
            AddNextSteps(flowRunModel, stepId, steps);
            return steps;
        }
        /// <summary>
        /// 递归添加一个步骤的后续步骤
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="stepId"></param>
        /// <param name="steps"></param>
        private void AddNextSteps(Model.FlowRun flowRunModel, Guid stepId, List<Model.FlowRunModel.Step> steps)
        {
            var nexts = GetNextSteps(flowRunModel, stepId);
            foreach (var step in nexts)
            {
                if (!steps.Exists(p => p.Id == step.Id))
                {
                    steps.Add(step);
                }
            }
            foreach (var step in nexts)
            {
                AddNextSteps(flowRunModel, step.Id, steps);
            }
        }

        /// <summary>
        /// 得到一个步骤的前面步骤集合
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public List<Model.FlowRunModel.Step> GetPrevSteps(Model.FlowRun flowRunModel, Guid stepId)
        {
            List<Model.FlowRunModel.Step> steps = new List<Model.FlowRunModel.Step>();
            if (null == flowRunModel || stepId.IsEmptyGuid())
            {
                return steps;
            }
            var lines = flowRunModel.Lines.FindAll(p => p.ToId == stepId);
            foreach (var line in lines)
            {
                var step = flowRunModel.Steps.Find(p => p.Id == line.FromId);
                if (null != step)
                {
                    steps.Add(step);
                }
            }
            return steps;
        }

        /// <summary>
        /// 得到两个步骤之间的步骤集合
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="stepId1">开始步骤Id</param>
        /// <param name="stepId2">结束步骤Id</param>
        /// <returns></returns>
        public List<Model.FlowRunModel.Step> GetRangeSteps(Model.FlowRun flowRunModel, Guid stepId1, Guid stepId2)
        {
            if (stepId1.IsEmptyGuid())
            {
                return new List<Model.FlowRunModel.Step>();
            }
            var nextSteps = GetNextSteps(flowRunModel, stepId1);
            foreach (var nextStep in nextSteps)
            {
                var allNextSteps = GetAllNextSteps(flowRunModel, nextStep.Id);
                if (allNextSteps.Exists(p => p.Id == stepId2))
                {
                    List<Model.FlowRunModel.Step> steps = new List<Model.FlowRunModel.Step>() { nextStep };
                    foreach (var step in allNextSteps)
                    {
                        steps.Add(step);
                        if (step.Id == stepId2)
                        {
                            return steps;
                        }
                    }
                }
            }
            return new List<Model.FlowRunModel.Step>();
        }
       

        /// <summary>
        /// 得到一个流程步骤的下一步骤
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <param name="stepId">步骤ID</param>
        /// <returns></returns>
        public List<Model.FlowRunModel.Step> GetNextSteps(Guid flowId, Guid stepId)
        {
            return GetNextSteps(GetFlowRunModel(flowId), stepId);
        }

        /// <summary>
        /// 根据ID得到流程名称
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public string GetName(Guid flowId)
        {
            if (flowId.IsEmptyGuid())
            {
                return string.Empty;
            }
            var flow = Get(flowId);
            return null == flow ? string.Empty : flow.Name;
        }

        /// <summary>
        /// 得到步骤名称
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public string GetStepName(Guid flowId, Guid stepId)
        {
            return flowId.IsEmptyGuid() ? string.Empty : GetStepName(GetFlowRunModel(flowId), stepId);
        }

        /// <summary>
        /// 得到步骤名称
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public string GetStepName(Model.FlowRun flowRunModel, Guid stepId)
        {
            if (stepId.IsEmptyGuid())
            {
                return string.Empty;
            }
            if (null == flowRunModel)
            {
                return string.Empty;
            }
            var step = flowRunModel.Steps.Find(p => p.Id == stepId);
            return null == step ? string.Empty : step.Name;
        }

        /// <summary>
        /// 得到一个用户可管理的流程
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Model.Flow> GetManageFlow(Guid userId)
        {
            var all = GetAll();
            if (all.Count == 0)
            {
                return new List<Model.Flow>();
            }
            return all.FindAll(p => p.Manager.ContainsIgnoreCase(userId.ToString()));
        }

        /// <summary>
        /// 得到一个用户可管理的流程
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Guid> GetManageFlowIds(Guid userId)
        {
            var flows = GetManageFlow(userId);
            List<Guid> guids = new List<Guid>();
            foreach (var flow in flows)
            {
                guids.Add(flow.Id);
            }
            return guids;
        }

        /// <summary>
        /// 得到一个用户可管理实例的流程
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Model.Flow> GetManageInstanceFlow(Guid userId)
        {
            var all = GetAll();
            if (all.Count == 0)
            {
                return new List<Model.Flow>();
            }
            return all.FindAll(p => p.InstanceManager.ContainsIgnoreCase(userId.ToString()));
        }

        /// <summary>
        /// 得到一个用户可管理实例的流程
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Guid> GetManageInstanceFlowIds(Guid userId)
        {
            var flows = GetManageInstanceFlow(userId);
            List<Guid> guids = new List<Guid>();
            foreach (var flow in flows)
            {
                guids.Add(flow.Id);
            }
            return guids;
        }

        /// <summary>
        /// 得到一个用户可以发起的流程运行时实体列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Model.FlowRun> GetStartFlows(Guid userId)
        {
            List<Model.FlowRun> flowRuns = new List<Model.FlowRun>();
            var flows = GetAll();
            User user = new User();
            foreach(var flow in flows)
            {
                var flowRunModel = GetFlowRunModel(flow.Id);
                if (null == flowRunModel || flowRunModel.Status != 1 || flowRunModel.FirstStepId.IsEmptyGuid())
                {
                    continue;
                }
                var firstStepModel = flowRunModel.Steps.Find(p => p.Id == flowRunModel.FirstStepId);
                if (null == firstStepModel)
                {
                    continue;
                }
                if (firstStepModel.StepBase.DefaultHandler.IsNullOrWhiteSpace() || user.Contains(firstStepModel.StepBase.DefaultHandler, userId))
                {
                    flowRuns.Add(flowRunModel);
                }
            }
            return flowRuns;
        }

        /// <summary>
        /// 得到导出时的流程JSON字符串
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public string GetExportFlowString(string ids)
        {
            if(ids.IsNullOrWhiteSpace())
            {
                return "";
            }
            string[] flowIds = ids.Split(',');
            JObject jObject = new JObject();
            JArray jArrayFlow = new JArray();
            JArray jArrayForm = new JArray();
            JArray jArrayApplibrary = new JArray();
            List<Guid> formIdList = new List<Guid>();//用来保存已添加的表单，避免重复
            List<Guid> applibraryIdList = new List<Guid>();//用来保存已添加的应用程序库，避免重复
            foreach (string flowId in flowIds)
            {
                if (!flowId.IsGuid(out Guid fid))
                {
                    continue;
                }
                var flowModel = Get(fid);
                if (null == flowModel)
                {
                    continue;
                }
                jArrayFlow.Add(JObject.FromObject(flowModel));
                //添加表单和应用程序库
                var flowRunModel = GetFlowRunModel(fid);
                if (flowModel.RunJSON.IsNullOrWhiteSpace() || flowModel.DesignerJSON.IsNullOrWhiteSpace())
                {
                    continue;
                }
                JObject flowObject = null;
                try
                {
                    flowObject = JObject.Parse(flowModel.RunJSON.IsNullOrWhiteSpace() ? flowModel.DesignerJSON : flowModel.RunJSON);
                }
                catch
                {

                }
                if (flowObject == null)
                {
                    continue;
                }
                JArray stepArray = flowObject.Value<JArray>("steps");
                if (stepArray == null)
                {
                    continue;
                }
                AppLibrary appLibrary = new AppLibrary();
                Form form = new Form();
                foreach (JObject stepObject in stepArray)
                {
                    JArray formArray = stepObject.Value<JArray>("forms");
                    JObject formObject = (JObject)formArray.First;
                    //PC表单
                    if (formObject != null && formObject.Value<string>("id").IsGuid(out Guid formId))
                    {
                        var appModel = appLibrary.Get(formId);
                        if (appModel != null)
                        {
                            if (!applibraryIdList.Contains(appModel.Id))
                            {
                                jArrayApplibrary.Add(JObject.FromObject(appModel));
                                applibraryIdList.Add(appModel.Id);
                            }
                            if (!appModel.Code.IsNullOrWhiteSpace() && appModel.Code.IsGuid(out Guid formGuidId))
                            {
                                var formModel = form.Get(formGuidId);
                                if (formModel != null)
                                {
                                    if (!formIdList.Contains(formModel.Id))
                                    {
                                        jArrayForm.Add(JObject.FromObject(formModel));
                                        formIdList.Add(formModel.Id);
                                    }
                                }
                            }
                        }
                    }
                    //移动端表单
                    if (formObject != null && formObject.Value<string>("idApp").IsGuid(out Guid appId))
                    {
                        var appModel = appLibrary.Get(appId);
                        if (appModel != null)
                        {
                            if (!applibraryIdList.Contains(appModel.Id))
                            {
                                jArrayApplibrary.Add(JObject.FromObject(appModel));
                                applibraryIdList.Add(appModel.Id);
                            }
                            if (!appModel.Code.IsNullOrWhiteSpace() && appModel.Code.IsGuid(out Guid formGuidId))
                            {
                                var formModel = form.Get(formGuidId);
                                if (formModel != null)
                                {
                                    if (!formIdList.Contains(formModel.Id))
                                    {
                                        jArrayForm.Add(JObject.FromObject(formModel));
                                        formIdList.Add(formModel.Id);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            jObject.Add("flows", jArrayFlow);
            jObject.Add("forms", jArrayForm);
            jObject.Add("applibrarys", jArrayApplibrary);
            return jObject.ToString();
        }

        /// <summary>
        /// 导入流程
        /// </summary>
        /// <param name="json"></param>
        /// <returns>返回1表示成功，其它为错误信息</returns>
        public string ImportFlow(string json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return "要导入的JSON为空!";
            }
            JObject jObject = null;
            try
            {
                jObject = JObject.Parse(json);
            }
            catch
            {
                return "json解析错误!";
            }
            var flows = jObject.Value<JArray>("flows");
            if (null != flows)
            {
                foreach (JObject flow in flows)
                {
                    Model.Flow flowModel = flow.ToObject<Model.Flow>();
                    if (null == flowModel)
                    {
                        continue;
                    }
                    if (Get(flowModel.Id) != null)
                    {
                        Update(flowModel);
                    }
                    else
                    {
                        Add(flowModel);
                    }
                }
            }
            var applibarys = jObject.Value<JArray>("applibrarys");
            AppLibrary appLibrary = new AppLibrary();
            if (null != applibarys)
            {
                foreach (JObject app in applibarys)
                {
                    Model.AppLibrary appLibraryModel = app.ToObject<Model.AppLibrary>();
                    if (null == appLibraryModel)
                    {
                        continue;
                    }
                    if (appLibrary.Get(appLibraryModel.Id) != null)
                    {
                        appLibrary.Update(appLibraryModel);
                    }
                    else
                    {
                        appLibrary.Add(appLibraryModel);
                    }
                }
            }
            var forms = jObject.Value<JArray>("forms");
            Form bform = new Form();
            if (null != forms)
            {
                foreach (JObject form in forms)
                {
                    Model.Form formModel = form.ToObject<Model.Form>();
                    if (null == formModel)
                    {
                        continue;
                    }
                    if (bform.Get(formModel.Id) != null)
                    {
                        bform.Update(formModel);
                    }
                    else
                    {
                        bform.Add(formModel);
                    }
                    //如果表单状态是发布，要发布表单
                    if (formModel.Status == 1)
                    {
                        #region 写入文件
                        string webRootPath = Tools.GetWebRootPath();
                        string path = webRootPath + "/RoadFlowResources/scripts/formDesigner/form/";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string file = path + formModel.Id + ".rfhtml";
                        Stream stream = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                        stream.SetLength(0);
                        StreamWriter sw = new StreamWriter(stream, Encoding.UTF8);
                        sw.Write(formModel.RunHtml);
                        sw.Close();
                        stream.Close();
                        #endregion
                    }
                }
            }
            return "1";
        }

        /// <summary>
        /// 流程另存为
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <param name="newFlowName">新的流程名称</param>
        /// <returns>返回guid字符串表示成功(新流程的ID)，其它为错误信息</returns>
        /// <param name="localizer">语言包</param>
        public string SaveAs(Guid flowId, string newFlowName, IStringLocalizer localizer = null)
        {
            var flowModel = Get(flowId);
            if (null == flowModel)
            {
                return localizer == null ? "未找到要另存的流程!" : localizer["SaveAsSave_NotFindFlow"];
            }
            flowModel.Id = Guid.NewGuid();
            flowModel.CreateDate = DateExtensions.Now;
            if (flowModel.InstallDate.HasValue)
            {
                flowModel.InstallDate = flowModel.CreateDate;
            }
            flowModel.Name = newFlowName;
            JObject jObject = null;
            if (!flowModel.RunJSON.IsNullOrWhiteSpace())
            {
                jObject = JObject.Parse(flowModel.RunJSON);
            }
            else if (!flowModel.DesignerJSON.IsNullOrWhiteSpace())
            {
                jObject = JObject.Parse(flowModel.DesignerJSON);
            }
            if (jObject != null)
            {
                jObject["id"] = flowModel.Id.ToString();
                jObject["name"] = flowModel.Name;
                JArray lines = jObject.Value<JArray>("lines");
                foreach (JObject stepJObject in jObject.Value<JArray>("steps"))
                {
                    string stepNewId = Guid.NewGuid().ToString();
                    string stepOldId = stepJObject.Value<string>("id");
                    foreach (JObject lineJObject in lines)
                    {
                        if (lineJObject.Value<string>("from").EqualsIgnoreCase(stepOldId))
                        {
                            lineJObject["from"] = stepNewId;
                        }
                        if (lineJObject.Value<string>("to").EqualsIgnoreCase(stepOldId))
                        {
                            lineJObject["to"] = stepNewId;
                        }
                    }
                    stepJObject["id"] = stepNewId;
                }
                foreach (JObject lineJObject1 in lines)
                {
                    lineJObject1["id"] = Guid.NewGuid().ToString();
                }
            }
            string json = jObject.ToString(Newtonsoft.Json.Formatting.None);
            if (!flowModel.RunJSON.IsNullOrWhiteSpace())
            {
                flowModel.RunJSON = json;
            }
            if (!flowModel.DesignerJSON.IsNullOrWhiteSpace())
            {
                flowModel.DesignerJSON = json;
            }
            Add(flowModel);
            return flowModel.Id.ToString();
        }

        /// <summary>
        /// 清除所有流程缓存
        /// </summary>
        public void ClearCache()
        {
            flowData.ClearCache();
        }

    }
}
