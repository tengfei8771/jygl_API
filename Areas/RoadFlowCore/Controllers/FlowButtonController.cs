using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoadFlow.Utility;

namespace RoadFlow.Mvc.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class FlowButtonController : Controller
    {
        [Validate]
        public IActionResult Index()
        {
            var buttons = new Business.FlowButton().GetAll();
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (var button in buttons)
            {
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", button.Id },
                    { "Title", button.Title },
                    { "Ico", button.Ico.IsNullOrWhiteSpace() ? "" : button.Ico.IsFontIco() ? "<i class=\"fa " + button.Ico + "\" style=\"font-size:14px;\"></i>" : "<img src=\"" + Url.Content("~" + button.Ico) + "\" alt=\"\" />" },
                    { "Note", button.Note },
                    { "Sort", button.Sort },
                    { "Opation", "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + button.Id + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>" }
                };
                jArray.Add(jObject);
            }

            ViewData["appId"] = Request.Querys("appid");
            ViewData["tabId"] = Request.Querys("tabid");
            ViewData["json"] = jArray.ToString();
            return View();
        }

        [Validate]
        public IActionResult Edit()
        {
            string buttonId = Request.Querys("buttonid");
            Business.FlowButton flowButton = new Business.FlowButton();
            Model.FlowButton flowButtonModel = null;
            if (buttonId.IsGuid(out Guid bid))
            {
                flowButtonModel = flowButton.Get(bid);
            }
            if (null == flowButtonModel)
            {
                flowButtonModel = new Model.FlowButton
                {
                    Id = Guid.NewGuid(),
                    Sort = flowButton.GetMaxSort()
                };
            }
            ViewData["queryString"] = Request.UrlQuery();
            return View(flowButtonModel);
        }

        [Validate]
        [ValidateAntiForgeryToken]
        public string Save(Model.FlowButton flowButtonModel)
        {
            if (!ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(ModelState);
            }
            Business.FlowButton flowButton = new Business.FlowButton();
            if (Request.Querys("buttonid").IsGuid(out Guid guid))
            {
                var oldModel = flowButton.Get(guid);
                string oldJSON = null == oldModel ? "" : oldModel.ToString();
                flowButton.Update(flowButtonModel);
                Business.Log.Add("修改了流程按钮-" + flowButtonModel.Title, type: Business.Log.Type.流程管理, oldContents: oldJSON, newContents: flowButtonModel.ToString());
            }
            else
            {
                flowButton.Add(flowButtonModel);
                Business.Log.Add("添加了流程按钮-" + flowButtonModel.Title, flowButtonModel.ToString(), Business.Log.Type.流程管理);
            }
            return "保存成功!";
        }

        [Validate]
        [ValidateAntiForgeryToken]
        public string Delete()
        {
            string ids = Request.Forms("ids");
            List<Model.FlowButton> flowButtons = new List<Model.FlowButton>();
            Business.FlowButton flowButton = new Business.FlowButton();
            var allButtons = flowButton.GetAll();
            foreach (string id in ids.Split(','))
            {
                if (!id.IsGuid(out Guid bid))
                {
                    continue;
                }
                var buttonModel = allButtons.Find(p => p.Id == bid);
                if (null == buttonModel)
                {
                    continue;
                }
                flowButtons.Add(buttonModel);
            }
            flowButton.Delete(flowButtons.ToArray());
            Business.Log.Add("删除了流程按钮", Newtonsoft.Json.JsonConvert.SerializeObject(flowButtons), Business.Log.Type.流程管理);
            return "删除成功!";
        }
    }
}