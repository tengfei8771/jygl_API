using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using RoadFlow.Utility;
using System.Xml.Linq;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;

namespace RoadFlow.Mvc.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class ControlsController : Controller
    {
        public ControlsController()
        {
            webRootPath = Current.WebRootPath;
            contentRootPath = Current.ContentRootPath;
            attachmentPath = "/Attachment/" + Current.UserId.ToUpperString() + "/";//上传附件路径
        }

        #region 选择组织架构
        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public IActionResult Member_Index()
        {
            string values = Request.Method.EqualsIgnoreCase("post") ? Request.Forms("value") : "";
            string eid = Request.Querys("eid");
            string isunit = Request.Querys("isunit");
            string isdept = Request.Querys("isdept");
            string isstation = Request.Querys("isstation");
            string isuser = Request.Querys("isuser");
            string ismore = Request.Querys("ismore");
            string isall = Request.Querys("isall");
            string isgroup = Request.Querys("isgroup");
            string isrole = Request.Querys("isrole");
            string rootid = Request.Querys("rootid");
            string ischangetype = Request.Querys("ischangetype");
            string isselect = Request.Querys("isselect");
            string ismobile = Request.Querys("ismobile");

            Business.Organize organize = new Business.Organize();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string value in values.Split(','))
            {
                if (value.IsNullOrEmpty())
                {
                    continue;
                }
                string name = organize.GetNames(value);
                if (name.IsNullOrEmpty())
                {
                    continue;
                }
                stringBuilder.AppendFormat("<div onclick=\"currentDel=this;showinfo('{0}');\" class=\"selectorDiv\" ondblclick=\"currentDel=this;del();\" value=\"{0}\">", value);
                stringBuilder.Append(name);
                stringBuilder.Append("</div>");
            }

            ViewData["eid"] = eid;
            ViewData["isunit"] = isunit;
            ViewData["isdept"] = isdept;
            ViewData["isstation"] = isstation;
            ViewData["isuser"] = isuser;
            ViewData["ismore"] = ismore;
            ViewData["isall"] = isall;
            ViewData["isgroup"] = isgroup;
            ViewData["isrole"] = isrole;
            ViewData["rootid"] = rootid;
            ViewData["ischangetype"] = ischangetype;
            ViewData["isselect"] = isselect;
            ViewData["values"] = values;
            ViewData["userprefix"] = Business.Organize.PREFIX_USER;
            ViewData["relationprefix"] = Business.Organize.PREFIX_RELATION;
            ViewData["workgroupprefix"] = Business.Organize.PREFIX_WORKGROUP;
            ViewData["defaultValues"] = stringBuilder.ToString();
            ViewData["ismobile"] = ismobile;
            return View();
        }
        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public string Member_GetNames()
        {
            return new Business.Organize().GetNames(Request.Forms("value"));
        }
        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public string Member_GetNote()
        {
            string id = Request.Querys("id");
            if (id.IsNullOrWhiteSpace())
            {
                return "";
            }
            Business.Organize organize = new Business.Organize();
            Business.User user = new Business.User();
            Business.OrganizeUser organizeUser = new Business.OrganizeUser();
            if (id.StartsWith(Business.Organize.PREFIX_USER))//人员
            {
                var organizeUserModel = organizeUser.GetMainByUserId(id.RemoveUserPrefix().ToGuid());
                return organize.GetParentsName(organizeUserModel.OrganizeId) + " \\ " + organize.GetName(organizeUserModel.OrganizeId);
            }
            else if (id.StartsWith(Business.Organize.PREFIX_RELATION))//兼职人员
            {
                var organizeUserModel = organizeUser.Get(id.RemoveUserRelationPrefix().ToGuid());
                return organize.GetParentsName(organizeUserModel.OrganizeId) + " \\ " + organize.GetName(organizeUserModel.OrganizeId) + "[兼任]";
            }
            else if (id.StartsWith(Business.Organize.PREFIX_WORKGROUP))//工作组
            {
                return "";
            }
            else if (id.IsGuid(out Guid gid))
            {
                return organize.GetParentsName(gid) + " \\ " + organize.GetName(gid);
            }
            return "";
        }
        #endregion

        #region 选择图标
        private readonly string webRootPath;
        private readonly string contentRootPath;
        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public IActionResult SelectIco_Index()
        {
            ViewData["source"] = Request.Querys("source");
            ViewData["id"] = Request.Querys("id");
            ViewData["isImg"] = Request.Querys("isimg");
            ViewData["isfont"] = Request.Querys("isfont");
            return View();
        }
        public string SelectIco_File()
        {
            XElement rootElement = new XElement("Root");
            string Path = Request.Querys("path");
            if (Path.IsNullOrWhiteSpace())
            {
                Path = "/RoadFlowResources/images/ico";
            }

            string showType = ",.jpg,.gif,.png,";
            //string webRootPath = _hostingEnvironment.WebRootPath;
            //string contentRootPath = _hostingEnvironment.ContentRootPath;

            if (!Directory.Exists(webRootPath + Path))
            {
                return rootElement.ToString();
            }

            DirectoryInfo folder = new DirectoryInfo(webRootPath + Path);
            XElement element;
            foreach (var item in folder.GetFiles().Where(p => (p.Attributes & FileAttributes.Hidden) == 0))
            {
                if (showType.IndexOf("," + item.Extension.ToLower() + ",") != -1)
                {
                    element = new XElement("Icon");
                    rootElement.Add(element);
                    element.SetAttributeValue("title", item.Name);
                    element.SetAttributeValue("path", "/RoadFlowResources/images/ico/" + item.Name);
                    element.SetAttributeValue("path1", "/RoadFlowResources/images/ico/" + item.Name);
                }
            }
            return rootElement.ToString();
        }
        #endregion

        #region 选择数据字典
        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public IActionResult Dictionary_Index()
        {
            string values = Request.Querys("values");
            string dataSource = Request.Querys("datasource");
            StringBuilder stringBuilder = new StringBuilder();
            Business.Dictionary dictionary = new Business.Dictionary();
            foreach (string value in values.Split(','))
            {
                switch (dataSource)
                {
                    case "0":
                        if (value.IsGuid(out Guid dictId))
                        {
                            var dictModel = dictionary.Get(dictId);
                            if (null != dictModel)
                            {
                                stringBuilder.Append("<div onclick=\"currentDel=this;showinfo('{0}');\" class=\"selectorDiv\" ondblclick=\"currentDel=this;del();\" value=\"" + value + "\">");
                                stringBuilder.Append(dictModel.Title);
                                stringBuilder.Append("</div>");
                            }
                        }
                        break;
                }
            }
            ViewData["defaults"] = stringBuilder.ToString();
            ViewData["ismore"] = Request.Querys("ismore");
            ViewData["isparent"] = Request.Querys("isparent");
            ViewData["ischild"] = Request.Querys("ischild");
            ViewData["isroot"] = Request.Querys("isroot");
            ViewData["root"] = Request.Querys("root");
            ViewData["eid"] = Request.Querys("eid");
            ViewData["datasource"] = dataSource;
            ViewData["ismobile"] = Request.Querys("ismobile");
            return View();
        }
        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public string Dictionary_GetNames()
        {
            string values = Request.Forms("values");
            StringBuilder stringBuilder = new StringBuilder();
            Business.Dictionary dictionary = new Business.Dictionary();
            foreach (string value in values.Split(','))
            {
                if (value.IsGuid(out Guid dictId))
                {
                    var dictModel = dictionary.Get(dictId);
                    if (null != dictModel)
                    {
                        stringBuilder.Append(dictModel.Title);
                        stringBuilder.Append("、");
                    }
                }
            }
            return stringBuilder.ToString().TrimEnd('、');
        }
        #endregion

        #region 附件上传
        private readonly string attachmentPath;
        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public IActionResult UploadFiles_Index()
        {
            JArray jArray = new JArray();
            string values = Request.Method.EqualsIgnoreCase("post") ? Request.Forms("value") : "";
            foreach (string value in (values ?? "").Split('|'))
            {
                string fileName = value.DESDecrypt();
                FileInfo fileInfo = new FileInfo(Business.UserFile.RootPath + fileName);
                if (!fileInfo.Exists)
                {
                    continue;
                }
                JObject jObject = new JObject
                {
                    { "id", value },
                    { "name", fileInfo.Name },
                    { "size", fileInfo.Length.ToFileSize() }
                };
                jArray.Add(jObject);
            }
            ViewData["fileType"] = Request.Querys("filetype");
            ViewData["eid"] = Request.Querys("eid");
            ViewData["userId"] = Current.UserId;
            ViewData["values"] = jArray.ToString();
            ViewData["ismobile"] = Request.Querys("ismobile");
            ViewData["filepath"] = Request.Querys("filepath");
            ViewData["isselectuserfile"] = Request.Querys("isselectuserfile");//是否可以选择用户文件
            return View();
        }
        [Validate(CheckApp = false, CheckUrl = false)]
        [ValidateAntiForgeryToken]
        public string UploadFiles_Save()
        {
            DateTime dateTime = Current.DateTime;
            string year = dateTime.ToString("yyyy");
            string month = dateTime.ToString("MM");
            string day = dateTime.ToString("dd");
            var files = Request.Form.Files;
            string filetype = Request.Forms("filetype");
            JObject jObject = new JObject();
            if (files.Count > 0)
            {
                var file = files[0];
                string extName = Path.GetExtension(file.FileName).TrimStart('.');
                if (!IsUpload(extName))
                {
                    jObject.Add("error", "不能上传该类型文件");
                    return jObject.ToString();
                }
                if (!filetype.IsNullOrWhiteSpace() && !("," + filetype + ",").ContainsIgnoreCase("," + extName + ","))
                {
                    jObject.Add("error", "不能上传该类型文件");
                    return jObject.ToString();
                }
                string saveDir = Business.UserFile.RootPath + attachmentPath + year + "/" + month + "/" + day + "/";
                string fileName = file.FileName.Replace(" ", "");
                string newFileName = GetUploadFileName(saveDir, fileName);
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }
                using (FileStream fs = System.IO.File.Create(saveDir + newFileName))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                jObject.Add("id", (attachmentPath + year + "/" + month + "/" + day + "/" + newFileName).DESEncrypt());
                jObject.Add("size", file.Length.ToFileSize());
            }
            return jObject.ToString();
        }
        /// <summary>
        /// 得到上传文件名，如果重名要重新命名
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetUploadFileName(string saveDir, string fileName)
        {
            if (System.IO.File.Exists(saveDir + fileName))
            {
                string extName = Path.GetExtension(fileName);
                string fName = Path.GetFileNameWithoutExtension(fileName) + "_" + Tools.GetRandomString(6).ToUpper();
                return GetUploadFileName(saveDir, fName + extName);
            }
            return fileName;
        }
        private (string, string) GetHeadType(string extName)
        {
            if (extName.IsNullOrWhiteSpace())
            {
                return ("attachment", "application/octet-stream");
            }
            string ext = extName.Trim().ToLower();
            if (",jpg,jpeg,png,gif,tif,tiff,".Contains("," + ext + ","))
            {
                return ("inline", "image/" + ext);
            }
            else if (",txt,".Contains("," + ext + ","))
            {
                return ("inline", "text/plain");
            }
            else if (",pdf,".Contains("," + ext + ","))
            {
                return ("inline", "application/pdf");
            }
            else if (",json,".Contains("," + ext + ","))
            {
                return ("inline", "application/json");
            }
            //else if (",doc,docx,dot,".Contains("," + ext + ","))
            //{
            //    return ("inline", "application/msword");
            //}
            //else if (",xls,xlsx,".Contains("," + ext + ","))
            //{
            //    return ("inline", "application/vnd.ms-excel");
            //}
            //else if (",ppt,pptx,pps,pot,ppa,".Contains("," + ext + ","))
            //{
            //    return ("inline", "application/vnd.ms-powerpoint");
            //}
            return ("attachment", "application/octet-stream");
        }
        /// <summary>
        /// 显示文件
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckUrl = false)]
        public void ShowFile()
        {
            string file = Request.Querys("file").DESDecrypt();
            if (file.IsNullOrWhiteSpace())
            {
                return;
            }
            bool fullPath = "1".Equals(Request.Querys("fullpath"));//是否是完整路径(个人文件管理中传过来的路径就是文件完整路径)
            bool checkShare = "1".Equals(Request.Querys("checkshare"));//是否检查是分享的文件（分享的文件要验证用户是否可以访问）
            if (checkShare)
            {
                string[] fileArray = file.Split('?');
                if (fileArray.Length < 2)
                {
                    return;
                }
                file = fileArray[0];
                string userId = fileArray[1];
                string file1 = fileArray.Length > 2 ? fileArray[2] : string.Empty;
                if (!userId.IsGuid(out Guid uid) || !uid.Equals(Current.UserIdOrWeiXinId) || !new Business.UserFileShare().IsAccess(file.DESEncrypt(), uid, file1))
                {
                    return;
                }
            }

            FileInfo tmpFile = new FileInfo(fullPath ? file : Business.UserFile.RootPath + file);
            if (!tmpFile.Exists)
            {
                return;
            }
            //检查如果路径不是规定的路径，否则不能访问
            if (fullPath && !Business.UserFile.HasAccess(tmpFile.DirectoryName, Guid.Empty))
            {
                return;
            }
            string fileName = tmpFile.Name.UrlEncode();
            //if (Request != null && (Request..StartsWith("IE", StringComparison.CurrentCultureIgnoreCase)
            //    || context.Request.Browser.Type.StartsWith("InternetExplorer", StringComparison.CurrentCultureIgnoreCase)))
            //{
            //    fileName = fileName.UrlEncode();
            //}
            Response.Headers.Add("Server-FileName", fileName);
            var tmpContentType = GetHeadType(Path.GetExtension(file).TrimStart('.'));
            Response.ContentType = tmpContentType.Item2;
            Response.Headers.Add("Content-Disposition", tmpContentType.Item1 + "; filename=" + fileName);
            Response.Headers.Add("Content-Length", tmpFile.Length.ToString());
            using (var tmpRead = tmpFile.OpenRead())
            {
                var tmpByte = new byte[2048];
                var i = tmpRead.Read(tmpByte, 0, tmpByte.Length);
                while (i > 0)
                {
                    Response.Body.Write(tmpByte, 0, i);
                    Response.Body.Flush();
                    i = tmpRead.Read(tmpByte, 0, tmpByte.Length);
                }
            }
            Response.Body.Flush();
            Response.Body.Close();
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckUrl = false)]
        [ValidateAntiForgeryToken]
        public string DeleteFile()
        {
            string file = Request.Forms("file").ToString().DESDecrypt();
            if (file.IsNullOrWhiteSpace())
            {
                return "文件为空!";
            }
            FileInfo fileInfo = new FileInfo(("1".Equals(Request.Forms("fullpath")) ? "" : attachmentPath) + file);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            return "1";
        }
        /// <summary>
        /// 保存编辑器上传的文件
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckUrl = false)]
        public string SaveCKEditorFiles()
        {
            var files = Request.Form.Files;
            JObject jObject = new JObject();
            if (files.Count == 0)
            {
                jObject.Add("number", -1);
                jObject.Add("message", "没有要上传的文件");
                return new JObject() { { "error", jObject } }.ToString();
            }
            var file = files[0];
            string extName = Path.GetExtension(file.FileName).TrimStart('.');
            if (!IsUpload(extName))
            {
                jObject.Add("number", -1);
                jObject.Add("message", "不能上传该类型文件");
                return new JObject() { { "error", jObject } }.ToString();
            }
            DateTime date = DateExtensions.Now;
            string dateString = date.Year.ToString() + "/" + date.ToString("MM") + "/" + date.ToString("dd");
            string saveDir = Business.UserFile.RootPath + attachmentPath + dateString + "/";
            string fileName = file.FileName.Replace(" ", "");
            string newFileName = GetUploadFileName(saveDir, fileName);
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            using (FileStream fs = System.IO.File.Create(saveDir + newFileName))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
            JObject jObject1 = new JObject
            {
                { "fileName", newFileName },
                { "uploaded", 1 },
                { "url", Url.Content("~/RoadFlowCore/Controls/ShowFile?file=") + (attachmentPath + dateString + "/" + newFileName).DESEncrypt() }
            };
            return jObject1.ToString();
        }
        /// <summary>
        /// 保存用户文件管理上传的文件
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckUrl = false)]
        public string UserFiles_Save()
        {
            string filepath = Request.Forms("filepath");
            var files = Request.Form.Files;
            JObject jObject = new JObject();
            if (files.Count == 0)
            {
                jObject.Add("number", -1);
                jObject.Add("message", "没有要上传的文件");
                return new JObject() { { "error", jObject } }.ToString();
            }
            var file = files[0];
            string extName = Path.GetExtension(file.FileName).TrimStart('.');
            if (!IsUpload(extName))
            {
                jObject.Add("number", -1);
                jObject.Add("message", "不能上传该类型文件");
                return new JObject() { { "error", jObject } }.ToString();
            }
            string saveDir = filepath.DESDecrypt() + "/";
            string fileName = file.FileName.Replace(" ", "");
            string newFileName = GetUploadFileName(saveDir, fileName);
            if (!Business.UserFile.HasAccess(saveDir, Current.UserId))
            {
                jObject.Add("number", -1);
                jObject.Add("message", "不能在该目录上传文件");
                return new JObject() { { "error", jObject } }.ToString();
            }
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            using (FileStream fs = System.IO.File.Create(saveDir + newFileName))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
            jObject.Add("id", (saveDir + newFileName).DESEncrypt());
            jObject.Add("size", file.Length.ToFileSize());
            return jObject.ToString();
        }
        /// <summary>
        /// 得到附件显示字符串
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckUrl = false)]
        public string UploadFiles_GetShowString()
        {
            string showtype = Request.Forms("showtype");//1显示为图片
            string width = Request.Forms("width");//图片宽度
            string height = Request.Forms("height");//图片高度
            string files = Request.Forms("files");
            if (showtype.ToInt(0) == 1)//显示为图片
            {
                return files.ToFilesImgString(width.ToInt(0), height.ToInt(0));
            }
            else
            {
                return files.ToFilesShowString(true);
            }
        }
        /// <summary>
        /// 检查文件是否可以上传
        /// </summary>
        /// <param name="extName"></param>
        /// <returns></returns>
        private bool IsUpload(string extName)
        {
            return Config.UploadFileExtNames.IsNullOrWhiteSpace()
                ? !",exe,msi,bat,cshtml,asp,aspx,ashx,ascx,cs,dll,js,vbs,css,".ContainsIgnoreCase("," + extName + ",")
                : ("," + Config.UploadFileExtNames + ",").ContainsIgnoreCase("," + extName + ",");
        }
        #endregion

        #region 弹出选择
        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public IActionResult SelectDiv_Index()
        {
            string applibaryid = Request.Querys("applibaryid");
            if (!applibaryid.IsGuid(out Guid appId))
            {
                return new ContentResult() { Content = "参数错误!" };
            }
            var app = new Business.AppLibrary().Get(appId);
            if (app != null && !app.Address.IsNullOrEmpty())
            {
                var query = Request.UrlQuery();
                string url = app.Address + (app.Address.Contains("?") ? "&" + query.TrimStart('?') : query);
                return Redirect(url);
            }
            else
            {
                return new ContentResult() { Content = "参数错误!" };
            }
        }
        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public string SelectDiv_GetTitle()
        {
            string applibaryid = Request.Querys("applibaryid");
            if (!applibaryid.IsGuid(out Guid appId))
            {
                return "";
            }
            var app = new Business.AppLibrary().Get(appId);
            if (null == app || app.Code.IsNullOrWhiteSpace() || !app.Code.IsGuid(out Guid pId))
            {
                return "";
            }
            string titlefield = Request.Querys("titlefield");
            string pkfield = Request.Querys("pkfield");
            string values = Request.Querys("values");
            return new Business.Program().GetTitles(values, pkfield, titlefield, pId);
        }
        #endregion
    }
}