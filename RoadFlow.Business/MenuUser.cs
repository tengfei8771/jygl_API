using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoadFlow.Business
{
    public class MenuUser
    {
        private readonly Data.MenuUser menuUserData;
        public MenuUser()
        {
            menuUserData = new Data.MenuUser();
        }
        /// <summary>
        /// 得到所有菜单使用人员
        /// </summary>
        /// <returns></returns>
        public List<Model.MenuUser> GetAll()
        {
            var all = menuUserData.GetAll(out bool isCache);
            if (!isCache)//如果不是从缓存中获取的，则更新菜单使用人员
            {
                UpdateAllUseUser(all);
            }
            return all;
        }
        /// <summary>
        /// 根据ID查询一个菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.MenuUser Get(Guid id)
        {
            return GetAll().Find(p => p.Id == id);
        }
        /// <summary>
        /// 添加一个菜单使用人员
        /// </summary>
        /// <param name="menuUser"></param>
        /// <returns></returns>
        public int Add(Model.MenuUser menuUser)
        {
            return menuUserData.Add(menuUser);
        }
        /// <summary>
        /// 修改一个菜单使用人员
        /// </summary>
        /// <param name="menuUser"></param>
        /// <returns></returns>
        public int Update(Model.MenuUser menuUser)
        {
            return menuUserData.Update(menuUser);
        }
        /// <summary>
        /// 更新一个机构的使用菜单
        /// </summary>
        /// <param name="menuUsers">实体数组</param>
        /// <param name="orgId">机构ID，人员u_,工作组w_</param>
        /// <returns></returns>
        public int Update(Model.MenuUser[] menuUsers, string orgId)
        {
            return menuUserData.Update(menuUsers, orgId);
        }
        /// <summary>
        /// 删除一个菜单使用人员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(Guid id)
        {
            return menuUserData.Delete(Get(id));
        }
        /// <summary>
        /// 删除一个菜单所有使用人员
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public int DeleteByMenuId(Guid menuId)
        {
            return menuUserData.Delete(GetListByMenuId(menuId).ToArray());
        }
        /// <summary>
        /// 查询一个菜单所有使用人员
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<Model.MenuUser> GetListByMenuId(Guid menuId)
        {
            return GetAll().FindAll(p => p.MenuId == menuId);
        }
        /// <summary>
        /// 更新所有菜单使用人员
        /// </summary>
        public void UpdateAllUseUser(List<Model.MenuUser> menuUsers = null)
        {
            if (null == menuUsers)
            {
                menuUsers = menuUserData.GetAll(out bool isCache);
            }
            Organize organize = new Organize();
            foreach (var menuUser in menuUsers)
            {
                menuUser.Users = organize.GetAllUsersId(menuUser.Organizes);
            }
            Cache.IO.Insert(Data.MenuUser.CACHEKEY, menuUsers);
        }
        /// <summary>
        /// 更新所有菜单使用人员(异步)
        /// </summary>
        public async void UpdateAllUseUserAsync()
        {
            await Task.Run(() =>
            {
                UpdateAllUseUser();
            });
        }
    }
}
