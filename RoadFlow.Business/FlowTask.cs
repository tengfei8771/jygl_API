using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;
using System.Linq;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using System.Threading;

namespace RoadFlow.Business
{
    public class FlowTask
    {
        private readonly Data.FlowTask flowTaskData;
        public FlowTask()
        {
            flowTaskData = new Data.FlowTask();
        }
        /// <summary>
        /// 查询一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.FlowTask Get(Guid id)
        {
            return flowTaskData.Get(id);
        }
        /// <summary>
        /// 根据子流程组ID查询主流程任务
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<Model.FlowTask> GetListBySubFlowGroupId(Guid groupId)
        {
            return flowTaskData.GetListBySubFlowGroupId(groupId);
        }
        /// <summary>
        /// 查询待办事项
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable GetWaitTask(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count)
        {
            return flowTaskData.GetWaitList(size, number, userId, flowId, title, startDate, endDate, order, out count);
        }

        /// <summary>
        /// 查询已办事项
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable GetCompletedTask(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count)
        {
            return flowTaskData.GetCompletedList(size, number, userId, flowId, title, startDate, endDate, order, out count);
        }

        /// <summary>
        /// 查询我发起的流程
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable GetMyStartList(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count)
        {
            return flowTaskData.GetMyStartList(size, number, userId, flowId, title, startDate, endDate, order, out count);
        }

        /// <summary>
        /// 查询已委托事项
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable GetEntrustTask(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count)
        {
            return flowTaskData.GetEntrustList(size, number, userId, flowId, title, startDate, endDate, order, out count);
        }

        /// <summary>
        /// 查询实例列表
        /// </summary>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="receiveId"></param>
        /// <param name="receiveDate1"></param>
        /// <param name="receiveDate2"></param>
        /// <param name="order"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DataTable GetInstanceList(int size, int number, string flowId, string title, string receiveId, string receiveDate1, string receiveDate2, string order, out int count)
        {
            return flowTaskData.GetInstanceList(size, number, flowId, title, receiveId, receiveDate1, receiveDate2, order, out count);
        }
        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="flowTask">任务实体</param>
        /// <returns></returns>
        public int Add(Model.FlowTask flowTask)
        {
            return flowTaskData.Add(flowTask);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">任务实体</param>
        /// <returns></returns>
        public int Update(Model.FlowTask flowTaskModel)
        {
            return flowTaskData.Update(flowTaskModel);
        }
        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="removeTasks">要删除的任务列表</param>
        /// <param name="updateTasks">要更新的任务列表</param>
        /// <param name="addTasks">要添加的任务列表</param>
        /// <param name="executeSqls">要执行的sql列表(sql,参数,0提交退回前 1提交退回后)</param>
        /// <returns></returns>
        public int Update(List<Model.FlowTask> removeTasks, List<Model.FlowTask> updateTasks, List<Model.FlowTask> addTasks, List<(string, object[], int)> executeSqls)
        {
            return flowTaskData.Update(removeTasks, updateTasks, addTasks, executeSqls);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="flowTaskModels">任务实体</param>
        /// <returns></returns>
        public int DeleteByGroupId(Model.FlowTask[] flowTaskModels)
        {
            return flowTaskData.DeleteByGroupId(flowTaskModels.First().GroupId);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="groupId">组ID</param>
        /// <returns></returns>
        public int DeleteByGroupId(Guid groupId)
        {
            return flowTaskData.DeleteByGroupId(groupId);
        }
        /// <summary>
        /// 删除一个流程的实例
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public int DeleteByFlowId(Guid flowId)
        {
            return flowTaskData.DeleteByFlowId(flowId);
        }
        /// <summary>
        /// 得到执行状态选项
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetExecuteTypeOptions(int value = -100)
        {
            //处理类型 处理类型 -1等待中 0未处理 1处理中 2已完成 3已退回 4他人已处理 5他人已退回 6已转交 7已委托 8已阅知 9已指派 10已跳转 11已终止 12他人已终止
            Dictionary<int, string> dicts = new Dictionary<int, string>() {
                { -1, "等待中"},
                { 0, "未处理"},
                { 1, "处理中"},
                { 2, "已完成"},
                { 3, "已退回"},
                { 4, "他人已处理"},
                { 5, "他人已退回"},
                { 6, "已转交"},
                { 7, "已委托"},
                { 8, "已阅知"},
                { 9, "已指派"},
                { 10, "已跳转"},
                { 11, "已终止"},
                { 12, "他人已终止"}
            };
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var dict in dicts)
            {
                stringBuilder.Append("<option value=\"" + dict.Key.ToString() + "\"");
                if (dict.Key == value)
                {
                    stringBuilder.Append(" selected=\"selected\"");
                }
                stringBuilder.Append(">" + dict.Value + "</option>");
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 得到执行状态选项(多语言)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetExecuteTypeOptions(IStringLocalizer localizer, int value = -100)
        {
            //处理类型 处理类型 -1等待中 0未处理 1处理中 2已完成 3已退回 4他人已处理 5他人已退回 6已转交 7已委托 8已阅知 9已指派 10已跳转 11已终止 12他人已终止
            Dictionary<int, string> dicts = new Dictionary<int, string>() {
                { -1, localizer["State_Wait"]},
                { 0, localizer["State_Untreated"]},
                { 1, localizer["State_Processing"]},
                { 2, localizer["State_Completed"]},
                { 3, localizer["State_Back"]},
                { 4, localizer["State_OtherProcess"]},
                { 5, localizer["State_OtherBack"]},
                { 6, localizer["State_Transferred"]},
                { 7, localizer["State_Entrusted"]},
                { 8, localizer["State_Knowing"]},
                { 9, localizer["State_Assigned"]},
                { 10, localizer["State_Skip"]},
                { 11, localizer["State_Terminated"]},
                { 12, localizer["State_OtherTerminated"]}
            };
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var dict in dicts)
            {
                stringBuilder.Append("<option value=\"" + dict.Key.ToString() + "\"");
                if (dict.Key == value)
                {
                    stringBuilder.Append(" selected=\"selected\"");
                }
                stringBuilder.Append(">" + dict.Value + "</option>");
            }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// 根据组ID查询列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<Model.FlowTask> GetListByGroupId(Guid groupId)
        {
            return flowTaskData.GetListByGroupId(groupId).OrderBy(p => p.Sort).ThenBy(p => p.StepSort).ThenBy(p => p.ReceiveTime).ThenBy(p => p.StepName).ThenBy(p => p.ReceiveName).ToList();
        }
        /// <summary>
        /// 根据组ID查询最新的一条
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Model.FlowTask GetMaxByGroupId(Guid groupId)
        {
            return flowTaskData.GetMaxByGroupId(groupId);
        }
        /// <summary>
        /// 得到流程第一步发起者ID
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>组织架构字符串 u_人员ID,r_组织架构与人员关系ID</returns>
        public Guid GetFirstSenderId(Guid groupId)
        {
            return GetFirstSenderId(GetListByGroupId(groupId));
        }
        /// <summary>
        /// 得到流程第一步发起者ID
        /// </summary>
        /// <param name="groupTasks"></param>
        /// <returns>组织架构字符串 u_人员ID,r_组织架构与人员关系ID</returns>
        public Guid GetFirstSenderId(List<Model.FlowTask> groupTasks)
        {
            if (groupTasks.Count == 0)
            {
                return Guid.Empty;
            }
            var task = groupTasks.Find(p => p.PrevId == Guid.Empty);
            return task == null ? Guid.Empty : task.SenderId;
        }
        /// <summary>
        /// 得到流程前一步处理者
        /// </summary>
        /// <param name="groupTasks"></param>
        /// <param name="taskId">当前任务ID</param>
        /// <returns></returns>
        public string GetPrevStepHandler(List<Model.FlowTask> groupTasks, Guid taskId)
        {
            var currentTask = groupTasks.Find(p => p.Id == taskId);
            if (null == currentTask)
            {
                return string.Empty;
            }
            var prevTask = groupTasks.Find(p => p.Id == currentTask.PrevId);
            if (null == prevTask)
            {
                return string.Empty;
            }
            var prevTasks = groupTasks.FindAll(p => p.PrevId == prevTask.PrevId);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var prev in prevTasks)
            {
                if (prev.TaskType == 5)//抄送不算
                {
                    continue;
                }
                if (prev.ReceiveOrganizeId.HasValue)
                {
                    stringBuilder.Append(Organize.PREFIX_RELATION + prev.ReceiveOrganizeId.Value.ToString());
                }
                else
                {
                    stringBuilder.Append(Organize.PREFIX_USER + prev.ReceiveId.ToString());
                }
                stringBuilder.Append(",");
            }
            return stringBuilder.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 得到某一个步骤的处理者
        /// </summary>
        /// <param name="groupTasks"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public string GetStepHandler(List<Model.FlowTask> groupTasks, Guid stepId)
        {
            var tasks = groupTasks.FindAll(p => p.StepId == stepId && p.TaskType != 5);
            if (!tasks.Any())
            {
                return string.Empty;
            }
            var maxSort = tasks.Max(p => p.Sort);
            var tasks1 = tasks.FindAll(p => p.Sort == maxSort);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var task in tasks1)
            {
                if (task.ReceiveOrganizeId.IsNotEmptyGuid())
                {
                    stringBuilder.Append(Organize.PREFIX_RELATION + task.ReceiveOrganizeId.Value.ToString());
                }
                else
                {
                    stringBuilder.Append(Organize.PREFIX_USER + task.ReceiveId.ToString());
                }
                stringBuilder.Append(",");
            }
            return stringBuilder.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 得到流程接收人为字段值时的字段值
        /// </summary>
        /// <param name="database">流程连接</param>
        /// <param name="fieldString">选择的字段</param>
        /// <param name="instanceId">主键值</param>
        /// <returns></returns>
        public string GetFieldValue(Model.FlowRunModel.Database database, string fieldString, string instanceId)
        {
            if (!fieldString.IsNullOrWhiteSpace() && !instanceId.IsNullOrWhiteSpace() && database != null)
            {
                string[] dbs = fieldString.Split('.');
                string linkId = string.Empty, tableName = string.Empty, field = string.Empty;
                if (dbs.Length > 0)
                {
                    linkId = dbs[0];
                }
                if (dbs.Length > 1)
                {
                    tableName = dbs[1];
                }
                if (dbs.Length > 2)
                {
                    field = dbs[2];
                }
                string primaryKey = database.PrimaryKey;
                if (linkId.IsGuid(out Guid linkGuid) && !tableName.IsNullOrWhiteSpace()
                    && !field.IsNullOrWhiteSpace() && !primaryKey.IsNullOrWhiteSpace())
                {
                    return new DbConnection().GetFieldValue(linkGuid, tableName, field, primaryKey, instanceId);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 得到一个步骤名称
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public string GetStepName(Model.FlowRun flowRunModel, Guid stepId)
        {
            if (null == flowRunModel)
            {
                return string.Empty;
            }
            var step = flowRunModel.Steps.Find(p => p.Id == stepId);
            return null == step ? string.Empty : step.Name;
        }

        /// <summary>
        /// 得到一个实例经过的步骤
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="groupTasks">组任务集合</param>
        /// <returns></returns>
        public List<Model.FlowRunModel.Step> GetTaskInstanceSteps(Model.FlowRun flowRunModel, List<Model.FlowTask> groupTasks)
        {
            var steps = groupTasks.OrderBy(p => p.Sort).GroupBy(p => p.StepId);
            List<Model.FlowRunModel.Step> groupSteps = new List<Model.FlowRunModel.Step>();
            foreach (var step in steps)
            {
                var stepModel = flowRunModel.Steps.Find(p => p.Id == step.Key);
                if (null != stepModel)
                {
                    groupSteps.Add(stepModel);
                }
            }
            return groupSteps;
        }

        /// <summary>
        /// 得到步骤由流程处理人设置的后续步骤处理人员
        /// </summary>
        /// <param name="groupTasks">实例组列表</param>
        /// <returns></returns>
        public Dictionary<Guid, string> GetNextStepsHandle(List<Model.FlowTask> groupTasks)
        {
            Dictionary<Guid, string> dict = new Dictionary<Guid, string>();
            foreach (Model.FlowTask flowTask in groupTasks.OrderBy(p => p.Sort))
            {
                if (flowTask.NextStepsHandle.IsNullOrWhiteSpace())
                {
                    continue;
                }
                try
                {
                    JArray jArray = JArray.Parse(flowTask.NextStepsHandle);
                    foreach (JObject jObject in jArray)
                    {
                        string handle = jObject.Value<string>("handle");
                        string stepId = jObject.Value<string>("stepId");
                        if (!stepId.IsGuid(out Guid stepGuid) || handle.IsNullOrWhiteSpace())
                        {
                            continue;
                        }
                        if (dict.ContainsKey(stepGuid))
                        {
                            dict[stepGuid] = handle;
                        }
                        else
                        {
                            dict.Add(stepGuid, handle);
                        }
                    }
                }
                catch { }
            }
            return dict;
        }


        /// <summary>
        /// 得到当前任务的后续接收步骤选择HTML
        /// </summary>
        /// <param name="flowRunModel">流程运行时实体</param>
        /// <param name="stepId">步骤ID</param>
        /// <param name="groupId">组ID</param>
        /// <param name="taskId">任务ID</param>
        /// <param name="instanceId">实例ID</param>
        /// <param name="userId">当前人员ID</param>
        /// <param name="sendSteps">可以发送的步骤</param>
        /// <param name="isFreeSend">是否是自由发送</param>
        /// <param name="isMobile">是否是移动端</param>
        /// <param name="groupTasks">组任务集合,为空则在方法中查询</param>
        /// <param name="localizer">多语言包</param>
        /// <returns>步骤选择html, 提醒信息, 可以发送的步骤集合</returns>
        public (string html, string message, List<Model.FlowRunModel.Step> sendSteps) GetNextSteps(Model.FlowRun flowRunModel, Guid stepId, Guid groupId, Guid taskId, string instanceId, Guid userId, bool isFreeSend, bool isMobile = false, List<Model.FlowTask> groupTasks = null, IStringLocalizer localizer = null)
        {
            groupTasks = groupTasks ?? GetListByGroupId(groupId);
            var currentTask = groupTasks.Find(p => p.Id == taskId);
            List<Model.FlowRunModel.Step> sendSteps = new List<Model.FlowRunModel.Step>();
            Flow flow = new Flow();
            var stepModel = flowRunModel.Steps.Find(p => p.Id == stepId);
            if (null == stepModel)
            {
                return ("", localizer == null ? "未找到步骤运行时!" : localizer["NotFoundFlowRunModel"], sendSteps);
            }

            List<(Model.FlowRunModel.Step, string)> nextStpes = new List<(Model.FlowRunModel.Step, string)>();
            bool isAddWrite = null != currentTask && currentTask.TaskType == 6;//是否是前加签

            //如果是发送到退回步骤
            bool isSendToBackStep = null != currentTask && currentTask.TaskType == 4 && 1 == stepModel.StepBase.SendToBackStep;
            if (isSendToBackStep)
            {
                string stepMembers = GetStepHandler(groupTasks, currentTask.PrevStepId);
                var backStep = flowRunModel.Steps.Find(p => p.Id == currentTask.PrevStepId);
                if (null != backStep)
                {
                    nextStpes.Add((backStep, stepMembers));
                }
            }
            else
            {
                //如果是按选择人员顺序处理
                bool isStepSort = false;
                int hanlderModel = stepModel.StepBase.HanlderModel;
                if (hanlderModel == 4)
                {
                    if (null != currentTask)
                    {
                        var task = groupTasks.Find(p => p.StepId == stepModel.Id && p.Sort == currentTask.Sort && p.StepSort == currentTask.StepSort + 1);
                        if (task != null)
                        {
                            nextStpes.Add((stepModel, task.ReceiveOrganizeId.HasValue
                                ? Organize.PREFIX_RELATION + task.ReceiveOrganizeId.Value.ToString()
                                : Organize.PREFIX_USER + task.ReceiveId.ToString()));
                            isStepSort = true;
                        }
                    }
                }

                if (isAddWrite)
                {
                    char[] otherType = currentTask.OtherType.ToString().ToCharArray();
                    if (otherType.Length != 3)
                    {
                        return ("", localizer == null ? "加签参数错误!" : localizer["FlowSend_AddWriteParamsError"], sendSteps);
                    }
                    int addType = otherType[1].ToString().ToInt();//加签类型 1前加签 2后加签 3并签
                    int executeType = otherType[2].ToString().ToInt(); ;//处理类型 1所有人同意，2一个同意 3顺序处理
                    if (executeType == 3)
                    {
                        var task = groupTasks.Find(p => p.StepId == currentTask.StepId && p.Sort == currentTask.Sort && p.StepSort == currentTask.StepSort + 1);
                        if (null != task)
                        {
                            nextStpes.Add((stepModel, task.ReceiveOrganizeId.HasValue
                                ? Organize.PREFIX_RELATION + task.ReceiveOrganizeId.Value.ToString()
                                : Organize.PREFIX_USER + task.ReceiveId.ToString()));
                        }
                    }
                    if (!nextStpes.Any())//前加签，发送给加签人
                    {
                        var addWriteTaskModel = Get(currentTask.PrevId);
                        if (null != addWriteTaskModel)
                        {
                            nextStpes.Add((stepModel,
                                addWriteTaskModel.ReceiveOrganizeId.HasValue
                                        ? Organize.PREFIX_RELATION + addWriteTaskModel.ReceiveOrganizeId.Value.ToString()
                                        : Organize.PREFIX_USER + addWriteTaskModel.ReceiveId.ToString()));
                        }
                    }
                }
                if (isFreeSend)//如果是自由发送，则在本步骤循环发送
                {
                    nextStpes.Add((stepModel, string.Empty));
                }
                if (!isStepSort && !isFreeSend && !isAddWrite)
                {
                    var nextStepsFlow = flow.GetNextSteps(flowRunModel, stepId);
                    foreach (var nextStep in nextStepsFlow)
                    {
                        nextStpes.Add((nextStep, string.Empty));
                    }
                }
            }

            if (nextStpes.Count == 0)
            {
                return ("", localizer == null ? "当前步骤没有后续步骤" : localizer["FlowSend_NotNextStep"], sendSteps);
            }

            #region 如果设置了跳过，要判断跳过条件
            User buser = new User();
            for (int i = 0; i < nextStpes.Count; i++)
            {
                var (nextStep, members) = nextStpes[i];
                if (nextStep.StepBase.SkipIdenticalUser == 1 || !nextStep.StepBase.SkipMethod.IsNullOrWhiteSpace())
                {
                    bool isSkip = false;
                    if (nextStep.StepBase.SkipIdenticalUser == 1)
                    {
                        var (defaultMembers, orgSelectType, selectRange) = GetDefaultMember(flowRunModel, stepModel, nextStep, groupTasks, taskId, instanceId, userId);
                        if (buser.Contains(defaultMembers, userId))
                        {
                            isSkip = true;
                        }
                    }
                    if (!isSkip && !nextStep.StepBase.SkipMethod.IsNullOrWhiteSpace())
                    {
                        Model.FlowRunModel.EventParam EventParam = new Model.FlowRunModel.EventParam();
                        EventParam.FlowId = flowRunModel.Id;
                        EventParam.GroupId = groupId;
                        EventParam.InstanceId = instanceId;
                        EventParam.Other = currentTask;
                        EventParam.StepId = stepId;
                        EventParam.TaskId = taskId;
                        EventParam.TaskTitle = null != currentTask ? currentTask.Title : string.Empty;
                        var (obj, err) = Tools.ExecuteMethod(nextStep.StepBase.SkipMethod.Trim(), EventParam);
                        if (obj != null && (obj.ToString().Equals("1") || obj.ToString().EqualsIgnoreCase("true")))
                        {
                            isSkip = true;
                        }
                    }
                    if (isSkip)
                    {
                        nextStpes.RemoveAt(i);
                        var nextNextSteps = flow.GetNextSteps(flowRunModel, nextStep.Id);
                        foreach (var nStep in nextNextSteps)
                        {
                            if (nextStep.StepBase.FlowType == 0)
                            {
                                var lineModel = flowRunModel.Lines.Find(p => p.FromId == nextStep.Id && p.ToId == nStep.Id);
                                if (lineModel != null)
                                {
                                    bool linePass = LinePass(lineModel, flowRunModel, stepModel, groupTasks, taskId, instanceId, userId, out string lineMsg);
                                    if (!linePass)
                                    {
                                        continue;
                                    }
                                }
                            }
                            nextStpes.Add((nStep, string.Empty));
                        }
                    }
                }
            }
            #endregion

            Dictionary<Guid, string> setStepsHanlde = GetNextStepsHandle(groupTasks);//得到由处理人员设置的后续步骤处理人员
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var (nextStep, members) in nextStpes)
            {
                //如果是系统控制，要判断线上的条件
                if (!isFreeSend && stepModel.StepBase.HanlderModel != 4 && stepModel.StepBase.FlowType == 0)
                {
                    var lineModel = flowRunModel.Lines.Find(p => p.FromId == stepModel.Id && p.ToId == nextStep.Id);
                    if (lineModel != null)
                    {
                        bool linePass = LinePass(lineModel, flowRunModel, stepModel, groupTasks, taskId, instanceId, userId, out string lineMsg);
                        if (!linePass)
                        {
                            continue;
                        }
                    }
                }

                string defaultHanlde = members;//默认处理人
                if (setStepsHanlde.ContainsKey(nextStep.Id))//如果处理人员设置的后续步骤的默认处理人员，则这里取设置的默认处理人员
                {
                    defaultHanlde = setStepsHanlde[nextStep.Id];
                }

                var (defaultMembers, orgSelectType, selectRange) = !defaultHanlde.IsNullOrWhiteSpace()
                    ? (defaultHanlde, "unit='1' dept='1' station='1' workgroup='1' user='1'", defaultHanlde)
                    : GetDefaultMember(flowRunModel, stepModel, nextStep, groupTasks, taskId, instanceId, userId);

                //如果设置了选择范围，则选择范围为设置值
                if (!nextStep.StepBase.SelectRange.IsNullOrWhiteSpace())
                {
                    selectRange = nextStep.StepBase.SelectRange;
                }

                if (nextStep.Dynamic == 2 && !nextStep.DynamicField.IsNullOrWhiteSpace() && flowRunModel.Databases.Count > 0)
                {
                    stringBuilder.Append(GetDynamicFieldStepHtml(flowRunModel, nextStep, instanceId, localizer));
                    sendSteps.Add(nextStep);
                    continue;
                }

                string inputChecked = string.Empty;
                string inputDisabled = stepModel.StepBase.FlowType == 0 ? " disabled=\"disbaled\"" : "";
                string inputType = stepModel.StepBase.FlowType == 1 ? "radio" : "checkbox";
                string memberDisabled = nextStep.StepBase.RunSelect == 0 ? " disabled=\"disbaled\"" : "";
                string ischangetype = selectRange.IsNullOrWhiteSpace() ? string.Empty : " isChangeType=\"0\"";//如果有选择范围则不能切换组织机构
                if (stepModel.StepBase.FlowType.In(1, 2) && nextStep.Id == nextStpes.First().Item1.Id)//如果是单选默认选中第一个
                {
                    inputChecked = " checked=\"checked\"";
                }
                else if (stepModel.StepBase.FlowType.In(0, 3))
                {
                    inputChecked = " checked=\"checked\"";
                }
                bool isSetTime = 1 == nextStep.SendSetWorkTime && !isAddWrite;//如果设置了在发送时指定完成时间(加签任务不指定时间)
                stringBuilder.Append("<tr");
                string trId = Guid.NewGuid().ToLowerNString();
                if (nextStep.Dynamic == 1)//动态步骤要标记行
                {
                    stringBuilder.Append(" data-dynamic=\"1\" id=\"" + trId + "\" data-beforestepid=\"" + nextStep.Id.ToString() + "\"");
                }
                stringBuilder.Append("><td>");
                stringBuilder.Append("<input type=\"" + inputType + "\"" + inputChecked + inputDisabled + " value=\"" + nextStep.Id.ToString() + "\" id=\"step_" + nextStep.Id.ToString() + "\" name=\"step\" style=\"vertical-align:middle;\" />");
                stringBuilder.Append("<label for=\"step_" + nextStep.Id.ToString() + "\" style=\"vertical-align:middle;\">" + nextStep.Name + "</label>");
                stringBuilder.Append("<input type=\"hidden\" id=\"name_" + nextStep.Id.ToString() + "\" value=\"" + nextStep.Name + "\" />");

                if (nextStep.Dynamic == 1)//动态步骤要加上添加步骤按钮
                {
                    stringBuilder.Append("<input type=\"hidden\" id=\"before_" + nextStep.Id.ToString() + "\" value=\"" + nextStep.Id.ToString() + "\" />");
                    stringBuilder.Append("<label class=\"flowsendstepadd\">");
                    stringBuilder.Append("<i onclick=\"dynamicAdd('" + trId + "', '" + nextStep.Id.ToString() + "', " + nextStep.SendSetWorkTime + ", this);\" class=\"fa fa-plus-square-o\" title=\"" + (localizer == null ? "添加步骤" : localizer["FlowSend_AddStep"]) + "\"></i>");
                    stringBuilder.Append("</label>");
                    stringBuilder.Append("<label style=\"margin-left:6px;\">");
                    stringBuilder.Append("<input type=\"radio\" value=\"0\" checked=\"checked\" title=\"" + (localizer == null ? "添加的步骤并行审批" : localizer["FlowSend_AddStepParallel"]) + "\" id=\"parallelorserial_" + nextStep.Id.ToString() + "_0\" name=\"parallelorserial_" + nextStep.Id.ToString() + "\" style=\"vertical-align:middle;\"/>");
                    stringBuilder.Append("<label for=\"parallelorserial_" + nextStep.Id.ToString() + "_0\" title=\"" + (localizer == null ? "添加的步骤并行审批" : localizer["FlowSend_AddStepParallel"]) + "\" style=\"vertical-align:middle;\">" + (localizer == null ? "并行审批" : localizer["FlowSend_AddStepParallel1"]) + "</label>");
                    stringBuilder.Append("<input type=\"radio\" value=\"1\" title=\"" + (localizer == null ? "添加的步骤串行审批" : localizer["FlowSend_AddStepSerial"]) + "\" id=\"parallelorserial_" + nextStep.Id.ToString() + "_1\" name=\"parallelorserial_" + nextStep.Id.ToString() + "\" style=\"vertical-align:middle;\"/>");
                    stringBuilder.Append("<label for=\"parallelorserial_" + nextStep.Id.ToString() + "_1\" title=\"" + (localizer == null ? "添加的步骤顺序审批" : localizer["FlowSend_AddStepSerial1"]) + "\" style=\"vertical-align:middle;\">" + (localizer == null ? "顺序审批" : localizer["FlowSend_AddStepSerial2"]) + "</label>");
                    stringBuilder.Append("</label>");
                }
                stringBuilder.Append("</td></tr>");
                stringBuilder.Append("<tr");
                if (nextStep.Dynamic == 1)//动态步骤要标记行
                {
                    stringBuilder.Append(" data-dynamic=\"1\" id=\"" + trId + "_1\" data-beforestepid=\"" + nextStep.Id.ToString() + "\"");
                }
                stringBuilder.Append("><td style=\"padding: 2px 0 4px 0;\">");
                stringBuilder.Append("<input type=\"text\" class=\"mymember\" opener=\"parent\" ismobile=\"" + (isMobile ? "1" : "0") + "\" id=\"user_" + nextStep.Id.ToString() + "\" name=\"user_" + nextStep.Id.ToString() + "\" value=\"" + defaultMembers + "\" " + orgSelectType + memberDisabled + ischangetype + " rootid=\"" + selectRange + "\" style=\"width:" + (isMobile && isSetTime ? "21%" : "43%") + "\" />");
                if (isSetTime)//是否需要指定时间
                {
                    stringBuilder.Append("<span style=\"margin-left:5px;\">" + (localizer == null ? "完成时间" : localizer["FlowSend_CompletedTime"]) + "：</span>");
                    stringBuilder.Append("<input type=\"text\" class=\"mycalendar\" istime=\"1\" dayafter=\"1\" value=\"\" style=\"width:110px;\" id=\"CompletedTime_" + nextStep.Id.ToString() + "\" name=\"CompletedTime_" + nextStep.Id.ToString() + "\" />");
                }
                stringBuilder.Append("</td></tr>");
                nextStep.RunDefaultMembers = defaultMembers;
                sendSteps.Add(nextStep);
            }
            return (stringBuilder.ToString(), "", sendSteps);
        }

        public string GetDynamicFieldStepHtml(Model.FlowRun flowRunModel, Model.FlowRunModel.Step step, string instanceId, IStringLocalizer localizer = null)
        {
            if (flowRunModel.Databases.Count == 0)
            {
                return string.Empty;
            }
            string[] fileds = step.DynamicField.Split('.');
            if (fileds.Length != 3)
            {
                return string.Empty;
            }
            Guid connId = fileds[0].ToGuid();
            string table = fileds[1];
            string field = fileds[2];
            string fieldVlaue = string.Empty;
            try
            {
                fieldVlaue = new DbConnection().GetFieldValue(connId, table, field, flowRunModel.Databases.First().PrimaryKey, instanceId);
            }
            catch
            {

            }
            if (fieldVlaue.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            string[] steps = fieldVlaue.Split(',');
            StringBuilder stringBuilder = new StringBuilder();
            int i = 0;
            foreach (string s in steps)
            {
                string id = Guid.NewGuid().ToString("N");
                string stepId = i == 0 ? step.Id.ToString() : Guid.NewGuid().ToString();
                string beforestepid = step.Id.ToString();
                string stepName = new Organize().GetNames(s);
                stringBuilder.Append("<tr data-dynamic=\"1\" id=\"" + id + "\" " +
                    "data-beforestepid=\"" + beforestepid + "\"><td><input type=\"hidden\" " +
                    "value=\"" + beforestepid + "\" id=\"before_" + stepId + "\">" +
                    (i == 0 ? "<input type=\"radio\" checked=\"checked\" style=\"display:none;\" value=\"0\" id=\"parallelorserial_" + beforestepid + "\" name=\"parallelorserial_" + beforestepid + "\"/>" : "") +
                    "<input type=\"checkbox\" checked=\"checked\" disabled=\"disbaled\" value=\"" + stepId + "\" " +
                    "id=\"step_" + stepId + "\" name=\"step\" style=\"vertical-align:middle;\">" +
                    "<label for=\"step_" + stepId + "\"><input type=\"text\" id=\"name_" + stepId + "\" class=\"mytext\" style=\"width: 130px;\" value=\""+ stepName + "\"></label>" +
                    "</td></tr>");
                stringBuilder.Append("<tr data-dynamic=\"1\" id=\"" + id + "_1\" " +
                    "data-beforestepid=\"" + beforestepid + "\"><td style=\"padding: 2px 0 4px 0;\">" +
                    "<input type=\"text\" class=\"mymember\" opener=\"parent\" ismobile=\"\" id=\"user_" + stepId + "\" " +
                    "name=\"user_" + stepId + "\" value=\"" + s + "\" ischangetype=\"1\" rootid=\"\" " +
                    "style=\"width:45%;\" readonly=\"\">");
                if (1 == step.SendSetWorkTime)
                {
                    stringBuilder.Append("<span style=\"margin-left:5px;\">"+ (localizer == null ? "完成时间" : localizer["FlowSend_CompletedTime"]) + "：</span><input type=\"text\" class=\"mycalendar\" istime=\"1\" dayafter=\"1\" value=\"\" " +
                        "style=\"width:120px;\" id=\"CompletedTime_" + stepId + "\" name=\"CompletedTime_" + stepId + "\">");
                }
                stringBuilder.Append("</td></tr>");
                i++;
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 得到退回步骤选择HTML
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <param name="stepId">步骤ID</param>
        /// <param name="groupId">组ID</param>
        /// <param name="taskId">任务ID</param>
        /// <param name="instanceId">实例ID</param>
        /// <param name="userId">当前人员ID</param>
        /// <param name="backSteps">可以退回的步骤</param>
        /// <param name="groupTasks">当前审批组任务列表，如果为空则在方法中查询</param>
        /// <param name="localizer">多语言包</param>
        /// <returns>步骤选择HTML，提示信息，退回的步骤集合</returns>
        public (string html, string message, List<Model.FlowRunModel.Step> backSteps) GetBackSteps(Guid flowId, Guid stepId, Guid groupId, Guid taskId, string instanceId, Guid userId, List<Model.FlowTask> groupTasks = null, IStringLocalizer localizer = null)
        {
            List<Model.FlowRunModel.Step> backSteps = new List<Model.FlowRunModel.Step>();
            Flow flow = new Flow();
            groupTasks = groupTasks ?? GetListByGroupId(groupId);
            var taskModel = groupTasks.Find(p => p.Id == taskId);
            if (taskModel == null)
            {
                return ("", localizer == null ? "未找到当前任务!" : localizer["NotFoundTask"], backSteps);
            }
            Model.FlowTask beforTask = null;
            if (Config.EnableDynamicStep && taskModel.BeforeStepId.IsEmptyGuid())
            {
                var beforTasks = groupTasks.OrderByDescending(p => p.Sort).Where(p => p.BeforeStepId.IsNotEmptyGuid());
                if (beforTasks.Any())
                {
                    beforTask = beforTasks.First();
                }
            }
            var flowRunModel = flow.GetFlowRunModel(flowId, true, beforTask);
            if (flowRunModel == null)
            {
                return ("", localizer == null ? "未找到流程运行时实体!" : localizer["NotFoundFlowRunModel"], backSteps);
            }
            var stepModel = flowRunModel.Steps.Find(p => p.Id == stepId);
            if (null == stepModel)
            {
                return ("", localizer == null ? "未找到步骤运行时实体!" : localizer["NotFoundStepModel"], backSteps);
            }
            if (flowRunModel.FirstStepId == stepModel.Id)
            {
                return ("", localizer == null ? "第一步不能退回!" : localizer["FlowBack_FirstStepCanotBack"], backSteps);
            }
            
            int backModel = stepModel.StepBase.BackModel;
            if (backModel == 0)
            {
                return ("", localizer == null ? "当前步骤设置为不能退回!" : localizer["FlowBack_CurrentStepCanotBack"], backSteps);
            }
            
            bool isOrderBackModel = false;//是否是按顺序处理退回
            bool isShowUserNames = false;//是否在退回步骤上显示用户姓名
            int hanlderModel = stepModel.StepBase.HanlderModel;
            if (taskModel.TaskType.In(6, 7))//如果是加签退回给加签人
            {
                var backTask = Get(taskModel.PrevId);
                if (null != backTask)
                {
                    stepModel.RunDefaultMembers = backTask.ReceiveOrganizeId.HasValue
                        ? Organize.PREFIX_RELATION + backTask.ReceiveOrganizeId.Value.ToString()
                        : Organize.PREFIX_USER + backTask.ReceiveId.ToString();
                    backSteps.Add(stepModel);
                    isShowUserNames = true;
                }
            }
            else if(backModel == 1 && hanlderModel == 4)
            {
                var backTask = groupTasks.Find(p => p.StepId == taskModel.StepId && p.TaskType != 5 && p.Sort == taskModel.Sort 
                && p.StepSort == taskModel.StepSort - 1);
                if (null != backTask)
                {
                    stepModel.RunDefaultMembers = backTask.ReceiveOrganizeId.HasValue
                        ? Organize.PREFIX_RELATION + backTask.ReceiveOrganizeId.Value.ToString()
                        : Organize.PREFIX_USER + backTask.ReceiveId.ToString();
                    backSteps.Add(stepModel);
                    isOrderBackModel = true;
                    isShowUserNames = true;
                }
            }
            if (!backSteps.Any())
            {
                int backType = stepModel.StepBase.BackType;
                switch (backType)
                {
                    case 0://退回前一步
                        var prevSteps = flow.GetPrevSteps(flowRunModel, taskModel.StepId);
                        foreach (var prevStep in prevSteps)
                        {
                            var prevTasks = groupTasks.FindAll(p => p.StepId == prevStep.Id && p.TaskType != 5);
                            if (prevTasks.Count > 0)
                            {
                                var prevMaxSort = prevTasks.Max(p => p.Sort);
                                var prevTask1 = prevTasks.FindAll(p => p.Sort == prevMaxSort);
                                if (prevTask1.Count > 0)
                                {
                                    StringBuilder userIds = new StringBuilder();
                                    foreach (var prevTask in prevTask1)
                                    {
                                        userIds.Append(prevTask.ReceiveOrganizeId.HasValue
                                            ? Organize.PREFIX_RELATION + prevTask.ReceiveOrganizeId.Value.ToString()
                                            : Organize.PREFIX_USER + prevTask.ReceiveId.ToString());
                                        userIds.Append(",");
                                    }
                                    prevStep.RunDefaultMembers = userIds.ToString().TrimEnd(',');
                                    backSteps.Add(prevStep);
                                }
                            }
                        }
                        break;
                    case 1://退回第一步
                        var firstStep = flowRunModel.Steps.Find(p => p.Id == flowRunModel.FirstStepId);
                        if (null != firstStep)
                        {
                            firstStep.RunDefaultMembers = Organize.PREFIX_USER + GetFirstSenderId(groupTasks).ToString();
                            backSteps.Add(firstStep);
                        }
                        break;
                    case 2://退回某一步
                        if (stepModel.StepBase.BackStepId.HasValue)//如果设计时选择了退回步骤
                        {
                            var step = flowRunModel.Steps.Find(p => p.Id == stepModel.StepBase.BackStepId.Value);
                            if (null != step)
                            {
                                step.RunDefaultMembers = GetStepHandler(groupTasks, step.Id);
                                backSteps.Add(step);
                            }
                        }
                        else//如果没有选，则列出步骤让用户选择
                        {
                            var groupSteps = GetTaskInstanceSteps(flowRunModel, groupTasks);
                            foreach (var step in groupSteps)
                            {
                                if (step.Id == taskModel.StepId)
                                {
                                    continue;
                                }
                                step.RunDefaultMembers = GetStepHandler(groupTasks, step.Id);
                                backSteps.Add(step);
                            }
                        }
                        break;
                }
            }

            if (!backSteps.Any())
            {
                return ("", localizer == null ? "未找到要退回的步骤!" : localizer["FlowBack_NotFoundBackStep"], backSteps);
            }

            StringBuilder stringBuilder = new StringBuilder();
            int stepIndex = 0;
            foreach (var backStep in backSteps)
            {
                //如果退回的接收步骤是顺序处理，则只退回给顺序处理的最后一个人。
                if (!isOrderBackModel && hanlderModel == 4)
                {
                    var stepTasks = groupTasks.FindAll(p => p.StepId == backStep.Id && p.TaskType != 5);
                    if (stepTasks.Count > 0)
                    {
                        var maxSort = stepTasks.Max(p => p.Sort);
                        var stepTasks1 = stepTasks.FindAll(p => p.Sort == maxSort).OrderByDescending(p => p.StepSort);
                        if (stepTasks1.Count() > 0)
                        {
                            backStep.RunDefaultMembers = stepTasks1.First().ReceiveOrganizeId.HasValue
                                ? Organize.PREFIX_RELATION + stepTasks1.First().ReceiveOrganizeId.Value.ToString()
                                : Organize.PREFIX_USER + stepTasks1.First().ReceiveId.ToString();
                        }
                    }
                }
                else if(backModel == 4)//独立退回，只退回给发送者
                {
                    backStep.RunDefaultMembers = Organize.PREFIX_USER + taskModel.SenderId.ToString();
                    isShowUserNames = true;
                }
                string checke = stepIndex++ == 0 ? "checked=\"checked\"" : "";//默认选中第一步
                stringBuilder.Append("<tr><td><div>");
                stringBuilder.Append("<input type=\"radio\" " + checke + " style=\"vertical-align:middle;\" value=\"" + backStep.Id + "\" name=\"stepid\" id=\"step_" + backStep.Id + "\"/>");
                stringBuilder.Append("<label style=\"vertical-align:middle;\" for=\"step_" + backStep.Id + "\">" + backStep.Name + (isShowUserNames ? "(" + new Organize().GetNames(backStep.RunDefaultMembers) + ")" : "") + "</label>");
                if (1 == stepModel.StepBase.BackSelectUser)//如果设置的是退回时可以选择接收人
                {
                    stringBuilder.Append("</div><div style=\"margin:1px 0 3px 0;\">");
                    stringBuilder.Append("<input type=\"text\" isChangeType=\"0\" class=\"mymember\" style=\"width:70%;\" id=\"user_" + backStep.Id + "\" rootid=\""+ backStep.RunDefaultMembers + "\" value=\"" + backStep.RunDefaultMembers + "\" />");
                    stringBuilder.Append("</div>");
                }
                else
                {
                    stringBuilder.Append("<input type=\"hidden\" id=\"user_" + backStep.Id + "\" value=\"" + backStep.RunDefaultMembers + "\" />");
                    stringBuilder.Append("</div>");
                }
                stringBuilder.Append("</tr></td>");
            }

            return (stringBuilder.ToString(), "", backSteps);
        }

        /// <summary>
        /// 判断线的条件是否满足
        /// </summary>
        /// <param name="lineModel"></param>
        /// <param name="flowRunModel"></param>
        /// <param name="currentStepModel"></param>
        /// <param name="groupTasks">当前组的任务列表</param>
        /// <param name="taskId"></param>
        /// <param name="instanceId"></param>
        /// <param name="currentUserId"></param>
        /// <param name="msg">不满足时的提示信息</param>
        /// <returns></returns>
        public bool LinePass(Model.FlowRunModel.Line lineModel, Model.FlowRun flowRunModel, Model.FlowRunModel.Step currentStepModel, List<Model.FlowTask> groupTasks, Guid taskId, string instanceId, Guid currentUserId, out string msg)
        {
            msg = string.Empty;
            if (null == lineModel)
            {
                return false;
            }
            #region 判断SQL条件
            if (!lineModel.SqlWhere.IsNullOrWhiteSpace())
            {
                if (flowRunModel.Databases.Count == 0)
                {
                    return false;
                }
                DbConnection dbConnection = new DbConnection();
                if (null == flowRunModel.Databases.First() || flowRunModel.Databases.First().ConnectionId.IsEmptyGuid() ||
                    flowRunModel.Databases.First().Table.IsNullOrWhiteSpace() ||
                    flowRunModel.Databases.First().PrimaryKey.IsNullOrWhiteSpace())
                {
                    return false;
                }
                var dbconnModel = dbConnection.Get(flowRunModel.Databases.First().ConnectionId);
                if (null == dbconnModel)
                {
                    return false;
                }
                string sql = "SELECT " + flowRunModel.Databases.First().PrimaryKey + " FROM " + flowRunModel.Databases.First().Table +
                    " WHERE " + flowRunModel.Databases.First().PrimaryKey + "={0}" + (lineModel.SqlWhere.Trim().ToLower().StartsWith("and") ? " " : " AND ") + lineModel.SqlWhere;
                try
                {
                    DataTable dataTable = dbConnection.GetDataTable(dbconnModel, sql, instanceId);
                    if (dataTable.Rows.Count == 0)
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            #endregion

            #region 判断组织架构
            Guid senderId = currentUserId;
            Guid sponserId = GetFirstSenderId(groupTasks);//发起者ID
            Organize organize = new Organize();
            User user = new User();
            if (sponserId.IsEmptyGuid() && currentStepModel.Id == flowRunModel.FirstStepId)//如果是第一步则发起者就是发送者
            {
                sponserId = senderId;
            }
            StringBuilder orgWheres = new StringBuilder();
            if (!lineModel.OrganizeExpression.IsNullOrEmpty())
            {
                JArray jArray = JArray.Parse(lineModel.OrganizeExpression);
                foreach (JObject json in jArray)
                {
                    if (json.Count == 0)
                    {
                        continue;
                    }
                    string usertype = json.Value<string>("usertype");
                    string in1 = json.Value<string>("in1");
                    string users = json.Value<string>("users");
                    string selectorganize = json.Value<string>("selectorganize");
                    string tjand = json.Value<string>("tjand");
                    string khleft = json.Value<string>("khleft");
                    string khright = json.Value<string>("khright");
                    Guid userid = "0".Equals(usertype) ? senderId : sponserId;
                    string memberid = "";
                    bool isin = false;
                    var leader = user.GetLeader(userid.ToString());
                    if ("0".Equals(users))
                    {
                        memberid = selectorganize;
                    }
                    else if ("1".Equals(users))
                    {
                        memberid = leader.leader;
                    }
                    else if ("2" == users)
                    {
                        memberid = leader.chargeLeader;
                    }
                    if ("0" == in1)
                    {
                        isin = user.IsIn(userid.ToString(), memberid);
                    }
                    else if ("1" == in1)
                    {
                        isin = !user.IsIn(userid.ToString(), memberid);
                    }
                    if (!khleft.IsNullOrEmpty())
                    {
                        orgWheres.Append(khleft);
                    }
                    orgWheres.Append(isin ? " true " : " false ");
                    if (!khright.IsNullOrEmpty())
                    {
                        orgWheres.Append(khright);
                    }
                    orgWheres.Append(tjand);
                }
                if (orgWheres.Length>0 && !(bool)Tools.ExecuteExpression(orgWheres.ToString()))
                {
                    return false;
                }
            }
            #endregion

            #region 判断自定义方法
            if (!lineModel.CustomMethod.IsNullOrWhiteSpace())
            {
                Model.FlowRunModel.EventParam eventParam = new Model.FlowRunModel.EventParam
                {
                    FlowId = flowRunModel.Id,
                    GroupId = groupTasks.Count > 0 ? groupTasks.First().GroupId : Guid.Empty,
                    InstanceId = instanceId,
                    StepId = currentStepModel.Id,
                    TaskId = taskId
                };
                var (obj, err) = Tools.ExecuteMethod(lineModel.CustomMethod.Trim(), eventParam);
                if (null == obj)
                {
                    return false;
                }
                string objString = obj.ToString();
                msg = objString;
                if (!"1".Equals(objString.ToLower()) && !"true".Equals(objString.ToLower()))
                {
                    return false;
                }
            }
            #endregion
            return true;
        }
        
        /// <summary>
        /// 得到步骤默认处理人员
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="currentStepModel"></param>
        /// <param name="nextStepModel"></param>
        /// <param name="groupTasks"></param>
        /// <param name="taskId"></param>
        /// <param name="instanceId"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        private (string defaultMembers, string orgSelectType, string selectRange) GetDefaultMember(Model.FlowRun flowRunModel, Model.FlowRunModel.Step currentStepModel, Model.FlowRunModel.Step nextStepModel, List<Model.FlowTask> groupTasks, Guid taskId, string instanceId, Guid currentUserId)
        {
            //如果是调试模式并且当前登录人员包含在调试人员中 则默认为当前人员
            if (flowRunModel.Debug > 0 && flowRunModel.DebugUserIds.ContainsIgnoreCase(currentUserId.ToString()))
            {
                return (Organize.PREFIX_USER + currentUserId.ToString(), "", "");
            }

            #region 判断处理者类型
            string members = string.Empty, selectType = string.Empty, selectRange = string.Empty;
            var currentTask = groupTasks.Find(p => p.Id == taskId);
            var firstSenderId = GetFirstSenderId(groupTasks);//发起者ID
            if (firstSenderId.IsEmptyGuid() && currentStepModel.Id == flowRunModel.FirstStepId)//如果发起者为空，并且当前是第一步则发起者是当前人员
            {
                firstSenderId = currentUserId;
            }
            User user = new User();
            switch (nextStepModel.StepBase.HandlerType.ToInt())
            {
                case 0:
                    selectType = "unit='1' dept='1' station='1' workgroup='1' user='1'";
                    break;
                case 1:
                    selectType = "unit='0' dept='1' station='0' workgroup='0' user='0'";
                    break;
                case 2:
                    selectType = "unit='0' dept='0' station='1' workgroup='0' user='0'";
                    break;
                case 3:
                    selectType = "unit='0' dept='0' station='0' workgroup='1' user='0'";
                    break;
                case 4:
                    selectType = "unit='0' dept='0' station='0' workgroup='0' user='1'";
                    break;
                case 5://发起者
                    members = Organize.PREFIX_USER + firstSenderId.ToString();
                    break;
                case 6://前一步骤处理者
                    members = currentTask == null ? Organize.PREFIX_USER + currentUserId : GetStepHandler(groupTasks, currentTask.StepId);
                    break;
                case 7://某一步骤处理者
                    members = nextStepModel.StepBase.HandlerStepId.HasValue ?
                        GetStepHandler(groupTasks, nextStepModel.StepBase.HandlerStepId.Value) : string.Empty;
                    if (members.IsNullOrWhiteSpace() && currentStepModel.Id == flowRunModel.FirstStepId)//如果是第一步就是当前人员ID
                    {
                        members = Organize.PREFIX_USER + currentUserId.ToString();
                    }
                    break;
                case 8://字段值
                    if (flowRunModel.Databases.Count > 0)
                    {
                        string fieldValue = string.Empty;// Tools.HttpContext.Request.Forms((flowRunModel.Databases.First().Table + "-" + nextStepModel.StepBase.ValueField).ToUpper());
                        members = !fieldValue.IsNullOrWhiteSpace() ? fieldValue : GetFieldValue(flowRunModel.Databases.First(), nextStepModel.StepBase.ValueField, instanceId);
                    }
                    break;
                case 9://发起者部门领导
                    members = user.GetLeader(firstSenderId.ToString()).leader;
                    break;
                case 10://发起者分管领导
                    members = user.GetLeader(firstSenderId.ToString()).chargeLeader;
                    break;
                case 11://前一步处理者部门领导
                    members = currentTask == null ? user.GetLeader(Organize.PREFIX_USER + currentUserId).leader 
                        : user.GetLeader(GetStepHandler(groupTasks, currentTask.StepId).Split(',').ToList()).leader;
                    break;
                case 12://前一步处理者分管领导
                    members = currentTask == null ? user.GetLeader(Organize.PREFIX_USER + currentUserId).chargeLeader 
                        : user.GetLeader(GetStepHandler(groupTasks, currentTask.StepId).Split(',').ToList()).chargeLeader;
                    break;
                case 13://发起者上级部门领导
                    members = user.GetParentLeader(firstSenderId.ToString()).leader;
                    break;
                case 14://前一步处理者上级部门领导
                    members = currentTask == null ? user.GetParentLeader(Organize.PREFIX_USER + currentUserId).leader
                        : user.GetParentLeader(GetStepHandler(groupTasks, currentTask.StepId).Split(',').ToList()).leader;
                    break;
                case 15://前一步处理者部门所有成员
                    members = currentTask == null ? user.GetUserIds(user.GetOrganizeUsers(Organize.PREFIX_USER + currentUserId)) 
                        : user.GetUserIds(user.GetOrganizeUsers(GetStepHandler(groupTasks, currentTask.StepId).Split(',').ToList()));
                    break;
                case 16://发起者部门所有成员
                    members = user.GetUserIds(user.GetOrganizeUsers(firstSenderId.ToString()));
                    break;
                case 17://发起者所有上级部门领导
                    members = user.GetAllParentLeader(firstSenderId.ToString()).leader;
                    break;
                case 18://前一步处理者所有上级部门领导
                    members = currentTask == null ? user.GetAllParentLeader(firstSenderId.ToString()).leader 
                        : user.GetAllParentLeader(GetStepHandler(groupTasks, currentTask.StepId).Split(',').ToList()).leader;
                    break;
            }
            #endregion

            #region SQL或方法
            string sqlOrMethod = nextStepModel.StepBase.DefaultHandlerSqlOrMethod;
            if (!sqlOrMethod.IsNullOrWhiteSpace())
            {
                //如果是以select开头说明是sql 
                if (sqlOrMethod.Trim().StartsWith("select", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (flowRunModel.Databases.Count > 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        DataTable dataTable = null;
                        try
                        {
                            dataTable = new DbConnection().GetDataTable(flowRunModel.Databases.First().ConnectionId, Wildcard.Filter(sqlOrMethod));
                        }
                        catch { }
                        if (dataTable != null && dataTable.Columns.Count > 0)
                        {
                            foreach (DataRow dr in dataTable.Rows)
                            {
                                stringBuilder.Append(dr[0].ToString());
                                stringBuilder.Append(",");
                            }
                        }
                        members = stringBuilder.ToString().TrimEnd(',');
                    }
                }
                else
                {
                    Model.FlowRunModel.EventParam eventParam = new Model.FlowRunModel.EventParam
                    {
                        FlowId = flowRunModel.Id,
                        GroupId = groupTasks.Count > 0 ? groupTasks.First().GroupId : Guid.Empty,
                        InstanceId = instanceId,
                        StepId = currentStepModel.Id,
                        TaskId = taskId
                    };
                    var (obj, err) = Tools.ExecuteMethod(sqlOrMethod.Trim(), eventParam);
                    if (null != obj)
                    {
                        members = obj.ToString();
                    }
                }
            }
            #endregion

            if (!nextStepModel.StepBase.DefaultHandler.IsNullOrWhiteSpace())
            {
                members = members + "," + nextStepModel.StepBase.DefaultHandler;
            }

            string members1 = members.IsNullOrWhiteSpace() ? string.Empty : members.Split(',').Distinct().ToList().JoinList();//去重

            //如果设置了处理者类型，则选择范围就是处理者类型
            if (nextStepModel.StepBase.HandlerType.ToInt() > 6)
            {
                selectRange = members1;
            }
            return (members1, selectType, selectRange);
        }

        /// <summary>
        /// 得到处理类别显示标题
        /// </summary>
        /// <param name="executeType"></param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        public string GetExecuteTypeTitle(int executeType, IStringLocalizer localizer = null)
        {
            string title = string.Empty;
            switch (executeType)
            {
                case -1:
                    title = localizer == null ? "等待中" : localizer["State_Wait"];
                    break; 
                case 0:
                    title = localizer == null ? "未处理" : localizer["State_Untreated"];
                    break;
                case 1:
                    title = localizer == null ? "处理中" : localizer["State_Processing"];
                    break;
                case 2:
                    title = localizer == null ? "已完成" : localizer["State_Completed"];
                    break;
                case 3:
                    title = localizer == null ? "已退回" : localizer["State_Back"];
                    break;
                case 4:
                    title = localizer == null ? "他人已处理" : localizer["State_OtherProcess"];
                    break;
                case 5:
                    title = localizer == null ? "他人已退回" : localizer["State_OtherBack"];
                    break;
                case 6:
                    title = localizer == null ? "已转交" : localizer["State_Transferred"];
                    break;
                case 7:
                    title = localizer == null ? "已委托" : localizer["State_Entrusted"];
                    break;
                case 8:
                    title = localizer == null ? "已阅知" : localizer["State_Knowing"];
                    break;
                case 9:
                    title = localizer == null ? "已指派" : localizer["State_Assigned"];
                    break;
                case 10:
                    title = localizer == null ? "已跳转" : localizer["State_Skip"];
                    break;
                case 11:
                    title = localizer == null ? "已终止" : localizer["State_Terminated"];
                    break;
                case 12:
                    title = localizer == null ? "他人已终止" : localizer["State_OtherTerminated"];
                    break;
                case 13:
                    title = localizer == null ? "已加签" : localizer["State_AddWrite"];
                    break;
            }
            return title;
        }

        /// <summary>
        /// 得到要抄送的人员
        /// </summary>
        /// <param name="stepModel"></param>
        /// <returns></returns>
        public List<Model.User> GetCopyForUsers(Model.FlowRunModel.Step stepModel, Model.FlowRun flowRunModel, Model.FlowRunModel.Execute executeModel, List<Model.FlowTask> groupTasks)
        {
            StringBuilder members = new StringBuilder();
            if (!stepModel.StepCopyFor.MemberId.IsNullOrWhiteSpace())
            {
                members.Append(stepModel.StepCopyFor.MemberId);
                members.Append(",");
            }
            #region sql或方法
            if (!stepModel.StepCopyFor.MethodOrSql.IsNullOrWhiteSpace())
            {
                string methodOrSql = stepModel.StepCopyFor.MethodOrSql.Trim();
                if (methodOrSql.StartsWith("select", StringComparison.CurrentCultureIgnoreCase))//sql
                {
                    if (flowRunModel.Databases.Count > 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        DataTable dataTable = new DbConnection().GetDataTable(flowRunModel.Databases.First().ConnectionId, Wildcard.Filter(methodOrSql));
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            stringBuilder.Append(dr[0]);
                            stringBuilder.Append(",");
                        }
                        members.Append(stringBuilder.ToString());
                        //members.Append(",");
                    }
                }
                else
                {
                    Model.FlowRunModel.EventParam eventParam = new Model.FlowRunModel.EventParam
                    {
                        FlowId = flowRunModel.Id,
                        GroupId = executeModel.GroupId,
                        InstanceId = executeModel.InstanceId,
                        StepId = stepModel.Id,
                        TaskId = executeModel.TaskId
                    };
                    var (obj, err) = Tools.ExecuteMethod(methodOrSql.Trim(), eventParam);
                    if (null != obj)
                    {
                        members.Append(obj.ToString());
                        members.Append(",");
                    }
                }
            }
            #endregion

            #region 处理者类型 
            if (!stepModel.StepCopyFor.HandlerType.IsNullOrWhiteSpace())
            {
                User user = new User();
                foreach (string type in stepModel.StepCopyFor.HandlerType.Split(','))
                {
                    if (!type.IsInt(out int handlerType))
                    {
                        continue;
                    }

                    string members1 = string.Empty;
                    var firstSenderId = GetFirstSenderId(groupTasks);
                    switch (handlerType)
                    {
                        case 0://发起者
                            members1 = Organize.PREFIX_USER + firstSenderId;
                            break;
                        case 1://前一步骤处理者
                            members1 = GetPrevStepHandler(groupTasks, executeModel.TaskId);
                            break;
                        case 2://发起者部门领导
                            members1 = user.GetLeader(firstSenderId.ToString()).leader;
                            break;
                        case 3://发起者分管领导
                            members1 = user.GetLeader(firstSenderId.ToString()).chargeLeader;
                            break;
                        case 4://发起者上级部门领导
                            members1 = user.GetParentLeader(firstSenderId.ToString()).leader;
                            break;
                        case 5://发起者部门所有成员
                            members1 = user.GetUserIds(user.GetOrganizeUsers(firstSenderId.ToString()));
                            break;
                        case 6://发起者所有上级部门领导
                            members1 = user.GetAllParentLeader(firstSenderId.ToString()).leader;
                            break;
                    }
                    members.Append(members1);
                    members.Append(",");
                }
            }
            #endregion

            #region 步骤
            if (!stepModel.StepCopyFor.Steps.IsNullOrWhiteSpace())
            {
                foreach (string stepId in stepModel.StepCopyFor.Steps.Split(','))
                {
                    if (stepId.IsGuid(out Guid guid))
                    {
                        members.Append(GetStepHandler(groupTasks, guid));
                        members.Append(",");
                    }
                }
            }
            #endregion

            return new Organize().GetAllUsers(members.ToString().TrimEnd(','));
        }

        /// <summary>
        /// 发送待办消息
        /// </summary>
        /// <param name="nextTasks"></param>
        /// <param name="sender"></param>
        /// <param name="sendModel">发送方式 0站内短信 1手机短信 2微信 3公众号</param>
        /// <param name="contents">消息内容</param>
        public async void SendMessage(List<Model.FlowTask> nextTasks, Model.User sender, string sendModel = "", string contents = "", IStringLocalizer localizer = null)
        {
            await Task.Run(() =>
            {
                if (null == nextTasks || nextTasks.Count == 0)
                {
                    return;
                }
                Message message = new Message();
                User user = new User();
                var httpContext = Tools.HttpContext;
                foreach (var nextTask in nextTasks)
                {
                    if (nextTask.Status == -1)//等待中的任务不发消息
                    {
                        continue;
                    }
                    if (null != sender && nextTask.ReceiveId == sender.Id)//自己处理的任务不给自己发消息
                    {
                        continue;
                    }
                    var userModel = user.Get(nextTask.ReceiveId);
                    if (null == userModel)
                    {
                        continue;
                    }
                    string appopenmodel = null == httpContext ? "0" : httpContext.Request.Querys("rf_appopenmodel");
                    string url = string.Format("/RoadFlowCore/FlowRun/{0}?flowid={1}&stepid={2}&taskid={3}&groupid={4}&instanceid={5}&appid={6}&tabid={7}&rf_appopenmodel={8}",
                       "Index", nextTask.FlowId, nextTask.StepId, nextTask.Id, nextTask.GroupId, nextTask.InstanceId,
                       null == httpContext ? "" : httpContext.Request.Querys("appid"), null == httpContext ? "" : httpContext.Request.Querys("tabid"),
                       appopenmodel);
                    //手机短信
                    if (("," + sendModel + ",").Contains(",1,") && !userModel.Mobile.IsNullOrWhiteSpace())
                    {
                        SMS.SendSMS(contents.IsNullOrWhiteSpace() ? (localizer == null ? "您有一个待办事项“" : localizer["Execute_SendMessageWait"])
                            + nextTask.Title + "”，" + (localizer == null ? "请尽快处理！" : localizer["Execute_SendMessageProcess"]) : contents, userModel.Mobile);
                    }
                    //站内消息
                    if (("," + sendModel + ",").Contains(",0,"))
                    {
                        message.Send(nextTask.ReceiveId, (contents.IsNullOrWhiteSpace() ? (localizer == null ? "您有一个待办事项“" : localizer["Execute_SendMessageWait"])
                            + nextTask.Title + "”" : contents) + "，<a href=\"javascript:;\" class=\"blue1\" onclick=\"top.openApp('" + url + "',"
                            + (appopenmodel.IsNullOrWhiteSpace() ? "0" : appopenmodel) + ",'" + nextTask.Title + "');top.closeMessage();\">"
                            + (localizer == null ? "点击处理！" : localizer["Execute_SendMessageClickProcess"]) + "</a>", "0",
                            sender);
                    }
                    //微信
                    if (Config.Enterprise_WeiXin_IsUse && ("," + sendModel + ",").Contains(",2,") && (!userModel.Mobile.IsNullOrWhiteSpace() || !userModel.Email.IsNullOrWhiteSpace()))
                    {
                        JObject msgJson = new JObject
                        {
                            { "title", localizer == null ? "待办事项" : localizer["Execute_SendMessageToItems"] },
                            { "description", !contents.IsNullOrWhiteSpace() ? contents : "<div class=\"gray\">" + (localizer==null?"发送人":localizer["Execute_SendMessageSender"]) + ":" + nextTask.SenderName
                            + "  时间:"+nextTask.ReceiveTime.ToShortDateTimeString() + "</div><div class=\"normal\">"
                            + (contents.IsNullOrWhiteSpace() ? (localizer == null ? "您有一个待办事项“" : localizer["Execute_SendMessageWait"]) 
                            + nextTask.Title + "”，" + (localizer == null ? "请尽快处理！" : localizer["Execute_SendMessageProcess"]) : contents) + "</div>" },
                            { "url", Config.Enterprise_WeiXin_WebUrl + url + "&ismobile=1" },
                            { "btntxt", localizer == null ? "处理" : localizer["Execute_SendMessageProcess1"] }
                        };
                        EnterpriseWeiXin.Common.SendMessage(userModel, msgJson, "textcard");
                    }
                    //微信公众号
                    if(Config.WeiXin_IsUse && ("," + sendModel + ",").Contains(",3,"))
                    {
                        if (!userModel.WeiXinOpenId.IsNullOrWhiteSpace())
                        {
                            WeiXin.TemplateMessage.SendWaitTaskMessage(nextTask);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 得到流程处理后的提示信息 
        /// </summary>
        /// <param name="executeResultModel"></param>
        /// <param name="executeModel"></param>
        /// <param name="currentStepModel"></param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        public string GetExecuteMessage(Model.FlowRunModel.ExecuteResult executeResultModel, Model.FlowRunModel.Execute executeModel, Model.FlowRunModel.Step currentStepModel, IStringLocalizer localizer = null)
        {
            if (!currentStepModel.SendShowMessage.IsNullOrWhiteSpace())//如果步骤设置了发送提示语，则直接返回
            {
                return Wildcard.Filter(currentStepModel.SendShowMessage);
            }
            string message = string.Empty;
            var nextTasks = executeResult.NextTasks.FindAll(p => p.TaskType != 5);
            switch (executeModel.ExecuteType)
            {
                case Model.FlowRunModel.Execute.Type.Submit:
                case Model.FlowRunModel.Execute.Type.FreeSubmit:
                    if (executeResultModel.IsSuccess && nextTasks.Count() > 0)
                    {
                        List<string> stepNames = new List<string>();
                        foreach (var task in nextTasks.GroupBy(p => p.StepName))
                        {
                            stepNames.Add(task.Key);
                        }
                        message = (localizer == null ? "已发送到：" : localizer["Execute_GetExecuteMessageSendTo"]) + stepNames.JoinList("、");
                    }
                    else
                    {
                        message = localizer == null ? "已发送,等待他人处理!" : localizer["Execute_GetExecuteMessageSendWait"];
                    }
                    break;
                case Model.FlowRunModel.Execute.Type.Completed:
                    message = executeResultModel.IsSuccess ? (localizer == null ? "已完成!" : localizer["Execute_GetExecuteMessageCompleted"])
                        : (localizer == null ? "已发送,等待他人处理!" : localizer["Execute_GetExecuteMessageSendWait"]);
                    break;
                case Model.FlowRunModel.Execute.Type.Back:
                    if (executeResultModel.IsSuccess && nextTasks.Count() > 0)
                    {
                        List<string> stepNames = new List<string>();
                        foreach (var task in nextTasks.GroupBy(p => p.StepName))
                        {
                            stepNames.Add(task.Key);
                        }
                        message = (localizer == null ? "已退回到：" : localizer["Execute_GetExecuteMessageBackTo"]) + stepNames.JoinList("、");
                    }
                    else
                    {
                        message = localizer == null ? "已退回,等待他人处理!" : localizer["Execute_GetExecuteMessageBackWait"];
                    }
                    break;
                case Model.FlowRunModel.Execute.Type.Save:
                    if (executeResultModel.IsSuccess)
                    {
                        message = localizer == null ? "已保存!" : localizer["Execute_GetExecuteMessageSaved"];
                    }
                    break;
                case Model.FlowRunModel.Execute.Type.Redirect:
                    if (executeResult.IsSuccess && nextTasks.Count() > 0)
                    {
                        List<string> stepNames = new List<string>();
                        foreach (var task in nextTasks.GroupBy(p => p.StepName))
                        {
                            stepNames.Add(task.Key);
                        }
                        message = (localizer == null ? "已转交给：" : localizer["Execute_GetExecuteMessageRedirectTo"]) + stepNames.JoinList("、");
                    }
                    else
                    {
                        message = localizer == null ? "没有接收人!" : localizer["Execute_RedirectReceiverEmpty"];
                    }
                    break;
                case Model.FlowRunModel.Execute.Type.CopyforCompleted:
                    if (executeResult.IsSuccess)
                    {
                        message = localizer == null ? "已完成!" : localizer["Execute_GetExecuteMessageCompleted"];
                    }
                    else
                    {
                        message = localizer == null ? "处理失败!" : localizer["Execute_GetExecuteMessageFail"];
                    }
                    break;
            }
            return message;
        }

        private Model.FlowRunModel.ExecuteResult executeResult;
        private List<Model.FlowTask> nextStepTasks;
        private List<Model.FlowTask> addTasks;//要添加的任务
        private List<Model.FlowTask> updateTasks;//要修改的任务
        private List<Model.FlowTask> removeTasks;//要删除的任务
        private List<(string, object[], int)> executeSqls;//要执行的SQL列表(sql,参数,0提交退回前 1提交退回后)
        private static readonly object lockObj = new object();//锁对象
        /// <summary>
        /// 执行流程
        /// </summary>
        /// <param name="executeModel"></param>
        /// <param name="flowRunModel">流程运行时实体</param>
        /// <param name="localizer">语言包</param>
        public Model.FlowRunModel.ExecuteResult Execute(Model.FlowRunModel.Execute executeModel, Model.FlowRun flowRunModel = null, IStringLocalizer localizer = null)
        {
            if (null == flowRunModel)
            {
                Model.FlowTask flowTaskModel = null;
                if (Config.EnableDynamicStep && executeModel.GroupId.IsNotEmptyGuid())
                {
                    var beforeTasks = GetListByGroupId(executeModel.GroupId).OrderByDescending(p => p.Sort).Where(p => p.BeforeStepId.IsNotEmptyGuid());
                    if (beforeTasks.Any())
                    {
                        flowTaskModel = beforeTasks.First();
                    }
                }
                flowRunModel = new Flow().GetFlowRunModel(executeModel.FlowId, true, flowTaskModel);
            }
            if (null == flowRunModel)
            {
                executeResult.DebugMessages = localizer == null ? "未找到流程运行时!" : localizer["Execute_NotFoundFlowRunModel"];
                executeResult.IsSuccess = false;
                executeResult.Messages = localizer == null ? "未找到流程运行时!" : localizer["Execute_NotFoundFlowRunModel"];
                return executeResult;
            }
            var currentStep = flowRunModel.Steps.Find(p => p.Id == executeModel.StepId);
            if (null == currentStep && executeModel.ExecuteType != Model.FlowRunModel.Execute.Type.AddWrite)
            {
                executeResult.DebugMessages = localizer == null ? "未找到流程运行时!" : localizer["Execute_NotFoundStepRunModel"];
                executeResult.IsSuccess = false;
                executeResult.Messages = localizer == null ? "未找到流程运行时!" : localizer["Execute_NotFoundStepRunModel"];
                return executeResult;
            }
            //组织事件参数
            Model.FlowRunModel.EventParam eventParam = new Model.FlowRunModel.EventParam
            {
                FlowId = executeModel.FlowId,
                GroupId = executeModel.GroupId,
                InstanceId = executeModel.InstanceId,
                StepId = executeModel.StepId,
                TaskId = executeModel.TaskId,
                TaskTitle = executeModel.Title,
                FlowRunModel = flowRunModel
            };

            lock(lockObj)
            {
                executeResult = new Model.FlowRunModel.ExecuteResult();
                nextStepTasks = new List<Model.FlowTask>();
                addTasks = new List<Model.FlowTask>();
                updateTasks = new List<Model.FlowTask>();
                removeTasks = new List<Model.FlowTask>();
                executeSqls = new List<(string, object[], int)>();

                switch (executeModel.ExecuteType)
                {
                    case Model.FlowRunModel.Execute.Type.Submit:
                    case Model.FlowRunModel.Execute.Type.FreeSubmit:
                    case Model.FlowRunModel.Execute.Type.Completed:
                        #region 提交前事件
                        if (!currentStep.StepEvent.SubmitBefore.IsNullOrWhiteSpace())//加签不执行事件
                        {
                            string eventName = currentStep.StepEvent.SubmitBefore.Trim();
                            //如果是以[sql]开头，则说明是SQL语句
                            if (eventName.StartsWith("[sql]"))
                            {
                                executeSqls.Add((Wildcard.Filter(eventName).TrimStart("[sql]".ToCharArray()), null, 0));
                            }
                            else
                            {
                                var (eventReturn, err) = Tools.ExecuteMethod(eventName, eventParam);
                                string eventReturnStr = string.Empty;
                                if (null != eventReturn)
                                {
                                    eventReturnStr = eventReturn.ToString();
                                }
                                if (null != err)
                                {
                                    Log.Add((localizer == null ? "执行提交前事件发生了错误-" : localizer["Execute_EventStepBeforeErrorLog"]) + flowRunModel.Name + "-" + currentStep.Name + "-" + currentStep.StepEvent.SubmitBefore, (localizer == null ? "参数：" : localizer["Execute_EventStepBeforeErrorParam"]) + eventParam.ToString() + (localizer == null ? " 错误：" : localizer["Execute_EventStepBeforeErrorError"]) + err.Message + err.StackTrace, Log.Type.流程运行);
                                }
                                if (!eventReturnStr.IsNullOrWhiteSpace() && !"1".Equals(eventReturnStr) && !"true".EqualsIgnoreCase(eventReturnStr))
                                {
                                    //提交前事件，如果事件返回值不为空并且为，1或true字符串，则表示错误信息，阻止流程提交。
                                    if ("false".EqualsIgnoreCase(eventReturnStr))
                                    {
                                        eventReturnStr = localizer == null ? "不能提交!" : localizer["Execute_EventStepBeforeErrorCannotSubmit"];
                                    }
                                    executeResult.DebugMessages = eventReturnStr;
                                    executeResult.IsSuccess = false;
                                    executeResult.Messages = eventReturnStr;
                                    return executeResult;
                                }
                            }
                        }
                        #endregion
                        Submit(flowRunModel, executeModel, localizer);
                        #region 提交后事件
                        if (!currentStep.StepEvent.SubmitAfter.IsNullOrWhiteSpace())
                        {
                            //第一步提交的时候taskID没有值，在这里赋值
                            if (null != executeResult.CurrentTask)
                            {
                                eventParam.TaskId = executeResult.CurrentTask.Id;
                                eventParam.GroupId = executeResult.CurrentTask.GroupId;
                                eventParam.TaskTitle = executeResult.CurrentTask.Title;
                                if (eventParam.InstanceId.IsNullOrWhiteSpace())
                                {
                                    eventParam.InstanceId = executeResult.CurrentTask.InstanceId;
                                }
                            }
                            string eventName = currentStep.StepEvent.SubmitAfter.Trim();
                            //如果是以[sql]开头，则说明是SQL语句
                            if (eventName.StartsWith("[sql]"))
                            {
                                object[] objects = new object[] { eventParam.TaskId, eventParam.InstanceId, eventParam.GroupId };
                                executeSqls.Add((Wildcard.Filter(eventName).TrimStart("[sql]".ToCharArray()), objects, 1));
                            }
                            else
                            {
                                var (eventReturn, err) = Tools.ExecuteMethod(eventName, eventParam);
                                string eventReturnStr = string.Empty;
                                if (null != eventReturn)
                                {
                                    eventReturnStr = eventReturn.ToString();
                                }
                                if (null != err)
                                {
                                    Log.Add((localizer == null ? "执行提交后事件发生了错误-" : localizer["Execute_EventStepAfterErrorLog"]) + flowRunModel.Name + "-" + currentStep.Name + "-" + currentStep.StepEvent.SubmitBefore, (localizer == null ? "参数：" : localizer["Execute_EventStepBeforeErrorParam"]) + eventParam.ToString() + (localizer == null ? " 错误：" : localizer["Execute_EventStepBeforeErrorError"]) + err.Message + err.StackTrace, Log.Type.流程运行);
                                }
                            }
                        }
                        #endregion
                        break;
                    case Model.FlowRunModel.Execute.Type.Back:
                        #region 退回前事件
                        if (!currentStep.StepEvent.BackBefore.IsNullOrWhiteSpace())
                        {
                            string eventName = currentStep.StepEvent.BackBefore.Trim();
                            //如果是以[sql]开头，则说明是SQL语句
                            if (eventName.StartsWith("[sql]"))
                            {
                                executeSqls.Add((Wildcard.Filter(eventName).TrimStart("[sql]".ToCharArray()), null, 0));
                            }
                            else
                            {
                                var (eventReturn, err) = Tools.ExecuteMethod(eventName, eventParam);
                                string eventReturnStr = string.Empty;
                                if (null != eventReturn)
                                {
                                    eventReturnStr = eventReturn.ToString();
                                }
                                if (null != err)
                                {
                                    Log.Add((localizer == null ? "执行退回前事件发生了错误-" : localizer["Execute_EventStepBackBeforeErrorLog"]) + flowRunModel.Name + "-" + currentStep.Name + "-" + currentStep.StepEvent.SubmitBefore, (localizer == null ? "参数：" : localizer["Execute_EventStepBeforeErrorParam"]) + eventParam.ToString() + (localizer == null ? " 错误：" : localizer["Execute_EventStepBeforeErrorError"]) + err.Message + err.StackTrace, Log.Type.流程运行);
                                }
                                if (!eventReturnStr.IsNullOrWhiteSpace() && !"1".Equals(eventReturnStr) && !"true".EqualsIgnoreCase(eventReturnStr))
                                {
                                    //退回前事件，如果事件返回值不为空并且为，1或true字符串，则表示错误信息，阻止流程提交。
                                    if ("false".EqualsIgnoreCase(eventReturnStr))
                                    {
                                        eventReturnStr = localizer == null ? "不能退回!" : localizer["Execute_EventStepBackBeforeErrorCannotBack"];
                                    }
                                    executeResult.DebugMessages = eventReturnStr;
                                    executeResult.IsSuccess = false;
                                    executeResult.Messages = eventReturnStr;
                                    return executeResult;
                                }
                            }
                        }
                        #endregion
                        Back(flowRunModel, executeModel, localizer);
                        #region 退回后事件
                        if (!currentStep.StepEvent.BackAfter.IsNullOrWhiteSpace())
                        {
                            string eventName = currentStep.StepEvent.BackAfter.Trim();
                            //如果是以[sql]开头，则说明是SQL语句
                            if (eventName.StartsWith("[sql]"))
                            {
                                executeSqls.Add((Wildcard.Filter(eventName).TrimStart("[sql]".ToCharArray()), null, 1));
                            }
                            else
                            {
                                var (eventReturn, err) = Tools.ExecuteMethod(eventName, eventParam);
                                string eventReturnStr = string.Empty;
                                if (null != eventReturn)
                                {
                                    eventReturnStr = eventReturn.ToString();
                                }
                                if (null != err)
                                {
                                    Log.Add((localizer == null ? "执行退回后事件发生了错误-" : localizer["Execute_EventStepBackAfterErrorLog"]) + flowRunModel.Name + "-" + currentStep.Name + "-" + currentStep.StepEvent.SubmitBefore, (localizer == null ? "参数：" : localizer["Execute_EventStepBeforeErrorParam"]) + eventParam.ToString() + (localizer == null ? " 错误：" : localizer["Execute_EventStepBeforeErrorError"]) + err.Message + err.StackTrace, Log.Type.流程运行);
                                }
                            }
                        }
                        #endregion
                        break;
                    case Model.FlowRunModel.Execute.Type.Save:
                        Save(flowRunModel, executeModel, localizer);
                        break;
                    case Model.FlowRunModel.Execute.Type.Redirect:
                        Redirect(flowRunModel, executeModel, localizer);
                        break;
                    case Model.FlowRunModel.Execute.Type.CopyforCompleted:
                        CopyForCompleted(flowRunModel, executeModel, localizer);
                        break;
                    case Model.FlowRunModel.Execute.Type.TaskEnd:
                        TaskEnd(flowRunModel, executeModel, localizer);
                        break;
                    case Model.FlowRunModel.Execute.Type.AddWrite:
                        AddWrite(flowRunModel, executeModel, localizer);
                        break;
                }
                int i = Update(removeTasks, updateTasks, addTasks, executeSqls);
                if (executeResult.DebugMessages.IsNullOrEmpty())
                {
                    executeResult.DebugMessages = (localizer == null ? "执行成功，影响的行数：" : localizer["Execute_ExecuteSuccess"]) + i.ToString();
                }
            }


            if (executeResult.Messages.IsNullOrWhiteSpace())
            {
                executeResult.Messages = GetExecuteMessage(executeResult, executeModel, currentStep, localizer);
            }
            if (executeModel.ExecuteType != Model.FlowRunModel.Execute.Type.Save)
            {
                //发送消息通知
                //SendMessage(executeResult.NextTasks, executeModel.Sender, "0,1,2,3", localizer: localizer);
            }
            return executeResult;
        }

        /// <summary>
        /// 提交任务
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="localizer">语言包</param>
        /// <param name="executeModel">执行参数实体</param>
        public void Submit(Model.FlowRun flowRunModel, Model.FlowRunModel.Execute executeModel, IStringLocalizer localizer = null)
        {
            Model.FlowTask currentTask;
            List<Model.FlowTask> groupTasks;
            //如果是第一步提交并且没有实例则先创建实例
            bool isFirstTask = executeModel.StepId == flowRunModel.FirstStepId && executeModel.TaskId.IsEmptyGuid();
            if (isFirstTask)
            {
                if (executeModel.Title.IsNullOrWhiteSpace())
                {
                    executeModel.Title = flowRunModel.Name + "-" + GetStepName(flowRunModel, executeModel.StepId);
                }
                currentTask = GetFirstTask(flowRunModel, executeModel);
                groupTasks = new List<Model.FlowTask>
                {
                    currentTask
                };
                executeModel.TaskId = currentTask.Id;
                executeModel.GroupId = currentTask.GroupId;
                //添加动态流程步骤JSON
                if (Config.EnableDynamicStep && executeModel.Steps.Exists(p => p.beforeStepId.IsNotEmptyGuid()))
                {
                    var newFlowRunModel = new FlowDynamic().Add(executeModel, groupTasks);
                    if (null != newFlowRunModel)
                    {
                        flowRunModel = newFlowRunModel;
                    }
                    executeModel.Steps.RemoveAll(p => p.parallelOrSerial.HasValue && p.parallelOrSerial == 1 && p.stepId != p.beforeStepId);
                }
                //=================
            }
            else
            {
                groupTasks = GetListByGroupId(executeModel.GroupId);
                currentTask = groupTasks.Find(p => p.Id == executeModel.TaskId);
            }
            executeResult.CurrentTask = currentTask;

            #region 判断是否能处理
            if (null == currentTask)
            {
                executeResult.DebugMessages = localizer == null ? "当前任务为空!" : localizer["Execute_SubmitTaskEmpty"];
                executeResult.IsSuccess = false;
                executeResult.Messages = localizer == null ? "当前任务为空!" : localizer["Execute_SubmitTaskEmpty"];
                return;
            }
            else if (currentTask.Status.In(-1, 2))
            {
                executeResult.DebugMessages = (localizer == null ? "当前任务" : localizer["Execute_SubmitCurrentTask"]) + (currentTask.Status == -1 ? (localizer == null ? "等待中" : localizer["Execute_SubmitCurrentTaskWait"]) : (localizer == null ? "已处理" : localizer["Execute_SubmitCurrentTaskProcessed"])) + "!";
                executeResult.IsSuccess = false;
                executeResult.Messages = executeResult.DebugMessages;
                return;
            }
            if (currentTask.ReceiveId != executeModel.Sender.Id && currentTask.IsAutoSubmit == 0)
            {
                executeResult.DebugMessages = localizer == null ? "当前任务接收人不属于当前用户!" : localizer["Execute_SubmitCurrentTaskReceiveUserError"];
                executeResult.IsSuccess = false;
                executeResult.Messages = localizer == null ? "您不能处理当前任务!" : localizer["Execute_SubmitCurrentTaskCanotProcess"];
                return;
            }
            var currentStep = flowRunModel.Steps.Find(p => p.Id == currentTask.StepId);
            if (null == currentStep)
            {
                executeResult.DebugMessages = localizer == null ? "未找到当前步骤运行时实体!" : localizer["NotFoundStepModel"];
                executeResult.IsSuccess = false;
                executeResult.Messages = localizer == null ? "未找到当前步骤运行时实体!" : localizer["NotFoundStepModel"];
                return;
            }
            #endregion

            if (executeModel.Title.IsNullOrWhiteSpace())
            {
                executeModel.Title = currentTask.Title;
            }

            if (currentTask.InstanceId.IsNullOrWhiteSpace() && !executeModel.InstanceId.IsNullOrWhiteSpace())
            {
                currentTask.InstanceId = executeModel.InstanceId;
            }

            if (currentTask.TaskType == 6)//处理前加签
            {
                BeforAddWrite(flowRunModel, executeModel, currentTask);
                return;
            }

            if (currentTask.OtherType == 1)//如果是子流程任务，这里要赋值，后续任务也要设置为1
            {
                executeModel.OtherType = 1;
            }

            bool currentStepIsPass = true;//当前步骤是否通过

            #region 如果当前任务是子流程任务并且是子流程完成才能提交，要判断子流程是否完成
            if (!currentTask.SubFlowGroupId.IsNullOrWhiteSpace() && currentStep.StepSubFlow.SubFlowStrategy != 1)
            {
                string[] subFlowGroupIds = currentTask.SubFlowGroupId.Split(',');
                foreach (string subId in subFlowGroupIds)
                {
                    if (subId.IsGuid(out Guid sid))
                    {
                        if (GetListByGroupId(sid).Exists(p => p.Status < 2 && p.TaskType != 5))
                        {
                            executeResult.DebugMessages = localizer == null ? "当前步骤对应的子流程还未完成，不能提交!" : localizer["Execute_SubmitSubflowUncompleted"];
                            executeResult.IsSuccess = false;
                            executeResult.Messages = localizer == null ? "当前步骤对应的子流程还未完成，不能提交!" : localizer["Execute_SubmitSubflowUncompleted"];
                            return;
                        }
                    }
                }
            }
            #endregion

            #region 处理策略判断
            int maxSort = groupTasks.FindAll(p => p.StepId == currentTask.StepId).Max(p => p.Sort);
            var levelTasks = groupTasks.FindAll(p => p.StepId == currentTask.StepId && p.Sort == maxSort && p.TaskType!=5);//当前任务的同级任务
            if (levelTasks.Count > 0)
            {
                int hanlderModel = currentStep.StepBase.HanlderModel;
                //如果是后加签策略要从加签中取
                if (currentTask.TaskType == 7)
                {
                    char[] otherType = currentTask.OtherType.ToString().ToCharArray();
                    if (otherType.Length == 3)
                    {
                        int addExecuteType = otherType[2].ToString().ToInt();//处理类型 1所有人同意，2一个同意 3顺序处理
                        if (addExecuteType == 1)
                        {
                            hanlderModel = 0;
                        }
                        else if (addExecuteType == 2)
                        {
                            hanlderModel = 1;
                        }
                        else if (addExecuteType == 3)
                        {
                            hanlderModel = 4;
                        }
                    }
                }
                switch (hanlderModel)
                {
                    case 0://所有人必须处理
                        currentStepIsPass = !levelTasks.Exists(p => p.Status != 2 && p.Id != currentTask.Id);
                        break;
                    case 1://一人同意即可
                        foreach (var task in levelTasks)//将当前任务除外的其它任务标记为他人已处理
                        {
                            if (task.Id == currentTask.Id || task.Status == 2)
                            {
                                continue;
                            }
                            var updateTask = task.Clone();
                            updateTask.ExecuteType = 4;
                            updateTask.Status = 2;
                            updateTask.CompletedTime1 = DateExtensions.Now;
                            updateTasks.Add(updateTask);
                        }
                        break;
                    case 2://依据人数比例
                        decimal percentage = currentStep.StepBase.Percentage;
                        currentStepIsPass = (levelTasks.Count(p => p.Status == 2) + 1) / levelTasks.Count * 100 >= percentage;
                        if (currentStepIsPass)
                        {
                            foreach (var task in levelTasks.FindAll(p => p.Status != 2 && p.Id != currentTask.Id))
                            {
                                var updateTask = task.Clone();
                                updateTask.ExecuteType = 4;
                                updateTask.Status = 2;
                                updateTask.CompletedTime1 = DateExtensions.Now;
                                updateTasks.Add(updateTask);
                            }
                        }
                        break;
                    case 3://独立处理

                        break;
                    case 4://按钮选择人员顺序处理
                        //将当前任务的下一个任务标记为待处理
                        var nextTask = levelTasks.FindAll(p => p.StepSort > currentTask.StepSort).OrderBy(p => p.StepSort);
                        if (nextTask.Count() > 0)
                        {
                            var updateTask = nextTask.First().Clone();
                            updateTask.ExecuteType = 0;
                            updateTask.Status = 0;
                            updateTasks.Add(updateTask);
                            currentStepIsPass = false;
                            executeResult.NextTasks.Add(updateTask);
                            executeResult.IsSuccess = true;
                        }
                        break;
                }
            }
            #endregion

            #region 完成当前任务
            var updateCurrentTask = currentTask.Clone();
            updateCurrentTask.ExecuteType = 2;
            updateCurrentTask.Comments = executeModel.Comment;
            updateCurrentTask.CompletedTime1 = DateExtensions.Now;
            updateCurrentTask.IsSign = executeModel.IsSign;
            updateCurrentTask.Status = 2;
            updateCurrentTask.Attachment = executeModel.Attachment;
            if (isFirstTask)
            {
                addTasks.Add(updateCurrentTask);
            }
            else
            {
                updateTasks.Add(updateCurrentTask);
            }
            #endregion

            #region 如果当前步骤通过，则添加下一步待办
            if (currentStepIsPass)
            {
                #region 完成当前步骤等待中的任务
                var waitTasks = levelTasks.FindAll(p => p.Status == -1);
                foreach (var waitTask in waitTasks)
                {
                    if (currentStep.StepBase.HanlderModel == 4)//如果是按顺序人员处理，则不能删除，因为下次退回的时候才能找到是哪些人。
                    {
                        var wTask = waitTask.Clone();
                        wTask.ExecuteType = 4;
                        wTask.Status = 2;
                        wTask.CompletedTime1 = DateExtensions.Now;
                        updateTasks.Add(wTask);
                    }
                    else
                    {
                        removeTasks.Add(waitTask);
                    }
                }
                #endregion

                foreach (var (stepId, stepName, beforeStepId, parallelorserial, receiveUsers, completedTime) in executeModel.Steps)
                {
                    var nextStpe = flowRunModel.Steps.Find(p => p.Id == stepId);
                    if (null == nextStpe)
                    {
                        continue;
                    }
                    DateTime? completedTime1 = completedTime;
                    DateTime? remindTime = new DateTime?();//提醒时间(如果任务设置了超期提示)
                    WorkDate workDate = new WorkDate();
                    if (!completedTime1.HasValue && nextStpe.WorkTime > 0)
                    {
                        completedTime1 = workDate.GetWorkDateTime((double)nextStpe.WorkTime);
                    }
                    if (completedTime1.HasValue && nextStpe.ExpiredPrompt > 0)
                    {
                        remindTime = workDate.GetWorkDateTime((double)(nextStpe.ExpiredPromptDays - nextStpe.ExpiredPromptDays * 2), completedTime1);
                    }

                    #region 如果下一步有步骤会签策略，则要判断步骤是否通过
                    bool stepPass = true;//步骤是否通过
                    int countersignature = nextStpe.StepBase.Countersignature;
                    if (countersignature != 0)
                    {
                        stepPass = false;
                        var prevSteps = new Flow().GetPrevSteps(flowRunModel, nextStpe.Id);
                        switch (countersignature)
                        {
                            case 1://所有步骤同意
                                bool allPass = true;
                                foreach (var prevStep in prevSteps)
                                {
                                    if (prevStep.Id == currentStep.Id)
                                    {
                                        continue;
                                    }
                                    if (!StepIsPass(flowRunModel, prevStep, nextStpe, groupTasks))
                                    {
                                        allPass = false;
                                        break;
                                    }
                                }
                                stepPass = allPass;
                                break;
                            case 2://一个步骤同意即可
                                stepPass = true;
                                foreach (var prevStep in prevSteps)
                                {
                                    if (prevStep.Id == currentStep.Id)
                                    {
                                        continue;
                                    }
                                    //将同级步骤的其它步骤任务标识为他人已处理
                                    var stepTasks = groupTasks.FindAll(p => p.Sort == currentTask.Sort && p.TaskType != 5);
                                    foreach (var stepTask in stepTasks)
                                    {
                                        if (stepTask.Status == 2 || stepTask.Id == currentTask.Id || updateTasks.Exists(p => p.Id == stepTask.Id))
                                        {
                                            continue;
                                        }
                                        var stepTask1 = stepTask.Clone();
                                        stepTask1.Status = 2;
                                        stepTask1.ExecuteType = 4;
                                        stepTask1.CompletedTime1 = DateExtensions.Now;
                                        updateTasks.Add(stepTask1);
                                    }
                                }
                                break;
                            case 3://步骤比例
                                int stepPassCount = 1;
                                foreach (var prevStep in prevSteps)
                                {
                                    if (StepIsPass(flowRunModel, prevStep, nextStpe, groupTasks))
                                    {
                                        stepPassCount++;
                                        if ((stepPassCount / prevSteps.Count) * 100 >= nextStpe.StepBase.CountersignaturePercentage)
                                        {
                                            stepPass = true;
                                            break;
                                        }
                                    }
                                }
                                if (stepPass)
                                {
                                    //将同级步骤的其它步骤任务标识为他人已处理
                                    var stepTasks = groupTasks.FindAll(p => p.Sort == currentTask.Sort && p.TaskType != 5);
                                    foreach (var stepTask in stepTasks)
                                    {
                                        if (stepTask.Status == 2 || stepTask.Id == currentTask.Id || updateTasks.Exists(p => p.Id == stepTask.Id))
                                        {
                                            continue;
                                        }
                                        var stepTask1 = stepTask.Clone();
                                        stepTask1.Status = 2;
                                        stepTask1.ExecuteType = 4;
                                        stepTask1.CompletedTime1 = DateExtensions.Now;
                                        updateTasks.Add(stepTask1);
                                    }
                                }
                                break;
                        }
                    }
                    if (!stepPass)
                    {
                        executeResult.Messages = localizer == null ? "已发送，等待其他步骤处理!" : localizer["Execute_SubmitSendWaitOtherStep"];
                        continue;
                    }
                    #endregion

                    #region 如果下一步是子流程步骤，则要发起子流程实例
                    if (nextStpe.Type == 1)
                    {
                        Guid subFlowId = nextStpe.StepSubFlow.SubFlowId;
                        var subFlowRunModel = new Flow().GetFlowRunModel(subFlowId);
                        if (null == subFlowRunModel)
                        {
                            continue;
                        }
                        string subFlowName = subFlowRunModel.Name;
                        var subFlowFirstStep = subFlowRunModel.Steps.Find(p => p.Id == subFlowRunModel.FirstStepId);
                        if (null == subFlowFirstStep)
                        {
                            continue;
                        }
                        string firstStepName = subFlowFirstStep.Name;
                        string subFlowActiveEvent = nextStpe.StepEvent.SubFlowActivationBefore;
                        int subFlowInstanceType = nextStpe.StepSubFlow.TaskType;
                        string subInstanceId = string.Empty;//子流程实例ID
                        string subTaskTitle = string.Empty;//子流程任务标题

                        #region 执行子流程激活前事件
                        if (!subFlowActiveEvent.IsNullOrWhiteSpace())
                        {
                            //组织事件参数
                            Model.FlowRunModel.EventParam eventParam = new Model.FlowRunModel.EventParam
                            {
                                FlowId = currentTask.FlowId,
                                GroupId = currentTask.GroupId,
                                InstanceId = currentTask.InstanceId,
                                StepId = currentTask.StepId,
                                TaskId = currentTask.Id,
                                TaskTitle = currentTask.Title
                            };
                            var (obj, err) = Tools.ExecuteMethod(subFlowActiveEvent, eventParam);
                            if (null != obj)
                            {
                                //事件返回包含两个字段的JSON字符串 instanceId(子流程业务表主键值) title(子流程任务标题)
                                try
                                {
                                    JObject jObject = JObject.Parse(obj.ToString());
                                    subInstanceId = jObject.Value<string>("instanceId");
                                    subTaskTitle = jObject.Value<string>("title");
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region 添加子流程第一步待办
                        int stepSort1 = 1;
                        Guid subGroupId = Guid.NewGuid();
                        StringBuilder subGropuIdBuilder = new StringBuilder();//记录子流程GROUPID, 关联主任务的SubFlowGroupId字段
                        foreach (var receiveUser in receiveUsers)
                        {
                            Guid groupid = subFlowInstanceType == 0 ? subGroupId : Guid.NewGuid();
                            if (subFlowInstanceType == 1)
                            {
                                subGropuIdBuilder.Append(groupid);
                                subGropuIdBuilder.Append(",");
                            }
                            Model.FlowTask task = new Model.FlowTask
                            {
                                ExecuteType = stepSort1 == 1 ? 0 : -1,
                                FlowId = subFlowId,
                                FlowName = subFlowName,
                                GroupId = groupid,
                                Id = Guid.NewGuid(),
                                InstanceId = subInstanceId,
                                IsSign = 0,
                                PrevId = Guid.Empty,
                                PrevStepId = Guid.Empty,
                                ReceiveId = receiveUser.Id,
                                ReceiveName = receiveUser.Name,
                                ReceiveTime = DateExtensions.Now,
                                SenderId = executeModel.Sender.Id,
                                SenderName = executeModel.Sender.Name,
                                Sort = 1,
                                Status = stepSort1 == 1 ? 0 : -1,
                                StepId = subFlowRunModel.FirstStepId,
                                StepName = firstStepName,
                                StepSort = nextStpe.StepBase.HanlderModel == 4 ? stepSort1++ : stepSort1,
                                TaskType = 0,
                                Title = subTaskTitle,
                                IsAutoSubmit = subFlowFirstStep.ExpiredExecuteModel,
                                CompletedTime = completedTime1,
                                OtherType = 1 //1表示是子流程
                            };
                            if (receiveUser.PartTimeId.HasValue)
                            {
                                task.ReceiveOrganizeId = receiveUser.PartTimeId;
                            }
                            if (receiveUser.Note.IsGuid(out Guid entuustId))//用人员实体的NOTE字段来存放委托人ID
                            {
                                task.EntrustUserId = entuustId;
                                task.Note = localizer == null ? "委托" : localizer["Execute_SubmitTaskNoteEntrust"];
                            }
                            addTasks.Add(task);
                            //executeResult.NextTasks.Add(task);
                        }
                        #endregion

                        #region 给当前处理者增加待办
                        var addTask = currentTask.Clone();
                        addTask.SenderId = currentTask.ReceiveId;
                        addTask.SenderName = currentTask.ReceiveName;
                        addTask.Status = 0;
                        addTask.ExecuteType = 0;
                        addTask.Comments = "";
                        addTask.IsSign = 0;
                        addTask.Sort = currentTask.Sort + 1;
                        addTask.OpenTime = null;
                        addTask.CompletedTime1 = null;
                        addTask.CompletedTime = completedTime1;
                        addTask.Id = Guid.NewGuid();
                        addTask.PrevId = currentTask.Id;
                        addTask.PrevStepId = currentTask.StepId;
                        addTask.Title = executeModel.Title;
                        addTask.ReceiveTime = DateExtensions.Now;
                        addTask.IsAutoSubmit = nextStpe.ExpiredExecuteModel;
                        addTask.StepId = nextStpe.Id;
                        addTask.StepName = nextStpe.Name;
                        addTask.SubFlowGroupId = subGropuIdBuilder.Length == 0 ? subGroupId.ToString() : subGropuIdBuilder.ToString().TrimEnd(',');
                        addTasks.Add(addTask);
                        executeResult.NextTasks.Add(addTask);
                        #endregion

                        continue;
                    }
                    #endregion

                    int stepSort = 1;
                    foreach (var receiveUser in receiveUsers)
                    {
                        //判断下一步待办中没有当前接收人的待办才添加，条件currentStep.StepBase.HanlderModel != 3为独立处理不判断
                        if (currentStep.StepBase.HanlderModel != 3 && (addTasks.Exists(p => p.ReceiveId == receiveUser.Id && p.StepId == nextStpe.Id) ||
                            groupTasks.Exists(p => p.ReceiveId == receiveUser.Id && p.StepId == nextStpe.Id && p.Status != 2)))
                        {
                            continue;
                        }
                        Model.FlowTask task = new Model.FlowTask
                        {
                            ExecuteType = stepSort == 1 ? 0 : -1,
                            FlowId = executeModel.FlowId,
                            FlowName = flowRunModel.Name,
                            GroupId = currentTask.GroupId,
                            Id = Guid.NewGuid(),
                            InstanceId = executeModel.InstanceId,
                            IsSign = 0,
                            OtherType = executeModel.OtherType,
                            PrevId = currentTask.Id,
                            PrevStepId = currentTask.StepId,
                            ReceiveId = receiveUser.Id,
                            ReceiveName = receiveUser.Name,
                            ReceiveTime = DateExtensions.Now,
                            SenderId = executeModel.Sender.Id,
                            SenderName = executeModel.Sender.Name,
                            Sort = currentTask.Sort + 1,
                            Status = stepSort == 1 ? 0 : -1,
                            StepId = stepId,
                            StepName = stepName.IsNullOrWhiteSpace() ? nextStpe.Name : stepName.Trim(),
                            StepSort = nextStpe.StepBase.HanlderModel == 4 ? stepSort++ : stepSort,
                            TaskType = 0,
                            Title = executeModel.Title,
                            IsAutoSubmit = nextStpe.ExpiredExecuteModel,
                            CompletedTime = completedTime1,
                            RemindTime = remindTime
                        };
                        if (receiveUser.PartTimeId.IsNotEmptyGuid())
                        {
                            task.ReceiveOrganizeId = receiveUser.PartTimeId;
                        }
                        if (receiveUser.Note.IsGuid(out Guid entuustId) && entuustId.IsNotEmptyGuid())//用人员实体的NOTE字段来存放委托人ID
                        {
                            task.EntrustUserId = entuustId;
                            //task.TaskType = 2;
                            task.Note = localizer == null ? "委托" : localizer["Execute_SubmitTaskNoteEntrust"];
                        }
                        if(nextStpe.DataEditModel == 1)//独立编辑这里不继承上一步实例ID
                        {
                            if (currentTask.TaskType == 4)//如果是退回又提交要查找退回前的实例ID
                            {
                                var backTask = groupTasks.Find(p => p.StepId == nextStpe.Id && p.ReceiveId == receiveUser.Id);
                                task.InstanceId = backTask?.InstanceId;
                            }
                            else
                            {
                                task.InstanceId = null;
                            }
                        }
                        if(beforeStepId.IsNotEmptyGuid())
                        {
                            task.BeforeStepId = beforeStepId.Value;
                        }
                        addTasks.Add(task);
                        executeResult.NextTasks.Add(task);
                    }

                    #region 步骤接收时抄送
                    if (nextStpe.StepCopyFor.CopyforTime == 0)
                    {
                        var copyForUsers = GetCopyForUsers(nextStpe, flowRunModel, executeModel, groupTasks);
                        foreach (var copyForUser in copyForUsers)
                        {
                            //判断下一步待办中没有当前接收人的待办才添加
                            if (addTasks.Exists(p => p.ReceiveId == copyForUser.Id && p.StepId == nextStpe.Id) ||
                                groupTasks.Exists(p => p.ReceiveId == copyForUser.Id && p.StepId == nextStpe.Id && p.Sort == currentTask.Sort + 1))
                            {
                                continue;
                            }
                            Model.FlowTask task = new Model.FlowTask
                            {
                                ExecuteType = 0,
                                FlowId = executeModel.FlowId,
                                FlowName = flowRunModel.Name,
                                GroupId = currentTask.GroupId,
                                Id = Guid.NewGuid(),
                                InstanceId = executeModel.InstanceId,
                                IsSign = 0,
                                Note = localizer == null ? "抄送" : localizer["Execute_SubmitTaskNoteCopyfor"],
                                OtherType = executeModel.OtherType,
                                PrevId = currentTask.Id,
                                PrevStepId = currentTask.StepId,
                                ReceiveId = copyForUser.Id,
                                ReceiveName = copyForUser.Name,
                                ReceiveTime = DateExtensions.Now,
                                SenderId = executeModel.Sender.Id,
                                SenderName = executeModel.Sender.Name,
                                Sort = currentTask.Sort + 1,
                                Status = 0,
                                StepId = stepId,
                                StepName = nextStpe.Name,
                                StepSort = 1,
                                TaskType = 5,
                                Title = executeModel.Title,
                                IsAutoSubmit = 0
                            };
                            addTasks.Add(task);
                            executeResult.NextTasks.Add(task);
                        }
                    }
                    #endregion
                }

                #region 步骤完成时抄送
                if(currentStep.StepCopyFor.CopyforTime == 1)
                {
                    var copyForUsers = GetCopyForUsers(currentStep, flowRunModel, executeModel, groupTasks);
                    foreach (var copyForUser in copyForUsers)
                    {
                        //判断当前步骤待办中没有当前接收人的待办才添加
                        if (groupTasks.Exists(p => p.ReceiveId == copyForUser.Id && p.StepId == currentStep.Id))
                        {
                            continue;
                        }
                        Model.FlowTask task = new Model.FlowTask
                        {
                            ExecuteType = 0,
                            FlowId = executeModel.FlowId,
                            FlowName = flowRunModel.Name,
                            GroupId = currentTask.GroupId,
                            Id = Guid.NewGuid(),
                            InstanceId = executeModel.InstanceId,
                            IsSign = 0,
                            Note = localizer == null ? "抄送" : localizer["Execute_SubmitTaskNoteCopyfor"],
                            OtherType = executeModel.OtherType,
                            PrevId = currentTask.PrevId,
                            PrevStepId = currentTask.PrevStepId,
                            ReceiveId = copyForUser.Id,
                            ReceiveName = copyForUser.Name,
                            ReceiveTime = DateExtensions.Now,
                            SenderId = executeModel.Sender.Id,
                            SenderName = executeModel.Sender.Name,
                            Sort = currentTask.Sort,
                            Status = 0,
                            StepId = currentStep.Id,
                            StepName = currentStep.Name,
                            StepSort = 1,
                            TaskType = 5,
                            Title = executeModel.Title,
                            IsAutoSubmit = 0
                        };
                        addTasks.Add(task);
                        //executeResult.NextTasks.Add(task);
                    }
                }
                #endregion
            }
            executeResult.IsSuccess = true;
            #endregion

            #region 判断整个流程是否完成
            bool flowInstanceIsCompleted = currentStepIsPass && addTasks.Count == 0;//当前步骤通过，并且没有新增的任务
            if (flowInstanceIsCompleted)
            {
                foreach (var task in groupTasks)
                {
                    if (task.Status != 2 && task.TaskType != 5)
                    {
                        var upTask = updateTasks.Find(p => p.Id == task.Id);
                        if (upTask == null || upTask.Status != 2)
                        {
                            var delTask = removeTasks.Find(p => p.Id == task.Id);
                            if (delTask == null)
                            {
                                flowInstanceIsCompleted = false;
                                break;
                            }
                        }
                    }
                }
            }

            #region 如果是子流程任务,检查主流程是否需要自动提交
            if (flowInstanceIsCompleted)
            {
                var firstTask = groupTasks.Find(p => p.PrevId == Guid.Empty && p.PrevStepId == Guid.Empty && p.StepId == flowRunModel.FirstStepId);
                if (null != firstTask && firstTask.OtherType == 1)
                {
                    var mainTasks = GetListBySubFlowGroupId(firstTask.GroupId);
                    if (mainTasks.Count > 0)
                    {
                        var mainFlowRunModel = new Flow().GetFlowRunModel(mainTasks.First().FlowId);
                        if (null != mainFlowRunModel)
                        {
                            var mainStepModel = mainFlowRunModel.Steps.Find(p => p.Id == mainTasks.First().StepId);
                            //如果主流程步骤设置为子流程完成后自动提交，则在这里提交主流程步骤
                            if (null != mainStepModel && mainStepModel.StepSubFlow.SubFlowStrategy == 2)
                            {
                                executeResult.AutoSubmitTasks.AddRange(mainTasks);
                            }
                            //执行子流程完成后事件
                            if (null != mainStepModel && !mainStepModel.StepEvent.SubFlowCompletedBefore.IsNullOrWhiteSpace())
                            {
                                Model.FlowRunModel.EventParam subFlowEeventParam = new Model.FlowRunModel.EventParam();
                                subFlowEeventParam.FlowId = mainTasks.First().FlowId;
                                subFlowEeventParam.GroupId = mainTasks.First().GroupId;
                                subFlowEeventParam.InstanceId = mainTasks.First().InstanceId;
                                subFlowEeventParam.StepId = mainTasks.First().StepId;
                                subFlowEeventParam.TaskId = mainTasks.First().Id;
                                subFlowEeventParam.TaskTitle = mainTasks.First().Title;
                                var (eventReturn, err) = Tools.ExecuteMethod(mainStepModel.StepEvent.SubFlowCompletedBefore.Trim(), subFlowEeventParam);
                                if (null != err)
                                {
                                    Log.Add((localizer == null ? "执行子流程完成后事件发生了错误-" : localizer["Execute_EventSubflowCompletedErrorLog"]) + mainFlowRunModel.Name + "-" + mainStepModel.Name + "-" + mainStepModel.StepEvent.SubFlowCompletedBefore, (localizer == null ? "参数：" : localizer["Execute_EventStepBeforeErrorParam"]) + subFlowEeventParam.ToString() + (localizer == null ? " 错误：" : localizer["Execute_EventStepBeforeErrorError"]) + err.Message + err.StackTrace, Log.Type.流程运行);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region 更新完成标识
            if (flowInstanceIsCompleted)
            {
                Guid connId = flowRunModel.TitleField.ConnectionId;
                string titleField = flowRunModel.TitleField.Field;
                string table = flowRunModel.TitleField.Table;
                string value = flowRunModel.TitleField.Value;
                var connModel = new DbConnection().Get(connId);
                if (null != connModel && !titleField.IsNullOrWhiteSpace() && !table.IsNullOrWhiteSpace())
                {
                    //这里检查一下，如果标识字段设置成了主键则不更新
                    var dbFirst = flowRunModel.Databases.Any() ? flowRunModel.Databases.First() : null;
                    string table1 = string.Empty, primaryKey = string.Empty;
                    if (dbFirst != null)
                    {
                        table1 = dbFirst.Table;
                        primaryKey = dbFirst.PrimaryKey;
                    }
                    if (table1.EqualsIgnoreCase(table) && !primaryKey.EqualsIgnoreCase(titleField))
                    {
                        string sql = "UPDATE " + table + " SET " + titleField + "={0} WHERE " + primaryKey + "={1}";
                        object[] parameteres = new object[] { value.IsNullOrEmpty() ? "1" : value, currentTask.InstanceId };
                        if (connModel.ConnString.EqualsIgnoreCase(Config.ConnectionString))//如果是使用的系统连接，则采用事务更新
                        {
                            executeSqls.Add((sql, parameteres, 1));
                        }
                        else
                        {
                            using (var db = new Mapper.DataContext(connModel.ConnType, connModel.ConnString))
                            {
                                db.Execute(sql, parameteres);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                //移出动态步骤缓存(表rf_flowdynamic的缓存)
                foreach(var dynamicStep in groupTasks.Where(p => p.BeforeStepId.IsNotEmptyGuid()))
                {
                    Cache.IO.Remove(Flow.CACHEKEY + "_" + dynamicStep.BeforeStepId.Value.ToNString() + "_" + dynamicStep.GroupId.ToNString());
                }
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 判断一个步骤是否通过
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="step"></param>
        /// <param name="countersignatureStep">会签步骤</param>
        /// <param name="groupTasks"></param>
        /// <returns></returns>
        private bool StepIsPass(Model.FlowRun flowRunModel, Model.FlowRunModel.Step step, Model.FlowRunModel.Step countersignatureStep, List<Model.FlowTask> groupTasks)
        {
            var stepTaskList = groupTasks.FindAll(p => p.StepId == step.Id && p.TaskType != 5);
            var maxSort = stepTaskList.Count == 0 ? -1 : stepTaskList.Max(p => p.Sort);
            var stepTasks = -1 == maxSort ? new List<Model.FlowTask>() : stepTaskList.FindAll(p => p.Sort == maxSort);
            if (stepTasks.Count == 0)
            {
                //这里要判断会签步骤的前一步到会签发起步骤之间有多步的情况
                if (countersignatureStep.StepBase.CountersignatureStartStepId.HasValue)
                {
                    var rangeSteps = new Flow().GetRangeSteps(flowRunModel, countersignatureStep.StepBase.CountersignatureStartStepId.Value, step.Id);
                    foreach (var rangeStep in rangeSteps)
                    {
                        if (groupTasks.Exists(p => p.StepId == rangeStep.Id && p.TaskType != 5))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                //如果没有设置起点步骤，则只判断会签的前一步，如果没有任务则算不通过
                else
                {
                    return false;
                }
            }
            bool isPass = true;
            switch (step.StepBase.HanlderModel)
            {
                case 0://所有人同意
                    isPass = !stepTasks.Exists(p => p.Status != 2);
                    break;
                case 1://一人同意即可
                    isPass = stepTasks.Exists(p => p.ExecuteType == 2);
                    break;
                case 2://依据人数比例
                    isPass = stepTasks.Count(p => p.Status == 2) / stepTasks.Count * 100 >= step.StepBase.Percentage;
                    break;
                case 3://独立处理
                    isPass = !stepTasks.Exists(p => p.Status != 2);
                    break;
                case 4://顺序处理
                    isPass = !stepTasks.Exists(p => p.Status != 2);
                    break;
            }
            return isPass;
        }

        /// <summary>
        /// 退回任务
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        /// <param name="localizer">语言包</param>
        public void Back(Model.FlowRun flowRunModel, Model.FlowRunModel.Execute executeModel, IStringLocalizer localizer = null)
        {
            List<Model.FlowTask> groupTasks = GetListByGroupId(executeModel.GroupId);
            Model.FlowTask currentTask = groupTasks.Find(p => p.Id == executeModel.TaskId);
            if (null == currentTask)
            {
                executeResult.DebugMessages = localizer == null ? "当前任务为空" : localizer["Execute_SubmitTaskEmpty"];
                executeResult.IsSuccess = false;
                executeResult.Messages = localizer == null ? "当前任务为空" : localizer["Execute_SubmitTaskEmpty"];
                return;
            }
            else if (currentTask.Status.In(-1, 2))
            {
                executeResult.DebugMessages = (localizer == null ? "当前任务" : localizer["Execute_SubmitCurrentTask"]) + (currentTask.Status == -1 ? (localizer == null ? "等待中" : localizer["Execute_SubmitCurrentTaskWait"]) : (localizer == null ? "已处理" : localizer["Execute_SubmitCurrentTaskProcessed"])) + "!";
                executeResult.IsSuccess = false;
                executeResult.Messages = executeResult.DebugMessages;
                return;
            }
            if (currentTask.ReceiveId != executeModel.Sender.Id)
            {
                executeResult.DebugMessages = localizer == null ? "当前任务接收人不属于当前用户" : localizer["Execute_SubmitCurrentTaskReceiveUserError"];
                executeResult.IsSuccess = false;
                executeResult.Messages = localizer == null ? "您不能处理当前任务" : localizer["Execute_SubmitCurrentTaskCanotProcess"];
                return;
            }
            var currentStep = flowRunModel.Steps.Find(p => p.Id == currentTask.StepId);
            if (null == currentStep)
            {
                executeResult.DebugMessages = localizer == null ? "未找到当前步骤运行时" : localizer["Execute_NotFoundStepRunModel"];
                executeResult.IsSuccess = false;
                executeResult.Messages = localizer == null ? "未找到当前步骤运行时" : localizer["Execute_NotFoundStepRunModel"];
                return;
            }
            executeResult.CurrentTask = currentTask;
            if (executeModel.Title.IsNullOrWhiteSpace())
            {
                executeModel.Title = currentTask.Title;
            }
            if (currentTask.OtherType == 1)//如果是子流程任务，这里要赋值，后续任务也要设置为1
            {
                executeModel.OtherType = 1;
            }

            bool isBackPass = true;//步骤退回是否通过
            int backModel = currentStep.StepBase.BackModel;
            int maxSort = groupTasks.FindAll(p => p.StepId == currentTask.StepId).Max(p => p.Sort);
            var levelTasks = groupTasks.FindAll(p => p.StepId == currentTask.StepId && p.Sort == maxSort && p.TaskType != 5);//当前任务的同级任务

            if (backModel == 1)
            {
                int handlerType = currentStep.StepBase.HanlderModel;
                switch (handlerType)
                {
                    case 0://所有人必须处理
                        backModel = 2;//所有人必须同意，只要有一个人退回就表示不同意，相当于一人退回全部退回
                        break;
                    case 1://一人同意即可
                        backModel = 3;//一人同意即可，则要所有人都退回才退回，相当于所有人退回才退回
                        break;
                    case 2://依据人数比例
                        backModel = 5;
                        break;
                    case 3://独立处理
                        backModel = 4;//独立处理相当于独立退回
                        break;
                    case 4://选择人员顺序处理
                        backModel = 6;
                        break;
                }
            }

            //加签退回要从加签时选择中读取策略
            if (currentTask.TaskType.In(6, 7))
            {
                char[] otherType = currentTask.OtherType.ToString().ToCharArray();
                if (otherType.Length == 3)
                {
                    int executeType = otherType[2].ToString().ToInt(); ;//处理类型 1所有人同意，2一个同意 3顺序处理
                    if (executeType == 1)
                    {
                        backModel = 2;
                    }
                    else if (executeType == 2)
                    {
                        backModel = 3;
                    }
                    else if (executeType == 3)
                    {
                        backModel = 2;
                        //backModel = 6;
                    }
                }
            }

            switch (backModel)
            {
                case 0:
                    executeResult.DebugMessages = localizer == null ? "当前步骤设置为不能退回" : localizer["Execute_BackCannotBack"];
                    executeResult.IsSuccess = false;
                    executeResult.Messages = localizer == null ? "当前步骤设置为不能退回" : localizer["Execute_BackCannotBack"];
                    return;
                case 1://根据处理策略退回
                    
                    break;
                case 2://一人退回全部退回
                    foreach (var task in levelTasks)
                    {
                        if (task.Id == currentTask.Id || task.Status == 2)
                        {
                            continue;
                        }
                        var updateTask = task.Clone();
                        updateTask.ExecuteType = 5;
                        updateTask.Status = 2;
                        updateTask.CompletedTime1 = DateExtensions.Now;
                        updateTasks.Add(updateTask);
                    }
                    break;
                case 3://所有人退回才退回
                    isBackPass = !levelTasks.Exists(p => p.Status != 2 && p.Id != currentTask.Id);
                    break;
                case 4://独立退回

                    break;
                case 5://依据人数比例退回
                    decimal percentage = currentStep.StepBase.Percentage;
                    isBackPass = ((levelTasks.Count(p => p.Status == 2 && p.ExecuteType == 3) + 1) /levelTasks.Count) * 100 >= percentage;
                    if (isBackPass)
                    {
                        foreach (var task in levelTasks.FindAll(p => p.Status != 2 && p.Id != currentTask.Id))
                        {
                            var updateTask = task.Clone();
                            updateTask.ExecuteType = 5;
                            updateTask.Status = 2;
                            updateTask.CompletedTime1 = DateExtensions.Now;
                            updateTasks.Add(updateTask);
                        }
                    }
                    break;
                case 6://选择人员顺序退回
                    if (currentTask.StepSort == 1)
                    {
                        
                    }
                    else
                    {
                        var prevTask = levelTasks.Find(p => p.StepSort == currentTask.StepSort - 1);
                        if (null != prevTask)
                        {
                            var updateTask = prevTask.Clone();
                            updateTask.ExecuteType = 0;
                            updateTask.Status = 0;
                            updateTask.CompletedTime1 = null;
                            updateTask.Note = currentTask.ReceiveName + (localizer == null ? "退回" : localizer["Execute_BackBack"]);
                            updateTask.Comments = "";
                            updateTask.IsSign = 0;
                            updateTask.OpenTime = null;
                            updateTasks.Add(updateTask);
                            executeResult.NextTasks.Add(updateTask);
                        }
                        isBackPass = false;
                    }
                    break;
            }

            //更新当前任务
            var updateCurrentTask = currentTask.Clone();
            updateCurrentTask.ExecuteType = 3;
            updateCurrentTask.Status = 2;
            updateCurrentTask.Comments = executeModel.Comment;
            updateCurrentTask.IsSign = executeModel.IsSign;
            updateCurrentTask.CompletedTime1 = DateExtensions.Now;
            updateCurrentTask.Attachment = executeModel.Attachment;
            updateTasks.Add(updateCurrentTask);

            if (isBackPass)//如果步骤退回通过，给退回接收人增加待办
            {
                //如果是选择人员顺序处理，则要将当前任务后面为等待中的任务删除
                if (currentStep.StepBase.HanlderModel == 4 && currentTask.StepSort == 1)
                {
                    var currentTasks = groupTasks.FindAll(p => p.StepId == currentStep.Id);
                    if (currentTasks.Count > 0)
                    {
                        int currentMaxSort = currentTasks.Max(p => p.Sort);
                        var currentTasks1 = currentTasks.FindAll(p => p.Sort == currentMaxSort && p.Id != currentTask.Id && p.Status < 2);
                        foreach (var task in currentTasks1)
                        {
                            var removeTask = task.Clone();
                            removeTasks.Add(removeTask);
                        }
                    }
                }

                foreach (var (stepId, stepName, dynamicStepId, parallelorserial, receiveUsers, completedTime) in executeModel.Steps)
                {
                    var nextStep = flowRunModel.Steps.Find(p => p.Id == stepId);
                    if (null == nextStep)
                    {
                        continue;
                    }
                    if (nextStep.StepBase.HanlderModel == 4)
                    {
                        var nextStepTasks = groupTasks.FindAll(p => p.StepId == nextStep.Id && p.TaskType != 5);
                        if (nextStepTasks.Count > 0)
                        {
                            int maxSort1 = nextStepTasks.Max(p => p.Sort);
                            var nextStepTasks1 = nextStepTasks.FindAll(p => p.Sort == maxSort1).OrderByDescending(p => p.StepSort);
                            foreach(var nextStepTask in nextStepTasks1)
                            {
                                var addTask = nextStepTask.Clone();
                                addTask.Id = Guid.NewGuid();
                                addTask.Title = executeModel.Title.IsNullOrWhiteSpace() ? currentTask.Title : executeModel.Title;
                                addTask.ExecuteType = 0;
                                addTask.Status = nextStepTask.Id == nextStepTasks1.First().Id ? 0 : -1;
                                addTask.ExecuteType = addTask.Status;
                                addTask.CompletedTime1 = null;
                                addTask.Comments = "";
                                addTask.IsSign = 0;
                                addTask.OpenTime = null;
                                addTask.Note = localizer == null ? "退回" : localizer["Execute_BackBack"];
                                addTask.Sort = currentTask.Sort + 1;
                                addTask.SenderId = currentTask.ReceiveId;
                                addTask.SenderName = currentTask.ReceiveName;
                                addTask.ReceiveTime = DateExtensions.Now;
                                addTask.PrevId = currentTask.Id;
                                addTask.PrevStepId = currentTask.StepId;
                                addTask.OtherType = executeModel.OtherType;
                                addTask.SubFlowGroupId = nextStepTask.SubFlowGroupId;
                                addTask.TaskType = 4;
                                addTask.CompletedTime = new DateTime?();
                                addTask.RemindTime = new DateTime?();
                                addTasks.Add(addTask);
                                executeResult.NextTasks.Add(addTask);
                            }
                        }
                        continue;
                    }

                    int stepSort = 1;
                    foreach (var receiveUser in receiveUsers)
                    {
                        //判断下一步待办中没有当前接收人的待办才添加
                        if (addTasks.Exists(p => p.ReceiveId == receiveUser.Id && p.StepId == nextStep.Id) ||
                            groupTasks.Exists(p => p.ReceiveId == receiveUser.Id && p.StepId == nextStep.Id && p.Status != 2))
                        {
                            continue;
                        }
                        //查找退回步骤的原始待办，确定subgroupid,子流程的情况
                        var oldTask = groupTasks.Find(p => p.StepId == stepId && p.ReceiveId == receiveUser.Id && p.Sort == currentTask.Sort - 1);
                        string subFlowGroupId = null != oldTask ? oldTask.SubFlowGroupId : string.Empty;
                        //设置完成时间
                        DateTime? completedTime1 = completedTime;
                        DateTime? remindTime = new DateTime?();//提醒时间(如果任务设置了超期提示)
                        WorkDate workDate = new WorkDate();
                        if (!completedTime1.HasValue && nextStep.WorkTime > 0)
                        {
                            completedTime1 = workDate.GetWorkDateTime((double)nextStep.WorkTime);
                        }
                        if(completedTime1.HasValue && nextStep.ExpiredPrompt > 0)
                        {
                            remindTime = workDate.GetWorkDateTime((double)(nextStep.ExpiredPromptDays - nextStep.ExpiredPromptDays * 2), completedTime1);
                        }
                        Model.FlowTask task = new Model.FlowTask
                        {
                            ExecuteType = 0,
                            FlowId = executeModel.FlowId,
                            FlowName = flowRunModel.Name,
                            GroupId = currentTask.GroupId,
                            Id = Guid.NewGuid(),
                            InstanceId = oldTask != null ? oldTask.InstanceId : executeModel.InstanceId,
                            IsSign = 0,
                            OtherType = executeModel.OtherType,
                            Note = localizer == null ? "退回" : localizer["Execute_BackBack"],
                            PrevId = currentTask.Id,
                            PrevStepId = currentTask.StepId,
                            ReceiveId = receiveUser.Id,
                            ReceiveName = receiveUser.Name,
                            ReceiveTime = DateExtensions.Now,
                            SenderId = currentTask.ReceiveId,
                            SenderName = currentTask.ReceiveName,
                            Sort = currentTask.Sort + 1,
                            Status = 0,
                            StepId = stepId,
                            StepName = nextStep.Name,
                            StepSort = stepSort,
                            TaskType = 4,
                            Title = executeModel.Title,
                            IsAutoSubmit = nextStep.ExpiredExecuteModel,
                            SubFlowGroupId = subFlowGroupId,
                            BeforeStepId = oldTask != null ? oldTask.BeforeStepId : new Guid?(),
                            CompletedTime = completedTime1,
                            RemindTime = remindTime
                        };
                        if (receiveUser.PartTimeId.HasValue)
                        {
                            task.ReceiveOrganizeId = receiveUser.PartTimeId;
                        }
                        if (receiveUser.Note.IsGuid(out Guid entuustId))//用人员实体的NOTE字段来存放委托人ID
                        {
                            task.EntrustUserId = entuustId;
                            //task.TaskType = 2;
                            task.Note = localizer == null ? "委托" : localizer["Execute_SubmitTaskNoteEntrust"];
                        }
                        addTasks.Add(task);
                        executeResult.NextTasks.Add(task);
                    }
                }
            }
            executeResult.IsSuccess = true;
        }

        /// <summary>
        /// 保存任务
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        /// <param name="localizer">语言包</param>
        public void Save(Model.FlowRun flowRunModel, Model.FlowRunModel.Execute executeModel, IStringLocalizer localizer = null)
        {
            //如果是第一步提交并且没有实例则先创建实例
            bool isFirstTask = executeModel.StepId == flowRunModel.FirstStepId && executeModel.TaskId.IsEmptyGuid();
            if (isFirstTask)
            {
                var currentTask = GetFirstTask(flowRunModel, executeModel);
                addTasks.Add(currentTask);
                executeResult.NextTasks.Add(currentTask);
                executeResult.CurrentTask = currentTask;
            }
            else
            {
                var currentTask = Get(executeModel.TaskId);
                if (null == currentTask)
                {
                    executeResult.DebugMessages = localizer == null ? "当前任务为空!" : localizer["Execute_SubmitTaskEmpty"];
                    executeResult.IsSuccess = false;
                    executeResult.Messages = localizer == null ? "当前任务为空!" : localizer["Execute_SubmitTaskEmpty"];
                    return;
                }
                else if (currentTask.Status.In(-1, 2))
                {
                    executeResult.DebugMessages = (localizer == null ? "当前任务" : localizer["Execute_SubmitCurrentTask"]) + (currentTask.Status == -1 ? (localizer == null ? "等待中" : localizer["Execute_SubmitCurrentTaskWait"]) : (localizer == null ? "已处理" : localizer["Execute_SubmitCurrentTaskProcessed"])) + "!";
                    executeResult.IsSuccess = false;
                    executeResult.Messages = executeResult.DebugMessages;
                    return;
                }
                if (!executeModel.Title.IsNullOrWhiteSpace())
                {
                    currentTask.Title = executeModel.Title;//更新标题
                }
                currentTask.InstanceId = executeModel.InstanceId;
                currentTask.Comments = executeModel.Comment;
                currentTask.Attachment = executeModel.Attachment;
                updateTasks.Add(currentTask);
                executeResult.NextTasks.Add(currentTask);
                executeResult.CurrentTask = currentTask;
            }
            executeResult.IsSuccess = true;
        }

        /// <summary>
        /// 创建第一个任务
        /// </summary>
        /// <param name="executeModel"></param>
        /// <returns></returns>
        public Model.FlowTask GetFirstTask(Model.FlowRun flowRunModel, Model.FlowRunModel.Execute executeModel)
        {
            if (executeModel.Title.IsNullOrWhiteSpace())
            {
                executeModel.Title = flowRunModel.Name + " - " + executeModel.Sender.Name;
            }
            Model.FlowTask task = new Model.FlowTask
            {
                Comments = executeModel.Comment,
                ExecuteType = 1,
                FlowId = executeModel.FlowId,
                FlowName = flowRunModel.Name,
                GroupId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                InstanceId = executeModel.InstanceId,
                IsSign = executeModel.IsSign,
                Note = executeModel.Note,
                OtherType = executeModel.OtherType,
                PrevId = Guid.Empty,
                PrevStepId = Guid.Empty,
                ReceiveId = executeModel.Sender.Id,
                ReceiveName = executeModel.Sender.Name,
                ReceiveTime = DateExtensions.Now,
                SenderId = executeModel.Sender.Id,
                SenderName = executeModel.Sender.Name,
                Sort = 1,
                Status = 1,
                StepId = flowRunModel.FirstStepId,
                StepName = GetStepName(flowRunModel, executeModel.StepId),
                StepSort = 1,
                TaskType = 0,
                Title = executeModel.Title,
                IsAutoSubmit = 0
            };
            return task;
        }

        /// <summary>
        /// 处理前加签任务
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        public void BeforAddWrite(Model.FlowRun flowRunModel, Model.FlowRunModel.Execute executeModel, Model.FlowTask currentTask)
        {
            char[] otherType = currentTask.OtherType.ToString().ToCharArray();
            if (otherType.Length != 3)
            {
                executeResult.DebugMessages = "加签参数错误!";
                executeResult.IsSuccess = false;
                executeResult.Messages = "加签参数错误!";
                return;
            }
            int addType = otherType[1].ToString().ToInt();//加签类型 1前加签 2后加签 3并签
            int executeType = otherType[2].ToString().ToInt(); ;//处理类型 1所有人同意，2一个同意 3顺序处理
            bool addIsPass = false;//加签是否通过
            var groupTasks = GetListByGroupId(currentTask.GroupId);
            var levelTask = groupTasks.FindAll(p => p.Sort == currentTask.Sort && p.Id != currentTask.Id);
            switch (executeType)
            {
                case 1://所有人同意
                    addIsPass = !levelTask.Exists(p => p.Status != 2);
                    break;
                case 2://一人同意
                    addIsPass = true;
                    foreach (var task in levelTask)
                    {
                        if (task.Status != 2)
                        {
                            task.Status = 2;
                            task.ExecuteType = 4;
                            updateTasks.Add(task);
                        }
                    }
                    break;
                case 3://顺序处理
                    var nextTask = levelTask.Find(p => p.StepSort == currentTask.StepSort + 1);
                    if (null != nextTask)
                    {
                        nextTask.Status = 0;
                        nextTask.ExecuteType = 0;
                        updateTasks.Add(nextTask);
                        executeResult.NextTasks = new List<Model.FlowTask>() { nextTask };
                    }
                    else
                    {
                        addIsPass = true;
                    }
                    break;
            }
            currentTask.Status = 2;
            currentTask.ExecuteType = 2;
            currentTask.IsSign = executeModel.IsSign;
            currentTask.Comments = executeModel.Comment;
            currentTask.ExecuteType = 2;
            currentTask.CompletedTime1 = DateExtensions.Now;
            updateTasks.Add(currentTask);
            executeResult.IsSuccess = true;
            if (addIsPass && addType == 1)
            {
                var prevTask = Get(currentTask.PrevId);
                if (null != prevTask)
                {
                    var addTask = prevTask.Clone();
                    addTask.Id = Guid.NewGuid();
                    addTask.PrevId = currentTask.Id;
                    addTask.Status = 0;
                    addTask.ExecuteType = 0;
                    addTask.IsSign = 0;
                    addTask.Comments = null;
                    addTask.CompletedTime1 = new DateTime?();
                    addTask.OpenTime = new DateTime?();
                    addTasks.Add(addTask);
                    executeResult.NextTasks = new List<Model.FlowTask>() { addTask };
                }
            }
        }

        /// <summary>
        /// 已阅知(抄送完成)
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        /// <param name="localizer">语言包</param>
        public void CopyForCompleted(Model.FlowRun flowRunModel, Model.FlowRunModel.Execute executeModel, IStringLocalizer localizer = null)
        {
            var currentTask = Get(executeModel.TaskId);
            if (null == currentTask)
            {
                executeResult.DebugMessages = localizer == null ? "当前任务为空!" : localizer["Execute_SubmitTaskEmpty"];
                executeResult.IsSuccess = false;
                executeResult.Messages = localizer == null ? "当前任务为空!" : localizer["Execute_SubmitTaskEmpty"];
                return;
            }
            else if (currentTask.Status.In(-1, 2))
            {
                executeResult.DebugMessages = (localizer == null ? "当前任务" : localizer["Execute_SubmitCurrentTask"]) + (currentTask.Status == -1 ? (localizer == null ? "等待中" : localizer["Execute_SubmitCurrentTaskWait"]) : (localizer == null ? "已处理" : localizer["Execute_SubmitCurrentTaskProcessed"])) + "!";
                executeResult.IsSuccess = false;
                executeResult.Messages = executeResult.DebugMessages;
                return;
            }
            currentTask.ExecuteType = 8;
            currentTask.Status = 2;
            currentTask.CompletedTime1 = DateExtensions.Now;
            updateTasks.Add(currentTask);
            executeResult.IsSuccess = true;
        }

        /// <summary>
        /// 转交任务
        /// </summary>
        /// <param name="currentTask">当前任务</param>
        /// <param name="users">要转交的人员</param>
        /// <param name="localizer">语言包</param>
        public void Redirect(Model.FlowRun flowRunModel, Model.FlowRunModel.Execute executeModel, IStringLocalizer localizer = null)
        {
            var groupTasks = GetListByGroupId(executeModel.GroupId);
            var currentTask = groupTasks.Find(p => p.Id == executeModel.TaskId);
            if (null == currentTask)
            {
                executeResult.DebugMessages = localizer == null ? "当前任务为空!" : localizer["Execute_SubmitTaskEmpty"];
                executeResult.IsSuccess = false;
                executeResult.Messages = localizer == null ? "当前任务为空!" : localizer["Execute_SubmitTaskEmpty"];
                return;
            }
            else if (currentTask.Status.In(-1, 2))
            {
                executeResult.DebugMessages = (localizer == null ? "当前任务" : localizer["Execute_SubmitCurrentTask"]) + (currentTask.Status == -1 ? (localizer == null ? "等待中" : localizer["Execute_SubmitCurrentTaskWait"]) : (localizer == null ? "已处理" : localizer["Execute_SubmitCurrentTaskProcessed"])) + "!";
                executeResult.IsSuccess = false;
                executeResult.Messages = executeResult.DebugMessages;
                return;
            }
            if (executeModel.Steps.Count == 0 || executeModel.Steps.First().receiveUsers.Count == 0)
            {
                executeResult.DebugMessages = localizer == null ? "要转交的步骤或接收人员为空" : localizer["Execute_RedirectStepReceiverEmpty"];
                executeResult.IsSuccess = false;
                executeResult.Messages = localizer == null ? "接收人员为空" : localizer["Execute_RedirectReceiverEmpty"];
                return;
            }
            currentTask.ExecuteType = 6;
            currentTask.Status = 2;
            currentTask.CompletedTime1 = DateExtensions.Now;
            updateTasks.Add(currentTask);
            executeResult.CurrentTask = currentTask;
            FlowEntrust flowEntrust = new FlowEntrust();
            User buser = new User();
            var receiveUsers = executeModel.Steps.First().receiveUsers;
            for (int i=0; i < receiveUsers.Count;i++)
            {
                var user = receiveUsers[i];

                #region 判断委托
                string entrustId = flowEntrust.GetEntrustUserId(currentTask.FlowId, user);
                bool isEntrust = !entrustId.IsNullOrWhiteSpace();
                if (isEntrust)
                {
                    var entrustUser = buser.Get(entrustId);
                    if (null != entrustUser)
                    {
                        var entrustUser1 = entrustUser.Clone();
                        entrustUser1.Note = user.Id.ToString();//用这个字段来保存委托人ID,在加入任务表的时候要用到
                        user = entrustUser1;
                    }
                }
                #endregion

                //判断下一步待办中没有当前接收人的待办才添加
                if (addTasks.Exists(p => p.ReceiveId == user.Id && p.StepId == currentTask.StepId)
                    || groupTasks.Exists(p => p.ReceiveId == user.Id && p.StepId == currentTask.StepId && p.Status != 2))
                {
                    continue;
                }

                Model.FlowTask task = currentTask.Clone();
                task.Id = Guid.NewGuid();
                task.SenderId = currentTask.ReceiveId;
                task.SenderName = currentTask.ReceiveName;
                task.ExecuteType = 0;
                task.Status = 0;
                task.Comments = "";
                task.IsSign = 0;
                task.ReceiveName = user.Name;
                task.ReceiveId = user.Id;
                task.CompletedTime1 = null;
                task.ReceiveTime = DateExtensions.Now;
                task.Note = localizer == null ? "转交" : localizer["Execute_Redirect"];
                if (user.PartTimeId.HasValue)
                {
                    task.ReceiveOrganizeId = user.PartTimeId;
                }
                if (isEntrust && user.Note.IsGuid(out Guid entuustId))//用人员实体的NOTE字段来存放委托人ID
                {
                    task.EntrustUserId = entuustId;
                }
                addTasks.Add(task);
                executeResult.NextTasks.Add(task);
            }
            executeResult.IsSuccess = true;
        }

        /// <summary>
        /// 终止任务
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        public void TaskEnd(Model.FlowRun flowRunModel, Model.FlowRunModel.Execute executeModel, IStringLocalizer localizer = null)
        {
            var groupTasks = GetListByGroupId(executeModel.GroupId);
            var currentTask = groupTasks.Find(p => p.Id == executeModel.TaskId);
            if (null == currentTask)
            {
                executeResult.DebugMessages = localizer == null ? "当前任务为空!" : localizer["Execute_SubmitTaskEmpty"];
                executeResult.IsSuccess = false;
                executeResult.Messages = localizer == null ? "当前任务为空!" : localizer["Execute_SubmitTaskEmpty"];
                return;
            }
            else if (currentTask.Status.In(-1, 2))
            {
                executeResult.DebugMessages = (localizer == null ? "当前任务" : localizer["Execute_SubmitCurrentTask"]) + (currentTask.Status == -1 ? (localizer == null ? "等待中" : localizer["Execute_SubmitCurrentTaskWait"]) : (localizer == null ? "已处理" : localizer["Execute_SubmitCurrentTaskProcessed"])) + "!";
                executeResult.IsSuccess = false;
                executeResult.Messages = executeResult.DebugMessages;
                return;
            }
            executeResult.CurrentTask = currentTask;
            //终止当前实例未完成的任务
            var tasks = groupTasks.FindAll(p => p.Status < 2);
            foreach (var task in tasks)
            {
                var updateTask = task.Clone();
                updateTask.ExecuteType = 12;
                updateTask.Status = 2;
                if (updateTask.Id == currentTask.Id)
                {
                    updateTask.Comments = executeModel.Comment;
                    updateTask.IsSign = executeModel.IsSign;
                    updateTask.ExecuteType = 11;
                }
                updateTask.CompletedTime1 = DateExtensions.Now;
                updateTasks.Add(updateTask);
            }
            executeResult.IsSuccess = true;
            executeResult.Messages = localizer == null ? "已终止!" : localizer["Execute_TaskEnd"];
        }

        /// <summary>
        /// 指派任务
        /// </summary>
        /// <param name="currentTask">当前任务</param>
        /// <param name="users">要指派的人员</param>
        /// <returns>返回1表示成功，其它为错误信息</returns>
        public string Designate(Model.FlowTask currentTask, List<Model.User> users)
        {
            currentTask.ExecuteType = 9;
            currentTask.Status = 2;
            currentTask.CompletedTime1 = DateExtensions.Now;
            List<Model.FlowTask> updateTaskList = new List<Model.FlowTask>
            {
                currentTask
            };
            var groupTasks = GetListByGroupId(currentTask.GroupId);
            List<Model.FlowTask> addTaskList = new List<Model.FlowTask>();
            FlowEntrust flowEntrust = new FlowEntrust();
            User user = new User();
            for (int i = 0; i < users.Count; i++)
            {
                var userModel = users[i];
                #region 判断委托
                string entrustId = flowEntrust.GetEntrustUserId(currentTask.FlowId, userModel);
                bool isEntrust = !entrustId.IsNullOrWhiteSpace();
                if (isEntrust)
                {
                    var entrustUser = user.Get(entrustId);
                    if (null != entrustUser)
                    {
                        var entrustUser1 = entrustUser.Clone();
                        entrustUser1.Note = userModel.Id.ToString();//用这个字段来保存委托人ID,在加入任务表的时候要用到
                        userModel = entrustUser1;
                    }
                }
                #endregion

                //判断下一步待办中没有当前接收人的待办才添加
                if (addTaskList.Exists(p => p.ReceiveId == userModel.Id && p.StepId == currentTask.StepId)
                    || groupTasks.Exists(p => p.ReceiveId == userModel.Id && p.Id != currentTask.Id && p.StepId == currentTask.StepId && p.Status != 2))
                {
                    continue;
                }

                var newTaskModel = currentTask.Clone();
                newTaskModel.Id = Guid.NewGuid();
                newTaskModel.ReceiveId = userModel.Id;
                newTaskModel.ReceiveName = userModel.Name;
                if (userModel.PartTimeId.HasValue)
                {
                    newTaskModel.ReceiveOrganizeId = userModel.PartTimeId.Value;
                }
                newTaskModel.ReceiveTime = DateExtensions.Now;
                newTaskModel.Status = 0;
                newTaskModel.ExecuteType = 0;
                newTaskModel.TaskType = 1;
                newTaskModel.Note = "指派";
                newTaskModel.Comments = "";
                newTaskModel.IsSign = 0;
                if (userModel.PartTimeId.HasValue)
                {
                    newTaskModel.ReceiveOrganizeId = userModel.PartTimeId.Value;
                }
                if (isEntrust && userModel.Note.IsGuid(out Guid entuustId))//用人员实体的NOTE字段来存放委托人ID
                {
                    newTaskModel.EntrustUserId = entuustId;
                }
                //newTaskModel.PrevId = currentTask.Id;
                addTaskList.Add(newTaskModel);
            }
            if (!addTaskList.Any())//如果没有指派给任何人，则不更新当前任务状态
            {
                updateTaskList.Clear();
            }
            SendMessage(addTaskList, User.CurrentUser);//发送待办消息
            return Update(null, updateTaskList, addTaskList, null) > 0 ? "1" : "没有指派给任何人员!";
        }

        /// <summary>
        /// 跳转任务
        /// </summary>
        /// <param name="currentTask">当前任务</param>
        /// <param name="steps">要跳转到的步骤</param>
        /// <param name="localizer">语言包</param>
        /// <returns>返回1表示成功，其它为错误信息</returns>
        public string GoTo(Model.FlowTask currentTask, Dictionary<Guid, List<Model.User>> steps, IStringLocalizer localizer = null)
        {
            if (null == currentTask)
            {
                return localizer == null ? "当前任务为空!" : localizer["ChangeStatus_NotFoundTask"];
            }
            List<Model.FlowTask> addTaskList = new List<Model.FlowTask>();
            List<Model.FlowTask> updateTaskList = new List<Model.FlowTask>();
            var groupTasks = GetListByGroupId(currentTask.GroupId);
            int maxSort = groupTasks.FindAll(p => p.StepId == currentTask.StepId).Max(p => p.Sort);
            var levelTasks = groupTasks.FindAll(p => p.StepId == currentTask.StepId && p.Sort == maxSort);
            foreach (var levelTask in levelTasks)//完成当前任务的同级任务
            {
                if (levelTask.Status != 2)
                {
                    levelTask.Status = 2;
                    levelTask.ExecuteType = 10;
                    levelTask.CompletedTime1 = DateExtensions.Now;
                    updateTaskList.Add(levelTask);
                }
            }
            var flowRunModel = new Flow().GetFlowRunModel(currentTask.FlowId, true, currentTask);
            if (null == flowRunModel)
            {
                return localizer == null ? "未找到流程运行时实体" : localizer["GoTo_NotFoundFlowRunModel"];
            }
            FlowEntrust flowEntrust = new FlowEntrust();
            User buser = new User();
            foreach (var step in steps)
            {
                var currentStep = flowRunModel.Steps.Find(p => p.Id == step.Key);
                if (null == currentStep)
                {
                    continue;
                }
                for (int i=0; i< step.Value.Count;i++)
                {
                    var user = step.Value[i];
                    #region 判断委托
                    string entrustId = flowEntrust.GetEntrustUserId(currentTask.FlowId, user);
                    bool isEntrust = !entrustId.IsNullOrWhiteSpace();
                    if (isEntrust)
                    {
                        var entrustUser = buser.Get(entrustId);
                        if (null != entrustUser)
                        {
                            var entrustUser1 = entrustUser.Clone();
                            entrustUser1.Note = user.Id.ToString();//用这个字段来保存委托人ID,在加入任务表的时候要用到
                            user = entrustUser1;
                        }
                    }
                    #endregion

                    //判断下一步待办中没有当前接收人的待办才添加
                    if (addTaskList.Exists(p => p.ReceiveId == user.Id && p.StepId == currentTask.StepId)
                        || groupTasks.Exists(p => p.ReceiveId == user.Id && p.StepId == currentTask.StepId && p.Status != 2))
                    {
                        continue;
                    }

                    Model.FlowTask task = new Model.FlowTask();
                    if (currentStep.WorkTime > 0)
                    {
                        task.CompletedTime = DateExtensions.Now.AddDays((double)currentStep.WorkTime);
                    }
                    if (user.PartTimeId.HasValue)
                    {
                        task.ReceiveOrganizeId = user.PartTimeId;
                    }
                    if (isEntrust && user.Note.IsGuid(out Guid entuustId))//用人员实体的NOTE字段来存放委托人ID
                    {
                        task.EntrustUserId = entuustId;
                    }
                    task.ExecuteType = 0;
                    task.FlowId = currentTask.FlowId;
                    task.FlowName = currentTask.FlowName;
                    task.GroupId = currentTask.GroupId;
                    task.Id = Guid.NewGuid();
                    task.InstanceId = currentTask.InstanceId;
                    task.IsAutoSubmit = currentStep.ExpiredExecuteModel;
                    task.IsSign = 0;
                    task.Note = localizer == null ? "跳转" : localizer["InstanceManage_GoTo"];
                    task.PrevId = currentTask.Id;
                    task.PrevStepId = currentTask.StepId;
                    task.ReceiveId = user.Id;
                    task.ReceiveName = user.Name;
                    task.ReceiveTime = DateExtensions.Now;
                    task.SenderId = currentTask.ReceiveId;
                    task.SenderName = currentTask.ReceiveName;
                    task.Sort = currentTask.Sort + 1;
                    task.Status = 0;
                    task.StepId = currentStep.Id;
                    task.StepName = currentStep.Name;
                    task.StepSort = 1;
                    task.TaskType = 9;
                    task.Title = currentTask.Title;
                    task.OtherType = currentTask.OtherType;
                    addTaskList.Add(task);
                }
            }
            SendMessage(addTaskList, User.CurrentUser);//发送待办消息
            return Update(null, updateTaskList, addTaskList, null)>0 ? "1" : (localizer == null ? "没有跳转给任何人员!" : localizer["SaveGoTo_NotUser"]);
        }

        /// <summary>
        /// 抄送任务
        /// </summary>
        /// <param name="currentTask">当前任务</param>
        /// <param name="users">接收人</param>
        /// <param name="localizer">语言包</param>
        /// <returns>返回1表示成功，其它为错误信息</returns>
        public string CopyFor(Model.FlowTask currentTask, List<Model.User> users, IStringLocalizer localizer = null)
        {
            if (null == currentTask || null == users)
            {
                return localizer == null ? "当前任务或当前用户为空!" : localizer["NotFoundTask"];
            }
            List<Model.FlowTask> addTaskList = new List<Model.FlowTask>();
            var groupTasks = GetListByGroupId(currentTask.GroupId);
            foreach (var user in users)
            {
                //判断下一步待办中没有当前接收人的待办才添加
                if (addTaskList.Exists(p => p.ReceiveId == user.Id && p.StepId == currentTask.StepId)
                    || groupTasks.Exists(p => p.ReceiveId == user.Id && p.StepId == currentTask.StepId && p.Status != 2))
                {
                    continue;
                }
                var task = currentTask.Clone();
                task.Id = Guid.NewGuid();
                task.ReceiveId = user.Id;
                task.ReceiveName = user.Name;
                task.ReceiveTime = DateExtensions.Now;
                task.SenderId = currentTask.ReceiveId;
                task.SenderName = currentTask.ReceiveName;
                task.CompletedTime1 = null;
                task.CompletedTime = null;
                task.Comments = "";
                task.ExecuteType = 0;
                task.Status = 0;
                task.TaskType = 5;
                task.Note = localizer == null ? "抄送" : localizer["Execute_SubmitTaskNoteCopyfor"];
                task.IsSign = 0;
                if (user.PartTimeId.HasValue)
                {
                    task.ReceiveOrganizeId = user.PartTimeId;
                }
                addTaskList.Add(task);
            }
            return Update(null, null, addTaskList, null) > 0 ? "1" : localizer == null ? "没有抄送给任何人员!" : localizer["CopyFor_NotUser"];
        }

        /// <summary>
        /// 加签
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        public void AddWrite(Model.FlowRun flowRunModel, Model.FlowRunModel.Execute executeModel, IStringLocalizer localizer = null)
        {
            JObject jObject = null;
            try
            {
                jObject = JObject.Parse(executeModel.ParamsJSON);
            }
            catch { }
            if (null == jObject)
            {
                executeResult.Messages = localizer == null ? "加签参数错误!" : localizer["Execute_AddWriteParamsError"];
                executeResult.DebugMessages = executeResult.Messages;
                executeResult.IsSuccess = false;
                return;
            }
            JObject jObject1 = jObject.Value<JObject>("addwrite");
            int addType = int.MinValue;
            int writeType = int.MinValue;
            string member = string.Empty;
            if (null != jObject1)
            {
                addType = jObject1.Value<string>("addtype").ToInt();
                writeType = jObject1.Value<string>("writetype").ToInt();
                member = jObject1.Value<string>("member");
            }
            if (addType == int.MinValue || writeType == int.MinValue || member.IsNullOrWhiteSpace())
            {
                executeResult.Messages = localizer == null ? "加签参数错误!" : localizer["Execute_AddWriteParamsError"];
                executeResult.DebugMessages = executeResult.Messages;
                executeResult.IsSuccess = false;
                return;
            }

            var currentTask = Get(executeModel.TaskId);
            if (null == currentTask)
            {
                executeResult.Messages = localizer == null ? "当前任务为空!" : localizer["Execute_SubmitTaskEmpty"];
                executeResult.DebugMessages = executeResult.Messages;
                executeResult.IsSuccess = false;
                return;
            }
            var users = new Organize().GetAllUsers(member);
            if (!users.Any())
            {
                executeResult.Messages = localizer == null ? "没有接收人" : localizer["Execute_RedirectReceiverEmpty"];
                executeResult.DebugMessages = executeResult.Messages;
                executeResult.IsSuccess = false;
                return;
            }
            FlowEntrust flowEntrust = new FlowEntrust();
            User buser = new User();
            int stepSort = 1;
            string stepName = currentTask.StepName + "(" + (addType == 1 ? (localizer == null ? "前加签" : localizer["Execute_AddWriteBefore"])
                : addType == 2 ? (localizer == null ? "后加签" : localizer["Execute_AddWriteAfter"]) : (localizer == null ? "并签" : localizer["Execute_AddWriteAnd"])) + ")";
            var groupTasks = GetListByGroupId(currentTask.GroupId);
            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                #region 判断委托
                string entrustId = flowEntrust.GetEntrustUserId(currentTask.FlowId, user);
                bool isEntrust = !entrustId.IsNullOrWhiteSpace();
                if (isEntrust)
                {
                    var entrustUser = buser.Get(entrustId);
                    if (null != entrustUser)
                    {
                        var entrustUser1 = entrustUser.Clone();
                        entrustUser1.Note = user.Id.ToString();//用这个字段来保存委托人ID,在加入任务表的时候要用到
                        user = entrustUser1;
                    }
                }
                #endregion

                int sort = addType == 3 ? currentTask.Sort : currentTask.Sort + 1;
                //判断下一步待办中没有当前接收人的待办才添加
                if (addTasks.Exists(p => p.ReceiveId == user.Id && p.StepId == currentTask.StepId && p.Sort == sort) ||
                    groupTasks.Exists(p => p.ReceiveId == user.Id && p.StepId == currentTask.StepId && p.Status != 2))
                {
                    continue;
                }

                Model.FlowTask flowTaskModel = new Model.FlowTask();
                if (user.Note.IsGuid(out Guid eId))
                {
                    flowTaskModel.EntrustUserId = eId;
                }
                flowTaskModel.ExecuteType = 0;
                flowTaskModel.FlowId = currentTask.FlowId;
                flowTaskModel.FlowName = currentTask.FlowName;
                flowTaskModel.GroupId = currentTask.GroupId;
                flowTaskModel.Id = Guid.NewGuid();
                flowTaskModel.InstanceId = currentTask.InstanceId;
                flowTaskModel.IsAutoSubmit = 0;
                //flowTaskModel.Note = "";
                flowTaskModel.OtherType = ("1" + addType.ToString() + writeType.ToString()).ToInt();
                flowTaskModel.PrevId = currentTask.Id;
                flowTaskModel.PrevStepId = currentTask.StepId;
                flowTaskModel.ReceiveId = user.Id;
                flowTaskModel.ReceiveName = user.Name;
                flowTaskModel.ReceiveOrganizeId = user.PartTimeId;
                flowTaskModel.ReceiveTime = DateExtensions.Now;
                flowTaskModel.SenderId = currentTask.ReceiveId;
                flowTaskModel.SenderName = currentTask.ReceiveName;
                flowTaskModel.Sort = sort;
                flowTaskModel.Status = stepSort == 1 ? 0 : -1;
                flowTaskModel.ExecuteType = flowTaskModel.Status;
                flowTaskModel.StepId = currentTask.StepId;
                flowTaskModel.StepName = stepName;
                flowTaskModel.StepSort = addType == 3 ? 1 : writeType == 3 ? stepSort++ : 1;//并签不能按顺序处理
                flowTaskModel.TaskType = 5 + addType;
                flowTaskModel.Title = currentTask.Title;
                addTasks.Add(flowTaskModel);
            }

            currentTask.Status = 2;
            currentTask.ExecuteType = 13;
            currentTask.Comments = executeModel.Comment;
            currentTask.CompletedTime1 = DateExtensions.Now;
            currentTask.IsSign = executeModel.IsSign;
            updateTasks.Add(currentTask);

            executeResult.Messages = localizer == null ? "加签成功!" : localizer["Execute_AddWriteSuccess"];
            executeResult.DebugMessages = executeResult.Messages;
            executeResult.IsSuccess = true;
            executeResult.NextTasks = addTasks;
        }

        /// <summary>
        /// 得到会签的发起步骤
        /// </summary>
        /// <param name="groupTasks">组任务</param>
        /// <param name="prevSteps">会签步骤的前面步骤列表</param>
        /// <returns></returns>
        public Guid GetJoinSignStepId(Model.FlowRun flowRunModel, List<Model.FlowRunModel.Step> prevSteps)
        {
            List<List<Model.FlowRunModel.Step>> stepList = new List<List<Model.FlowRunModel.Step>>();
            Guid firstStepId = flowRunModel.FirstStepId;
            Flow flow = new Flow();
            foreach (var prevStep in prevSteps)
            {
                stepList.Add(flow.GetRangeSteps(flowRunModel, firstStepId, prevStep.Id));
            }
            if (!stepList.Any())
            {
                return Guid.Empty;
            }
            var firstSteps = stepList.First();
            stepList.Remove(firstSteps);
            List<Model.FlowRunModel.Step> newSteps = new List<Model.FlowRunModel.Step>();
            foreach (var step in firstSteps)
            {
                foreach (var steps in stepList)
                {
                    var newStep = steps.Find(p => p.Id == step.Id);
                    if (null != newStep)
                    {
                        newSteps.Add(newStep);
                    }
                }
            }
            return newSteps.Any() ? newSteps.Last().Id : Guid.Empty;
        }

        /// <summary>
        /// 开始一个任务
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <param name="instanceId">实例ID(流程对应业务表ID)</param>
        /// <param name="title">任务标题</param>
        /// <param name="sender">发送人员</param>
        /// <param name="users">任务接收人员</param>
        /// <param name="completedTime">要求完成时间(不要求可为null)</param>
        /// <returns></returns>
        public Model.FlowRunModel.ExecuteResult StartFlow(Guid flowId, string instanceId, string title, Model.User sender, List<Model.User> users, DateTime? completedTime = null)
        {
            Model.FlowRunModel.ExecuteResult executeResult = new Model.FlowRunModel.ExecuteResult();
            if (flowId.IsEmptyGuid())
            {
                executeResult.Messages = "流程ID为空";
                executeResult.DebugMessages = executeResult.Messages;
                executeResult.IsSuccess = false;
                return executeResult;
            }
            if (null == users || !users.Any())
            {
                executeResult.Messages = "没有接收人";
                executeResult.DebugMessages = executeResult.Messages;
                executeResult.IsSuccess = false;
                return executeResult;
            }
            var flowRunModel = new Flow().GetFlowRunModel(flowId);
            if (null == flowRunModel)
            {
                executeResult.Messages = "未找到流程运行时实体";
                executeResult.DebugMessages = executeResult.Messages;
                executeResult.IsSuccess = false;
                return executeResult;
            }
            if (flowRunModel.Status != 1)
            {
                executeResult.Messages = "流程未发布";
                executeResult.DebugMessages = executeResult.Messages + " - 流程状态：" + flowRunModel.Status.ToString();
                executeResult.IsSuccess = false;
                return executeResult;
            }
            var stepModel = flowRunModel.Steps.Find(p => p.Id == flowRunModel.FirstStepId);
            if (null == stepModel)
            {
                executeResult.Messages = "流程没有第一步";
                executeResult.DebugMessages = executeResult.Messages;
                executeResult.IsSuccess = false;
                return executeResult;
            }
            List<Model.FlowTask> flowTasks = new List<Model.FlowTask>();
            Guid groupId = Guid.NewGuid();
            FlowEntrust flowEntrust = new FlowEntrust();
            User buser = new User();
            for(int i=0;i<users.Count;i++)
            {
                var user = users[i];

                #region 判断委托
                string entrustId = flowEntrust.GetEntrustUserId(flowId, user);
                bool isEntrust = !entrustId.IsNullOrWhiteSpace();//是否有委托
                if (isEntrust)
                {
                    var entrustUser = buser.Get(entrustId);
                    if (null != entrustUser)
                    {
                        var entrustUser1 = entrustUser.Clone();
                        entrustUser1.Note = user.Id.ToString();//用这个字段来保存委托人ID,在加入任务表的时候要用到
                        user = entrustUser1;
                    }
                }
                #endregion

                Model.FlowTask flowTaskModel = new Model.FlowTask();
                flowTaskModel.CompletedTime = completedTime ?? (stepModel.WorkTime > 0 ? DateExtensions.Now.AddDays((double)stepModel.WorkTime) : new DateTime?());
                if (isEntrust && user.Note.IsGuid(out Guid eid))
                {
                    flowTaskModel.EntrustUserId = eid;
                }
                flowTaskModel.ExecuteType = 0;
                flowTaskModel.FlowId = flowId;
                flowTaskModel.FlowName = flowRunModel.Name;
                flowTaskModel.GroupId = groupId;
                flowTaskModel.Id = Guid.NewGuid();
                flowTaskModel.InstanceId = instanceId;
                flowTaskModel.IsAutoSubmit = stepModel.ExpiredExecuteModel;
                flowTaskModel.PrevId = Guid.Empty;
                flowTaskModel.PrevStepId = Guid.Empty;
                flowTaskModel.ReceiveId = user.Id;
                flowTaskModel.ReceiveName = user.Name;
                flowTaskModel.ReceiveOrganizeId = user.PartTimeId;
                flowTaskModel.ReceiveTime = DateExtensions.Now;
                flowTaskModel.SenderId = sender == null ? Guid.Empty : sender.Id;
                flowTaskModel.SenderName = sender == null ? "" : sender.Name;
                flowTaskModel.Sort = 1;
                flowTaskModel.Status = 0;
                flowTaskModel.StepId = flowRunModel.FirstStepId;
                flowTaskModel.StepName = stepModel.Name;
                flowTaskModel.StepSort = 1;
                flowTaskModel.TaskType = 0;
                flowTaskModel.Title = title.IsNullOrWhiteSpace() ? flowRunModel.Name + " - " + stepModel.Name + " -" + user.Name : title;
                flowTasks.Add(flowTaskModel);
            }
            int count = Update(null, null, flowTasks, null);
            executeResult.Messages = "发起成功";
            executeResult.DebugMessages = executeResult.Messages;
            executeResult.IsSuccess = true;
            executeResult.NextTasks = flowTasks;
            return executeResult;
        }

        /// <summary>
        /// 判断一个任务是否可以催办
        /// </summary>
        /// <param name="flowTask"></param>
        /// <param name="isWithdraw">是否可以收回</param>
        /// <returns></returns>
        public bool IsHasten(Guid taskId, out bool isWithdraw)
        {
            isWithdraw = false;
            var task = Get(taskId);
            if (null == task)
            {
                return false;
            }
            var groupTasks = GetListByGroupId(task.GroupId);
            var nextTasks = groupTasks.FindAll(p => p.PrevId == taskId);
            isWithdraw = nextTasks.Any() && !nextTasks.Any(p => p.Status != 0);
            return groupTasks.Exists(p => p.PrevId == taskId && p.Status.In(0, 1));
        }

        /// <summary>
        /// 判断一个委托任务是否可以收回
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool IsEntrustWithdraw(Guid taskId)
        {
            var task = Get(taskId);
            if (null == task)
            {
                return false;
            }
            return task.Status < 1;
        }

        /// <summary>
        /// 判断一个任务是否可以作废
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool IsDelete(Guid taskId, Model.FlowRun flowRunModel = null)
        {
            var task = Get(taskId);
            if (null == task)
            {
                return false;
            }
            if (null == flowRunModel)
            {
                flowRunModel = new Flow().GetFlowRunModel(task.FlowId);
            }
            if (null == flowRunModel)
            {
                return false;
            }
            //如果是第一步并且当前处理者是发起者，可以作废
            if (task.StepId == flowRunModel.FirstStepId && task.ReceiveId == GetFirstSenderId(task.GroupId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 得到前一步实例ID
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public string GetPrevInstanceID(string taskId)
        {
            if(!taskId.IsGuid(out Guid taskGuid))
            {
                return string.Empty;
            }
            var taskModel = Get(taskGuid);
            if(null == taskModel || taskModel.PrevId.IsEmptyGuid())
            {
                return string.Empty;
            }
            var prevTaskModel = Get(taskModel.PrevId);
            return null == prevTaskModel ? string.Empty : prevTaskModel.InstanceId;
        }

        /// <summary>
        /// 得到前一步实例标题
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public string GetPrevTitle(string taskId)
        {
            if (!taskId.IsGuid(out Guid taskGuid))
            {
                return string.Empty;
            }
            var taskModel = Get(taskGuid);
            if (null == taskModel || taskModel.PrevId.IsEmptyGuid())
            {
                return string.Empty;
            }
            var prevTaskModel = Get(taskModel.PrevId);
            return null == prevTaskModel ? string.Empty : prevTaskModel.Title;
        }

        /// <summary>
        /// 将JSON参数转换为执行实体
        /// </summary>
        /// <param name="json"></param>
        /// <returns>执行实体，错误信息</returns>
        public (Model.FlowRunModel.Execute, string) GetExecuteModel(string json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return (null, "参数为空");
            }
            JObject jObject = null;
            try
            {
                jObject = JObject.Parse(json);
            }
            catch
            {
                return (null, "参数解析错误");
            }
            string taskId = jObject.Value<string>("id");
            string flowId = jObject.Value<string>("flowId");
            if (!taskId.IsGuid(out Guid taskGuid) && !flowId.IsGuid(out Guid flowGuid))
            {
                return (null, "任务ID和流程ID同时为空");
            }
            var currentTask = taskGuid.IsEmptyGuid() ? null : Get(taskGuid);
            if (flowGuid.IsEmptyGuid() && null != currentTask)
            {
                flowGuid = currentTask.FlowId;
            }
            var flowRunModel = new Flow().GetFlowRunModel(flowGuid, true, currentTask);
            if (null == flowRunModel)
            {
                return (null, "流程运行时实体为空");
            }
            string instanceId = jObject.Value<string>("instanceId");
            string title = jObject.Value<string>("title");
            string comment = jObject.Value<string>("comment");
            string type = jObject.Value<string>("type");
            string note = jObject.Value<string>("note");
            string senderId = jObject.Value<string>("senderId");
            if (!senderId.IsGuid(out Guid senderGuid))
            {
                return (null, "发送人为空");
            }
            var senderUser = new User().Get(senderGuid);
            if (null == senderUser)
            {
                return (null, "没有找到发送人");
            }
            if (type.IsNullOrEmpty())
            {
                return (null, "执行类型为空");
            }
            int sign = jObject.Value<string>("sign").ToInt(0);
            JArray stepsArray = jObject.Value<JArray>("steps");
            Model.FlowRunModel.Execute executeModel = new Model.FlowRunModel.Execute
            {
                Comment = comment,
                FlowId = currentTask == null ? flowGuid : currentTask.FlowId
            };
            executeModel.ExecuteType = GetExecuteType(type);
            executeModel.GroupId = currentTask == null ? Guid.Empty : currentTask.GroupId;
            executeModel.InstanceId = currentTask == null ? string.Empty : currentTask.InstanceId;
            executeModel.IsSign = sign;
            executeModel.Note = note;
            executeModel.Sender = senderUser;
            executeModel.StepId = currentTask == null ? flowRunModel.FirstStepId : currentTask.StepId;
            executeModel.TaskId = currentTask == null ? Guid.Empty : currentTask.Id;
            executeModel.Title = title;
            List<(Guid, string, Guid?, int?, List<Model.User>, DateTime?)> steps = new List<(Guid, string, Guid?, int?, List<Model.User>, DateTime?)>();
            Organize organize = new Organize();
            foreach (JObject step in stepsArray)
            {
                Guid id = step.Value<string>("id").ToGuid();
                var stepModel = flowRunModel.Steps.Find(p => p.Id == id);
                if (null == stepModel)
                {
                    continue;
                }
                string name = step.Value<string>("name");//步骤名称
                string beforeStepId = step.Value<string>("beforestepid");//动态步骤的原步骤ID
                string parallelorserial = step.Value<string>("parallelorserial");//0并且 1串行
                string member = step.Value<string>("member");
                string completedtime = step.Value<string>("completedtime");
                DateTime? comTime = completedtime.IsDateTime(out DateTime dt) ?
                    dt : stepModel.WorkTime > 0 ?
                    DateExtensions.Now.AddDays((double)stepModel.WorkTime) : new DateTime?();
                steps.Add((id, name,
                    beforeStepId.IsGuid(out Guid beforeStepGuid) ? new Guid?(beforeStepGuid) : new Guid?(),
                    parallelorserial.IsInt(out int parallelorserialInt) ? new int?(parallelorserialInt) : new int?(),
                    organize.GetAllUsers(member), comTime));
            }
            executeModel.Steps = steps;
            return (executeModel, string.Empty);
        }

        /// <summary>
        /// 根据字符串处理类型，得到枚举处理类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Model.FlowRunModel.Execute.Type GetExecuteType(string type)
        {
            if (type.IsNullOrWhiteSpace())
            {
                return Model.FlowRunModel.Execute.Type.Save;
            }
            switch (type.ToLower())
            {
                case "freesubmit": //自由流程发送
                    return Model.FlowRunModel.Execute.Type.FreeSubmit;
                case "submit":
                    return Model.FlowRunModel.Execute.Type.Submit;
                case "save":
                    return Model.FlowRunModel.Execute.Type.Save;
                case "back":
                    return Model.FlowRunModel.Execute.Type.Back;
                case "completed":
                    return Model.FlowRunModel.Execute.Type.Completed;
                case "redirect":
                    return Model.FlowRunModel.Execute.Type.Redirect;
                case "addwrite":
                    return Model.FlowRunModel.Execute.Type.AddWrite;
                case "copyforcompleted": //抄送完成(已阅知)
                    return Model.FlowRunModel.Execute.Type.CopyforCompleted;
                case "taskend":
                    return Model.FlowRunModel.Execute.Type.TaskEnd;
                default:
                    return Model.FlowRunModel.Execute.Type.Save;
            }
        }

        /// <summary>
        /// 自动执行一个任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="type">执行类型</param>
        /// <returns></returns>
        public Model.FlowRunModel.ExecuteResult AutoSubmit(Guid taskId, string type = "submit")
        {
            Model.FlowRunModel.ExecuteResult result = new Model.FlowRunModel.ExecuteResult();
            var currentTask = Get(taskId);
            if (null == currentTask)
            {
                result.Messages = "当前任务为空!";
                result.DebugMessages = result.Messages;
                result.IsSuccess = false;
                return result;
            }
            var flowRunModel = new Flow().GetFlowRunModel(currentTask.FlowId, true, currentTask);
            if (null == flowRunModel)
            {
                result.Messages = "未找到流程运行时实体!";
                result.DebugMessages = result.Messages;
                result.IsSuccess = false;
                return result;
            }
            var (html, message, sendSteps) = GetNextSteps(flowRunModel, currentTask.StepId, currentTask.GroupId, 
                taskId, currentTask.InstanceId, currentTask.ReceiveId, false);
            JArray jArray = new JArray();
            foreach (var nextStep in sendSteps)
            {
                JObject stepObject = new JObject
                {
                    { "id", nextStep.Id },
                    { "member", nextStep.RunDefaultMembers },
                    { "completedtime", nextStep.WorkTime > 0 ? new WorkDate().GetWorkDateTime((double)nextStep.WorkTime).ToDateTimeString() : string.Empty }
                };
                jArray.Add(stepObject);
            }
            JObject jObject = new JObject
            {
                { "id", taskId.ToString() }, //任务ID，第一个任务为空
                { "flowId", currentTask.FlowId }, //流程ID，和任务ID不能同时为空
                { "instanceId", currentTask.InstanceId }, //实例ID
                { "title", currentTask.Title }, //任务标题
                { "comment", "" }, //处理意见
                { "sign", 0 }, //是否签章
                { "senderId", currentTask.SenderId }, //发送人ID
                { "type", type },
                { "note", "" }, //备注
                { "steps", jArray }
            };
            var (executeModel, errMsg) = GetExecuteModel(jObject.ToString());
            if (null == executeModel)
            {
                result.Messages = errMsg;
                result.DebugMessages = result.Messages;
                result.IsSuccess = false;
                return result;
            }
            executeModel.IsAutoSubmit = true;
            //executeModel.OtherType = 2;// OtherType 为2表示自动提交
            return Execute(executeModel);
        }

        /// <summary>
        /// 自动提交超时任务
        /// </summary>
        public void AutoSubmitExpireTask(Dictionary<string, string> lang = null)
        {
            DataTable expireDt = flowTaskData.GetExpireTasks();
            foreach (DataRow dr in expireDt.Rows)
            {
                if (dr["Id"].ToString().IsGuid(out Guid taskId))
                {
                    var result = AutoSubmit(taskId);
                    //如果提交失败，则改为不自动提交
                    if (!result.IsSuccess)
                    {
                        var currentTask = Get(taskId);
                        if (null != currentTask)
                        {
                            currentTask.IsAutoSubmit = 2;
                            Update(currentTask);
                        }
                    }
                    Log.Add((lang == null ? "超时自动提交了任务-" : lang["AutoSubmitExpireTask_Log"]) + taskId.ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(result), Log.Type.流程运行);
                }
            }
        }

        /// <summary>
        /// 得到获取动态步骤的任务实体
        /// </summary>
        /// <param name="groupId">组ID</param>
        /// <param name="taskId">当前任务ID，如果为空，则返回的Model.FlowTask当前任务实体也为空</param>
        /// <returns>(动态任务, 当前任务, 任务组List)</returns>
        public (Model.FlowTask, Model.FlowTask, List<Model.FlowTask>) GetDynamicTask(Guid groupId, Guid? taskId = null)
        {
            if (!Config.EnableDynamicStep || groupId.IsEmptyGuid())
            {
                return (null, taskId.HasValue ? Get(taskId.Value) : null, null);
            }
            var groupTasks = GetListByGroupId(groupId);
            if(groupTasks.Count == 0)
            {
                return (null, null, groupTasks);
            }
            Model.FlowTask dynamicTask = null;
            Model.FlowTask currentTask = null;
            var sortTasks = groupTasks.FindAll(p => p.BeforeStepId.IsNotEmptyGuid()).OrderByDescending(p => p.Sort);
            if (sortTasks.Any())
            {
                dynamicTask = sortTasks.First();
            }
            if (taskId.IsNotEmptyGuid())
            {
                currentTask = groupTasks.Find(p => p.Id == taskId.Value);
            }
            return (dynamicTask, currentTask, groupTasks);
        }

        /// <summary>
        /// 到提醒时间的任务提醒
        /// </summary>
        public void RemindTask(Dictionary<string, string> lang = null)
        {
            DataTable expireDt = flowTaskData.GetRemindTasks();
            Message message = new Message();
            foreach (DataRow dr in expireDt.Rows)
            {
                string contents = (lang == null ? "您有一个待办事项“" : lang["RemindTask_Todoitems"]) + dr["Title"].ToString() 
                    + "”，" + (lang == null ? "将在" : lang["RemindTask_Bein"]) 
                    + dr["CompletedTime"].ToString().ToDateTime().ToShortDateTimeString() 
                    + (lang == null ? "超期，请尽快处理!" : lang["RemindTask_Overdue"]);
                string msg = message.Send(dr["ReceiveId"].ToString().ToGuid(), contents, "0,1,2");
                if ("1".Equals(msg))//如果消息发送成功，将提醒时间延后一天。
                {
                    int i = flowTaskData.UpdateRemind(dr["Id"].ToString().ToGuid(), DateExtensions.Now.AddDays(1));
                }
                Log.Add((lang == null ? "待办任务超期提醒消息" : lang["RemindTask_LogTitle"]) + "-" + dr["ReceiveName"], contents, Log.Type.流程运行, 
                    others: (lang == null ? "发送消息返回：" : lang["RemindTask_LogReturn"]) + msg);
            }
        }
    }
}
