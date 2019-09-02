using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoadFlow.Utility;

namespace RoadFlow.Mvc.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class FlowCommentController : Controller
    {
        [Validate]
        public IActionResult Index()
        {
            bool isoneself = "1".Equals(Request.Querys("isoneself"));
            Guid userId = Current.UserId;
            var all = new Business.FlowComment().GetAll();
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            Business.User user = new Business.User();
            foreach (var comment in all)
            {
                if (isoneself && comment.UserId != userId)
                {
                    continue;
                }
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject
                {
                    { "id", comment.Id },
                    { "Comments", comment.Comments },
                    { "UserId", !comment.UserId.IsEmptyGuid() ? user.GetName(comment.UserId) : "全部人员" },
                    { "AddType", comment.AddType == 0 ? "用户添加" : "管理员添加" },
                    { "Sort", comment.Sort },
                    { "Opation", "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + comment.Id + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>" }
                };

                jArray.Add(jObject);
            }

            ViewData["json"] = jArray.ToString();
            ViewData["appId"] = Request.Querys("appid");
            ViewData["tabId"] = Request.Querys("tabid");
            ViewData["isoneself"] = Request.Querys("isoneself");
            return View();
        }

        [Validate]
        public IActionResult Edit()
        {
            Business.FlowComment flowComment = new Business.FlowComment();
            Model.FlowComment flowCommentModel = null;
            string commentId = Request.Querys("commentid");
            string isOneSelf = Request.Querys("isoneself"); 
            if (commentId.IsGuid(out Guid cid))
            {
                flowCommentModel = flowComment.Get(cid);
            }
            if (null == flowCommentModel)
            {
                flowCommentModel = new Model.FlowComment
                {
                    Id = Guid.NewGuid(),
                    Sort = flowComment.GetMaxSort(),
                    AddType = "1".Equals(isOneSelf) ? 0 : 1
                };
                if ("1".Equals(isOneSelf))
                {
                    flowCommentModel.UserId = Current.UserId;
                }
            }
            ViewData["isOneSelf"] = isOneSelf;
            ViewData["queryString"] = Request.UrlQuery();
            return View(flowCommentModel);
        }

        [Validate]
        [ValidateAntiForgeryToken]

        public string Save(Model.FlowComment flowCommentModel)
        {
            if (!Request.Forms("UserId").IsNullOrWhiteSpace())
            {
                flowCommentModel.UserId = new Business.User().GetUserId(Request.Forms("UserId"));
            }
            Business.FlowComment flowComment = new Business.FlowComment();
            if (Request.Querys("commentid").IsGuid(out Guid guid))
            {
                var oldModel = flowComment.Get(guid);
                string oldJSON = null == oldModel ? "" : oldModel.ToString();
                flowComment.Update(flowCommentModel);
                Business.Log.Add("修改了流程意见-" + flowCommentModel.Id, type: Business.Log.Type.系统管理, oldContents: oldJSON, newContents: flowCommentModel.ToString());
            }
            else
            {
                flowComment.Add(flowCommentModel);
                Business.Log.Add("添加了流程意见-" + flowCommentModel.Id, flowCommentModel.ToString(), Business.Log.Type.系统管理);
            }
            return "保存成功!";
        }

        [Validate]
        [ValidateAntiForgeryToken]
        public string Delete()
        {
            string ids = Request.Forms("ids");
            List<Model.FlowComment> flowComments = new List<Model.FlowComment>();
            Business.FlowComment flowComment = new Business.FlowComment();
            var all = flowComment.GetAll();
            foreach (string id in ids.Split(','))
            {
                if (!id.IsGuid(out Guid fid))
                {
                    continue;
                }
                var comment = all.Find(p => p.Id == fid);
                if (null == comment)
                {
                    continue;
                }
                flowComments.Add(comment);
            }

            flowComment.Delete(flowComments.ToArray());
            Business.Log.Add("删除了流程意见", Newtonsoft.Json.JsonConvert.SerializeObject(flowComments), Business.Log.Type.流程管理);
            return "删除成功!";
        }
    }
}