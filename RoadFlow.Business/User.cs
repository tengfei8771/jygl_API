using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace RoadFlow.Business
{
    public class User
    {
        private readonly Data.User userData;
        public User()
        {
            userData = new Data.User();
        }
        /// <summary>
        /// 得到系统所有用户
        /// </summary>
        /// <returns></returns>
        public List<Model.User> GetAll()
        {
            return new Integrate.Organize().GetAllUser();
        }

        /// <summary>
        /// 当前登录用户ID
        /// </summary>
        public static Guid CurrentUserId
        {
            get
            {
                var context = Tools.HttpContext;
                if (null == context)
                {
                    return Guid.Empty;
                }
                string id = context.Session.GetString(Config.UserIdSessionKey);
                if (id.IsGuid(out Guid uid))
                {
                    return uid;
                }
                string AccessToken = context.Request.Cookies["Admin-Token"];
                string userId = UIDP.UTILITY.AccessTokenTool.GetUserId(AccessToken);
                if (userId.IsGuid(out Guid uid1))
                {
                    return uid1;
                }
                if (Config.IsDebug && Config.DebugUserId.IsGuid(out Guid debugUserId))
                {
                    return debugUserId;
                }
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 当前登录用户实体
        /// </summary>
        public static Model.User CurrentUser
        {
            get
            {
                Guid userId = CurrentUserId;
                userId = userId.IsEmptyGuid() ? EnterpriseWeiXin.Common.GetUserId() : userId;
                return userId.IsEmptyGuid() ? null : new User().Get(userId);
            }
        }

        /// <summary>
        /// 当前登录用户姓名
        /// </summary>
        public static string CurrentUserName
        {
            get
            {
                var userModel = CurrentUser;
                return null == CurrentUser ? string.Empty : CurrentUser.Name;
            }
        }

        /// <summary>
        /// 添加一个用户
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <param name="organizeUser">用户与架构关系实体</param>
        /// <returns></returns>
        public int Add(Model.User user, Model.OrganizeUser organizeUser)
        {
            int i = userData.Add(user, organizeUser);
            //更新菜单
            new MenuUser().UpdateAllUseUserAsync();
            return i;
        }
        /// <summary>
        /// 删除一个用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(Guid id)
        {
            var organizeUsers = new OrganizeUser().GetListByUserId(id);//要删除人员与组织机构之间关系表
            var userModel = Get(id);
            if (null == userModel)
            {
                return 0;
            }
            //同步企业微信人员
            if (Config.Enterprise_WeiXin_IsUse)
            {
                new EnterpriseWeiXin.Organize().DeleteUser(userModel.Account);
            }
            Log.Add("删除了一个用户-" + userModel.Name, userModel.ToString(), Log.Type.系统管理, others: Newtonsoft.Json.JsonConvert.SerializeObject(organizeUsers));
            return userData.Delete(userModel, organizeUsers.ToArray());
        }
        /// <summary>
        /// 更新一个用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Update(Model.User user)
        {
            return userData.Update(user);
        }
        /// <summary>
        /// 根据ID查询一个用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.User Get(Guid id)
        {
            return GetAll().Find(p => p.Id == id);
        }
        /// <summary>
        /// 得到一个用户实体
        /// </summary>
        /// <param name="id">如果id以r_开头则表示是兼职organizeuser表id</param>
        /// <returns></returns>
        public Model.User Get(string id)
        {
            Guid userId = Guid.Empty;
            Guid orgUserId = Guid.Empty;
            if (id.IsGuid(out Guid uid))
            {
                userId = uid;
            }
            else if (id.StartsWith(Organize.PREFIX_USER))
            {
                userId = id.RemoveUserPrefix().ToGuid();
            }
            else if (id.StartsWith(Organize.PREFIX_RELATION))
            {
                var model = new OrganizeUser().Get(id.RemoveUserRelationPrefix().ToGuid());
                if (null != model)
                {
                    userId = model.UserId;
                    orgUserId = model.Id;
                }
            }
            if (userId.IsEmptyGuid())
            {
                return null;
            }
            else
            {
                var userModel = Get(userId);
                if (null != userModel && orgUserId.IsNotEmptyGuid())
                {
                    userModel.PartTimeId = orgUserId;
                }
                return userModel;
            }
            
        }
        /// <summary>
        /// 根据兼职ID得到一个用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.User GetByRelationId(string id)
        {
            if (id.StartsWith(Organize.PREFIX_RELATION))
            {
                var organizeUserModel = new OrganizeUser().Get(id.RemoveUserRelationPrefix().ToGuid());
                return null != organizeUserModel ? Get(organizeUserModel.UserId) : null;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据帐号查询一个用户
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <returns></returns>
        public Model.User GetByAccount(string account)
        {
            return GetAll().Find(p => p.Account.EqualsIgnoreCase(account));
        }
        /// <summary>
        /// 根据微信openid查询一个用户
        /// </summary>
        /// <param name="openId">openId</param>
        /// <returns></returns>
        public Model.User GetByWeiXinOpenId(string openId)
        {
            return GetAll().Find(p => !p.WeiXinOpenId.IsNullOrWhiteSpace() && p.WeiXinOpenId.Equals(openId));
        }
        /// <summary>
        /// 得到用户头像图片路径
        /// </summary>
        /// <param name="user"></param>
        /// <param name="wwwrootPath">资源目录路径</param>
        /// <returns></returns>
        public string GetHeadImageSrc(Model.User user, string wwwrootPath)
        {
            if (null == user)
            {
                return "";
            }
            string src = user.HeadImg;
            if (!src.IsNullOrWhiteSpace() && System.IO.File.Exists(wwwrootPath + src))
            {
                return src;
            }
            else
            {
                return "/RoadFlowResources/Images/userHeads/default.jpg";
            }
        }
        /// <summary>
        /// 得到用户加密后的密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public string GetMD5Password(Guid userId, string password)
        {
            return (userId.ToString().ToUpper() + password).MD5();
        }
        /// <summary>
        /// 判断一个用户是否被冻结（所在岗位部门被冻结也算）
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsFrozen(Model.User user)
        {
            if (user.Status != 0)
            {
                return true;
            }
            var organzieUsers = new OrganizeUser().GetListByUserId(user.Id);
            Organize organize = new Organize();
            foreach (var organizeUser in organzieUsers)
            {
                var organizeModel = organize.Get(organizeUser.OrganizeId);
                if (null != organizeModel)
                {
                    if (organizeModel.Status != 0 || organize.GetAllParents(organizeModel.Id, false).Exists(p => p.Status != 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 检查一个帐号是否重复
        /// </summary>
        /// <param name="account">帐号</param>
        /// <param name="id">人员ID</param>
        /// <returns></returns>
        public bool CheckAccount(string account, Guid id)
        {
            var user = GetByAccount(account);
            if (null == user)
            {
                return false;
            }
            return user.Id != id;
        }
        /// <summary>
        /// 得到添加一个人员添加时的初始密码
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <returns></returns>
        public string GetInitPassword(Guid id)
        {
            return GetMD5Password(id, Config.InitUserPassword);
        }
        /// <summary>
        /// 初始化人员密码
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <returns></returns>
        public bool InitUserPassword(Guid id)
        {
            var userModel = Get(id);
            if (null == userModel)
            {
                return false;
            }
            userModel.Password = GetInitPassword(id);
            Update(userModel);
            return true;
        }
        /// <summary>
        /// 得到一个人员的主要组织显示
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <param name="isShowRoot">是否显示根</param>
        /// <returns></returns>
        public string GetOrganizeMainShowHtml(Guid id, bool isShowRoot = true)
        {
            var organizeUser = new OrganizeUser().GetMainByUserId(id);
            if (null == organizeUser)
            {
                return "";
            }
            else
            {
                Organize organize = new Organize();
                string parentsName = organize.GetParentsName(organizeUser.OrganizeId, isShowRoot);
                return (parentsName.IsNullOrWhiteSpace() ? "" : parentsName + " \\ ") + organize.GetName(organizeUser.OrganizeId);
            }
        }
        /// <summary>
        /// 得到一个人员的所在组织显示
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <param name="isShowRoot">是否显示根</param>
        /// <returns></returns>
        public string GetOrganizesShowHtml(Guid id, bool isShowRoot = true)
        {
            var organizeUsers = new OrganizeUser().GetListByUserId(id);
            StringBuilder stringBuilder = new StringBuilder();
            Organize organize = new Organize();
            foreach (var organizeUser in organizeUsers)
            {
                var parentsName = organize.GetParentsName(organizeUser.OrganizeId, isShowRoot);
                stringBuilder.Append("<div>");
                if (!parentsName.IsNullOrWhiteSpace())
                {
                    stringBuilder.Append(parentsName + " \\ ");
                }
                stringBuilder.Append(organize.GetName(organizeUser.OrganizeId));
                if (organizeUser.IsMain == 0)
                {
                    stringBuilder.Append("<span style='color:#999;'>[兼任]</span>");
                }
                stringBuilder.Append("</div>");
            }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// 得到用户实际ID（有可能字符串是r_关系ID）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Guid GetUserId(string userId)
        {
            if (userId.IsNullOrWhiteSpace())
            {
                return Guid.Empty;
            }
            else if (userId.StartsWith(Organize.PREFIX_USER))
            {
                return userId.RemoveUserPrefix().ToGuid();
            }
            else if (userId.IsGuid(out Guid uid))
            {
                return uid;
            }
            else if (userId.StartsWith(Organize.PREFIX_RELATION))
            {
                var orgId = userId.RemoveUserRelationPrefix().ToGuid();
                var orgUser = new OrganizeUser().Get(orgId);
                if (null != orgUser)
                {
                    return orgUser.UserId;
                }
            }
            return Guid.Empty;
        }
        /// <summary>
        /// 根据ID得到一个人员的姓名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetName(Guid id)
        {
            var userModel = Get(id);
            return null == userModel ? string.Empty : userModel.Name;
        }

        /// <summary>
        /// 根据一组ID字符串得到人员的姓名
        /// </summary>
        /// <param name="id">逗号分开的ID</param>
        /// <returns></returns>
        public string GetNames(string ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string id in ids.Split(','))
            {
                string userId = id;
                if (id.StartsWith(Organize.PREFIX_USER))
                {
                    userId = id.RemoveUserPrefix();
                }
                else if (id.StartsWith(Organize.PREFIX_RELATION))
                {
                    var organizeUser = new OrganizeUser().Get(id.RemoveUserRelationPrefix().ToGuid());
                    if (null != organizeUser)
                    {
                        userId = organizeUser.UserId.ToString();
                    }
                }
                if (!userId.IsGuid(out Guid uid))
                {
                    continue;
                }
                var userModel = Get(uid);
                if (null != userModel)
                {
                    stringBuilder.Append(userModel.Name);
                    stringBuilder.Append("、");
                }
            }
            return stringBuilder.ToString().TrimEnd('、');
        }

        /// <summary>
        /// 得到一个人员的部门领导和分管领导
        /// </summary>
        /// <param name="userId">guid(人ID),u_guid（人ID）,r_guid（关系表ID）</param>
        /// <returns>部门领导，分管领导</returns>
        public (string leader, string chargeLeader) GetLeader(string userId)
        {
            var organize = GetDept(userId); //new Organize().Get(GetOrganizeId(userId));
            return null == organize ? ("", "") : (organize.Leader, organize.ChargeLeader);
        }

        /// <summary>
        /// 得到一批人员的部门领导和分管领导
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public (string leader, string chargeLeader) GetLeader(List<string> userIds)
        {
            StringBuilder leaderBuilder = new StringBuilder();
            StringBuilder chargeLeaderBuilder = new StringBuilder();
            foreach (string userId in userIds)
            {
                (string leader, string chargeLeader) = GetLeader(userId);
                leaderBuilder.Append(leader);
                leaderBuilder.Append(",");
                chargeLeaderBuilder.Append(chargeLeader);
                chargeLeaderBuilder.Append(",");
            }
            return (leaderBuilder.ToString().TrimEnd(','), chargeLeaderBuilder.ToString().TrimEnd(','));
        }

        /// <summary>
        /// 判断一个人员是否在一个组织架构字符串里
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberIds"></param>
        /// <returns></returns>
        public bool IsIn(string userId, string memberIds)
        {
            var users = new Organize().GetAllUsers(memberIds);
            Guid userGuid = Guid.Empty;
            if (userId.IsGuid(out Guid uid))
            {
                userGuid = uid;
            }
            else if (userId.StartsWith(Organize.PREFIX_USER))
            {
                userGuid = userId.RemoveUserPrefix().ToGuid();
            }
            else if (userId.StartsWith(Organize.PREFIX_RELATION))
            {
                userGuid = userId.RemoveUserRelationPrefix().ToGuid();
            }
            if (userGuid.IsEmptyGuid())
            {
                return false;
            }
            return users.Exists(p => p.Id == userGuid);
        }

        /// <summary>
        /// 判断一个人员是否在一个组织架构字符串里
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberIds"></param>
        /// <returns></returns>
        public bool Exists(string userId, string memeberIds)
        {
            return IsIn(userId, memeberIds);
        }

        /// <summary>
        /// 得到一个人员所在的组织架构ID
        /// </summary>
        /// <param name="userId">guid(人ID),u_guid（人ID）,r_guid（关系表ID）</param>
        /// <returns></returns>
        public Guid GetOrganizeId(string userId)
        {
            Guid organizeId = Guid.Empty;
            if (userId.IsGuid(out Guid uid))
            {
                var organizeUser = new OrganizeUser().GetMainByUserId(uid);
                if (null != organizeUser)
                {
                    organizeId = organizeUser.OrganizeId;
                }
            }
            else if (userId.StartsWith(Organize.PREFIX_USER))
            {
                var organizeUser = new OrganizeUser().GetMainByUserId(userId.RemoveUserPrefix().ToGuid());
                if (null != organizeUser)
                {
                    organizeId = organizeUser.OrganizeId;
                }
            }
            else if (userId.StartsWith(Organize.PREFIX_RELATION))
            {
                var organizeUser = new OrganizeUser().Get(userId.RemoveUserRelationPrefix().ToGuid());
                if (null != organizeUser)
                {
                    organizeId = organizeUser.OrganizeId;
                }
            }
            return organizeId;
        }

        /// <summary>
        /// 得到一个人员所在的部门
        /// </summary>
        /// <param name="userId">guid(人ID),u_guid（人ID）,r_guid（关系表ID）</param>
        /// <returns></returns>
        public Model.Organize GetDept(string userId)
        {
            Guid organizeId = GetOrganizeId(userId);
            if (organizeId.IsEmptyGuid())
            {
                return null;
            }
            var org = new Organize().Get(organizeId);
            if (org.Type == 2)
            {
                return org;
            }
            var parents = new Organize().GetAllParents(organizeId, false);
            return parents.Find(p => p.Type == 2);
        }

        /// <summary>
        /// 得到一个人员所在的岗位
        /// </summary>
        /// <param name="userId">guid(人ID),u_guid（人ID）,r_guid（关系表ID）</param>
        /// <returns></returns>
        public Model.Organize GetStation(string userId)
        {
            Guid organizeId = GetOrganizeId(userId);
            if (organizeId.IsEmptyGuid())
            {
                return null;
            }
            var org = new Organize().Get(organizeId);
            if (org.Type == 3)
            {
                return org;
            }
            var parents = new Organize().GetAllParents(organizeId, false);
            return parents.Find(p => p.Type == 3);
        }

        /// <summary>
        /// 得到一个人员所在的单位
        /// </summary>
        /// <param name="userId">guid(人ID),u_guid（人ID）,r_guid（关系表ID）</param>
        /// <returns></returns>
        public Model.Organize GetUnit(string userId)
        {
            Guid organizeId = GetOrganizeId(userId);
            if (organizeId.IsEmptyGuid())
            {
                return null;
            }
            var org = new Organize().Get(organizeId);
            if (org.Type == 1)
            {
                return org;
            }
            var parents = new Organize().GetAllParents(organizeId, false);
            return parents.Find(p => p.Type == 1);
        }

        /// <summary>
        /// 得到一个人员所在的工作组
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Model.WorkGroup> GetWorkGroups(Guid userId)
        {
            List<Model.WorkGroup> workGroups = new List<Model.WorkGroup>();
            if (userId.IsEmptyGuid())
            {
                return workGroups;
            }
            WorkGroup workGroup = new WorkGroup();
            var wgs = workGroup.GetAll();
            foreach (var wg in wgs)
            {
                if (workGroup.GetAllUsers(wg.Id).Exists(p => p.Id == userId))
                {
                    workGroups.Add(wg);
                }
            }
            return workGroups;
        }

        /// <summary>
        /// 得到一个人员所在的工作组ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetWorkGroupsId(Guid userId)
        {
            var workGroups = GetWorkGroups(userId);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var wg in workGroups)
            {
                stringBuilder.Append(wg.Id);
                stringBuilder.Append(",");
            }
            return stringBuilder.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 得到一个人员所在的工作组名称
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetWorkGroupsName(Guid userId)
        {
            var workGroups = GetWorkGroups(userId);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var wg in workGroups)
            {
                stringBuilder.Append(wg.Name);
                stringBuilder.Append("、");
            }
            return stringBuilder.ToString().TrimEnd('、');
        }

        /// <summary>
        /// 得到一人员的上级部门
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>guid(人ID),u_guid（人ID）,r_guid（关系表ID）</returns>
        public Model.Organize GetParentOrganize(string userId)
        {
            Guid organizeId = GetOrganizeId(userId);
            return new Organize().GetParent(organizeId);
        }

        /// <summary>
        /// 得到一个人员的上级部门领导
        /// </summary>
        /// <param name="userId">guid(人ID),u_guid（人ID）,r_guid（关系表ID）</param>
        /// <returns></returns>
        public (string leader, string chargeLeader) GetParentLeader(string userId)
        {
            var parent = GetParentOrganize(userId);
            return null == parent ? (string.Empty, string.Empty) : (parent.Leader, parent.ChargeLeader);
        }
        /// <summary>
        /// 得到一批人员的上级部门领导
        /// </summary>
        /// <param name="userId">guid(人ID),u_guid（人ID）,r_guid（关系表ID）</param>
        /// <returns></returns>
        public (string leader, string chargeLeader) GetParentLeader(List<string> userIds)
        {
            StringBuilder leaderBuilder = new StringBuilder();
            StringBuilder chargeLeaderBuilder = new StringBuilder();
            foreach (string userId in userIds)
            {
                (string leader, string chargeLeader) = GetParentLeader(userId);
                leaderBuilder.Append(leader);
                leaderBuilder.Append(",");
                chargeLeaderBuilder.Append(chargeLeader);
                chargeLeaderBuilder.Append(",");
            }
            return (leaderBuilder.ToString().TrimEnd(','), chargeLeaderBuilder.ToString().TrimEnd(','));
        }
        /// <summary>
        /// 得到一个人员的所有上级部门领导
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public (string leader, string chargeLeader) GetAllParentLeader(string userId)
        {
            var parentId = GetOrganizeId(userId);
            if (parentId.IsEmptyGuid())
            {
                return (string.Empty, string.Empty);
            }
            var allParents = new Organize().GetAllParents(parentId);
            StringBuilder leader = new StringBuilder();
            StringBuilder charegLeader = new StringBuilder();
            foreach (var parent in allParents)
            {
                leader.Append(parent.Leader);
                leader.Append(",");
                charegLeader.Append(parent.ChargeLeader);
                charegLeader.Append(",");
            }
            return (leader.ToString().TrimEnd(','), charegLeader.ToString().TrimEnd(','));
        }
        /// <summary>
        /// 得到一批人员的所有上级部门领导
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public (string leader, string chargeLeader) GetAllParentLeader(List<string> userIds)
        {
            StringBuilder leader = new StringBuilder();
            StringBuilder charegLeader = new StringBuilder();
            foreach (string userId in userIds)
            {
                var parentId = GetOrganizeId(userId);
                if (parentId.IsEmptyGuid())
                {
                    return (string.Empty, string.Empty);
                }
                var allParents = new Organize().GetAllParents(parentId);
                foreach (var parent in allParents)
                {
                    leader.Append(parent.Leader);
                    leader.Append(",");
                    charegLeader.Append(parent.ChargeLeader);
                    charegLeader.Append(",");
                }
            }
            return (leader.ToString().TrimEnd(','), charegLeader.ToString().TrimEnd(','));
        }
        /// <summary>
        /// 得到一个人员所在部门所有人员
        /// </summary>
        /// <param name="userId">guid(人ID),u_guid（人ID）,r_guid（关系表ID）</param>
        /// <returns></returns>
        public List<Model.User> GetOrganizeUsers(string userId)
        {
            Guid organzieId = GetOrganizeId(userId);
            return new Organize().GetAllUsers(organzieId);
        }
        /// <summary>
        /// 得到一批人员所在部门所有人员
        /// </summary>
        /// <param name="userId">guid(人ID),u_guid（人ID）,r_guid（关系表ID）</param>
        /// <returns></returns>
        public List<Model.User> GetOrganizeUsers(List<string> userIds)
        {
            List<Model.User> users = new List<Model.User>();
            Organize organize = new Organize();
            foreach (string userId in userIds)
            {
                Guid organzieId = GetOrganizeId(userId);
                users.AddRange(organize.GetAllUsers(organzieId));
            }
            return users;
        }
        /// <summary>
        /// 得到人员ID字符串
        /// </summary>
        /// <param name="users"></param>
        /// <returns>u_人员ID1,u_人员ID2</returns>
        public string GetUserIds(List<Model.User> users)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var user in users)
            {
                stringBuilder.Append(Organize.PREFIX_USER);
                stringBuilder.Append(user.Id);
                stringBuilder.Append(",");
            }
            return stringBuilder.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 生成签章图片
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public Bitmap CreateSignImage(string UserName)
        {
            if (UserName.IsNullOrEmpty())
            {
                return null;
            }
            Random rand = new Random(UserName.GetHashCode());
            Size ImageSize = Size.Empty;
            Font myFont = new Font("LiSu", 16);
            // 计算图片大小 
            using (Bitmap bmp1 = new Bitmap(5, 5))
            {
                using (Graphics g = Graphics.FromImage(bmp1))
                {
                    SizeF size = g.MeasureString(UserName, myFont, 10000);
                    ImageSize.Width = (int)size.Width + 4;
                    ImageSize.Height = (int)size.Height;
                }
            }
            // 创建图片 
            Bitmap bmp = new Bitmap(ImageSize.Width, ImageSize.Height);
            // 绘制文本 
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                using (StringFormat f = new StringFormat())
                {
                    f.Alignment = StringAlignment.Center;
                    f.LineAlignment = StringAlignment.Center;
                    f.FormatFlags = StringFormatFlags.NoWrap;
                    g.DrawString(
                        UserName,
                        myFont,
                        Brushes.Red,
                        new RectangleF(
                        0,
                        2,
                        ImageSize.Width,
                        ImageSize.Height),
                        f);
                }
            }

            // 随机制造噪点 (用户名绑定)
            Color c = Color.Red;
            int x, y;
            int num = ImageSize.Width * ImageSize.Height * 8 / 100;
            for (int iCount = 0; iCount < num; iCount++)
            {
                x = rand.Next(0, 4);
                y = rand.Next(ImageSize.Height);
                bmp.SetPixel(x, y, c);

                x = rand.Next(ImageSize.Width - 4, ImageSize.Width);
                y = rand.Next(ImageSize.Height);
                bmp.SetPixel(x, y, c);

            }

            int num1 = ImageSize.Width * ImageSize.Height * 20 / 100;
            for (int iCount = 0; iCount < num1; iCount++)
            {
                x = rand.Next(ImageSize.Width);
                y = rand.Next(0, 4);
                bmp.SetPixel(x, y, c);

                x = rand.Next(ImageSize.Width);
                y = rand.Next(ImageSize.Height - 4, ImageSize.Height);
                bmp.SetPixel(x, y, c);
            }

            int num2 = ImageSize.Width * ImageSize.Height / 150;
            for (int iCount = 0; iCount < num2; iCount++)
            {
                x = rand.Next(ImageSize.Width);
                y = rand.Next(ImageSize.Height);
                bmp.SetPixel(x, y, c);
            }
            myFont.Dispose();
            return bmp;
        }

        /// <summary>
        /// 得到人员的签章图片路径
        /// </summary>
        /// <returns></returns>
        public string GetSignSrc(string userId = "")
        {
            return "/RoadFlowResources/images/userSigns/" + ( userId.IsNullOrWhiteSpace() ? CurrentUserId.ToUpperString() : userId.ToUpper()) + "/default.png";
        }

        /// <summary>
        /// 判断一个人员ID是否在一个组织机构字符串中
        /// </summary>
        /// <param name="organizeIds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool Contains(string organizeIds, Guid userId)
        {
            return new Organize().GetAllUsers(organizeIds).Exists(p => p.Id == userId);
        }
        /// <summary>
        /// 得到一个人员的手机号
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetMobile(Guid userId)
        {
            var model = Get(userId);
            return null == model ? "" : model.Mobile.Trim1();
        }
    }
}
