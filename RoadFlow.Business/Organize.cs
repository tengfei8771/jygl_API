using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace RoadFlow.Business
{
    public class Organize
    {
        private readonly Data.Organize organizeData;
        public Organize()
        {
            organizeData = new Data.Organize();
        }

        /// <summary>
        /// 人员前缀
        /// </summary>
        public const string PREFIX_USER = "u_";
        /// <summary>
        /// 工作组前缀
        /// </summary>
        public const string PREFIX_WORKGROUP = "w_";
        /// <summary>
        /// 人员兼职前缀
        /// </summary>
        public const string PREFIX_RELATION = "r_";
        

        /// <summary>
        /// 查询所有组织机构
        /// </summary>
        /// <returns></returns>
        public List<Model.Organize> GetAll()
        {
            return new Integrate.Organize().GetAllOrganize();
        }
        /// <summary>
        /// 添加一个组织机构
        /// </summary>
        /// <param name="organize"></param>
        /// <returns></returns>
        public int Add(Model.Organize organize)
        {
            return organizeData.Add(organize);
        }
        /// <summary>
        /// 更新一个组织机构
        /// </summary>
        /// <param name="organize"></param>
        /// <returns></returns>
        public int Update(Model.Organize organize)
        {
            int i = organizeData.Update(organize);
            //更新菜单
            new MenuUser().UpdateAllUseUserAsync();
            return i;
        }
        /// <summary>
        /// 更新一批组织机构
        /// </summary>
        /// <param name="organizes">组织实体数组</param>
        public int Update(Model.Organize[] organizes)
        {
            int i = organizeData.Update(organizes);
            //更新菜单
            new MenuUser().UpdateAllUseUserAsync();
            return i;
        }
        /// <summary>
        /// 删除一个组织机构
        /// </summary>
        /// <param name="id"></param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        public int Delete(Guid id, IStringLocalizer localizer = null)
        {
            var org = Get(id);
            if (null == org)
            {
                return 0;
            }
            if (id == GetRootId())//不能删除根
            {
                Log.Add(localizer == null ? "删除组织架构根节点失败-不能删除根" : localizer["Delete_CanotRootLog"], "", Log.Type.系统管理);
                return 0;
            }
            var allChilds = GetAllChilds(id);//要删除所有下级机构;
            allChilds.Add(org);
            List<Model.User> allUsers = new List<Model.User>();//要删除机构下所有人员
            List<Model.OrganizeUser> allOrganizeUser = new List<Model.OrganizeUser>();//要删除所有机构与人员关系
            OrganizeUser organizeUser = new OrganizeUser();
            foreach (var child in allChilds)
            {
                allUsers.AddRange(GetAllUsers(child.Id, false));
                allOrganizeUser.AddRange(organizeUser.GetListByOrganizeId(child.Id));
            }
            int i = organizeData.Delete(allChilds.ToArray(), allUsers.ToArray(), allOrganizeUser.ToArray());

            //同步企业微信人员
            if (Config.Enterprise_WeiXin_IsUse)
            {
                EnterpriseWeiXin.Organize wxOrganize = new EnterpriseWeiXin.Organize();
                foreach (var user in allUsers)
                {
                    wxOrganize.DeleteUser(user.Account);
                }
            }

            Log.Add((localizer == null ? "删除了组织架构及其下级和人员-" : localizer["Delete_Log"]) + org.Name + (localizer == null ? "-共" : localizer["Delete_Log1"]) + i.ToString() + (localizer == null ? "条数据" : localizer["Delete_Log2"])
                , (localizer == null ? "组织：" : localizer["Delete_Log3"]) + Newtonsoft.Json.JsonConvert.SerializeObject(allChilds) +
                 (localizer == null ? "人员：" : localizer["Delete_Log4"]) + Newtonsoft.Json.JsonConvert.SerializeObject(allUsers) +
                 (localizer == null ? "人员与架构关系：" : localizer["Delete_Log5"]) + Newtonsoft.Json.JsonConvert.SerializeObject(allOrganizeUser), Log.Type.系统管理);
            return i;
        }
        /// <summary>
        /// 根据ID查询一个组织机构
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Organize Get(Guid id)
        {
            return GetAll().Find(p => p.Id == id);
        }

        /// <summary>
        /// 得到根
        /// </summary>
        /// <returns></returns>
        public Model.Organize GetRoot()
        {
            var root = GetAll().Find(p => p.ParentId == Guid.Empty);
            return root;
        }

        /// <summary>
        /// 得到根ID
        /// </summary>
        /// <returns></returns>
        public Guid GetRootId()
        {
            var root = GetRoot();
            return null == root ? Guid.Empty : root.Id;
        }
        /// <summary>
        /// 得到上级机构
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Organize GetParent(Guid id)
        {
            var org = Get(id);
            return null == org ? null : Get(org.ParentId);
        }
        /// <summary>
        /// 得到所有上级
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isMe">是否包含自己</param>
        /// <returns></returns>
        public List<Model.Organize> GetAllParents(Guid id, bool isMe = true)
        {
            List<Model.Organize> organizes = new List<Model.Organize>();
            var org = Get(id);
            if (null == org)
            {
                return organizes;
            }
            if (isMe)
            {
                organizes.Add(org);
            }
            var all = GetAll();
            AddParent(org, organizes, all);
            return organizes;
        }
        private void AddParent(Model.Organize organize, List<Model.Organize> organizes, List<Model.Organize> all)
        {
            if (null == organize)
            {
                return;
            }
            var parent = all.Find(p => p.Id == organize.ParentId);
            if (null != parent)
            {
                organizes.Add(parent);
                AddParent(parent, organizes, all);
            }
        }
        /// <summary>
        /// 得到下级组织机构
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Model.Organize> GetChilds(Guid id)
        {
            var childs = GetAll().FindAll(p => p.ParentId == id).OrderBy(p=>p.Sort).ToList();
            return childs;
        }
        /// <summary>
        /// 得到所有下级组织机构
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isMe">是否包含自己</param>
        /// <returns></returns>
        public List<Model.Organize> GetAllChilds(Guid id, bool isMe = false)
        {
            List<Model.Organize> organizes = new List<Model.Organize>();
            var org = Get(id);
            if (null == org)
            {
                return organizes;
            }
            if (isMe)
            {
                organizes.Add(org);
            }
            var all = GetAll();
            AddChilds(org, organizes, all);
            return organizes;
        }
        private void AddChilds(Model.Organize organize, List<Model.Organize> organizes, List<Model.Organize> all)
        {
            if (null == organize)
            {
                return;
            }
            var childs = all.FindAll(p => p.ParentId == organize.Id).OrderBy(p => p.Sort).ToList();
            foreach (var child in childs)
            {
                organizes.Add(child);
                AddChilds(child, organizes, all);
            }
        }
        /// <summary>
        /// 判断一个组织架构下是否有人员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasUsers(Guid id)
        {
            return new OrganizeUser().HasUser(id);
        }
        /// <summary>
        /// 判断一个组织架构是否有下级组织架构
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasChilds(Guid id)
        {
            return GetAll().Exists(p => p.ParentId == id);
        }
        /// <summary>
        /// 根据ID字符串得到所有人员
        /// </summary>
        /// <param name="idString">u_人员,id,w_工作且,r_兼职</param>
        /// <returns></returns>
        public List<Model.User> GetAllUsers(string idString)
        {
            List<Model.User> users = new List<Model.User>();
            if (idString.IsNullOrWhiteSpace())
            {
                return users;
            }
            User user = new User();
            OrganizeUser organizeUser = new OrganizeUser();
            WorkGroup workGroup = new WorkGroup();
            foreach (string id in idString.Split(','))
            {
                if (id.IsGuid(out Guid guid))
                {
                    users.AddRange(GetAllUsers(guid));
                }
                else if (id.StartsWith(PREFIX_USER)) //ID是一个人员ID
                {
                    var userModel = user.Get(id.RemoveUserPrefix().ToGuid());
                    if (null != userModel)
                    {
                        users.Add(userModel);
                    }
                }
                else if (id.StartsWith(PREFIX_RELATION)) //ID是一个人员兼职ID
                {
                    var organizeUserModel = organizeUser.Get(id.RemoveUserRelationPrefix().ToGuid());
                    if (null != organizeUserModel)
                    {
                        var userModel = user.Get(organizeUserModel.UserId);
                        if (null != userModel)
                        {
                            userModel.PartTimeId = organizeUserModel.Id;
                            users.Add(userModel);
                        }
                    }
                }
                else if (id.StartsWith(PREFIX_WORKGROUP)) //ID是一个工作组ID
                {
                    users.AddRange(workGroup.GetAllUsers(id.RemoveWorkGroupPrefix().ToGuid()));
                }
            }
            return users.Distinct(new Model.User()).ToList();
        }
        /// <summary>
        /// 根据ID字符串得到所有人员ID
        /// </summary>
        /// <param name="idString">u_人员,id,w_工作且,r_兼职</param>
        /// <returns>逗号分开的ID</returns>
        public string GetAllUsersId(string idString)
        {
            var users = GetAllUsers(idString);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var user in users)
            {
                stringBuilder.Append(user.Id);
                stringBuilder.Append(",");
            }
            return stringBuilder.ToString().TrimEnd(',');
        }
        /// <summary>
        /// 得到一个机构下所有人员
        /// </summary>
        /// <param name="id">机构ID</param>
        /// <param name="hasPartTime">是否包含兼任人员</param>
        /// <returns></returns>
        public List<Model.User> GetAllUsers(Guid id, bool hasPartTime = true)
        {
            List<Model.User> users = new List<Model.User>();
            OrganizeUser organizeUser = new OrganizeUser();
            User user = new User();
            var allChilds = GetAllChilds(id, true);
            foreach (var child in allChilds)
            {
                var organizeUsers = organizeUser.GetListByOrganizeId(child.Id);
                foreach (var organizeUserModel in organizeUsers)
                {
                    if (!hasPartTime && organizeUserModel.IsMain != 1)//如果不包含兼任人员，则跳过
                    {
                        continue;
                    }
                    var userModel = user.Get(organizeUserModel.UserId);
                    if (organizeUserModel.IsMain == 0)
                    {
                        userModel.PartTimeId = organizeUserModel.Id;
                    }
                    if (null != userModel)
                    {
                        users.Add(userModel);
                    }
                }
            }
            return users.Distinct(new Model.User()).ToList();
        }
        /// <summary>
        /// 得到一个机构下人员
        /// </summary>
        /// <param name="id">机构ID</param>
        /// <param name="hasPartTime">是否包含兼任人员</param>
        /// <returns></returns>
        public List<Model.User> GetUsers(Guid id, bool hasPartTime = true)
        {
            List<Model.User> users = new List<Model.User>();
            OrganizeUser organizeUser = new OrganizeUser();
            User user = new User();
            var organizeUsers = organizeUser.GetListByOrganizeId(id);
            foreach (var organizeUserModel in organizeUsers)
            {
                if (!hasPartTime && organizeUserModel.IsMain != 1)//如果不包含兼任人员，则跳过
                {
                    continue;
                }
                var userModel = user.Get(organizeUserModel.UserId);
                if (organizeUserModel.IsMain == 0)
                {
                    userModel.PartTimeId = organizeUserModel.Id;
                }
                if (null != userModel)
                {
                    users.Add(userModel);
                }
            }
            return users.Distinct(new Model.User()).ToList();
        }
        /// <summary>
        /// 得到一个机构的下级机构的最大序号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetMaxSort(Guid id)
        {
            var childs = GetChilds(id);
            return childs.Count == 0 ? 5 : childs.Max(p => p.Sort) + 5;
        }
        /// <summary>
        /// 得到一机构的所有上级机构名称
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isRoot">是否显示根</param>
        /// <returns></returns>
        public string GetParentsName(Guid id, bool isRoot = true)
        {
            var parents = GetAllParents(id, false);
            parents.Reverse();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var parent in parents)
            {
                if (!isRoot && parent.ParentId.IsEmptyGuid())
                {
                    continue;
                }
                stringBuilder.Append(parent.Name);
                stringBuilder.Append(" \\ ");
            }
            return stringBuilder.ToString().TrimEnd(' ', '\\', ' ');
        }
        /// <summary>
        /// 根据机构ID得到名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetName(Guid id)
        {
            var org = Get(id);
            return null == org ? string.Empty : org.Name;
        }
        /// <summary>
        /// 根据ID字符串得到名称
        /// </summary>
        /// <param name="idString">逗号分开的人员ID，机构ID，工作组ID等</param>
        /// <returns></returns>
        public string GetNames(string idString)
        {
            if (idString.IsNullOrWhiteSpace())
            {
                return "";
            }
            User user = new User();
            OrganizeUser organizeUser = new OrganizeUser();
            StringBuilder stringBuilder = new StringBuilder();
            WorkGroup workGroup = new WorkGroup();
            foreach (string id in idString.Split(','))
            {
                if (id.IsGuid(out Guid orgId))
                {
                    stringBuilder.Append(GetName(orgId));
                    stringBuilder.Append("、");
                }
                else if (id.StartsWith(PREFIX_USER))
                {
                    stringBuilder.Append(user.GetName(id.RemoveUserPrefix().ToGuid()));
                    stringBuilder.Append("、");
                }
                else if (id.StartsWith(PREFIX_RELATION))
                {
                    var organizeUserModel = organizeUser.Get(id.RemoveUserRelationPrefix().ToGuid());
                    if (null != organizeUserModel)
                    {
                        stringBuilder.Append(user.GetName(organizeUserModel.UserId));
                        stringBuilder.Append("、");
                    }
                }
                else if (id.StartsWith(PREFIX_WORKGROUP))
                {
                    stringBuilder.Append(workGroup.GetName(id.RemoveWorkGroupPrefix().ToGuid()));
                    stringBuilder.Append("、");
                }
            }
            return stringBuilder.ToString().TrimEnd('、');
        }

        /// <summary>
        /// 根据JSON字符串得到组织机构选择属性
        /// </summary>
        /// <param name="json">例：{"dept":"1","station":"0"}</param>
        /// <returns>dept="1" station="0"</returns>
        public string GetOrganizeAttrString(string json)
        {
            Newtonsoft.Json.Linq.JObject jObject = null;
            try
            {
                jObject = Newtonsoft.Json.Linq.JObject.Parse(json);
            }
            catch { }
            if (null == jObject)
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            string unit = jObject.Value<string>("unit");
            string dept = jObject.Value<string>("dept");
            string station = jObject.Value<string>("station");
            string user = jObject.Value<string>("user");
            string more = jObject.Value<string>("more");
            string group = jObject.Value<string>("group");
            string role = jObject.Value<string>("role");
            string rootid = jObject.Value<string>("rootid");
            stringBuilder.Append(" unit=\"" + (unit.IsNullOrWhiteSpace() ? "0" : unit) + "\"");
            stringBuilder.Append(" dept=\"" + (dept.IsNullOrWhiteSpace() ? "0" : dept) + "\"");
            stringBuilder.Append(" station=\"" + (station.IsNullOrWhiteSpace() ? "0" : station) + "\"");
            stringBuilder.Append(" user=\"" + (user.IsNullOrWhiteSpace() ? "0" : user) + "\"");
            stringBuilder.Append(" more=\"" + (more.IsNullOrWhiteSpace() ? "0" : more) + "\"");
            stringBuilder.Append(" group=\"" + (group.IsNullOrWhiteSpace() ? "0" : group) + "\"");
            stringBuilder.Append(" role=\"" + (role.IsNullOrWhiteSpace() ? "0" : role) + "\"");
            stringBuilder.Append(" rootid=\"" + (rootid.IsNullOrWhiteSpace() ? "0" : rootid) + "\"");
            return stringBuilder.ToString();
        }
    }
}
