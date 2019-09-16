using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class Menu
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_menu";
        /// <summary>
        /// 得到所有菜单
        /// </summary>
        /// <returns></returns>
        public List<Model.Menu> GetAll()
        {
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var menus = db.QueryAll<Model.Menu>();
                    Cache.IO.Insert(CACHEKEY, menus);
                    return menus;
                }
            }
            else
            {
                return (List<Model.Menu>)obj;
            }
        }
        /// <summary>
        /// 根据ID查询一个菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Menu Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.Menu>(id);
            }
        }
        /// <summary>
        /// 得到一个菜单的下级菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Model.Menu> GetChilds(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.Menu>("SELECT * FROM RF_Menu WHERE ParentId={0}", id);
            }
        }
        /// <summary>
        /// 添加一个菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public int Add(Model.Menu menu)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(menu);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="menu">菜单实体</param>
        public int Update(Model.Menu menu)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(menu);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="menus">菜单实体</param>
        public int Update(Model.Menu[] menus)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.UpdateRange(menus);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一组菜单
        /// </summary>
        /// <param name="menu">菜单实体</param>
        /// <returns></returns>
        public int Delete(Model.Menu[] menus)
        {
            int i = 0;
            MenuUser menuUser = new MenuUser();
            using (var db = new DataContext())
            {
                db.RemoveRange(menus);
                foreach (var menu in menus)
                {
                    //删除菜单ID关联的menuuser表
                    var menuUsers = menuUser.GetListByMenuId(menu.Id);
                    if (menuUsers.Count > 0)
                    {
                        db.RemoveRange(menuUsers);
                    }

                    //删除用户设置的快捷菜单
                    var userShortcuts = new UserShortcut().GetListByMenuId(menu.Id);
                    if (userShortcuts.Count > 0)
                    {
                        db.RemoveRange(userShortcuts);
                    }
                }
                i = db.SaveChanges();
            }
            menuUser.ClearCache();
            new UserShortcut().ClearCache();
            ClearCache();
            return i;
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            Cache.IO.Remove(CACHEKEY);
            Cache.IO.Remove(CACHEKEY + "_applibrary");
        }
        /// <summary>
        /// 得到菜单和应用程序库关联表
        /// </summary>
        /// <returns></returns>
        public DataTable GetMenuAppDataTable()
        {
            object obj = Cache.IO.Get(CACHEKEY + "_applibrary");
            if (null != obj)
            {
                return (DataTable)obj;
            }
            else
            {
                using (var db = new DataContext())
                {
                    string sql = @"SELECT a.*,b.Address,b.OpenMode,b.Width,b.Height FROM RF_Menu a LEFT JOIN RF_AppLibrary b ON a.AppLibraryId=b.Id ORDER BY a.Sort";
                    DataTable dt = db.GetDataTable(sql);
                    Cache.IO.Insert(CACHEKEY + "_applibrary", dt);
                    return dt;
                }
            }
        }
    }
}
