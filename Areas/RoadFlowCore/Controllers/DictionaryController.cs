using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoadFlow.Utility;

namespace RoadFlow.Mvc.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class DictionaryController : Controller
    {
        [Validate]
        public IActionResult Index()
        {
            ViewData["query"] = "appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            ViewData["rootId"] = new Business.Dictionary().GetRootId();
            return View();
        }

        [Validate]
        public IActionResult Tree()
        {
            ViewData["query"] = "appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            return View();
        }

        [Validate]
        public IActionResult Body()
        {
            string dictId = Request.Querys("id");
            string parentId = Request.Querys("parentid");
            Model.Dictionary dictionaryModel = null;
            Business.Dictionary dictionary = new Business.Dictionary();
            if (dictId.IsGuid(out Guid guid))
            {
                dictionaryModel = dictionary.Get(guid);
            }
            if (null == dictionaryModel)
            {
                dictionaryModel = new Model.Dictionary
                {
                    Id = Guid.NewGuid(),
                    ParentId = parentId.ToGuid(),
                    Sort = dictionary.GetMaxSort(parentId.ToGuid())
                };
            }
            ViewData["id"] = dictId.IsNullOrWhiteSpace() ? "" : dictId;
            ViewData["query"] = Request.UrlQuery();
            ViewData["query1"] = "appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            ViewData["refreshId"] = dictionaryModel.ParentId;
            ViewData["isRoot"] = dictionaryModel.ParentId == Guid.Empty ? "1" : "0";//是否是根节点
            return View(dictionaryModel);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string SaveBody(Model.Dictionary dictionaryModel)
        {
            if (!ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(ModelState);
            }
            Business.Dictionary dictionary = new Business.Dictionary();
            if (Request.Querys("id").IsGuid(out Guid guid))
            {
                var oldModel = dictionary.Get(guid);
                string oldJSON = null == oldModel ? "" : oldModel.ToString();
                dictionary.Update(dictionaryModel);
                Business.Log.Add("修改了数据字典-" + dictionaryModel.Title, type: Business.Log.Type.系统管理, oldContents: oldJSON, newContents: dictionaryModel.ToString());
            }
            else
            {
                dictionary.Add(dictionaryModel);
                Business.Log.Add("添加了数据字典-" + dictionaryModel.Title, dictionaryModel.ToString(), Business.Log.Type.系统管理);
            }
            return "保存成功";
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string DeleteBody()
        {
            string id = Request.Querys("id");
            if (id.IsGuid(out Guid guid))
            {
                if (guid == new Business.Dictionary().GetRootId())
                {
                    return "请勿删除根字典!";
                }
                if (guid == "ed6f44b8-a3bc-4743-9fae-c3607406f88f".ToGuid())
                {
                    return "请勿删除系统字典!";
                }
                var dictionaries = new Business.Dictionary().Delete(guid);
                Business.Log.Add("删除了数据字典", Newtonsoft.Json.JsonConvert.SerializeObject(dictionaries), Business.Log.Type.系统管理);
                return "共删除了" + dictionaries.Count + "条记录";
            }
            else
            {
                return "Id错误";
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult Sort()
        {
            string id = Request.Querys("id");
            Business.Dictionary dictionary = new Business.Dictionary();
            ViewData["queryString"] = Request.UrlQuery();
            if (id.IsGuid(out Guid guid))
            {
                var dict = dictionary.Get(guid);
                var childs = dictionary.GetChilds(dict.ParentId);
                ViewData["refreshId"] = dict.ParentId;
                return View(childs);
            }
            else
            {
                return new ContentResult() { Content = "没有找到当前字典项" };
            }
        }

        /// <summary>
        /// 保存排序
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string SaveSort()
        {
            string sort = Request.Forms("sort");
            Business.Dictionary dictionary = new Business.Dictionary();
            int i = 0;
            List<Model.Dictionary> dictionaries = new List<Model.Dictionary>();
            foreach (string id in sort.Split(','))
            {
                if (id.IsGuid(out Guid guid))
                {
                    var dict = dictionary.Get(guid);
                    if (null != dict)
                    {
                        dict.Sort = i+=5;
                        dictionaries.Add(dict);
                    }
                }
            }
            dictionary.Update(dictionaries.ToArray());
            return "保存成功!";
        }

        /// <summary>
        /// 检查代码是否重复
        /// </summary>
        /// <returns></returns>
        public string CheckCode()
        {
            string id = Request.Querys("id");
            string code = Request.Forms("value");
            if (code.IsNullOrEmpty())
            {
                return "1";
            }
            return id.IsGuid(out Guid guid) ? new Business.Dictionary().CheckCode(guid, code) ? "1" : "唯一代码重复" : "id错误";
        }

        /// <summary>
        /// 加载树JSON
        /// </summary>
        /// <returns></returns>
        public string Tree1()
        {
            string rootId = Request.Querys("root");
            string tempitem = Request.Querys("tempitem");//需要临时添加的项（如表单管理流程管理需要额外添加已删除的项目）
            string tempitemid = Request.Querys("tempitemid");//需要临时添加项的ID
            Business.Dictionary dictionary = new Business.Dictionary();
            if (rootId.IsNullOrEmpty())
            {
                rootId = dictionary.GetRootId().ToString();
            }
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            string[] rootIds = rootId.Split(',');
            foreach (string rid in rootIds)
            {
                if(!rid.IsGuid(out Guid rootGuid))
                {
                    continue;
                }
                var rootDict = dictionary.Get(rootGuid);
                if (null == rootDict)
                {
                    continue;
                }
               
                var childs = dictionary.GetChilds(rootGuid);
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", rootDict.Id },
                    { "parentID", rootDict.ParentId },
                    { "title", rootDict.Status == 1 ? "<span style='color:#999'>"+rootDict.Title+"[作废]</span>" : rootDict.Title},
                    { "type", childs.Count > 0 ? "0" : "2" },
                    { "ico", "fa-briefcase" },
                    { "hasChilds", childs.Count }
                };

                Newtonsoft.Json.Linq.JArray jArray1 = new Newtonsoft.Json.Linq.JArray();
                foreach (var child in childs)
                {
                    Newtonsoft.Json.Linq.JObject jObject1 = new Newtonsoft.Json.Linq.JObject
                    {
                        { "id", child.Id },
                        { "parentID", rootDict.Id },
                        { "title", child.Status == 1 ? "<span style='color:#999'>"+child.Title+"[作废]</span>" : child.Title},
                        { "type", "2" },
                        { "ico", "" },
                        { "hasChilds", dictionary.HasChilds(child.Id) ? 1 : 0 },
                        { "childs", new Newtonsoft.Json.Linq.JArray() }
                    };
                    jArray1.Add(jObject1);
                }


                if (!tempitem.IsNullOrWhiteSpace() && !tempitemid.IsNullOrWhiteSpace() && rid.Equals(rootIds[rootIds.Length - 1]))
                {
                    Newtonsoft.Json.Linq.JObject tempObject = new Newtonsoft.Json.Linq.JObject
                    {
                        { "id", tempitemid },
                        { "parentID", rootDict.Id },
                        { "title", tempitem},
                        { "type", "2" },
                        { "ico", "" },
                        { "hasChilds", 0 },
                        { "childs", new Newtonsoft.Json.Linq.JArray() }
                    };
                    jArray1.Add(tempObject);
                }

                jObject.Add("childs", jArray1);
                jArray.Add(jObject);
            }
            return jArray.ToString();
        }

        public string TreeRefresh()
        {
            string refreshId = Request.Querys("refreshid");
            if (!refreshId.IsGuid(out Guid rid))
            {
                return "[]";
            }
            Business.Dictionary dictionary = new Business.Dictionary();
            var childs = dictionary.GetChilds(rid);
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (var child in childs)
            {
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", child.Id },
                    { "parentID", child.ParentId },
                    { "title", child.Status == 1 ? "<span style='color:#999'>"+child.Title+"[作废]</span>" : child.Title},
                    { "type", "2" },
                    { "ico", "" },
                    { "hasChilds", dictionary.HasChilds(child.Id) ? 1 :0 }
                };
                jArray.Add(jObject);
            }
            return jArray.ToString();
        }

        /// <summary>
        /// 导出
        /// </summary>
        [Validate]
        public void Export()
        {
            string json = new Business.Dictionary().GetExportString(Request.Querys("id"));
            byte[] contents = System.Text.Encoding.UTF8.GetBytes(json);
            Response.Headers.Add("Server-FileName", "dictionary.json");
            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition", "attachment; filename=dictionary.json");
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
            Business.Dictionary dictionary = new Business.Dictionary();
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
                string msg = dictionary.Import(json);
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