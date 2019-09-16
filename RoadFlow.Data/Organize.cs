using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Mapper;


namespace RoadFlow.Data
{
    public class Organize
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_organize";
        private static List<Model.Organize> OrganizeList = null;
        /// <summary>
        /// 得到所有组织
        /// </summary>
        /// <returns></returns>
        public List<Model.Organize> GetAll()
        {
            if(OrganizeList != null)
            {
                return OrganizeList;
            }
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var organizes = db.QueryAll<Model.Organize>();
                    Cache.IO.Insert(CACHEKEY, organizes);
                    OrganizeList = organizes;
                    return organizes;
                }
            }
            else
            {
                var organizes = (List<Model.Organize>)obj;
                OrganizeList = organizes;
                return organizes;
            }
        }
        /// <summary>
        /// 添加一个组织
        /// </summary>
        /// <param name="organize"></param>
        /// <returns></returns>
        public int Add(Model.Organize organize)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(organize);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新组织
        /// </summary>
        /// <param name="organize">组织实体</param>
        public int Update(Model.Organize organize)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(organize);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新一批组织
        /// </summary>
        /// <param name="organizes">组织实体数组</param>
        public int Update(Model.Organize[] organizes)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.UpdateRange(organizes);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一个组织
        /// </summary>
        /// <param name="organizes">组织实体数组</param>
        /// <param name="users">要删除机构下所有人员的数组</param>
        /// <param name="organizeUsers">要删除机构下所有人员与机构关系表数组</param>
        /// <returns></returns>
        public int Delete(Model.Organize[] organizes, Model.User[] users, Model.OrganizeUser[] organizeUsers)
        {
            ClearCache();
            new OrganizeUser().ClearCache();
            new User().ClearCache();
            using (var db = new DataContext())
            {
                db.RemoveRange(organizes);
                db.RemoveRange(users);
                db.RemoveRange(organizeUsers);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            OrganizeList = null;
            Cache.IO.Remove(CACHEKEY);
            new HomeSet().ClearCache();//清除首页设置缓存
        }
    }
}
