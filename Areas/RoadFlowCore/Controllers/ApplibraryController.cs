using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoadFlow.Utility;
using System.Text;

namespace RoadFlow.Mvc.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class ApplibraryController : Controller
    {
        [Validate]
        public IActionResult Index()
        {
            ViewData["iframeid"] = Request.Querys("appid") + "_iframe";
            ViewData["query"] = "appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            return View();
        }

        [Validate]
        public IActionResult Tree()
        {
            ViewData["rootId"] = new Business.Dictionary().GetIdByCode("system_applibrarytype");
            ViewData["query"] = "appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            return View();
        }

        [Validate]
        public IActionResult List()
        {
            string appId = Request.Querys("appid");
            string tabId = Request.Querys("tabid");
            string typeId = Request.Querys("typeid");

            ViewData["appId"] = appId;
            ViewData["tabId"] = appId;
            ViewData["typeId"] = typeId;
            ViewData["query"] = "typeid=" + typeId + "&appid=" + appId + "&tabid=" + tabId;
            return View();
        }

        [Validate]
        public string Query()
        {
            string Title = Request.Forms("Title");
            string Address = Request.Forms("Address");
            string typeid = Request.Forms("typeid");
            string sidx = Request.Forms("sidx");
            string sord = Request.Forms("sord");

            int size = Tools.GetPageSize();
            int number = Tools.GetPageNumber();
            string order = (sidx.IsNullOrEmpty() ? "Type,Title" : sidx) + " " + (sord.IsNullOrEmpty() ? "ASC" : sord);
            Business.Dictionary dictionary = new Business.Dictionary();
            if (typeid.IsGuid(out Guid typeId))
            {
                var childsId = dictionary.GetAllChildsId(typeId);
                typeid = childsId.JoinSqlIn();
            }
            
            var appLibraries = new Business.AppLibrary().GetPagerList(out int count, size, number, Title, Address, typeid, order);
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (System.Data.DataRow dr in appLibraries.Rows)
            {
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", dr["Id"].ToString() },
                    { "Title", dr["Title"].ToString() },
                    { "Address", dr["Address"].ToString() },
                    { "TypeTitle", dictionary.GetTitle(dr["Type"].ToString().ToGuid()) },
                    { "Opation", "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + dr["Id"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>" +
                    "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"editButton('" + dr["Id"].ToString() + "');return false;\"><i class=\"fa fa-square-o\"></i>按钮</a>" }
                };
                jArray.Add(jObject);
            }
            return "{\"userdata\":{\"total\":" + count + ",\"pagesize\":" + size + ",\"pagenumber\":" + number + "},\"rows\":" + jArray.ToString() + "}";
        }
        
        [Validate]
        public IActionResult Edit()
        {
            string id = Request.Querys("id");
            string appId = Request.Querys("appid");
            string tabId = Request.Querys("tabid");
            string typeId = Request.Querys("typeid");
            string pageSize = Request.Querys("pagesize");
            string pageNumber = Request.Querys("pagenumber");
            Business.AppLibrary appLibrary = new Business.AppLibrary();
            Model.AppLibrary appLibraryModel = null;
            if (id.IsGuid(out Guid guid))
            {
                appLibraryModel = appLibrary.Get(guid);
            }
            if (null == appLibraryModel)
            {
                appLibraryModel = new Model.AppLibrary()
                {
                    Id = Guid.NewGuid(),
                };
                if (typeId.IsGuid(out Guid type))
                {
                    appLibraryModel.Type = type;
                }
            }

            ViewData["typeOptions"] = new Business.Dictionary().GetOptionsByCode("system_applibrarytype", value: appLibraryModel.Type.ToString());
            ViewData["openModelOptions"] = new Business.Dictionary().GetOptionsByCode("system_appopenmodel", Business.Dictionary.ValueField.Value, appLibraryModel.OpenMode.ToString());
            ViewData["appId"] = appId;
            ViewData["tabId"] = appId;
            ViewData["typeId"] = typeId;
            ViewData["pageSize"] = pageSize;
            ViewData["pageNumber"] = pageNumber;
            ViewData["queryString"] = Request.UrlQuery();

            return View(appLibraryModel);
        }

        [Validate]
        [ValidateAntiForgeryToken]
        public string Save(Model.AppLibrary appLibraryModel)
        {
            if (!ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(ModelState);
            }
            Business.AppLibrary appLibrary = new Business.AppLibrary();
            if (Request.Querys("id").IsGuid(out Guid guid))
            {
                var oldModel = appLibrary.Get(guid);
                string oldJSON = null == oldModel ? "" : oldModel.ToString();
                appLibrary.Update(appLibraryModel);
                Business.Log.Add("修改了应用程序库-" + appLibraryModel.Title, type: Business.Log.Type.系统管理, oldContents: oldJSON, newContents: appLibraryModel.ToString());
            }
            else
            {
                appLibrary.Add(appLibraryModel);
                Business.Log.Add("添加了应用程序库-" + appLibraryModel.Title, appLibraryModel.ToString(), Business.Log.Type.系统管理);
            }
            return "保存成功!";
        }

        /// <summary>
        /// 导出
        /// </summary>
        [Validate]
        public void Export()
        {
            string json = new Business.AppLibrary().GetExportString(Request.Querys("ids"));
            byte[] contents = Encoding.UTF8.GetBytes(json);
            Response.Headers.Add("Server-FileName", "dictionary.json");
            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition", "attachment; filename=applibrary.json");
            Response.Headers.Add("Content-Length", contents.Length.ToString());
            Response.Body.Write(contents);
            Response.Body.Flush();
        }

        /// <summary>
        /// 导入
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
            Business.AppLibrary appLibrary = new Business.AppLibrary();
            StringBuilder stringBuilder = new StringBuilder();
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
                string json = Encoding.UTF8.GetString(b);
                string msg = appLibrary.Import(json);
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
        [ValidateAntiForgeryToken]
        public string Delete()
        {
            string ids = Request.Forms("ids");
            var models = new Business.AppLibrary().Delete(ids);
            Business.Log.Add("删除了应用程序库", Newtonsoft.Json.JsonConvert.SerializeObject(models), Business.Log.Type.系统管理);
            return "删除成功!";
        }

        [Validate]
        public IActionResult Button()
        {
            string id = Request.Querys("id");
            Business.SystemButton systemButton = new Business.SystemButton();
            var buttons = new Business.AppLibraryButton().GetListByApplibraryId(id.ToGuid());
            ViewData["buttonJSON"] = Newtonsoft.Json.JsonConvert.SerializeObject(new Business.SystemButton().GetAll());
            ViewData["queryString"] = Request.UrlQuery();
            ViewData["buttonOptions"] = systemButton.GetOptions();
            ViewData["buttonTypeOptions"] = systemButton.GetButtonTypeOptions();
            return View(buttons);
        }

        [Validate]
        [ValidateAntiForgeryToken]
        public string SaveButton()
        {
            string buttonindex = Request.Forms("buttonindex");
            string id = Request.Querys("id");
            Business.AppLibraryButton appLibraryButton = new Business.AppLibraryButton();
            var buttons = appLibraryButton.GetListByApplibraryId(id.ToGuid());
            List<Tuple<Model.AppLibraryButton, int>> tuples = new List<Tuple<Model.AppLibraryButton, int>>();
            foreach (var button in buttons)
            {
                if (!buttonindex.ContainsIgnoreCase(button.Id.ToString()))
                {
                    tuples.Add(new Tuple<Model.AppLibraryButton, int>(button, 0));
                }
            }

            foreach (string index in buttonindex.Split(','))
            {
                string button_ = Request.Forms("button_" + index);
                string buttonname_ = Request.Forms("buttonname_" + index);
                string buttonevents_ = Request.Forms("buttonevents_" + index);
                string buttonico_ = Request.Forms("buttonico_" + index);
                string showtype_ = Request.Forms("showtype_" + index);
                string buttonsort_ = Request.Forms("buttonsort_" + index);
                if (buttonname_.IsNullOrEmpty())
                {
                    continue;
                }
                if (index.IsGuid(out Guid indexId))
                {
                    var button = buttons.Find(p => p.Id == indexId);
                    if (null != button)
                    {
                        button.ButtonId = button_.ToGuid();
                        button.AppLibraryId = id.ToGuid();
                        button.Events = buttonevents_;
                        button.Ico = buttonico_;
                        button.IsValidateShow = 1;
                        button.Name = buttonname_;
                        button.ShowType = showtype_.ToInt(0);
                        button.Sort = buttonsort_.ToInt(0);
                        tuples.Add(new Tuple<Model.AppLibraryButton, int>(button, 1));
                        continue;
                    }
                }

                var buttonModel = new Model.AppLibraryButton
                {
                    Id = Guid.NewGuid(),
                    ButtonId = button_.ToGuid(),
                    AppLibraryId = id.ToGuid(),
                    Events = buttonevents_,
                    Ico = buttonico_,
                    IsValidateShow = 1,
                    Name = buttonname_,
                    ShowType = showtype_.ToInt(0),
                    Sort = buttonsort_.ToInt(0)
                };
                tuples.Add(new Tuple<Model.AppLibraryButton, int>(buttonModel, 2));
            }
            int i = appLibraryButton.Update(tuples);
            Business.Log.Add("保存了应用程序库按钮-影响行数-" + i, Newtonsoft.Json.JsonConvert.SerializeObject(tuples), Business.Log.Type.系统管理);
            return "保存成功!";
        }

        /// <summary>
        /// 得到一个类别的应用下拉选项
        /// </summary>
        /// <returns></returns>
        public string GetOptionsByAppType()
        {
            string type = Request.Forms("type");
            string value = Request.Forms("value");
            var childs = new Business.AppLibrary().GetListByType(type.ToGuid());
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var child in childs)
            {
                stringBuilder.Append("<option value=\"" + child.Id + "\"");
                if (child.Id == value.ToGuid())
                {
                    stringBuilder.Append(" selected=\"selected\"");
                }
                stringBuilder.Append(">");
                stringBuilder.Append(child.Title);
                stringBuilder.Append("</option>");
            }
            return stringBuilder.ToString();
        }
    }
}