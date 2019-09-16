using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Utility;
using Newtonsoft.Json.Linq;

namespace RoadFlow.Business
{
    /// <summary>
    /// 动态流程业务类
    /// </summary>
    public class FlowDynamic
    {
        private readonly Data.FlowDynamic flowDynamicData;
        public FlowDynamic()
        {
            flowDynamicData = new Data.FlowDynamic();
        }

        /// <summary>
        /// 查询一个动态流程
        /// </summary>
        /// <param name="StepId">动态步骤ID</param>
        /// <param name="groupId">组ID</param>
        /// <returns></returns>
        public Model.FlowDynamic Get(Guid StepId, Guid groupId)
        {
            return flowDynamicData.Get(StepId, groupId);
        }
        /// <summary>
        /// 添加一个动态流程
        /// </summary>
        /// <param name="flowDynamic">动态流程实体</param>
        /// <returns></returns>
        public int Add(Model.FlowDynamic flowDynamic)
        {
            return flowDynamicData.Add(flowDynamic);
        }

        /// <summary>
        /// 添加动态流程
        /// </summary>
        /// <param name="executeModel">流程执行参数实体</param>
        /// <param name="groupTasks">当前组任务List</param>
        /// <returns></returns>
        public Model.FlowRun Add(Model.FlowRunModel.Execute executeModel, List<Model.FlowTask> groupTasks)
        {
            if (null == executeModel || executeModel.GroupId.IsEmptyGuid())
            {
                return null;
            }
            string oldFlowJSON = string.Empty;
            if (groupTasks != null && groupTasks.Count > 0 && groupTasks.Exists(p => p.BeforeStepId.IsNotEmptyGuid()))
            {
                var flowDynamicModel = new FlowDynamic().Get(groupTasks.OrderByDescending(p => p.Sort).Where(p => p.BeforeStepId.IsNotEmptyGuid()).First().BeforeStepId.Value, executeModel.GroupId);
                if (null != flowDynamicModel)
                {
                    oldFlowJSON = flowDynamicModel.FlowJSON;
                }
            }
            if(oldFlowJSON.IsNullOrWhiteSpace())
            {
                var flowModel = new Flow().Get(executeModel.FlowId);
                if (null != flowModel)
                {
                    oldFlowJSON = flowModel.RunJSON.IsNullOrWhiteSpace() ? flowModel.DesignerJSON : flowModel.RunJSON;
                }
            }
            if (oldFlowJSON.IsNullOrWhiteSpace())
            {
                return null;
            }
            JObject jObject = null;
            try
            {
                jObject = JObject.Parse(oldFlowJSON);
            }
            catch
            {
                return null;
            }
            if (!jObject.ContainsKey("steps"))
            {
                return null;
            }
            var stepArray = jObject.Value<JArray>("steps");
            var lineArray = jObject.Value<JArray>("lines");
            var steps = executeModel.Steps.FindAll(p => p.beforeStepId.IsNotEmptyGuid()).GroupBy(p => p.beforeStepId).ToList();
            string flowJSON = string.Empty;
            User buser = new User();
            foreach (var step in steps)
            {
                int stepIndex = 0;
                foreach (var (stepId, stepName, beforeStepId, parallelorserial, receiveUsers, completedTime) in step)
                {
                    var oldStepJsons = stepArray.Where(p => p["id"].ToString().EqualsIgnoreCase(beforeStepId.Value.ToString()));
                    if (oldStepJsons.Count() == 0)
                    {
                        stepIndex++;
                        continue;
                    }
                    var oldStepJson = oldStepJsons.First();
                    if (stepId != beforeStepId)
                    {
                        var stepJson = oldStepJson.DeepClone();
                        stepJson["id"] = stepId;
                        stepJson["name"] = stepName;
                        stepJson["dynamic"] = 0;
                        stepJson["behavior"]["runSelect"] = 0;
                        stepJson["behavior"]["defaultHandler"] = buser.GetUserIds(receiveUsers);
                        stepJson["position"]["y"] = stepJson["position"]["y"].ToString().ToInt() + (70 * stepIndex);
                        stepArray.Add(stepJson);
                    }
                    else
                    {
                        oldStepJson["dynamic"] = 0;
                        oldStepJson["behavior"]["runSelect"] = 0;
                        oldStepJson["behavior"]["defaultHandler"] = buser.GetUserIds(receiveUsers);
                    }
                    //如果是串行
                    if (parallelorserial.HasValue && parallelorserial.Value == 1)
                    {
                        if (stepIndex == step.Count() - 1)
                        {
                            var oldLines = lineArray.Where(p => p["from"].ToString().EqualsIgnoreCase(beforeStepId.Value.ToString()));
                            if (oldLines.Any())
                            {
                                var lineJson = oldLines.First().DeepClone();
                                lineJson["id"] = Guid.NewGuid().ToString();
                                lineJson["to"] = step.ElementAt(stepIndex).stepId.ToString();
                                lineJson["from"] = stepIndex == 0 ? beforeStepId.Value.ToString() : step.ElementAt(stepIndex - 1).stepId.ToString();
                                if (stepId != beforeStepId)
                                {
                                    lineArray.Add(lineJson);
                                }
                                if (!step.Where(p => p.stepId == oldLines.First()["to"].ToString().ToGuid()).Any())
                                {
                                    oldLines.First()["from"] = step.Last().stepId.ToString();
                                }
                            }
                            else
                            {
                                var oldLines1 = lineArray.Where(p => p["to"].ToString().EqualsIgnoreCase(beforeStepId.Value.ToString()));
                                if (oldLines1.Any())
                                {
                                    var lineJson = oldLines1.First().DeepClone();
                                    lineJson["id"] = Guid.NewGuid().ToString();
                                    lineJson["to"] = step.ElementAt(stepIndex).stepId.ToString();
                                    lineJson["from"] = stepIndex == 0 ? beforeStepId.Value.ToString() : step.ElementAt(stepIndex - 1).stepId.ToString();
                                    if (stepId != beforeStepId)
                                    {
                                        lineArray.Add(lineJson);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var oldLines = lineArray.Where(p => p["to"].ToString().EqualsIgnoreCase(beforeStepId.Value.ToString()));
                            if (oldLines.Any())
                            {
                                var lineJson = oldLines.First().DeepClone();
                                lineJson["id"] = Guid.NewGuid().ToString();
                                lineJson["from"] = stepIndex == 0 ? beforeStepId.Value.ToString() : step.ElementAt(stepIndex - 1).stepId.ToString();
                                lineJson["to"] = step.ElementAt(stepIndex).stepId.ToString();
                                if (stepId != beforeStepId)
                                {
                                    lineArray.Add(lineJson);
                                }
                            }
                        }
                    }
                    //并行
                    else
                    {
                        var oldLines = lineArray.Where(p => p["to"].ToString().EqualsIgnoreCase(beforeStepId.Value.ToString()) && p["from"].ToString().EqualsIgnoreCase(executeModel.StepId.ToString()));
                        if (oldLines.Any())
                        {
                            var lineJson = oldLines.First().DeepClone();
                            lineJson["id"] = Guid.NewGuid().ToString();
                            lineJson["to"] = stepId;
                            if (stepId != beforeStepId)
                            {
                                lineArray.Add(lineJson);
                            }
                        }
                        var oldLines1 = lineArray.Where(p => p["from"].ToString().EqualsIgnoreCase(beforeStepId.Value.ToString()));
                        int j = oldLines1.Count();
                        for (int i = 0; i < j; i++)
                        {
                            var lineJson = oldLines1.ElementAt(i).DeepClone();
                            lineJson["id"] = Guid.NewGuid().ToString();
                            lineJson["from"] = stepId;
                            if (stepId != beforeStepId)
                            {
                                lineArray.Add(lineJson);
                            }
                        }
                    }
                    stepIndex++;
                }
                var dynamicModel = Get(step.Key.Value, executeModel.GroupId);
                string json = jObject.ToString(Newtonsoft.Json.Formatting.None);
                if (null == dynamicModel)
                {
                    dynamicModel = new Model.FlowDynamic() { StepId = step.Key.Value, GroupId = executeModel.GroupId, FlowJSON = json };
                    Add(dynamicModel);
                }
                else
                {
                    dynamicModel.FlowJSON = json;
                    Update(dynamicModel);
                }
                Cache.IO.Remove(Flow.CACHEKEY + "_" + dynamicModel.StepId.ToNString() + "_" + dynamicModel.GroupId.ToNString());
                if (step.Key == steps.Last().Key)
                {
                    flowJSON = json;
                }
            }
            return new Flow().GetFlowRunModel(flowJSON, out string msg);
        }

        /// <summary>
        /// 更新动态流程
        /// </summary>
        /// <param name="flowDynamic">动态流程实体</param>
        public int Update(Model.FlowDynamic flowDynamic)
        {
            return flowDynamicData.Update(flowDynamic);
        }
        /// <summary>
        /// 删除动态流程
        /// </summary>
        /// <param name="flowDynamic">动态流程实体</param>
        /// <returns></returns>
        public int Delete(Model.FlowDynamic flowDynamic)
        {
            return flowDynamicData.Delete(flowDynamic);
        }

        /// <summary>
        /// 删除动态流程
        /// </summary>
        /// <param name="stepId">动态步骤ID</param>
        /// <param name="groupId">组ID</param>
        /// <returns></returns>
        public int Delete(Guid stepId, Guid groupId)
        {
            return flowDynamicData.Delete(stepId, groupId);
        }
    }
}
