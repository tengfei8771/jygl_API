using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoadFlow.Utility;

namespace RoadFlow.Mvc.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class FlowDesignerController : Controller
    {
        [Validate]
        public IActionResult Index()
        {
            ViewData["query"] = "appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            return View();
        }
        [Validate]
        public IActionResult Tree()
        {
            ViewData["rootId"] = new Business.Dictionary().GetIdByCode("system_applibrarytype_flow").ToString();
            ViewData["appId"] = Request.Querys("appid");
            ViewData["tabId"] = Request.Querys("tabid");
            ViewData["openerid"] = Request.Querys("openerid");
            return View();
        }
        [Validate]
        public IActionResult List()
        {
            string typeId = Request.Querys("typeid");
            ViewData["appId"] = Request.Querys("appid");
            ViewData["tabId"] = Request.Querys("tabid");
            ViewData["typeId"] = typeId;
            ViewData["query"] = "typeid=" + typeId + "&appid=" + Request.Querys("appid");
            return View();
        }
        [Validate]
        public string Query()
        {
            string flow_name = Request.Forms("flow_name");
            string typeid = Request.Forms("typeid");
            string sidx = Request.Forms("sidx");
            string sord = Request.Forms("sord");

            int size = Tools.GetPageSize();
            int number = Tools.GetPageNumber();
            string order = (sidx.IsNullOrEmpty() ? "CreateDate" : sidx) + " " + (sord.IsNullOrEmpty() ? "DESC" : sord);
            if (typeid.IsGuid(out Guid typeId))
            {
                var childsId = new Business.Dictionary().GetAllChildsId(typeId);
                typeid = childsId.JoinSqlIn();
            }
            Business.Flow flow = new Business.Flow();
            Business.FlowApiSystem flowApiSystem = new Business.FlowApiSystem();
            var flows = flow.GetPagerList(out int count, size, number, flow.GetManageFlowIds(Current.UserId), flow_name, typeid, order);
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            Business.User user = new Business.User();
            foreach (System.Data.DataRow dr in flows.Rows)
            {
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", dr["Id"].ToString() },
                    { "Name", dr["Name"].ToString() },
                    { "CreateDate", dr["CreateDate"].ToString().ToDateTime().ToDateTimeString() },
                    { "CreateUser", user.GetName(dr["CreateUser"].ToString().ToGuid()) },
                    { "Status", flow.GetStatusTitle(dr["Status"].ToString().ToInt()) },
                    { "SystemId",  flowApiSystem.GetName(dr["SystemId"].ToString().ToGuid())},
                    { "Note", dr["Note"].ToString() },
                    { "Opation", "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"openflow('" +dr["Id"].ToString() + "', '" + dr["Name"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>" }
                };
                jArray.Add(jObject);
            }
            return "{\"userdata\":{\"total\":" + count + ",\"pagesize\":" + size + ",\"pagenumber\":" + number + "},\"rows\":" + jArray.ToString() + "}";
        }

        [Validate]
        public IActionResult ListDelete()
        {
            string typeId = Request.Querys("typeid");
            ViewData["appId"] = Request.Querys("appid");
            ViewData["tabId"] = Request.Querys("tabid");
            ViewData["typeId"] = typeId;
            ViewData["query"] = "typeid=" + typeId + "&appid=" + Request.Querys("appid");
            return View();
        }

        [Validate]
        public string QueryDelete()
        {
            string flow_name = Request.Forms("flow_name");
            string typeid = Request.Forms("typeid");
            string sidx = Request.Forms("sidx");
            string sord = Request.Forms("sord");

            int size = Tools.GetPageSize();
            int number = Tools.GetPageNumber();
            string order = (sidx.IsNullOrEmpty() ? "CreateDate" : sidx) + " " + (sord.IsNullOrEmpty() ? "DESC" : sord);
            Business.Flow flow = new Business.Flow();
            var flows = flow.GetPagerList(out int count, size, number, flow.GetManageFlowIds(Current.UserId), flow_name, "", order, 3);
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            Business.User user = new Business.User();
            foreach (System.Data.DataRow dr in flows.Rows)
            {
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", dr["Id"].ToString() },
                    { "Name", dr["Name"].ToString() },
                    { "CreateDate", dr["CreateDate"].ToString().ToDateTime().ToDateTimeString() },
                    { "CreateUser", user.GetName(dr["CreateUser"].ToString().ToGuid()) },
                    { "Status", flow.GetStatusTitle(dr["Status"].ToString().ToInt()) },
                    { "Note", dr["Note"].ToString() },
                    { "Opation", "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"reply('" +dr["Id"].ToString() + "', '" + dr["Name"].ToString() + "');return false;\"><i class=\"fa fa-reply\"></i>还原</a>" }
                };
                jArray.Add(jObject);
            }
            return "{\"userdata\":{\"total\":" + count + ",\"pagesize\":" + size + ",\"pagenumber\":" + number + "},\"rows\":" + jArray.ToString() + "}";
        }

        [Validate]
        public IActionResult Index1()
        {
            ViewData["appId"] = Request.Querys("appid");
            ViewData["tabId"] = Request.Querys("tabid");
            ViewData["flowId"] = Request.Querys("flowid");
            ViewData["isNewFlow"] = Request.Querys("isnewflow");
            return View();
        }
        /// <summary>
        /// 设置流程属性
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult Set_Flow()
        {
            string flowId = Request.Querys("flowid");
            string isAdd = Request.Querys("isadd");
            if (flowId.IsNullOrWhiteSpace())
            {
                flowId = Guid.NewGuid().ToString();
            }
            ViewData["isAdd"] = isAdd;
            ViewData["openerid"] = Request.Querys("openerid");
            ViewData["flowId"] = flowId;
            ViewData["defaultManager"] = Business.Organize.PREFIX_USER + Current.UserId;
            ViewData["dbconnOptions"] = new Business.DbConnection().GetOptions();
            ViewData["flowTypeOptions"] = new Business.Dictionary().GetOptionsByCode("system_applibrarytype_flow", value: "");
            ViewData["flowSystemOptions"] = new Business.FlowApiSystem().GetAllOptions();
            return View();
        }

        /// <summary>
        /// 设置步骤属性
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult Set_Step()
        {
            ViewData["stepId"] = Request.Querys("id");
            ViewData["x"] = Request.Querys("x");
            ViewData["y"] = Request.Querys("y");
            ViewData["width"] = Request.Querys("width");
            ViewData["height"] = Request.Querys("height");
            ViewData["issubflow"] = Request.Querys("issubflow");
            ViewData["openerid"] = Request.Querys("openerid");
            ViewData["formTypes"] = new Business.Dictionary().GetOptionsByCode("system_applibrarytype", existsFlowType: false);
            ViewData["flowOptions"] = new Business.Flow().GetOptions();
            return View();
        }

        /// <summary>
        /// 设置连线属性
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult Set_Line()
        {
            ViewData["openerid"] = Request.Querys("openerid");
            ViewData["lineId"] = Request.Querys("id");
            ViewData["fromId"] = Request.Querys("from");
            ViewData["toId"] = Request.Querys("to");
            return View();
        }

        /// <summary>
        /// 得到流程JSON
        /// </summary>
        /// <returns></returns>
        [Validate]
        public string GetJSON()
        {
            //如果包含动态步骤则JSON要从rf_flowdynamic表中取
            string dynamicStepId = Request.Querys("dynamicstepid");//动态步骤步骤id
            string groupId = Request.Querys("groupid");//组id
            if (dynamicStepId.IsGuid(out Guid dynamicStepGuid) && groupId.IsGuid(out Guid groupGuid))
            {
                var flowDynamicModel = new Business.FlowDynamic().Get(dynamicStepGuid, groupGuid);
                return null == flowDynamicModel ? "{}" : flowDynamicModel.FlowJSON;
            }
            //========================================

            string flowId = Request.Querys("flowid");
            if (!flowId.IsGuid(out Guid fId))
            {
                return "{}";
            }
            else
            {
                var flow = new Business.Flow().Get(fId);
                return null == flow ? "" : flow.DesignerJSON;
            }
        }

        /// <summary>
        /// 导出流程
        /// </summary>
        /// <returns></returns>
        [Validate]
        public void Export()
        {
            string json = new Business.Flow().GetExportFlowString(Request.Querys("flowid"));
            byte[] contents = System.Text.Encoding.UTF8.GetBytes(json);
            Response.Headers.Add("Server-FileName", "exportflow.json");
            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition", "attachment; filename=exportflow.json");
            Response.Headers.Add("Content-Length", contents.Length.ToString());
            Response.Body.Write(contents);
            Response.Body.Flush();
        }

        /// <summary>
        /// 导入流程
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult Import()
        {
            ViewData["queryString"] = Request.UrlQuery();
            return View();
        }

        [Validate]
        [ValidateAntiForgeryToken]
        public IActionResult ImportSave()
        {
            var files = Request.Form.Files;
            if (files.Count == 0)
            {
                ViewData["errmsg"] = "您没有选择要导入的文件!";
                return View();
            }
            Business.Flow flow = new Business.Flow();
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            foreach (var file in files)
            {
                var stream = file.OpenReadStream();
                int count = (int)stream.Length;
                byte[] b = new byte[count];
                int readCount = 0;
                while (readCount < count)
                {
                    readCount += stream.Read(b, readCount, 1024);
                }
                string json = System.Text.Encoding.UTF8.GetString(b);
                string msg = flow.ImportFlow(json);
                if (!"1".Equals(msg))
                {
                    stringBuilder.Append(msg + "，");
                }
            }
            if (stringBuilder.Length > 0)
            {
                ViewData["errmsg"] = stringBuilder.ToString().TrimEnd('，');
            }
            ViewData["queryString"] = Request.UrlQuery();
            return View();
        }

        [Validate]
        public IActionResult Opation()
        {
            string op = Request.Querys("op");
            string msg = string.Empty;
            switch (op)
            {
                case "save":
                    msg = "正在保存...";
                    break;
                case "install":
                    msg = "正在安装...";
                    break;
                case "uninstall":
                    msg = "正在卸载...";
                    break;
                case "delete":
                    msg = "正在删除...";
                    break;
            }
            ViewData["msg"] = msg;
            ViewData["appId"] = Request.Querys("appid");
            ViewData["openerid"] = Request.Querys("openerid");
            ViewData["op"] = op;
            return View();
        }

        /// <summary>
        /// 保存流程
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string Save()
        {
            string json = Request.Forms("json");
            string msg = new Business.Flow().Save(json);
            return "1".Equals(msg) ? "保存成功!" : msg;
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult SaveAs()
        {
            ViewData["queryString"] = Request.UrlQuery();
            ViewData["query"] = "typeid=" + Request.Querys("typeid") + "&appid=" + Request.Querys("appid")
                + "&rf_appopenmodel=" + Request.Querys("rf_appopenmodel");
            return View();
        }

        [Validate]
        public string SaveAsSave()
        {
            string newFlowName = Request.Forms("newflowname");
            string flowId = Request.Querys("flowid");
            if (newFlowName.IsNullOrWhiteSpace())
            {
                return "{\"success\":0,\"msg\":\"新的流程名称不能为空!\"}";
            }
            if (!flowId.IsGuid(out Guid flowGuid))
            {
                return "{\"success\":0,\"msg\":\"流程Id错误!\"}";
            }
            string msg = new Business.Flow().SaveAs(flowGuid, newFlowName.Trim());
            if (msg.IsGuid(out Guid newFlowGuid))
            {
                return "{\"success\":1,\"msg\":\"另存成功!\",\"newId\":\"" + msg + "\"}";
            }
            else
            {
                return "{\"success\":0,\"msg\":\"" + msg + "\"}"; ;
            }
        }

        /// <summary>
        /// 安装流程
        /// </summary>
        /// <returns></returns>
        public string Install()
        {
            string json = Request.Forms("json");
            string msg = new Business.Flow().Install(json);
            return "1".Equals(msg) ? "安装成功!" : msg;
        }

        /// <summary>
        /// 删除,卸载流程
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string UnInstall()
        {
            string[] flowIds = Request.Forms("flowid").Split(',');
            int thoroughDelete = Request.Forms("thoroughdelete").ToInt(0);//是否彻底删除
            string status = Request.Forms("status");
            Business.Flow flow = new Business.Flow();
            foreach (var flowId in flowIds)
            {
                var flowModel = flow.Get(flowId.ToGuid());
                if (null == flowModel)
                {
                    continue;
                }
                if (0 == thoroughDelete)//作删除标记或卸载标记
                {
                    int status1 = status.ToInt(3);
                    Business.Log.Add(("2".Equals(status) ? "卸载" : "删除") + "了流程-" + flowModel.Name, flowModel.ToString(), Business.Log.Type.流程管理);
                    flowModel.Status = status1;
                    flow.Update(flowModel);
                }
                else//彻底删除
                {
                    flow.Delete(flowModel);
                    //删除应用程序库
                    Business.AppLibrary appLibrary = new Business.AppLibrary();
                    var appModel = appLibrary.GetByCode(flowModel.Id.ToString());
                    if (null != appModel)
                    {
                        new Business.AppLibrary().Delete(appModel.Id);
                    }
                    //删除流程实例
                    new Business.FlowTask().DeleteByFlowId(flowModel.Id);
                    Business.Log.Add("彻底删除了流程-" + flowModel.Name, flowModel.ToString(), Business.Log.Type.流程管理);
                }
                flow.ClearCache(flowModel.Id);
            }
            return "1";
        }

        /// <summary>
        /// 还原
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string Reply()
        {
            string flowid = Request.Forms("flowid");
            if (!flowid.IsGuid(out Guid fid))
            {
                return "流程ID错误!";
            }
            Business.Flow flow = new Business.Flow();
            var flowModel = flow.Get(fid);
            if (null == flowModel)
            {
                return "没有找到要还原的流程!";
            }
            flowModel.Status = 0;
            flow.Update(flowModel);
            return "还原成功!";
        }
    }
}