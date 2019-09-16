using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using System.IO;

namespace RoadFlow.Mvc.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class FormDesignerPluginController : Controller
    {
        [Validate(CheckApp = false)]
        public IActionResult Attribute()
        {
            ViewData["userId"] = Business.Organize.PREFIX_USER + Current.UserId.ToString();
            ViewData["dbconnOptions"] = new Business.DbConnection().GetOptions();
            ViewData["formTypeOptions"] = new Business.Dictionary().GetOptionsByCode("system_applibrarytype_form");
            return View(); 
        }

        [Validate(CheckApp = false)]
        public IActionResult Events()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Text()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Textarea()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Select()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Radio()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Checkbox()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Hidden()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Button()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Html()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Label()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Datetime()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Organize()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Lrselect()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult SerialNumber()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Files()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult SubTable()
        {
            ViewData["appid"] = Request.Querys("appid");
            ViewData["formTypes"] = new Business.Dictionary().GetOptionsByCode("system_applibrarytype_form");
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult SubtableSet()
        {
            ViewData["eid"] = Request.Querys("eid");
            ViewData["dbconn"] = Request.Querys("dbconn");
            ViewData["secondtable"] = Request.Querys("secondtable");
            ViewData["connOptions"] = new Business.DbConnection().GetOptions(Request.Querys("dbconn"));
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult SelectDiv()
        {
            ViewData["formTypes"] = new Business.Dictionary().GetOptionsByCode("system_applibrarytype");
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Signature()
        {
            return View();
        }

        [Validate(CheckApp = false)]
        public IActionResult DataTable()
        {
            return View();
        }


        [Validate(CheckApp = false)]
        public string SaveForm()
        {
            string attr = Request.Forms("attr");
            string events = Request.Forms("event");
            string subtable = Request.Forms("subtable");
            string html = Request.Forms("html");

            JObject jObject = null;
            try
            {
                jObject = JObject.Parse(attr);
            }
            catch
            {
                return "属性JSON解析错误!";
            }
            string id = jObject.Value<string>("id");
            string name = jObject.Value<string>("name");
            string formType = jObject.Value<string>("formType");
            string manageUser = jObject.Value<string>("manageUser");
            if (!id.IsGuid(out Guid guid))
            {
                return "表单ID不能为空!";
            }
            if (name.IsNullOrWhiteSpace())
            {
                return "表单名称为空,请在表单属性中填写名称!";
            }
            if (!formType.IsGuid(out Guid typeId))
            {
                return "表单分类不能为空,请在表单属性中选择分类!";
            }
            if (manageUser.IsNullOrWhiteSpace())//如果没有指定管理者，则默认为创建人员
            {
                manageUser = Business.Organize.PREFIX_USER + Current.UserId.ToString();
            }
            Business.Form form = new Business.Form();
            Model.Form formModel = form.Get(guid);
            bool isAdd = false;
            if (null == formModel)
            {
                formModel = new Model.Form
                {
                    Id = guid,
                    Status = 0,
                    CreateDate = DateExtensions.Now,
                    CreateUserId = Current.UserId,
                    CreateUserName = Current.UserName
                };
                isAdd = true;
            }
            formModel.Name = name.Trim();
            formModel.FormType = typeId;
            formModel.EventJSON = events;
            formModel.SubtableJSON = subtable;
            formModel.attribute = attr;
            formModel.Html = html;
            formModel.EditDate = DateExtensions.Now;
            formModel.ManageUser = manageUser.ToLower();
            int i = isAdd ? form.Add(formModel) : form.Update(formModel);
            Business.Log.Add("保存了表单-" + name, formModel.ToString(), Business.Log.Type.流程管理);
            return "保存成功!";
        }

        [Validate(CheckApp = false)]
        public string PublishForm()
        {
            string attr = Request.Forms("attr");
            string events = Request.Forms("event");
            string subtable = Request.Forms("subtable");
            string html = Request.Forms("html");
            string formHtml = Request.Forms("formHtml");

            JObject jObject = null;
            try
            {
                jObject = JObject.Parse(attr);
            }
            catch
            {
                return "属性JSON解析错误!";
            }
            string id = jObject.Value<string>("id");
            string name = jObject.Value<string>("name");
            string formType = jObject.Value<string>("formType");
            string manageUser = jObject.Value<string>("manageUser");
            if (!id.IsGuid(out Guid guid))
            {
                return "表单ID不能为空!";
            }
            if (name.IsNullOrWhiteSpace())
            {
                return "表单名称为空,请在表单属性中填写名称!";
            }
            if (!formType.IsGuid(out Guid typeId))
            {
                return "表单分类不能为空,请在表单属性中选择分类!";
            }
            if (manageUser.IsNullOrWhiteSpace())//如果没有指定管理者，则默认为创建人员
            {
                manageUser = Business.Organize.PREFIX_USER + Current.UserId.ToString();
            }

            #region 保存数据表
            Business.Form form = new Business.Form();
            Model.Form formModel = form.Get(guid);
            bool isAdd = false;
            if (null == formModel)
            {
                formModel = new Model.Form
                {
                    Id = guid,
                    Status = 0,
                    CreateDate = DateExtensions.Now,
                    CreateUserId = Current.UserId,
                    CreateUserName = Current.UserName
                };
                isAdd = true;
            }
            formModel.Name = name.Trim();
            formModel.FormType = typeId;
            formModel.EventJSON = events;
            formModel.SubtableJSON = subtable;
            formModel.attribute = attr;
            formModel.Html = html;
            formModel.EditDate = DateExtensions.Now;
            formModel.Status = 1;
            formModel.RunHtml = formHtml;
            formModel.ManageUser = manageUser.ToLower();
            int i = isAdd ? form.Add(formModel) : form.Update(formModel);
            #endregion

            #region 写入文件
            string webRootPath = Current.WebRootPath;
            string path = webRootPath + "/RoadFlowResources/scripts/formDesigner/form/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string file = path + formModel.Id + ".rfhtml";
            Stream stream = System.IO.File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            stream.SetLength(0);
            StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.UTF8);
            sw.Write(formHtml);
            sw.Close();
            stream.Close();
            #endregion

            #region 写入应用程序库
            Business.AppLibrary appLibrary = new Business.AppLibrary();
            var appModel = appLibrary.GetByCode(formModel.Id.ToString());
            bool add = false;
            if (null == appModel)
            {
                add = true;
                appModel = new Model.AppLibrary
                {
                    Id = Guid.NewGuid(),
                    Code = formModel.Id.ToString()
                };
            }
            appModel.Title = formModel.Name;

            appModel.Title_en = formModel.Name;
            appModel.Title_zh = formModel.Name;

            appModel.Type = formModel.FormType;
            appModel.Address = formModel.Id.ToString() + ".rfhtml";
            int j = add ? appLibrary.Add(appModel) : appLibrary.Update(appModel);
            #endregion

            Business.Log.Add("发布了表单-" + name, formModel.ToString(), Business.Log.Type.流程管理, others: formHtml);
            return "发布成功!";
        }
    }
}