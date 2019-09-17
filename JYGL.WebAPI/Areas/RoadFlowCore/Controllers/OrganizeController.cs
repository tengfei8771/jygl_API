using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoadFlow.Utility;
using Microsoft.AspNetCore.Http;

namespace RoadFlow.Mvc.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class OrganizeController : Controller
    {
        public string SetSession()
        {
            string userId = Request.Querys("userid");
            if (!userId.IsGuid())
            {
                return "0";
            }
           // HttpContext.Session.SetString(Config.UserIdSessionKey, userId);
            //HttpContext.Response.Cookies.Append(Config.UserIdSessionKey, userId);
            return "1";
        }

        #region 组织架构相关操作
        [Validate]
        public IActionResult Index()
        {
            var rootOrg = new Business.Organize().GetRoot();
            ViewData["queryString"] = Request.UrlQuery();
            ViewData["bodyUrl"] = null == rootOrg ? string.Empty :
                "Body?orgid=" + rootOrg.Id.ToString() + "&orgparentid=" + rootOrg.ParentId.ToString() + "&type=" + rootOrg.Type.ToString()
                + "&showtype=0&appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            return View();
        }

        [Validate]
        public IActionResult Tree()
        {
            string appId = Request.Querys("appid");
            string tabId = Request.Querys("tabid");

            ViewData["query"] = "appid=" + appId + "&tabid=" + tabId;
            ViewData["appId"] = appId;

            return View();
        }

        public IActionResult Empty()
        {
            return View();
        }

        [Validate]
        public IActionResult Body()
        {
            string orgId = Request.Querys("orgid");
            string parentId = Request.Querys("orgparentid");
            string isAddDept = Request.Querys("isadddept");
            string type = Request.Querys("type");
            string showType = Request.Querys("showtype");
            string appId = Request.Querys("appid");
            string tabId = Request.Querys("tabid");
            Model.Organize organizeModel = null;
            Business.Organize organize = new Business.Organize();
            if (orgId.IsGuid(out Guid guid) && !"1".Equals(isAddDept))
            {
                organizeModel = organize.Get(guid);
            }
            if (null == organizeModel)
            {
                organizeModel = new Model.Organize
                {
                    Id = Guid.NewGuid(),
                    ParentId = orgId.ToGuid(),
                    Sort = organize.GetMaxSort(orgId.ToGuid())
                };
                organizeModel.IntId = organizeModel.Id.ToInt();
                ViewData["parentsName"] = "";
            }
            else
            {
                organizeModel.IntId = organizeModel.Id.ToInt();
                ViewData["parentsName"] = organize.GetParentsName(organizeModel.Id);
            }
            ViewData["orgId"] = orgId;
            ViewData["isAddDept"] = isAddDept;
            ViewData["queryString"] = Request.UrlQuery();
            ViewData["refreshId"] = organizeModel.ParentId;
            ViewData["rootId"] = organize.GetRootId();
            ViewData["returnUrl"] = "Body?orgid=" + orgId + "&orgparentid=" + parentId + "&type=" + type + "&showtype=" + showType + "&appid" + appId + "&tabid=" + tabId;
            return View(organizeModel);
        }

        /// <summary>
        /// 保存机构
        /// </summary>
        /// <param name="organizeModel"></param>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string Save(Model.Organize organizeModel)
        {
            if (!ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(ModelState);
            }
            Business.Organize organize = new Business.Organize();
            if (!"1".Equals(Request.Querys("isadddept")) && Request.Querys("orgid").IsGuid(out Guid guid))
            {
                var oldModel = organize.Get(guid);
                string oldJSON = null == oldModel ? "" : oldModel.ToString();
                organize.Update(organizeModel);
                if (organizeModel.Status == 1)
                {
                    //如果是冻结了机构，则要向企业微信冻结下面的所有人员
                    if(Config.Enterprise_WeiXin_IsUse)
                    {
                        var allUsers = new Business.Organize().GetAllUsers(organizeModel.Id, false);
                        Business.EnterpriseWeiXin.Organize wxOrganize = new Business.EnterpriseWeiXin.Organize();
                        foreach (var user in allUsers)
                        {
                            user.Status = 1;
                            wxOrganize.UpdateUser(user);
                        }
                    }
                }
                Business.Log.Add("修改了组织机构-" + organizeModel.Name, type: Business.Log.Type.系统管理, oldContents: oldJSON, newContents: organizeModel.ToString());
            }
            else
            {
                organize.Add(organizeModel);
                Business.Log.Add("添加了组织机构-" + organizeModel.Name, organizeModel.ToString(), Business.Log.Type.系统管理);
            }
            return "保存成功!";
        }

        /// <summary>
        /// 移动机构
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string DeptMove()
        {
            string toOrgId = Request.Forms("toOrgId");
            string orgid = Request.Querys("orgid");
            if (!toOrgId.IsGuid(out Guid toGuid))
            {
                return "请选择要移动到的组织架构";
            }
            if (!orgid.IsGuid(out Guid orgId))
            {
                return "没有找到当前组织架构";
            }
            if (toGuid == orgId)
            {
                return "不能将自己移动到自己";
            }
            Business.Organize organize = new Business.Organize();
            if (orgId == organize.GetRootId())
            {
                return "不能移动根";
            }
            var org = organize.Get(orgId);
            if (null == org)
            {
                return "没有找到当前组织架构";
            }
            var toOrg = organize.Get(toGuid);
            if (null == toOrg)
            {
                return "没有找到要移动到的组织架构";
            }
            org.ParentId = toGuid;
            org.Sort = organize.GetMaxSort(toGuid);
            organize.Update(org);
            //同步企业微信人员(更新机构下所有人员的职务)
            if (Config.Enterprise_WeiXin_IsUse)
            {
                var allUsers = new Business.Organize().GetAllUsers(org.Id, false);
                Business.EnterpriseWeiXin.Organize wxOrganize = new Business.EnterpriseWeiXin.Organize();
                foreach (var user in allUsers)
                {
                    wxOrganize.UpdateUser(user);
                }
            }
            Business.Log.Add("移动了组织架构-" + org.Name + "到" + toOrg.Name, org.Id + "&" + toOrg.Id, Business.Log.Type.系统管理);
            return "移动成功!";
        }

        /// <summary>
        /// 机构排序
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult Sort()
        {
            string parentId = Request.Querys("orgparentid");
            IEnumerable<Model.Organize> organizes = new List<Model.Organize>();
            if (parentId.IsGuid(out Guid guid))
            {
                organizes = new Business.Organize().GetChilds(guid);
            }
            ViewData["queryString"] = Request.UrlQuery();
            ViewData["refreshId"] = parentId;
            return View(organizes);
        }
        /// <summary>
        /// 保存机构排序
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string SaveSort()
        {
            string sorts = Request.Forms("sort");
            Business.Organize organize = new Business.Organize();
            List<Model.Organize> organizes = new List<Model.Organize>();
            int i = 0;
            foreach (string sort in sorts.Split(','))
            {
                if (sort.IsGuid(out Guid orgId))
                {
                    var org = organize.Get(orgId);
                    if (null != org)
                    {
                        org.Sort = i += 5;
                        organizes.Add(org);
                    }
                }
            }
            organize.Update(organizes.ToArray());
            return "排序成功!";
        }
        /// <summary>
        /// 删除机构
        /// </summary>
        /// <returns></returns>
        public string Delete()
        {
            string orgId = Request.Querys("orgid");
            if (!orgId.IsGuid(out Guid guid))
            {
                return "id错误!";
            }
            if (guid == new Business.Organize().GetRootId())
            {
                return "请勿删除组织机构根!";
            }
            int i = new Business.Organize().Delete(guid);
            return i > 0 ? "删除成功!" : "删除失败!";
        }
        #endregion

        #region 人员相关操作
        /// <summary>
        /// 人员
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult Users()
        {
            string userId = Request.Querys("userid");
            string type = Request.Querys("type");
            string parentId = Request.Querys("orgparentid");
            string orgId = Request.Querys("orgid");
            string appId = Request.Querys("appid");
            string tabId = Request.Querys("tabid");
            string isAddUser = Request.Querys("isadduser");

            Business.User user = new Business.User();
            Model.User userModel = null;
            if (userId.IsGuid(out Guid guid))
            {
                userModel = user.Get(guid);
            }
            if (null == userModel)
            {
                userModel = new Model.User()
                {
                    Id = Guid.NewGuid(),
                    Password = "1"
                };
                ViewData["organizes"] = "";
                ViewData["workgroups"] = "";
            }
            else
            {
                ViewData["organizes"] = user.GetOrganizesShowHtml(userModel.Id);
                ViewData["workgroups"] = user.GetWorkGroupsName(userModel.Id);
            }
            string query = "userid=" + Request.Querys("userid") + "&orgparentid=" + Request.Querys("orgparentid") + "&type=" + Request.Querys("type") +
                "&showtype=" + Request.Querys("showtype") + "&appid=" + Request.Querys("appid") + "&tabid=" + Request.Querys("tabid");
            ViewData["isAddUser"] = isAddUser;
            ViewData["refreshId"] = orgId.IsGuid() ?  orgId : parentId;
            ViewData["queryString"] = Request.UrlQuery();
            ViewData["prevUrl"] = ("Users?" + query).UrlEncode();
            ViewData["returnUrl"] = Request.Querys("returnurl");
            return View(userModel);
        }
        /// <summary>
        /// 检查帐号是否重复
        /// </summary>
        /// <returns></returns>
        public string CheckAccount()
        {
            string id = Request.Querys("id");
            string account = Request.Forms("value");
            if (account.IsNullOrEmpty())
            {
                return "账号不能为空";
            }
            return id.IsGuid(out Guid guid) ? !new Business.User().CheckAccount(account, guid) ? "1" : "帐号重复" : "ID错误";
        }
        /// <summary>
        /// 根据名称得到帐号
        /// </summary>
        /// <returns></returns>
        [Validate]
        public string GetAccountByName()
        {
            string name = Request.Forms("name");
            return name.ToPinYing();
        }
        /// <summary>
        /// 保存人员
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string SaveUser(Model.User userModel)
        {
            if (!ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(ModelState);
            }
            Business.User user = new Business.User();
            if (!"1".Equals(Request.Querys("isadduser")) && Request.Querys("userid").IsGuid(out Guid guid))
            {
                var oldModel = user.Get(guid);
                string oldJSON = null == oldModel ? "" : oldModel.ToString();
                user.Update(userModel);
                //同步企业微信人员
                if (Config.Enterprise_WeiXin_IsUse)
                {
                    try
                    {
                        new Business.EnterpriseWeiXin.Organize().UpdateUser(userModel);
                    }
                    catch(Exception e)
                    {
                        Business.Log.Add(e);
                    }
                }
                Business.Log.Add("修改了人员-" + userModel.Name, type: Business.Log.Type.系统管理, oldContents: oldJSON, newContents: userModel.ToString());
            }
            else
            {
                string orgId = Request.Querys("orgid");
                if (orgId.IsGuid(out Guid orgId1))
                {
                    Model.OrganizeUser organizeUser = new Model.OrganizeUser
                    {
                        Id = Guid.NewGuid(),
                        IsMain = 1,
                        OrganizeId = orgId1,
                        UserId = userModel.Id,
                        Sort = new Business.OrganizeUser().GetMaxSort(orgId1)
                    };
                    userModel.Password = user.GetInitPassword(userModel.Id);
                    user.Add(userModel, organizeUser);
                    //同步企业微信人员
                    if (Config.Enterprise_WeiXin_IsUse)
                    {
                        try
                        {
                            new Business.EnterpriseWeiXin.Organize().AddUser(userModel);
                        }
                        catch (Exception e)
                        {
                            Business.Log.Add(e);
                        }
                    }
                    Business.Log.Add("添加了人员-" + userModel.Name, userModel.ToString() + "-" + organizeUser.ToString(), Business.Log.Type.系统管理);
                }
                else
                {
                    return "未找到人员对应的组织架构";
                }
            }
            return "保存成功!";
        }
        /// <summary>
        /// 初始化人员密码
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string InitUserPassword()
        {
            string userId = Request.Querys("userid");
            if (!userId.IsGuid(out Guid guid))
            {
                return "用户ID错误!";
            }
            bool success = new Business.User().InitUserPassword(guid);
            Business.Log.Add("初始化了人员密码-" + userId, success ? "初始化成功!" : "初始化失败!", Business.Log.Type.系统管理);
            return success ? "初始化成功!" : "初始化失败!";
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string DeleteUser()
        {
            string userId = Request.Querys("userid");
            if (!userId.IsGuid(out Guid guid))
            {
                return "用户ID错误";
            }
            new Business.User().Delete(guid);
            return "删除成功!";
        }
        /// <summary>
        /// 人员排序
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult UserSort()
        {
            string orgId = Request.Querys("orgparentid");
            Business.Organize organize = new Business.Organize();
            Business.User user = new Business.User();
            var users = organize.GetAllUsers(orgId.ToGuid());

            ViewData["queryString"] = Request.UrlQuery();
            ViewData["refreshId"] = orgId;
            return View(users);
        }
        /// <summary>
        /// 保存人员排序
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string UserSortSave()
        {
            string sort = Request.Forms("sort");
            string orgId = Request.Querys("orgparentid");
            Business.OrganizeUser organizeUser = new Business.OrganizeUser();
            List<Model.OrganizeUser> organizeUsers = organizeUser.GetListByOrganizeId(orgId.ToGuid());
            int i = 0;
            foreach (string id in sort.Split(','))
            {
                if (id.IsGuid(out Guid userId))
                {
                    var organizeUserModel = organizeUsers.Find(p => p.UserId == userId);
                    if (null != organizeUserModel)
                    {
                        organizeUserModel.Sort = i += 5;
                    }
                }
            }
            organizeUser.Update(organizeUsers.ToArray());
            return "排序成功!";
        }
        /// <summary>
        /// 调动人员
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string MoveUser()
        {
            string toOrgId = Request.Forms("toOrgId");
            string isjz = Request.Forms("isjz");
            string userId = Request.Querys("userid");
            if (!toOrgId.IsGuid(out Guid toId))
            {
                return "没有选择要调往的组织";
            }
            var organizeModel = new Business.Organize().Get(toId);
            if (null == organizeModel)
            {
                return "没有找到要调往的组织";
            }
            if (!userId.IsGuid(out Guid uId))
            {
                return "人员ID错误";
            }
            Business.OrganizeUser organizeUser = new Business.OrganizeUser();
            if ("1".Equals(isjz))//兼职调动
            {
                Model.OrganizeUser organizeUserModel = new Model.OrganizeUser
                {
                    Id = Guid.NewGuid(),
                    IsMain = 0,
                    OrganizeId = toId,
                    UserId = uId,
                    Sort = organizeUser.GetMaxSort(organizeModel.ParentId)
                };
                organizeUser.Add(organizeUserModel);
            }
            else //全职
            {
                List<Tuple<Model.OrganizeUser, int>> tuples = new List<Tuple<Model.OrganizeUser, int>>();
                var organizeUsers = organizeUser.GetListByUserId(uId);
                foreach (var organizeUserModel in organizeUsers)
                {
                    if (organizeUserModel.IsMain == 1)
                    {
                        organizeUserModel.OrganizeId = toId;
                        organizeUserModel.Sort = organizeUser.GetMaxSort(organizeModel.ParentId);
                        tuples.Add(new Tuple<Model.OrganizeUser, int>(organizeUserModel, 2));
                    }
                    else
                    {
                        tuples.Add(new Tuple<Model.OrganizeUser, int>(organizeUserModel, 0));
                    }
                }
                organizeUser.Update(tuples);
                //同步企业微信人员(更新人员所在职务)
                if (Config.Enterprise_WeiXin_IsUse)
                {
                    new Business.EnterpriseWeiXin.Organize().UpdateUser(new Business.User().Get(uId));
                }
            }
            Business.Log.Add("调动了人员-" + uId + ("1".Equals(isjz) ? "-兼任" : "-全职"), toOrgId, Business.Log.Type.系统管理);
            return "调动成功!";
        }
        #endregion

        #region 工作组相关操作
        [Validate]
        public IActionResult WorkGroup()
        {
            string workgroupid = Request.Querys("workgroupid");
            string type = Request.Querys("type");
            string showtype = Request.Querys("showtype");
            string appid = Request.Querys("appid");
            string tabid = Request.Querys("tabid");
            string query = "type=" + type + "&showtype=" + showtype + "&appid=" + appid + "&tabid=" + tabid;

            Model.WorkGroup workGroupModel = null;
            Business.WorkGroup workGroup = new Business.WorkGroup();
            if (workgroupid.IsGuid(out Guid wid))
            {
                workGroupModel = workGroup.Get(wid);
            }
            if (null == workGroupModel)
            {
                workGroupModel = new Model.WorkGroup() {
                    Id = Guid.NewGuid(),
                    Sort = workGroup.GetMaxSort()
                };
                workGroupModel.IntId = workGroupModel.Id.ToInt();
            }

            ViewData["query"] = query;
            ViewData["queryString"] = Request.UrlQuery();
            ViewData["prevUrl"] = ("WorkGroup" + Request.UrlQuery()).UrlEncode();
            return View(workGroupModel);
        }
        /// <summary>
        /// 保存工作组
        /// </summary>
        /// <param name="workGroupModel"></param>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string SaveWorkGroup(Model.WorkGroup workGroupModel)
        {
            if (!ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(ModelState);
            }
            Business.WorkGroup workGroup = new Business.WorkGroup();
            string workgroupid = Request.Querys("workgroupid");
            if (workgroupid.IsGuid(out Guid wid))
            {
                var oldModel = workGroup.Get(wid);
                string oldJSON = null == oldModel ? "" : oldModel.ToString();
                workGroup.Update(workGroupModel);
                Business.Log.Add("修改了工作组-" + workGroupModel.Name, "", Business.Log.Type.系统管理, oldJSON, workGroupModel.ToString());
            }
            else
            {
                workGroup.Add(workGroupModel);
                Business.Log.Add("添加了工作组-" + workGroupModel.Name, workGroupModel.ToString(), Business.Log.Type.系统管理);
            }
            return "保存成功!";
        }
        /// <summary>
        /// 删除工作组
        /// </summary>
        /// <returns></returns>
        [Validate]
        [ValidateAntiForgeryToken]
        public string DeleteWorkGroup()
        {
            string workgroupid = Request.Querys("workgroupid");
            if (!workgroupid.IsGuid(out Guid wid))
            {
                return "ID错误";
            }
            Business.WorkGroup workGroup = new Business.WorkGroup();
            var workGroupModel = workGroup.Get(wid);
            if (null == workGroupModel)
            {
                return "未找到要删除的工作组";
            }
            workGroup.Delete(workGroupModel);
            Business.Log.Add("删除了工作组-" + workGroupModel.Name, workGroupModel.ToString(), Business.Log.Type.系统管理);
            return "删除成功!";
        }
        [Validate]
        public IActionResult WorkGroupSort()
        {
            List<Model.WorkGroup> workGroups = new Business.WorkGroup().GetAll();
            ViewData["queryString"] = Request.UrlQuery();
            return View(workGroups);
        }
        [Validate]
        [ValidateAntiForgeryToken]
        public string WorkGroupSortSave()
        {
            Business.WorkGroup workGroup = new Business.WorkGroup();
            var all = workGroup.GetAll();
            string sorts = Request.Forms("sort");
            int i = 0;
            foreach (string sort in sorts.Split(','))
            {
                if (!sort.IsGuid(out Guid wid))
                {
                    continue;
                }
                var workGroupModel = all.Find(p => p.Id == wid);
                if (null != workGroupModel)
                {
                    workGroupModel.Sort = i += 5;
                }
            }
            workGroup.Update(all.ToArray());
            return "排序成功!"; 
        }
        #endregion

        #region 设置菜单
        [Validate]
        public IActionResult SetMenu()
        {
            string id = Request.Querys("orgid");
            if (id.IsNullOrWhiteSpace())
            {
                id = Request.Querys("workgroupid");
                if (!id.IsNullOrWhiteSpace())
                {
                    id = Business.Organize.PREFIX_WORKGROUP + id;
                }
            }
            if (id.IsNullOrWhiteSpace())
            {
                id = Request.Querys("userid");
                if (!id.IsNullOrWhiteSpace())
                {
                    id = Business.Organize.PREFIX_USER + id;
                }
            }
            ViewData["tableHtml"] = new Business.Menu().GetMenuTreeTableHtml(id);
            ViewData["prevUrl"] = Request.Querys("prevurl").UrlDecode();
            ViewData["queryString"] = Request.UrlQuery();
            return View();
        }
        [Validate]
        [ValidateAntiForgeryToken]
        public string SaveSetMenu()
        {
            string id = Request.Querys("orgid");
            if (id.IsNullOrWhiteSpace())
            {
                id = Request.Querys("workgroupid");
                if (!id.IsNullOrWhiteSpace())
                {
                    id = Business.Organize.PREFIX_WORKGROUP + id;
                }
            }
            if (id.IsNullOrWhiteSpace())
            {
                id = Request.Querys("userid");
                if (!id.IsNullOrWhiteSpace())
                {
                    id = Business.Organize.PREFIX_USER + id;
                }
            }
            Business.MenuUser menuUser = new Business.MenuUser();
            Business.Organize organize = new Business.Organize();
            string menuids = Request.Forms("menuid");
            List<Model.MenuUser> menuUsers = new List<Model.MenuUser>();
            foreach (string menuId in menuids.Split(','))
            {
                if (!menuId.IsGuid(out Guid mid))
                {
                    continue;
                }
                Model.MenuUser menuUserModel = new Model.MenuUser()
                {
                    Id = Guid.NewGuid(),
                    Buttons = Request.Forms("button_"+menuId),
                    MenuId = mid,
                    Organizes = id,
                    Params = Request.Forms("params_"+menuId)
                };
                menuUsers.Add(menuUserModel);
            }
            int i = menuUser.Update(menuUsers.ToArray(), id);
            Business.Log.Add("设置了组织架构菜单-" + id, "影响行数:" + i.ToString(), Business.Log.Type.系统管理);
            return "设置成功!";
        }
        [Validate(CheckApp = false)]
        public IActionResult ShowUserMenu()
        {
            string userName = string.Empty;
            string userId = Request.Querys("userid");
            if (userId.IsGuid(out Guid guid))
            {
                userName = new Business.User().GetName(guid);
            }
            ViewData["prevUrl"] = Request.Querys("prevUrl");
            ViewData["userName"] = userName;
            ViewData["userId"] = userId;
            return View();
        }
        #endregion

        #region 加载组织机构树JSON
        [Validate(CheckApp = false)]
        public string Tree1()
        {
            int showType = Request.Querys("showtype").ToString().ToInt(0);//显示类型 0组织架构 1角色组
            string rootId = Request.Querys("rootid");
            string searchWord = Request.Querys("searchword");
            bool showUser = !"0".Equals(Request.Querys("shouser"));

            Business.Organize organize = new Business.Organize();
            Business.User user = new Business.User();
            Business.WorkGroup workGroup = new Business.WorkGroup();
            Business.OrganizeUser organizeUser = new Business.OrganizeUser();
            var organizeUserList = organizeUser.GetAll();
            Guid orgRootId = organize.GetRootId();

            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();

            #region 搜索
            if (!searchWord.IsNullOrWhiteSpace())
            {
                Guid parentId = Guid.NewGuid();
                if (1 == showType)//搜索工作组
                {
                    var workGroups = workGroup.GetAll().FindAll(p => p.Name.ContainsIgnoreCase(searchWord.Trim()));
                    Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject()
                    {
                        { "id", parentId},
                        { "parentID", Guid.Empty},
                        { "title", "查询“"+searchWord+"”的结果"},
                        { "ico", "fa-search"},
                        { "link", ""},
                        { "type", 1},
                        { "hasChilds", workGroups.Count}
                    };
                    jArray.Add(jObject);
                    Newtonsoft.Json.Linq.JArray jArray1 = new Newtonsoft.Json.Linq.JArray();
                    foreach (var group in workGroups)
                    {
                        Newtonsoft.Json.Linq.JObject jObject1 = new Newtonsoft.Json.Linq.JObject()
                        {
                            { "id", group.Id},
                            { "parentID", parentId},
                            { "title", group.Name},
                            { "ico", "fa-slideshare"},
                            { "link", ""},
                            { "type", 5},
                            { "hasChilds", 0}
                        };
                        jArray1.Add(jObject1);
                    }
                    jObject.Add("childs", jArray1);
                }
                else //搜索组织和人员
                {
                    var organizes = organize.GetAll().FindAll(p => p.Name.ContainsIgnoreCase(searchWord.Trim()));
                    var users = user.GetAll().FindAll(p => p.Name.ContainsIgnoreCase(searchWord.Trim()));
                    Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject()
                    {
                        { "id", parentId},
                        { "parentID", Guid.Empty},
                        { "title", "查询“"+searchWord+"”的结果"},
                        { "ico", "fa-search"},
                        { "link", ""},
                        { "type", 1},
                        { "hasChilds", organizes.Count + users.Count}
                    };
                    jArray.Add(jObject);
                    Newtonsoft.Json.Linq.JArray jArray1 = new Newtonsoft.Json.Linq.JArray();
                    foreach (var organizeModel in organizes)
                    {
                        Newtonsoft.Json.Linq.JObject jObject1 = new Newtonsoft.Json.Linq.JObject()
                        {
                            { "id", organizeModel.Id},
                            { "parentID", parentId},
                            { "title", organizeModel.Name},
                            { "ico", ""},
                            { "link", ""},
                            { "type", organizeModel.Type},
                            { "hasChilds", organize.HasChilds(organizeModel.Id) ? 1 : 0}
                        };
                        jArray1.Add(jObject1);
                    }
                    foreach (var userModel in users)
                    {
                        Newtonsoft.Json.Linq.JObject jObject1 = new Newtonsoft.Json.Linq.JObject()
                        {
                            { "id", userModel.Id},
                            { "parentID", parentId},
                            { "title", userModel.Name},
                            { "ico", "fa-user"},
                            { "link", ""},
                            { "userID", userModel.Id},
                            { "type", 4},
                            { "hasChilds", 0}
                        };
                        jArray1.Add(jObject1);
                    }
                    jObject.Add("childs", jArray1);
                }
                return jArray.ToString();
            }
            #endregion

            #region 显示角色组
            if (1 == showType)
            {
                var workgroups = workGroup.GetAll();
                foreach (var workgroupModel in workgroups)
                {
                    Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject()
                    {
                        { "id", workgroupModel.Id},
                        { "parentID", Guid.Empty},
                        { "title", workgroupModel.Name},
                        { "ico", "fa-slideshare"},
                        { "link", ""},
                        { "type", 5},
                        { "hasChilds", 0}
                    };
                    jArray.Add(jObject);
                }
                return jArray.ToString();
            }
            #endregion

            if (rootId.IsNullOrWhiteSpace())
            {
                rootId = orgRootId.ToString();
            }
            #region 添加根节点
            string[] rootIdArray = rootId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string root in rootIdArray)
            {
                if (root.IsGuid(out Guid guid))//组织机构
                {
                    var organizeModel = organize.Get(guid);
                    if (null != organizeModel)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject()
                        {
                            { "id", organizeModel.Id},
                            { "parentID", organizeModel.ParentId},
                            { "title", organizeModel.Name},
                            { "ico", organizeModel.Id == orgRootId ? "fa-sitemap" : ""},
                            { "link", ""},
                            { "type", organizeModel.Type},
                            { "hasChilds", organize.HasChilds(organizeModel.Id) ? 1 : showUser ? organize.HasUsers(organizeModel.Id) ? 1 : 0 : 0}
                        };
                        jArray.Add(jObject);
                    }
                }
                else if (root.StartsWith(Business.Organize.PREFIX_USER))//人员
                {
                    var userModel = user.Get(root.RemoveUserPrefix().ToGuid());
                    if (null != userModel)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject()
                        {
                            { "id", userModel.Id},
                            { "parentID", Guid.Empty},
                            { "title", userModel.Name},
                            { "ico", "fa-user"},
                            { "link", ""},
                            { "type", 4},
                            { "hasChilds", 0}
                        };
                        jArray.Add(jObject);
                    }
                }
                else if (root.StartsWith(Business.Organize.PREFIX_RELATION))//兼职人员
                {
                    var organizeUserModel = organizeUser.Get(root.RemoveUserRelationPrefix().ToGuid());
                    if (null != organizeUserModel)
                    {
                        var userModel = user.Get(organizeUserModel.UserId);
                        if (null != userModel)
                        {
                            Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject()
                            {
                                { "id", organizeUserModel.Id},
                                { "parentID", Guid.Empty},
                                { "title", userModel.Name + "<span style='color:#666;'>[兼任]</span>"},
                                { "ico", "fa-user"},
                                { "link", ""},
                                { "type", 6},
                                { "userID", userModel.Id},
                                { "hasChilds", 0}
                            };
                            jArray.Add(jObject);
                        }
                    }
                }
                else if (root.StartsWith(Business.Organize.PREFIX_WORKGROUP))//工作组
                {
                    var workgroupModel = workGroup.Get(root.RemoveWorkGroupPrefix().ToGuid());
                    if (null != workgroupModel)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject()
                        {
                            { "id", workgroupModel.Id},
                            { "parentID", Guid.Empty},
                            { "title", workgroupModel.Name},
                            { "ico", "fa-slideshare"},
                            { "link", ""},
                            { "type", 5},
                            { "hasChilds", workGroup.GetAllUsers(workgroupModel.Id).Count}
                        };
                        jArray.Add(jObject);
                    }
                }
            }
            #endregion

            #region 只有一个根时显示二级
            if (rootIdArray.Length == 1)
            {
                string rootIdString = rootIdArray[0];
                if (rootIdString.IsGuid(out Guid guid))
                {
                    var jObject0 = (Newtonsoft.Json.Linq.JObject)jArray[0];
                    Newtonsoft.Json.Linq.JArray jArray0 = new Newtonsoft.Json.Linq.JArray();
                    var childs = organize.GetChilds(guid);
                    var organizeUser1 = organizeUserList.FindAll(p => p.OrganizeId == guid);
                    foreach (var child in childs)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject()
                        {
                            { "id", child.Id},
                            { "parentID", child.ParentId},
                            { "title", child.Name},
                            { "ico", ""},
                            { "link", ""},
                            { "type", child.Type},
                            { "hasChilds", organize.HasChilds(child.Id) ? 1 : showUser ? organize.HasUsers(child.Id) ? 1 : 0 : 0}
                        };
                        jArray0.Add(jObject);
                    }
                    if (showUser)
                    {
                        var users = organize.GetUsers(guid);
                        foreach (var userModel in users)
                        {
                            var organizeUserModel1 = organizeUser1.Find(p => p.UserId == userModel.Id);
                            bool isPartTime = organizeUserModel1.IsMain != 1;//是否是兼职
                            Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject()
                            {
                                { "id", isPartTime ? organizeUserModel1.Id : userModel.Id},
                                { "parentID", guid},
                                { "title", userModel.Name + (isPartTime ? "<span style='color:#666;'>[兼任]</span>" : "")},
                                { "ico", "fa-user"},
                                { "link", ""},
                                { "userID", userModel.Id},
                                { "type", isPartTime ? 6 : 4},
                                { "hasChilds", 0}
                            };
                            jArray0.Add(jObject);
                        }
                    }
                    jObject0.Add("childs", jArray0);
                }
                else if (rootIdString.StartsWith(Business.Organize.PREFIX_WORKGROUP))
                {
                    var jObject0 = (Newtonsoft.Json.Linq.JObject)jArray[0];
                    Newtonsoft.Json.Linq.JArray jArray0 = new Newtonsoft.Json.Linq.JArray();
                    var users = workGroup.GetAllUsers(rootIdString.RemoveWorkGroupPrefix().ToGuid());
                    foreach (var userModel in users)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject()
                        {
                            { "id", userModel.Id},
                            { "parentID", rootIdString.RemoveWorkGroupPrefix()},
                            { "title", userModel.Name},
                            { "ico", "fa-user"},
                            { "link", ""},
                            { "type", 4},
                            { "hasChilds", 0}
                        };
                        jArray0.Add(jObject);
                    }
                    jObject0.Add("childs", jArray0);
                }
            }
            #endregion

            return jArray.ToString();
        }

        [Validate(CheckApp = false)]
        public string TreeRefresh()
        {
            int showType = Request.Querys("showtype").ToString().ToInt(0);
            bool showUser = !"0".Equals(Request.Querys("shouser"));
            string refreshId = Request.Querys("refreshid");
            Business.Organize organize = new Business.Organize();
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();

            #region 显示工作组
            if (1 == showType)
            {
                return "";
            }
            #endregion

            if (refreshId.IsGuid(out Guid guid))
            {
                var childs = organize.GetChilds(guid);
                foreach (var child in childs)
                {
                    Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject()
                    {
                        { "id", child.Id},
                        { "parentID", child.ParentId},
                        { "title", child.Name},
                        { "ico", ""},
                        { "link", ""},
                        { "type", child.Type},
                        { "hasChilds", organize.HasChilds(child.Id) ? 1 : showUser ? organize.HasUsers(child.Id) ? 1 : 0 : 0}
                    };
                    jArray.Add(jObject);
                }
                if (showUser)
                {
                    var users = organize.GetUsers(guid);
                    var organizeUsers = new Business.OrganizeUser().GetListByOrganizeId(guid);
                    foreach (var userModel in users)
                    {
                        var organizeUserModel1 = organizeUsers.Find(p => p.UserId == userModel.Id);
                        bool isPartTime = organizeUserModel1.IsMain != 1;//是否是兼职
                        Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject()
                        {
                            { "id", isPartTime ? organizeUserModel1.Id : userModel.Id},
                            { "parentID", guid},
                            { "title", userModel.Name + (isPartTime ? "<span style='color:#666;'>[兼任]</span>" : "")},
                            { "ico", "fa-user"},
                            { "link", ""},
                            { "userID", userModel.Id},
                            { "type", isPartTime ? 6 : 4},
                            { "hasChilds", 0}
                        };
                        jArray.Add(jObject);
                    }
                }
            }
            return jArray.ToString();
        }
        #endregion
    }
}