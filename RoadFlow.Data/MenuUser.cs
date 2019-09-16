using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Utility;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class MenuUser
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        public const string CACHEKEY = "roadflow_cache_menuuser";

        /// <summary>
        /// 得到所有菜单使用人员
        /// </summary>
        /// <param name="isCache">是否是从缓存获取的</param>
        /// <returns></returns>
        public List<Model.MenuUser> GetAll(out bool isCache)
        {
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                isCache = false;
                using (var db = new DataContext())
                {
                    var menuUsers = db.QueryAll<Model.MenuUser>();
                    Cache.IO.Insert(CACHEKEY, menuUsers);
                    return menuUsers;
                }
            }
            else
            {
                isCache = true;
                return (List<Model.MenuUser>)obj;
            }
        }

        /// <summary>
        /// 添加一个菜单使用人员
        /// </summary>
        /// <param name="menuUser"></param>
        /// <returns></returns>
        public int Add(Model.MenuUser menuUser)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(menuUser);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新菜单使用人员
        /// </summary>
        /// <param name="menuUser">菜单使用人员实体</param>
        public int Update(Model.MenuUser menuUser)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(menuUser);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新一个机构的使用菜单
        /// </summary>
        /// <param name="menuUsers">实体数组</param>
        /// <param name="orgId">机构ID，人员u_,工作组w_</param>
        /// <returns></returns>
        public int Update(Model.MenuUser[] menuUsers, string orgId)
        {
            if (null == menuUsers || menuUsers.Length == 0)
            {
                return 0;
            }
            int i = 0;
            using (var db = new DataContext())
            {
                db.RemoveRange(GetListByOrgId(orgId).ToArray());
                db.AddRange(menuUsers);
                i = db.SaveChanges();
            }
            ClearCache();
            return i;
        }
        /// <summary>
        /// 更新一批使用菜单
        /// </summary>
        /// <param name="menuUsers">实体数组</param>
        /// <returns></returns>
        public int Update(Model.MenuUser[] menuUsers)
        {
            if (null == menuUsers || menuUsers.Length == 0)
            {
                return 0;
            }
            ClearCache();
            using (var db = new DataContext())
            {
                db.UpdateRange(menuUsers);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一个菜单
        /// </summary>
        /// <param name="menuUser">菜单使用人员实体</param>
        /// <returns></returns>
        public int Delete(Model.MenuUser menuUser)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Remove(menuUser);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一批菜单
        /// </summary>
        /// <param name="menuUsers">菜单使用人员实体</param>
        /// <returns></returns>
        public int Delete(Model.MenuUser[] menuUsers)
        {
            if (null == menuUsers || menuUsers.Length == 0)
            {
                return 0;
            }
            ClearCache();
            using (var db = new DataContext())
            {
                db.RemoveRange(menuUsers);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 根据菜单ID得到List
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<Model.MenuUser> GetListByMenuId(Guid menuId)
        {
            return GetAll(out bool cache).FindAll(p => p.MenuId == menuId);
        }
        /// <summary>
        /// 根据机构ID得到List
        /// </summary>
        /// <param name="orgId">机构ID，人员u_,工作组w_</param>
        /// <returns></returns>
        public List<Model.MenuUser> GetListByOrgId(string orgId)
        {
            return GetAll(out bool cache).FindAll(p => p.Organizes.EqualsIgnoreCase(orgId));
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
