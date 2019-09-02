using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoadFlow.Utility;

namespace RoadFlow.Mvc.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class DbConnectionController : Controller
    {
        [Validate]
        public IActionResult Index()
        {
            var all = new Business.DbConnection().GetAll();
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (var model in all)
            {
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", model.Id },
                    { "Name", model.Name },
                    { "ConnType", model.ConnType },
                    { "ConnString", model.ConnString },
                    { "Note", model.Note },
                    { "Opation", "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"add('" + model.Id + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>" +
                    "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"test('" + model.Id + "');return false;\"><i class=\"fa fa-magic\"></i>测试</a>"}
                };
                jArray.Add(jObject);
            }
            ViewData["json"] = jArray.ToString();
            ViewData["tabId"] = Request.Querys("tabid");
            ViewData["query"] = "appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            return View();
        }
        [Validate]
        public IActionResult Edit()
        {
            string connId = Request.Querys("connid");
            Business.DbConnection dbConnection = new Business.DbConnection();
            Model.DbConnection dbConnectionModel = null;
            if (connId.IsGuid(out Guid cId))
            {
                dbConnectionModel = dbConnection.Get(cId);
            }
            if (null == dbConnectionModel)
            {
                dbConnectionModel = new Model.DbConnection
                {
                    Id = Guid.NewGuid(),
                    Sort = dbConnection.GetMaxSort()
                };
            }
            ViewData["connTypeOptions"] = dbConnection.GetConnTypeOptions(dbConnectionModel.ConnType);
            ViewData["queryString"] = Request.UrlQuery();
            return View(dbConnectionModel);
        }

        [Validate]
        [ValidateAntiForgeryToken]
        public string SaveEdit(Model.DbConnection dbConnectionModel)
        {
            if (!ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(ModelState);
            }
            Business.DbConnection dbConnection = new Business.DbConnection();
            if (Request.Querys("connid").IsGuid(out Guid guid))
            {
                var oldModel = dbConnection.Get(guid);
                string oldJSON = null == oldModel ? "" : oldModel.ToString();
                dbConnection.Update(dbConnectionModel);
                Business.Log.Add("修改了数据连接-" + dbConnectionModel.Name, type: Business.Log.Type.系统管理, oldContents: oldJSON, newContents: dbConnectionModel.ToString());
            }
            else
            {
                dbConnection.Add(dbConnectionModel);
                Business.Log.Add("添加了数据连接-" + dbConnectionModel.Name, dbConnectionModel.ToString(), Business.Log.Type.系统管理);
            }
            return "保存成功!";
        }

        [Validate]
        [ValidateAntiForgeryToken]
        public string Delete()
        {
            string ids = Request.Forms("ids");
            Business.DbConnection dbConnection = new Business.DbConnection();
            List<Model.DbConnection> dbConnections = new List<Model.DbConnection>();
            foreach (string id in ids.Split(','))
            {
                if (!id.IsGuid(out Guid connId))
                {
                    continue;
                }
                var connModel = dbConnection.Get(connId);
                if (null == connModel)
                {
                    continue;
                }
                dbConnections.Add(connModel);
            }
            dbConnection.Delete(dbConnections.ToArray());
            Business.Log.Add("删除了数据连接", Newtonsoft.Json.JsonConvert.SerializeObject(dbConnections), Business.Log.Type.系统管理);
            return "删除成功!";
        }

        /// <summary>
        /// 测试连接是否正常
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string TestConn()
        {
            string connId = Request.Querys("connid");
            if (!connId.IsGuid(out Guid id))
            {
                return "连接ID错误!";
            }
            string msg = new Business.DbConnection().TestConnection(id);
            return "1".Equals(msg) ? "连接成功!" : msg;
        }

        /// <summary>
        /// 测试SQL语句是否正确
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        public string TestSql()
        {
            string connId = Request.Forms("connid");
            string sql = Request.Forms("sql");
            string msg = connId.IsGuid(out Guid cid) ? new Business.DbConnection().TestSQL(cid, sql) : "连接ID错误!";
            return msg.IsInt() ? "1" : msg;
        }

        /// <summary>
        /// 得到所有表下拉选项
        /// </summary>
        /// <returns></returns>
        [Validate]
        public string GetTableOptions()
        {
            string connId = Request.Querys("connid");
            string table = Request.Querys("table");
            return connId.IsGuid(out Guid cid) ? new Business.DbConnection().GetTableOptions(cid, table) : "";
        }

        /// <summary>
        /// 得到表所有字段下拉选项
        /// </summary>
        /// <returns></returns>
        [Validate]
        public string GetFieldsOptions()
        {
            string connId = Request.Querys("connid");
            string table = Request.Querys("table");
            string field = Request.Querys("field");
            return connId.IsGuid(out Guid cid) ? new Business.DbConnection().GetTableFieldOptions(cid, table, field) : "";
        }

        /// <summary>
        /// 得到一个流程所有的表和字段
        /// </summary>
        /// <returns></returns>
        public string GetTableJSON()
        {
            string dbs = Request.Forms("dbs");
            if (dbs.IsNullOrWhiteSpace())
            {
                return "[]";
            }
            Newtonsoft.Json.Linq.JArray jArray = null;
            try
            {
                jArray = Newtonsoft.Json.Linq.JArray.Parse(dbs);
            }
            catch
            {
                return "[]";
            }
            if (null == jArray)
            {
                return "[]";
            }
            Business.DbConnection dbConnection = new Business.DbConnection();
            Newtonsoft.Json.Linq.JArray jArray1 = new Newtonsoft.Json.Linq.JArray();
            foreach (Newtonsoft.Json.Linq.JObject jObject in jArray)
            {
                string table = jObject.Value<string>("table");
                string connId = jObject.Value<string>("link");
                int type = 0;
                if (!connId.IsGuid(out Guid cid))
                {
                    continue;
                }
                var dbConnModel = dbConnection.Get(cid);
                if (null == dbConnModel)
                {
                    continue;
                }
                Newtonsoft.Json.Linq.JObject jObject1 = new Newtonsoft.Json.Linq.JObject
                {
                    { "table", table },
                    { "connId", connId },
                    { "type", type },
                    { "connName", dbConnModel.Name + "(" + dbConnModel.ConnType + ")" }
                };
                var fields = dbConnection.GetTableFields(cid, table);
                Newtonsoft.Json.Linq.JArray fieldArray = new Newtonsoft.Json.Linq.JArray();
                foreach (var field in fields)
                {
                    Newtonsoft.Json.Linq.JObject jObject2 = new Newtonsoft.Json.Linq.JObject
                    {
                        { "name", field.FieldName },
                        { "comment", field.Comment }
                    };
                    fieldArray.Add(jObject2);
                }
                jObject1.Add("fields", fieldArray);
                jArray1.Add(jObject1);
            }
            return jArray1.ToString();
        }
    }
}