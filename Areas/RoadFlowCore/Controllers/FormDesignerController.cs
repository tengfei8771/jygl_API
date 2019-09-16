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
    public class FormDesignerController : Controller
    {
        [Validate]
        public IActionResult Index()
        {
            string queryString = Request.UrlQuery();
            ViewData["queryString"] = queryString.IsNullOrWhiteSpace() ? "?1=1" : Request.UrlQuery();
            return View();
        }

        [Validate]
        public IActionResult Tree()
        {
            ViewData["appId"] = Request.Querys("appid");
            ViewData["iframeId"] = Request.Querys("iframeid");
            ViewData["openerId"] = Request.Querys("openerid");
            ViewData["rootId"] = new Business.Dictionary().GetIdByCode("system_applibrarytype_form");
            return View();
        }

        [Validate]
        public IActionResult List()
        {
            ViewData["appId"] = Request.Querys("appid");
            ViewData["iframeId"] = Request.Querys("iframeid");
            ViewData["openerId"] = Request.Querys("openerid");
            ViewData["query"] = "typeid=" + Request.Querys("typeid") + "&appid=" + Request.Querys("appid")
                + "&iframeid=" + Request.Querys("iframeid") + "&openerid=" + Request.Querys("openerid");
            return View();
        }

        [Validate]
        public string QueryList()
        {
            string form_name = Request.Forms("form_name");
            string typeid = Request.Querys("typeid");
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
            Business.Form form = new Business.Form();
            var forms = form.GetPagerList(out int count, size, number, Current.UserId, form_name, typeid, order);
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            Business.User user = new Business.User();
            foreach (System.Data.DataRow dr in forms.Rows)
            {
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", dr["Id"].ToString() },
                    { "Name", dr["Name"].ToString() },
                    { "CreateUserName", dr["CreateUserName"].ToString() },
                    { "CreateTime",  dr["CreateDate"].ToString().ToDateTime().ToDateTimeString() },
                    { "LastModifyTime", dr["EditDate"].ToString().ToDateTime().ToDateTimeString() },
                    { "Edit", "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"openform('" + dr["Id"].ToString() + "', '" + dr["Name"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>" }
                };
                jArray.Add(jObject);
            }
            return "{\"userdata\":{\"total\":" + count + ",\"pagesize\":" + size + ",\"pagenumber\":" + number + "},\"rows\":" + jArray.ToString() + "}";
        }

        /// <summary>
        /// 已删除列表
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult ListDelete()
        {
            ViewData["appId"] = Request.Querys("appid");
            ViewData["iframeId"] = Request.Querys("iframeid");
            ViewData["openerId"] = Request.Querys("openerid");
            ViewData["query"] = "typeid=" + Request.Querys("typeid") + "&appid=" + Request.Querys("appid")
                + "&iframeid=" + Request.Querys("iframeid") + "&openerid=" + Request.Querys("openerid");
            return View();
        }

        [Validate]
        public string QueryDeleteList()
        {
            string form_name = Request.Forms("form_name");
            string typeid = Request.Querys("typeid");
            string sidx = Request.Forms("sidx");
            string sord = Request.Forms("sord");

            int size = Tools.GetPageSize();
            int number = Tools.GetPageNumber();
            string order = (sidx.IsNullOrEmpty() ? "CreateDate" : sidx) + " " + (sord.IsNullOrEmpty() ? "DESC" : sord);
            Business.Form form = new Business.Form();
            var forms = form.GetPagerList(out int count, size, number, Current.UserId, form_name, "", order, 2);
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            Business.User user = new Business.User();
            foreach (System.Data.DataRow dr in forms.Rows)
            {
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", dr["Id"].ToString() },
                    { "Name", dr["Name"].ToString() },
                    { "CreateUserName", dr["CreateUserName"].ToString() },
                    { "CreateTime",  dr["CreateDate"].ToString().ToDateTime().ToDateTimeString() },
                    { "LastModifyTime", dr["EditDate"].ToString().ToDateTime().ToDateTimeString() },
                    { "Edit", "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"reply('" + dr["Id"].ToString() + "', '" + dr["Name"].ToString() + "');return false;\"><i class=\"fa fa-reply\"></i>还原</a>" }
                };
                jArray.Add(jObject);
            }
            return "{\"userdata\":{\"total\":" + count + ",\"pagesize\":" + size + ",\"pagenumber\":" + number + "},\"rows\":" + jArray.ToString() + "}";
        }

        [Validate]
        public IActionResult Index1()
        {
            string formid = Request.Querys("formid");
            string attr = "{}", subtable = "[]", events = "[]", html = string.Empty;
            if (formid.IsGuid(out Guid fid))
            {
                var formModel = new Business.Form().Get(fid);
                if (null != formModel)
                {
                    if (!formModel.ManageUser.ContainsIgnoreCase(Current.UserId.ToLowerString()))
                    {
                        return new ContentResult() { Content = "您不能管理当前表单" };
                    }
                    attr = formModel.attribute;
                    subtable = formModel.SubtableJSON;
                    events = formModel.EventJSON;
                    html = formModel.Html;
                }
            }
            ViewData["attr"] = attr;
            ViewData["subtable"] = subtable;
            ViewData["events"] = events;
            ViewData["html"] = html;
            ViewData["isNewForm"] = Request.Querys("isnewform");
            ViewData["typeId"] = Request.Querys("typeid");
            ViewData["query"] = "typeid=" + Request.Querys("typeid") + "&appid=" + Request.Querys("appid")
               + "&iframeid=" + Request.Querys("iframeid") + "&openerid=" + Request.Querys("openerid");
            ViewData["dbconnOptions"] = new Business.DbConnection().GetOptions();
            return View();
        }

        [Validate]
        [ValidateAntiForgeryToken]
        public string Delete()
        {
            string[] formids = Request.Forms("formid").Split(',');
            int thoroughDelete = Request.Forms("thoroughdelete").ToInt(0);//是否彻底删除
            foreach (string formid in formids)
            {
                if (!formid.IsGuid(out Guid fid))
                {
                    return "表单ID为空!";
                }
                Business.Form form = new Business.Form();
                var formModel = form.Get(fid);
                if (null == formModel)
                {
                    return "没有找到要删除的表单!";
                }
                int i = new Business.Form().DeleteAndApplibrary(formModel, thoroughDelete);
                Business.Log.Add((0 != thoroughDelete ? "彻底" : "") + "删除了表单-" + formModel.Name, formModel.ToString(), Business.Log.Type.流程管理);
            }
            return "删除成功!";
        }

        /// <summary>
        /// 还原
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string Reply()
        {
            string formid = Request.Forms("formid");
            if (!formid.IsGuid(out Guid fid))
            {
                return "表单ID错误!";
            }
            Business.Form form = new Business.Form();
            var formModel = form.Get(fid);
            if (null == formModel)
            {
                return "没有找到要还原的表单!";
            }
            formModel.Status = 0;
            form.Update(formModel);
            return "还原成功!";
        }

        /// <summary>
        /// 得到下拉联动选项
        /// </summary>
        /// <returns></returns>
        public string GetChildOptions()
        {
            string source = Request.Forms("source");
            string value = Request.Forms("value");
            string connid = Request.Forms("connid");
            string text = Request.Forms("text");
            string dictvaluefield = Request.Forms("dictvaluefield");
            string dictid = Request.Forms("dictid");
            string dictIschild = Request.Forms("dictIschild");
            string defaultvalue = Request.Forms("defaultvalue");
            return new Business.Form().GetChildOptions(source, connid, text, value, dictvaluefield, dictid, defaultvalue, "1".Equals(dictIschild));
        }

        /// <summary>
        /// 导出表单
        /// </summary>
        [Validate]
        public void Export()
        {
            string json = new Business.Form().GetExportFormString(Request.Querys("formid"));
            byte[] contents = System.Text.Encoding.UTF8.GetBytes(json);
            Response.Headers.Add("Server-FileName", "exportform.json");
            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition", "attachment; filename=exportform.json");
            Response.Headers.Add("Content-Length", contents.Length.ToString());
            Response.Body.Write(contents);
            Response.Body.Flush();
        }

        /// <summary>
        /// 导入表单
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
            Business.Form form = new Business.Form();
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
                string msg = form.ImportForm(json);
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
    }
}