using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;

namespace RoadFlow.Business
{
    /// <summary>
    /// 通配符类
    /// </summary>
    public class Wildcard
    {
        private static readonly string[] wildcardList = new string[] {
            "{<UserId>}",
            "{<UserName>}",
            "{<UserDeptId>}",
            "{<UserDeptName>}",
            "{<UserStationId>}",
            "{<UserStationName>}",
            "{<UserWorkGroupId>}",
            "{<UserWorkGroupName>}",
            "{<UserDeptLeaderId>}",
            "{<UserDeptLeaderName>}",
            "{<UserCharegLeaderId>}",
            "{<UserCharegLeaderName>}",
            "{<UserUnitId>}",
            "{<UserUnitName>}",
            "{<InitiatorId>}",
            "{<InitiatorName>}",
            "{<InitiatorDeptId>}",
            "{<InitiatorDeptName>}",
            "{<InitiatorStationId>}",
            "{<InitiatorStationName>}",
            "{<InitiatorRoleId>}",
            "{<InitiatorRoleName>}",
            "{<InitiatorUnitId>}",
            "{<InitiatorUnitName>}",
            "{<InitiatorLeaderId>}",
            "{<InitiatorLeaderName>}",
            "{<InitiatorCharegId>}",
            "{<InitiatorCharegName>}",
            "{<ShortDate>}",
            "{<LongDate>}",
            "{<ShortDateTime>}",
            "{<LongDateTime>}",
            "{<ShortDateTimeSecond>}",
            "{<LongDateTimeSecond>}",
            "{<FlowId>}",
            "{<FlowName>}",
            "{<StepId>}",
            "{<StepName>}",
            "{<TaskId>}",
            "{<InstanceId>}",
            "{<GroupId>}",
            "{<PrevInstanceId>}",
            "{<PrevFlowTitle>}",
            "{<Guid>}",
            "{<EmptyGuid>}",

            "{Query<",
            "{Form<",
            "{DataRow<",
            "{Date<",
            "{Method<",
            "{JArray<",
            "{JObject<"
        };

        public static string GetWildcardValue(string wildcard, Model.User userModel, object obj)
        {
            Organize organize = new Organize();
            User user = new User();
            Microsoft.AspNetCore.Http.HttpRequest request = Tools.HttpContext.Request;
            switch (wildcard.ToLower())
            {
                case "{<userid>}"://当前用户ID
                    userModel = userModel ?? User.CurrentUser;
                    return userModel == null ? "" : userModel.Id.ToUpperString();
                case "{<username>}"://当前用户姓名
                    userModel = userModel ?? User.CurrentUser;
                    return userModel == null ? "" : userModel.Name;
                case "{<userdeptid>}"://当前用户部门ID
                    userModel = userModel ?? User.CurrentUser;
                    if (userModel == null)
                    {
                        return "";
                    }
                    var dept = user.GetDept(userModel.Id.ToString());
                    return dept == null ? "" : dept.Id.ToUpperString();
                case "{<userdeptname>}"://当前用户部门名称
                    userModel = userModel ?? User.CurrentUser;
                    if (userModel == null)
                    {
                        return "";
                    }
                    var dept1 = user.GetDept(userModel.Id.ToString());
                    return dept1 == null ? "" : dept1.Name;
                case "{<userstationid>}"://当前用户岗位ID
                    userModel = userModel ?? User.CurrentUser;
                    if (userModel == null)
                    {
                        return "";
                    }
                    var station = user.GetStation(userModel.Id.ToString());
                    return station == null ? "" : station.Id.ToUpperString();
                case "{<userstationname>}"://当前用户岗位名称
                    userModel = userModel ?? User.CurrentUser;
                    if (userModel == null)
                    {
                        return "";
                    }
                    var station1 = user.GetStation(userModel.Id.ToString());
                    return station1 == null ? "" : station1.Name;
                case "{<userworkgroupid>}"://当前用户工作组ID
                    userModel = userModel ?? User.CurrentUser;
                    return userModel == null ? "" : user.GetWorkGroupsId(userModel.Id);
                case "{<userworkgroupname>}"://当前用户工作组名称
                    userModel = userModel ?? User.CurrentUser;
                    return userModel == null ? "" : user.GetWorkGroupsName(userModel.Id);
                case "{<userdeptleaderid>}"://当前用户部门领导ID
                    userModel = userModel ?? User.CurrentUser;
                    return userModel == null ? "" : user.GetLeader(userModel.Id.ToString()).leader;
                case "{<userdeptleadername>}"://当前用户部门领导姓名
                    userModel = userModel ?? User.CurrentUser;
                    return userModel == null ? "" : user.GetNames(user.GetLeader(userModel.Id.ToString()).leader);
                case "{<usercharegleaderid>}"://当前用户分管领导ID
                    userModel = userModel ?? User.CurrentUser;
                    return userModel == null ? "" : user.GetLeader(userModel.Id.ToString()).chargeLeader;
                case "{<usercharegleadername>}"://当前用户分管领导姓名
                    userModel = userModel ?? User.CurrentUser;
                    return userModel == null ? "" : user.GetNames(user.GetLeader(userModel.Id.ToString()).chargeLeader);
                case "{<userunitid>}"://当前用户单位ID
                    userModel = userModel ?? User.CurrentUser;
                    if (userModel == null)
                    {
                        return "";
                    }
                    var unit = user.GetUnit(userModel.Id.ToString());
                    return unit == null ? "" : unit.Id.ToUpperString();
                case "{<userunitname>}"://当前用户单位名称
                    userModel = userModel ?? User.CurrentUser;
                    if (userModel == null)
                    {
                        return "";
                    }
                    var unit1 = user.GetUnit(userModel.Id.ToString());
                    return unit1 == null ? "" : unit1.Name;
                case "{<initiatorid>}"://发起者ID
                    userModel = userModel ?? User.CurrentUser;
                    var firstId = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                    return firstId.IsEmptyGuid() && userModel != null ? userModel.Id.ToString() : firstId.ToString();
                case "{<initiatorname>}"://发起者姓名
                    userModel = userModel ?? User.CurrentUser;
                    var firstId1 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                    return firstId1.IsEmptyGuid() && userModel != null ? userModel.Name : user.GetName(firstId1);
                case "{<initiatordeptid>}"://发起者部门ID
                    userModel = userModel ?? User.CurrentUser;
                    var firstId2 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                    if (firstId2.IsEmptyGuid() && null != userModel)
                    {
                        firstId2 = userModel.Id;
                    }
                    var dept2 = user.GetDept(firstId2.ToString());
                    return null == dept2 ? "" : dept2.Id.ToString();
                case "{<initiatordeptname>}"://发起者部门名称
                    userModel = userModel ?? User.CurrentUser;
                    var firstId3 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                    if (firstId3.IsEmptyGuid() && null != userModel)
                    {
                        firstId3 = userModel.Id;
                    }
                    var dept3 = user.GetDept(firstId3.ToString());
                    return dept3 == null ? "" : dept3.Name;
                case "{<initiatorstationid>}"://发起者岗位ID
                    userModel = userModel ?? User.CurrentUser;
                    var firstId4 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                    if (firstId4.IsEmptyGuid() && null != userModel)
                    {
                        firstId4 = userModel.Id;
                    }
                    var station4 = user.GetStation(firstId4.ToString());
                    return station4 == null ? "" : station4.Id.ToString();
                case "{<initiatorstationname>}"://发起者岗位名称 
                    var firstId5 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                    userModel = userModel ?? User.CurrentUser;
                    if (firstId5.IsEmptyGuid() && null != userModel)
                    {
                        firstId5 = userModel.Id;
                    }
                    var station5 = user.GetStation(firstId5.ToString());
                    return station5 == null ? "" : station5.Name;
                case "{<initiatorworkgroupid>}"://发起者角色组ID
                    var firstId6 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                    userModel = userModel ?? User.CurrentUser;
                    if (firstId6.IsEmptyGuid() && null != userModel)
                    {
                        firstId6 = userModel.Id;
                    }
                    return user.GetWorkGroupsId(firstId6);
                case "{<initiatorworkgroupname>}"://发起者角色组名称
                    var firstId7 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToString().ToGuid());
                    userModel = userModel ?? User.CurrentUser;
                    if (firstId7.IsEmptyGuid() && null != userModel)
                    {
                        firstId7 = userModel.Id;
                    }
                    return user.GetWorkGroupsName(firstId7);
                case "{<initiatorunitid>}"://发起者单位ID
                    var firstId8 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                    userModel = userModel ?? User.CurrentUser;
                    if (firstId8.IsEmptyGuid() && null != userModel)
                    {
                        firstId8 = userModel.Id;
                    }
                    var unit8 = user.GetUnit(firstId8.ToString());
                    return unit8 == null ? "" : unit8.Id.ToString();
                case "{<initiatorunitname>}"://发起者单位名称
                    var firstId9 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                    userModel = userModel ?? User.CurrentUser;
                    if (firstId9.IsEmptyGuid() && null != userModel)
                    {
                        firstId9 = userModel.Id;
                    }
                    var unit9 = user.GetUnit(firstId9.ToString());
                    return unit9 == null ? "" : unit9.Name;
                case "{<initiatorleaderid>}"://发起者部门领导ID
                    var firstId10 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                    userModel = userModel ?? User.CurrentUser;
                    if (firstId10.IsEmptyGuid() && null != userModel)
                    {
                        firstId10 = userModel.Id;
                    }
                    return user.GetLeader(firstId10.ToString()).leader;
                case "{<initiatorleadername>}"://发起者部门领导姓名
                    var firstId11 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                    userModel = userModel ?? User.CurrentUser;
                    if (firstId11.IsEmptyGuid() && null != userModel)
                    {
                        firstId11 = userModel.Id;
                    }
                    return user.GetNames(user.GetLeader(firstId11.ToString()).leader);
                case "{<initiatorcharegid>}"://发起者分管领导ID
                    var firstId12 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                    userModel = userModel ?? User.CurrentUser;
                    if (firstId12.IsEmptyGuid() && null != userModel)
                    {
                        firstId12 = userModel.Id;
                    }
                    return user.GetLeader(firstId12.ToString()).chargeLeader;
                case "{<initiatorcharegname>}"://发起者分管领导姓名
                    var firstId13 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                    userModel = userModel ?? User.CurrentUser;
                    if (firstId13.IsEmptyGuid() && null != userModel)
                    {
                        firstId13 = userModel.Id;
                    }
                    return user.GetNames(user.GetLeader(firstId13.ToString()).chargeLeader);
                case "{<shortdate>}"://短日期格式(yyyy-MM-dd)
                    return DateExtensions.Now.ToString("yyyy-MM-dd");
                case "{<longdate>}"://长日期格式(yyyy年MM月dd日)
                    return DateExtensions.Now.ToString("yyyy年MM月dd日");
                case "{<shortdatetime>}"://短日期时间(yyyy-MM-dd HH:mm)
                    return DateExtensions.Now.ToString("yyyy-MM-dd HH:mm");
                case "{<longdatetime>}"://长日期格式(yyyy年MM月dd日 HH时mm分)
                    return DateExtensions.Now.ToString("yyyy年MM月dd日 HH时mm分");
                case "{<shortdatetimesecond>}"://短日期时间(yyyy-MM-dd HH:mm:ss)
                    return DateExtensions.Now.ToString("yyyy-MM-dd HH:mm:ss");
                case "{<longdatetimesecond>}"://长日期格式(yyyy年MM月dd日 HH时mm分ss秒)
                    return DateExtensions.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒");
                case "{<flowid>}"://流程ID
                    return request.Querys("flowid");
                case "{<flowname>}"://流程名称
                    string flowId = request.Querys("flowid");
                    return !flowId.IsGuid(out Guid fid) ? "" : new Flow().GetName(fid);
                case "{<stepid>}"://流程步骤ID
                    return request.Querys("stepid");
                case "{<stepname>}"://流程步骤名称
                    string flowId1 = request.Querys("flowid");
                    string stepId = request.Querys("stepid");
                    return flowId1.IsGuid(out Guid fid1) && stepId.IsGuid(out Guid sid) ? new Flow().GetStepName(fid1, sid) : string.Empty;
                case "{<taskid>}"://任务ID
                    return request.Querys("taskid");
                case "{<instanceid>}"://实例ID
                    return request.Querys("instanceid");
                case "{<groupid>}"://组ID
                    return request.Querys("groupid");
                case "{<previnstanceid>}"://前一步实例ID
                    return new FlowTask().GetPrevInstanceID(request.Querys("taskid"));
                case "{<prevflowtitle>}"://前一步流程任务标题
                    return new FlowTask().GetPrevTitle(request.Querys("taskid"));
                case "{<guid>}"://随机生成UUID
                    return Guid.NewGuid().ToString();
                case "{<emptyguid>}"://空UUID
                    return Guid.Empty.ToString();
            }
            return "";
        }

        /// <summary>
        /// 过滤通配符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="currentUser">当前登录用户实体</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Filter(string str, Model.User user = null, object obj = null)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return "";
            }
            Microsoft.AspNetCore.Http.HttpRequest request = Tools.HttpContext.Request;
            foreach (string wildcard in wildcardList)
            {
                while (str.ContainsIgnoreCase(wildcard))
                {
                    string value = string.Empty;
                    string wildcard1 = wildcard;
                    if ("{Query<".EqualsIgnoreCase(wildcard))
                    {
                        string key = str.Substring(str.IndexOf("{Query<") + 7);
                        string key1 = key.Substring(0, key.IndexOf(">}"));
                        if (!key1.IsNullOrWhiteSpace())
                        {
                            wildcard1 = wildcard + key1 + ">}";
                            value = request.Querys(key1);
                        }
                    }
                    else if ("{Form<".EqualsIgnoreCase(wildcard))
                    {
                        string key = str.Substring(str.IndexOf("{Form<") + 6);
                        string key1 = key.Substring(0, key.IndexOf(">}"));
                        if (!key1.IsNullOrWhiteSpace())
                        {
                            wildcard1 = wildcard + key1 + ">}";
                            value = request.Forms(key1);
                        }
                    }
                    else if ("{DataRow<".EqualsIgnoreCase(wildcard))
                    {
                        string key = str.Substring(str.IndexOf("{DataRow<") + 9);
                        string key1 = key.Substring(0, key.IndexOf(">}"));
                        if (!key1.IsNullOrWhiteSpace())
                        {
                            wildcard1 = wildcard + key1 + ">}";
                            var dr = (System.Data.DataRow)obj;
                            try
                            {
                                value = dr[key1].ToString();
                            }
                            catch
                            {
                                value = "";
                            }
                        }
                    }
                    else if ("{Method<".EqualsIgnoreCase(wildcard))
                    {
                        //执行一个方法替换通配符 object为方法参数
                        string key = str.Substring(str.IndexOf("{Method<") + 8);
                        string key1 = key.Substring(0, key.IndexOf(">}"));
                        if (!key1.IsNullOrWhiteSpace())
                        {
                            wildcard1 = wildcard + key1 + ">}";
                            var (o, err) = obj == null ? Tools.ExecuteMethod(key1) : Tools.ExecuteMethod(key1, obj);
                            value = null == o ? "" : o.ToString();
                        }
                    }
                    else if ("{Date<".EqualsIgnoreCase(wildcard))
                    {
                        //取当前日期替换指定格式
                        string key = str.Substring(str.IndexOf("{Date<") + 6);
                        string key1 = key.Substring(0, key.IndexOf(">}"));
                        if (!key1.IsNullOrWhiteSpace())
                        {
                            wildcard1 = wildcard + key1 + ">}";
                            value = DateExtensions.Now.ToString(key1);
                        }
                    }
                    else if ("{Object<".EqualsIgnoreCase(wildcard))
                    {
                        //将对象转换为字符串替换
                        string key = str.Substring(str.IndexOf("{Object<") + 8);
                        string key1 = key.Substring(0, key.IndexOf(">}"));
                        if (!key1.IsNullOrWhiteSpace())
                        {
                            wildcard1 = wildcard + key1 + ">}";
                            value = obj == null  ? "" : obj.ToString();
                        }
                    }
                    else if ("{JArray<".EqualsIgnoreCase(wildcard))
                    {
                        //取json数组值
                        string key = str.Substring(str.IndexOf("{JArray<") + 8);
                        string key1 = key.Substring(0, key.IndexOf(">}"));
                        if (!key1.IsNullOrWhiteSpace())
                        {
                            wildcard1 = wildcard + key1 + ">}";
                            Newtonsoft.Json.Linq.JArray jArray = (Newtonsoft.Json.Linq.JArray)obj;
                            if (null != jArray)
                            {
                                foreach (Newtonsoft.Json.Linq.JObject jObject in jArray)
                                {
                                    if (jObject.ContainsKey(key1))
                                    {
                                        value = jObject.Value<string>(key1);
                                        break;
                                    }
                                }
                                if (value.IsNullOrEmpty())
                                {
                                    //这里循环找是取表单数据形式的JSON [{"name":"","value":""}] 这里取表单字段对应的值
                                    foreach (Newtonsoft.Json.Linq.JObject jObject in jArray)
                                    {
                                        if (jObject.ContainsKey("name") && jObject.Value<string>("name").Equals(key1))
                                        {
                                            value = jObject.Value<string>("value");
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                value = "";
                            }
                        }
                    }
                    else if ("{JObject<".EqualsIgnoreCase(wildcard))
                    {
                        //取json对象值
                        string key = str.Substring(str.IndexOf("{JObject<") + 9);
                        string key1 = key.Substring(0, key.IndexOf(">}"));
                        if (!key1.IsNullOrWhiteSpace())
                        {
                            wildcard1 = wildcard + key1 + ">}";
                            Newtonsoft.Json.Linq.JObject jObject = (Newtonsoft.Json.Linq.JObject)obj;
                            if (null != jObject && jObject.ContainsKey(key1))
                            {
                                value = jObject.Value<string>(key1);
                            }
                            else
                            {
                                value = "";
                            }
                        }
                    }
                    else
                    {
                        value = GetWildcardValue(wildcard, user, obj);
                    }
                    str = str.ReplaceIgnoreCase(wildcard1, value);
                }
            }
            return str;
        }
    }
}