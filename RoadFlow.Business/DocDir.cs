using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using System.Linq;

namespace RoadFlow.Business
{ 
    public class DocDir
    {
        private readonly Data.DocDir docDirData;
        public DocDir()
        {
            docDirData = new Data.DocDir();
        }
        /// <summary>
        /// 得到所有目录
        /// </summary>
        /// <returns></returns>
        public List<Model.DocDir> GetAll()
        {
            return docDirData.GetAll();
        }
        /// <summary>
        /// 查询一个实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.DocDir Get(Guid id)
        {
            var all = GetAll();
            return all.Count == 0 ? null : all.Find(p => p.Id == id);
        }
        /// <summary>
        /// 添加一个目录
        /// </summary>
        /// <param name="docDir">目录实体</param>
        /// <returns></returns>
        public int Add(Model.DocDir docDir)
        {
            return docDirData.Add(docDir);
        }
        /// <summary>
        /// 更新目录
        /// </summary>
        /// <param name="docDir">目录实体</param>
        public int Update(Model.DocDir docDir)
        {
            return docDirData.Update(docDir);
        }
        /// <summary>
        /// 得到根栏目
        /// </summary>
        /// <returns></returns>
        public Model.DocDir GetRoot()
        {
            return docDirData.GetRoot();
        }
        /// <summary>
        /// 删除一个目录
        /// </summary>
        /// <param name="docDir">目录实体</param>
        /// <returns></returns>
        public int Delete(Model.DocDir docDir)
        {
            var childs = GetAllChilds(docDir.Id);
            return docDirData.Delete(childs.ToArray());
        }
        /// <summary>
        /// 得到一个栏目的下级栏目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Model.DocDir> GetChilds(Guid id)
        {
            var all = GetAll();
            if (all.Count == 0)
            {
                return new List<Model.DocDir>();
            }
            else
            {
                return all.FindAll(p => p.ParentId == id);
            }
        }
        /// <summary>
        /// 得到一个栏目的所有下级栏目
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isMe">是否包含自己</param>
        /// <returns></returns>
        public List<Model.DocDir> GetAllChilds(Guid id, bool isMe = true)
        {
            List<Model.DocDir> docDirs = new List<Model.DocDir>();
            var all = GetAll();
            if (all.Count == 0)
            {
                return docDirs;
            }
            var docDir = all.Find(p => p.Id == id);
            if (null == docDir)
            {
                return docDirs;
            }
            if (isMe)
            {
                docDirs.Add(docDir);
            }
            AddChilds(docDir, docDirs, all);
            return docDirs;
        }
        private void AddChilds(Model.DocDir docDir, List<Model.DocDir> docDirs, List<Model.DocDir> all)
        {
            var childs = all.FindAll(p => p.ParentId == docDir.Id);
            foreach (var child in childs)
            {
                docDirs.Add(child);
                AddChilds(child, docDirs, all);
            }
        }
        /// <summary>
        /// 得到所有下级ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isMe"></param>
        /// <returns></returns>
        public List<Guid> GetAllChildsId(Guid id, bool isMe = true)
        {
            var allChilds = GetAllChilds(id, isMe);
            List<Guid> guids = new List<Guid>();
            foreach (var child in allChilds)
            {
                guids.Add(child.Id);
            }
            return guids;
        }
        /// <summary>
        /// 得到一个人员的目录树
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetTreeJson(Guid userId)
        {
            var childs = GetDisplayChilds(Guid.Empty, userId);
            JArray jArray = new JArray();
            foreach (var child in childs)
            {
                var childs1 = GetDisplayChilds(child.Id, userId);
                JObject rootObject = new JObject
                {
                    { "id", child.Id },
                    { "parentID", child.ParentId },
                    { "title", child.Name },
                    { "type", 0 },
                    { "ico", "fa-suitcase" },
                    { "color", "" },
                    { "hasChilds", childs1.Count}
                };
                JArray jArray1 = new JArray();
                foreach (var child1 in childs1)
                {
                    JObject jObject = new JObject
                    {
                        { "id", child1.Id },
                        { "parentID", child1.ParentId },
                        { "title", child1.Name },
                        { "type", 0 },
                        { "ico", "" },
                        { "color", "" },
                        { "hasChilds", GetDisplayChilds(child1.Id, userId).Count}
                    };
                    jArray1.Add(jObject);
                }
                rootObject.Add("childs", jArray1);
                jArray.Add(rootObject);
            }
            return jArray.ToString();
        }
        /// <summary>
        /// 得到一个人员的二次加载目录树
        /// </summary>
        /// <param name="docDirId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetRefreshJson(Guid docDirId, Guid userId)
        {
            JArray jArray = new JArray();
            var childs = GetDisplayChilds(docDirId, userId);
            foreach (var child in childs)
            {
                JObject jObject = new JObject
                {
                    { "id", child.Id },
                    { "parentID", child.ParentId },
                    { "title", child.Name },
                    { "type", 0 },
                    { "ico", "" },
                    { "color", "" },
                    { "hasChilds", GetDisplayChilds(child.Id, userId).Count}
                };
                jArray.Add(jObject);
            }
            return jArray.ToString();
        }
        /// <summary>
        /// 得到可以显示的下级目录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Model.DocDir> GetDisplayChilds(Guid id, Guid userId)
        {
            List<Model.DocDir> docDirs = new List<Model.DocDir>();
            var childs = GetChilds(id);
            foreach (var child in childs)
            {
                if (IsDisplay(child, userId))
                {
                    docDirs.Add(child);
                }
            }
            return docDirs;
        }
        /// <summary>
        /// 判断一个栏目对于当前用户是否可以显示
        /// </summary>
        /// <param name="docDir"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsDisplay(Model.DocDir docDir, Guid userId)
        {
            if (null == docDir)
            {
                return false;
            }
            if (IsRead(docDir, userId) || IsManage(docDir, userId) || IsPublish(docDir, userId))
            {
                return true;
            }
            var allChilds = GetAllChilds(docDir.Id, false);
            foreach (var child in allChilds)
            {
                if (IsRead(child, userId) || IsManage(child, userId) || IsPublish(child, userId))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断一个人员是否对栏目有管理权
        /// </summary>
        /// <param name="docDir"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsManage(Model.DocDir docDir, Guid userId)
        {
            if (null == docDir)
            {
                return false;
            }
            return new User().Exists(userId.ToString(), docDir.ManageUsers);
        }
        /// <summary>
        /// 判断一个人员是否对栏目有阅读权
        /// </summary>
        /// <param name="docDir"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsRead(Model.DocDir docDir, Guid userId)
        {
            if (null == docDir)
            {
                return false;
            }
            return docDir.ReadUsers.IsNullOrWhiteSpace() ||  new User().Exists(userId.ToString(), docDir.ReadUsers);
        }

        /// <summary>
        /// 判断一个人员是否对栏目有发布权
        /// </summary>
        /// <param name="docDir"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsPublish(Model.DocDir docDir, Guid userId)
        {
            if (null == docDir)
            {
                return false;
            }
            return new User().Exists(userId.ToString(), docDir.PublishUsers);
        }
        /// <summary>
        /// 得到一个栏目下的最大排序号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetMaxSort(Guid id)
        {
            var childs = GetChilds(id);
            return childs.Count == 0 ? 5 : childs.Max(p => p.Sort) + 5;
        }
        /// <summary>
        /// 得到栏目名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetName(Guid id)
        {
            var dir = Get(id);
            return null == dir ? string.Empty : dir.Name;
        }
        /// <summary>
        /// 判断一个栏目下是否有文档
        /// </summary>
        /// <param name="dirId"></param>
        /// <returns></returns>
        public bool HasDoc(Guid dirId)
        {
            var childIds = GetAllChildsId(dirId);
            return docDirData.HasDoc(childIds);
        }
        /// <summary>
        /// 得到一个人员有阅读权限的栏目
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Model.DocDir> GetReadDirs(Guid userId)
        {
            var all = GetAll();
            List<Model.DocDir> docDirs = new List<Model.DocDir>();
            foreach (var docDir in all)
            {
                if (IsRead(docDir, userId))
                {
                    docDirs.Add(docDir);
                }
            }
            return docDirs;
        }
        /// <summary>
        /// 得到一个栏目的所有上级
        /// </summary>
        /// <param name="dirId"></param>
        /// <param name="hasMe">是否包含自己</param>
        /// <param name="hasRoot">是否包含根栏目</param>
        /// <returns></returns>
        public List<Model.DocDir> GetAllParents(Guid dirId, bool hasMe = true, bool hasRoot = true)
        {
            var all = GetAll();
            List<Model.DocDir> docDirs = new List<Model.DocDir>();
            var dir = all.Find(p => p.Id == dirId);
            if (null == dir)
            {
                return docDirs;
            }
            if (hasMe)
            {
                docDirs.Add(dir);
            }
            AddParent(docDirs, all, dir.ParentId);
            if (!hasRoot)
            {
                docDirs.RemoveAll(p => p.ParentId == Guid.Empty);
            }
            return docDirs;
        }
        private void AddParent(List<Model.DocDir> docDirs, List<Model.DocDir> all, Guid parentId)
        {
            var dir = all.Find(p => p.Id == parentId);
            if (null != dir)
            {
                docDirs.Add(dir);
                AddParent(docDirs, all, dir.ParentId);
            }
        }
        /// <summary>
        /// 得到栏目的所有上级名称
        /// </summary>
        /// <param name="dirId"></param>
        /// <param name="hasMe"></param>
        /// <param name="hasRoot"></param>
        /// <returns></returns>
        public string GetAllParentNames(Guid dirId, bool hasMe = true, bool hasRoot = true, string split = "\\")
        {
            var parents = GetAllParents(dirId, hasMe, hasRoot);
            StringBuilder stringBuilder = new StringBuilder();
            parents.Reverse();
            foreach (var parent in parents)
            {
                stringBuilder.Append(parent.Name);
                stringBuilder.Append(split);
            }
            return stringBuilder.ToString().TrimEnd(split.ToCharArray());
        }
    }
}
