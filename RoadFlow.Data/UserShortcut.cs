using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class UserShortcut
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_usershortcut";
        /// <summary>
        /// 得到所有快捷菜单
        /// </summary>
        /// <returns></returns>
        public List<Model.UserShortcut> GetAll()
        {
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var appLibraries = db.QueryAll<Model.UserShortcut>();
                    Cache.IO.Insert(CACHEKEY, appLibraries);
                    return appLibraries;
                }
            }
            else
            {
                return (List<Model.UserShortcut>)obj;
            }
        }
        /// <summary>
        /// 得到一个菜单的所有记录
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<Model.UserShortcut> GetListByMenuId(Guid menuId)
        {
            return GetAll().FindAll(p => p.MenuId == menuId).OrderBy(p => p.Sort).ToList();
        }
        /// <summary>
        /// 得到一个用户的所有记录
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<Model.UserShortcut> GetListByUserId(Guid userId)
        {
            return GetAll().FindAll(p => p.UserId == userId).OrderBy(p => p.Sort).ToList();
        }
        /// <summary>
        /// 添加一批快捷菜单(先删除再添加)
        /// </summary>
        /// <param name="userShortcuts"></param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public int Add(Model.UserShortcut[] userShortcuts, Guid userId)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                //先删除
                db.Execute("DELETE FROM RF_UserShortcut WHERE UserId={0}", userId);
                if (userShortcuts.Length > 0)
                {
                    db.AddRange(userShortcuts);
                }
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userShortcuts"></param>
        /// <returns></returns>
        public int Update(Model.UserShortcut[] userShortcuts)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.UpdateRange(userShortcuts);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除一个用户的快捷菜单
        /// </summary>
        /// <param name="userShortcuts">快捷菜单实体</param>
        /// <returns></returns>
        public int Delete(Guid userId)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_UserShortcut WHERE UserId={0}", userId);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            Cache.IO.Remove(CACHEKEY);
        }
    }
}
