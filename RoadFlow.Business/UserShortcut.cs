using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Business
{
    public class UserShortcut
    {
        private readonly Data.UserShortcut userShortcutData;
        public UserShortcut()
        {
            userShortcutData = new Data.UserShortcut();
        }
        /// <summary>
        /// 得到所有快捷菜单
        /// </summary>
        /// <returns></returns>
        public List<Model.UserShortcut> GetAll()
        {
            return userShortcutData.GetAll();
        }
        /// <summary>
        /// 得到快捷菜单
        /// </summary>
        /// <returns></returns>
        public Model.UserShortcut Get(Guid id)
        {
            return userShortcutData.GetAll().Find(p => p.Id == id);
        }
        /// <summary>
        /// 得到一个菜单的所有记录
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<Model.UserShortcut> GetListByMenuId(Guid menuId)
        {
            return userShortcutData.GetListByMenuId(menuId);
        }
        /// <summary>
        /// 得到一个用户的所有记录
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<Model.UserShortcut> GetListByUserId(Guid userId)
        {
            return userShortcutData.GetListByUserId(userId);
        }
        /// <summary>
        /// 添加一批快捷菜单(先删除再添加)
        /// </summary>
        /// <param name="userShortcuts"></param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public int Add(Model.UserShortcut[] userShortcuts, Guid userId)
        {
            return userShortcutData.Add(userShortcuts, userId);
        }
        /// <summary>
        /// 删除一个用户的快捷菜单
        /// </summary>
        /// <param name="userShortcuts">快捷菜单实体</param>
        /// <returns></returns>
        public int Delete(Guid userId)
        {
            return userShortcutData.Delete(userId);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userShortcuts"></param>
        /// <returns></returns>
        public int Update(Model.UserShortcut[] userShortcuts)
        {
            return userShortcutData.Update(userShortcuts);
        }
    }
}
