using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using RoadFlow.Utility;
using System.Linq;

namespace RoadFlow.Business
{
    public class Menu
    {
        private readonly Data.Menu menuData;
        public Menu()
        {
            menuData = new Data.Menu();
        }

        /// <summary>
        /// 得到所有菜单
        /// </summary>
        /// <returns></returns>
        public List<Model.Menu> GetAll()
        {
            return menuData.GetAll();
        }

        /// <summary>
        /// 根据ID查询一个菜单 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Menu Get(Guid id)
        {
            return menuData.Get(id);
        }

        /// <summary>
        /// 得到一个菜单的下级菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Model.Menu> GetChilds(Guid id)
        {
            var childs = menuData.GetChilds(id);
            return childs.Any() ? childs.OrderBy(p => p.Sort).ToList() : childs;
        }

        /// <summary>
        /// 添加一个菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public int Add(Model.Menu menu)
        {
            return menuData.Add(menu);
        }

        /// <summary>
        /// 更新一个菜单 
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public int Update(Model.Menu menu)
        {
            return menuData.Update(menu);
        }

        /// <summary>
        /// 更新一批菜单 
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public int Update(Model.Menu[] menus)
        {
            return menuData.Update(menus);
        }

        /// <summary>
        /// 删除一组菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(Model.Menu[] menus)
        {
            return menuData.Delete(menus);
        }

        /// <summary>
        /// 查询菜单和应用程序库关联表
        /// </summary>
        /// <returns></returns>
        public DataTable GetMenuAppDataTable()
        {
            return menuData.GetMenuAppDataTable();
        }

        /// <summary>
        /// 得到下级菜单最大排序号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetMaxSort(Guid id)
        {
            var childs = GetChilds(id);
            return childs.Count == 0 ? 5 : childs.Max(p => p.Sort) + 5;
        }
        /// <summary>
        /// 得到根菜单
        /// </summary>
        /// <returns></returns>
        public Model.Menu GetRoot()
        {
            var all = GetAll();
            return all.Find(p => p.ParentId == Guid.Empty);
        }

        /// <summary>
        /// 得到菜单JSON(树型)
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="rootDir"></param>
        /// <param name="showSource">是否显示菜单来源(在查看人员菜单设置时用到)</param>
        /// <returns></returns>
        public string GetUserMenuJsonString(Guid userID, string rootDir = "", bool showSource = false)
        {
            Menu Menu = new Menu();
            AppLibrary Applibary = new AppLibrary();
            DataTable appDt = menuData.GetMenuAppDataTable();
            if (appDt.Rows.Count == 0)
            {
                return "[]";
            }

            var root = appDt.Select(string.Format("ParentId='{0}'", Guid.Empty.ToString()));
            if (root.Length == 0)
            {
                return "[]";
            }
            List<Model.MenuUser> menuusers = new MenuUser().GetAll();
            var apps = appDt.Select(string.Format("ParentId='{0}'", root[0]["Id"].ToString()));

            StringBuilder json = new StringBuilder("", 1000);
            DataRow rootDr = root[0];
            string params0 = string.Empty;
            //var menu0 = menuusers.Find(p => p.MenuID == rootDr["ID"].ToString().ToGuid() && p.SubPageID == Guid.Empty && p.Users.Contains(userID.ToString(), StringComparison.CurrentCultureIgnoreCase));
            //if (menu0 != null)
            //{
            //    params0 = menu0.Params;
            //}

            //多语言从不同的字段读取标题
            string language = Tools.GetCurrentLanguage();
            string titleKey = language.Equals("en-US") ? "Title_en" : language.Equals("zh") ? "Title_zh" : "Title";

            json.Append("{");
            json.AppendFormat("\"id\":\"{0}\",", rootDr["Id"].ToString());
            json.AppendFormat("\"title\":\"{0}\",", rootDr[titleKey].ToString().Trim());
            json.AppendFormat("\"ico\":\"{0}\",", rootDr["Ico"].ToString());
            json.AppendFormat("\"color\":\"{0}\",", rootDr["IcoColor"].ToString());
            json.AppendFormat("\"link\":\"{0}\",", GetAddress(rootDr).ToString(), params0);
            json.AppendFormat("\"model\":\"{0}\",", rootDr["OpenMode"].ToString());
            json.AppendFormat("\"width\":\"{0}\",", rootDr["Width"].ToString());
            json.AppendFormat("\"height\":\"{0}\",", rootDr["Height"].ToString());
            json.AppendFormat("\"hasChilds\":\"{0}\",", apps.Length > 0 ? "1" : "0");
            json.AppendFormat("\"childs\":[");

            StringBuilder json1 = new StringBuilder(apps.Length * 100);
            string sourceMember = string.Empty;

            #region 加载个人快捷方式
            if (!showSource)
            {
                var shortcuts = new UserShortcut().GetListByUserId(userID);
                if (shortcuts.Count > 0)
                {
                    json1.Append("{");
                    json1.AppendFormat("\"id\":\"{0}\",", Guid.NewGuid());
                    json1.AppendFormat("\"title\":\"{0}\",", "快捷菜单");
                    json1.AppendFormat("\"ico\":\"{0}\",", "");
                    json1.AppendFormat("\"color\":\"{0}\",", "");
                    json1.AppendFormat("\"link\":\"{0}\",", "");
                    json1.AppendFormat("\"model\":\"{0}\",", "");
                    json1.AppendFormat("\"width\":\"{0}\",", "");
                    json1.AppendFormat("\"height\":\"{0}\",", "");
                    json1.AppendFormat("\"hasChilds\":\"1\",");
                    json1.AppendFormat("\"childs\":[");
                    StringBuilder jsonShortcut = new StringBuilder();
                    foreach (var shortcut in shortcuts)
                    {

                        string params1 = string.Empty;
                        var menu = menuusers.FindAll(p => p.MenuId == shortcut.MenuId && p.Users.ContainsIgnoreCase(userID.ToString()));
                        if (menu.Count > 0)
                        {
                            StringBuilder psb = new StringBuilder();
                            foreach (var m in menu.FindAll(p => !p.Params.IsNullOrEmpty()).GroupBy(p => p.Params))
                            {
                                psb.Append(m.Key.Trim1());
                                psb.Append("&");
                            }
                            params1 = psb.ToString().TrimEnd('&');
                        }
                        if (!HasUse(shortcut.MenuId, userID, menuusers, out sourceMember, out params1, showSource))
                        {
                            continue;
                        }
                        var menudts = appDt.Select(string.Format("ID='{0}'", shortcut.MenuId.ToString()));
                        if (menudts.Length > 0)
                        {
                            DataRow dr = menudts[0];
                            var childs = appDt.Select("ParentID='" + dr["ID"].ToString() + "'");
                            jsonShortcut.Append("{");
                            jsonShortcut.AppendFormat("\"id\":\"{0}\",", dr["ID"].ToString());
                            jsonShortcut.AppendFormat("\"title\":\"{0}\",", dr[titleKey].ToString().Trim1());
                            jsonShortcut.AppendFormat("\"ico\":\"{0}\",", dr["Ico"].ToString());
                            jsonShortcut.AppendFormat("\"color\":\"{0}\",", dr["IcoColor"].ToString());
                            jsonShortcut.AppendFormat("\"link\":\"{0}\",", GetAddress(dr, params1));
                            jsonShortcut.AppendFormat("\"model\":\"{0}\",", dr["OpenMode"].ToString());
                            jsonShortcut.AppendFormat("\"width\":\"{0}\",", dr["Width"].ToString());
                            jsonShortcut.AppendFormat("\"height\":\"{0}\",", dr["Height"].ToString());
                            jsonShortcut.AppendFormat("\"hasChilds\":\"{0}\",", childs.Length > 0 ? "1" : "0");
                            jsonShortcut.AppendFormat("\"childs\":[]");
                            jsonShortcut.Append("},");

                        }
                    }
                    json1.Append(jsonShortcut.Length > 0 ? jsonShortcut.ToString().TrimEnd(',') : "");
                    json1.Append("]");
                    json1.Append("}");
                    if (apps.Length > 0)
                    {
                        json1.Append(",");
                    }
                }
            }
            #endregion

            for (int i = 0; i < apps.Length; i++)
            {
                string params1 = string.Empty;
                DataRow dr = apps[i];
                if (!HasUse(dr["Id"].ToString().ToGuid(), userID, menuusers, out sourceMember, out params1, showSource))
                {
                    continue;
                }
                var childs = appDt.Select("ParentId='" + dr["Id"].ToString() + "'");
                bool hasChilds = false;
                foreach (var child in childs)
                {
                    if (HasUse(child["ID"].ToString().ToGuid(), userID, menuusers, out sourceMember, out params1, showSource))
                    {
                        hasChilds = true;
                        break;
                    }
                }
                json1.Append("{");
                json1.AppendFormat("\"id\":\"{0}\",", dr["ID"].ToString());
                json1.AppendFormat("\"title\":\"{0}{1}\",", dr[titleKey].ToString().Trim1(), showSource ? "<span style='margin-left:4px;color:#666;'>[" + sourceMember + "]</span>" : "");
                json1.AppendFormat("\"ico\":\"{0}\",", dr["Ico"].ToString());
                json1.AppendFormat("\"color\":\"{0}\",", dr["IcoColor"].ToString());
                json1.AppendFormat("\"link\":\"{0}\",", GetAddress(dr, params1));
                json1.AppendFormat("\"model\":\"{0}\",", dr["OpenMode"].ToString());
                json1.AppendFormat("\"width\":\"{0}\",", dr["Width"].ToString());
                json1.AppendFormat("\"height\":\"{0}\",", dr["Height"].ToString());
                json1.AppendFormat("\"hasChilds\":\"{0}\",", hasChilds ? "1" : "0");
                json1.AppendFormat("\"childs\":[");

                json1.Append("]");
                json1.Append("}");
                json1.Append(",");
            }
            json.Append(json1.ToString().TrimEnd(','));
            json.Append("]");
            json.Append("}");
            return json.ToString();
        }

        /// <summary>
        /// 得到菜单刷新JSON(树型)
        /// </summary>
        /// <returns></returns>
        public string GetUserMenuRefreshJsonString(Guid userID, Guid refreshID, string rootDir = "", bool showSource = false)
        {
            DataTable appDt1 = menuData.GetMenuAppDataTable();
            var dv = appDt1.DefaultView;
            dv.RowFilter = string.Format("ParentId='{0}'", refreshID);
            dv.Sort = "Sort";
            var appDt = dv.ToTable();
            if (appDt.Rows.Count == 0)
            {
                return "[]";
            }
            int count = appDt.Rows.Count;
            StringBuilder json = new StringBuilder("[", count * 100);
            List<Model.MenuUser> menuusers = new MenuUser().GetAll();
            string sourceMember = string.Empty;

            //多语言从不同的字段读取标题
            string language = Tools.GetCurrentLanguage();
            string titleKey = language.Equals("en-US") ? "Title_en" : language.Equals("zh") ? "Title_zh" : "Title";

            foreach (DataRow dr in appDt.Rows)
            {
                string params1 = string.Empty;
                if (!HasUse(dr["Id"].ToString().ToGuid(), userID, menuusers, out sourceMember, out params1, showSource))
                {
                    continue;
                }
                var childs = appDt1.Select(string.Format("ParentId='{0}'", dr["Id"].ToString()));
                bool hasChilds = false;
                foreach (var child in childs)
                {
                    if (HasUse(child["ID"].ToString().ToGuid(), userID, menuusers, out sourceMember, out params1, showSource))
                    {
                        hasChilds = true;
                        break;
                    }
                }
                json.Append("{");
                json.AppendFormat("\"id\":\"{0}\",", dr["ID"].ToString());
                json.AppendFormat("\"title\":\"{0}{1}\",", dr[titleKey].ToString().Trim1(), showSource ? "<span style='margin-left:4px;color:#666;'>[" + sourceMember + "]</span>" : "");
                json.AppendFormat("\"ico\":\"{0}\",", dr["Ico"].ToString());
                json.AppendFormat("\"color\":\"{0}\",", dr["IcoColor"].ToString());
                json.AppendFormat("\"link\":\"{0}\",", GetAddress(dr, params1));
                json.AppendFormat("\"model\":\"{0}\",", dr["OpenMode"].ToString());
                json.AppendFormat("\"width\":\"{0}\",", dr["Width"].ToString());
                json.AppendFormat("\"height\":\"{0}\",", dr["Height"].ToString());
                json.AppendFormat("\"hasChilds\":\"{0}\",", hasChilds ? "1" : "0");
                json.AppendFormat("\"childs\":[");
                json.Append("]");
                json.Append("}");
                json.Append(",");
            }

            return json.ToString().TrimEnd(',') + "]";
        }


        /// <summary>
        /// 得到用户一级菜单(缩小显示)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public string GetUserMinMenuHtml(Guid userId)
        {
            DataTable appDt = menuData.GetMenuAppDataTable();
            if (appDt.Rows.Count == 0)
            {
                return "";
            }
            List<Model.MenuUser> menuUsers = new MenuUser().GetAll();
            string sourceMember = string.Empty;
            StringBuilder menuSb = new StringBuilder();

            //加载一级菜单
            var root = appDt.Select(string.Format("ParentId='{0}'", Guid.Empty.ToString()));
            if (root.Length == 0)
            {
                return menuSb.ToString();
            }

            //多语言从不同的字段读取标题
            string language = Tools.GetCurrentLanguage();
            string titleKey = language.Equals("en-US") ? "Title_en" : language.Equals("zh") ? "Title_zh" : "Title";

            #region 得到用户快捷菜单
            var shortcuts = new UserShortcut().GetListByUserId(userId);
            if (shortcuts.Count > 0)
            {
                foreach (var shortcut in shortcuts)
                {
                    string params1 = string.Empty;
                    if (!HasUse(shortcut.MenuId, userId, menuUsers, out sourceMember, out params1))
                    {
                        continue;
                    }
                    var menudts = appDt.Select(string.Format("Id='{0}'", shortcut.MenuId.ToString()));
                    if (menudts.Length > 0)
                    {
                        DataRow dr = menudts[0];
                        string icoColor = dr["IcoColor"].ToString();
                        var childs = appDt.Select("ParentID='" + shortcut.MenuId.ToString() + "'");
                        bool hasChilds = false;
                        foreach (var child in childs)
                        {
                            if (HasUse(child["Id"].ToString().ToGuid(), userId, menuUsers, out sourceMember, out params1))
                            {
                                hasChilds = true;
                                break;
                            }
                        }
                        menuSb.Append("<div class=\"menulistdiv11\" title=\"" + dr["Title"].ToString().Trim1() + "\" onclick=\"menuClick1(this);\" data-id=\"" + dr["ID"].ToString().ToUpper()
                           + "\" data-title=\"" + dr[titleKey].ToString().Trim() + "\" data-model=\"" + dr["OpenMode"].ToString()
                           + "\" data-width=\"" + dr["Width"].ToString() + "\" data-height=\"" + dr["Height"].ToString()
                           + "\" data-childs=\"" + (hasChilds ? "1" : "0") + "\" data-url=\"" + GetAddress(dr, params1) + "\" data-parent=\"" + Guid.Empty.ToString() + "\" style=\""
                           + (icoColor.IsNullOrEmpty() ? "" : "color:" + icoColor.Trim1() + ";") + "\">");
                        menuSb.Append("<div class=\"menulistdiv12\">");
                        string appIco = dr["Ico"].ToString();
                        if (appIco.IsNullOrEmpty())
                        {
                            menuSb.Append("<i class=\"fa fa-th-list\" style=\"margin-right:3px;vertical-align:middle\"></i>");
                        }
                        else if (appIco.IsFontIco())
                        {
                            menuSb.Append("<i class=\"fa " + appIco + "\" style=\"margin-right:3px;vertical-align:middle\"></i>");
                        }
                        else
                        {
                            menuSb.Append("<img src=\"" + appIco + "\" style=\"vertical-align:middle\" alt=\"\"/>");
                        }
                        menuSb.Append("</div>");
                        menuSb.Append("</div>");
                    }
                }
            }
            #endregion

            var apps = appDt.Select(string.Format("ParentId='{0}'", root[0]["Id"].ToString()));
            for (int i = 0; i < apps.Length; i++)
            {
                string params1 = string.Empty;
                DataRow dr = apps[i];
                if (!HasUse(dr["Id"].ToString().ToGuid(), userId, menuUsers, out sourceMember, out params1))
                {
                    continue;
                }
                var childs = appDt.Select("ParentId='" + dr["Id"].ToString() + "'");
                bool hasChilds = false;
                foreach (var child in childs)
                {
                    if (HasUse(child["Id"].ToString().ToGuid(), userId, menuUsers, out sourceMember, out params1))
                    {
                        hasChilds = true;
                        break;
                    }
                }

                string icoColor = dr["IcoColor"].ToString();
                menuSb.Append("<div class=\"menulistdiv11\" title=\"" + dr["Title"].ToString().Trim1() + "\" onclick=\"menuClick1(this);\" data-id=\"" + dr["ID"].ToString().ToUpper()
                           + "\" data-title=\"" + dr[titleKey].ToString().Trim() + "\" data-model=\"" + dr["OpenMode"].ToString()
                           + "\" data-width=\"" + dr["Width"].ToString() + "\" data-height=\"" + dr["Height"].ToString()
                           + "\" data-childs=\"" + (hasChilds ? "1" : "0") + "\" data-url=\"" + GetAddress(dr, params1) + "\" data-parent=\"" + Guid.Empty.ToString() + "\" style=\""
                           + (icoColor.IsNullOrEmpty() ? "" : "color:" + icoColor.Trim1() + ";") + "\">");
                menuSb.Append("<div class=\"menulistdiv12\">");
                string appIco = dr["Ico"].ToString();
                if (appIco.IsNullOrEmpty())
                {
                    menuSb.Append("<i class=\"fa fa-th-list\" style=\"margin-right:3px;vertical-align:middle\"></i>");
                }
                else if (appIco.IsFontIco())
                {
                    menuSb.Append("<i class=\"fa " + appIco + "\" style=\"margin-right:3px;vertical-align:middle\"></i>");
                }
                else
                {
                    menuSb.Append("<img src=\"" + appIco + "\" style=\"vertical-align:middle\" alt=\"\"/>");
                }
                menuSb.Append("</div>");
                menuSb.Append("</div>");

            }
            return menuSb.ToString();
        }

        /// <summary>
        /// 得到用户一级菜单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public string GetUserMenuHtml(Guid userId)
        {
            DataTable appDt = menuData.GetMenuAppDataTable();
            if (appDt.Rows.Count == 0)
            {
                return "";
            }
            List<Model.MenuUser> menuUsers = new MenuUser().GetAll();
            string sourceMember = string.Empty;
            StringBuilder menuSb = new StringBuilder();

            //加载一级菜单
            var root = appDt.Select(string.Format("ParentId='{0}'", Guid.Empty.ToString()));
            if (root.Length == 0)
            {
                return menuSb.ToString();
            }

            //多语言从不同的字段读取标题
            string language = Tools.GetCurrentLanguage();
            string titleKey = language.Equals("en-US") ? "Title_en" : language.Equals("zh") ? "Title_zh" : "Title";

            #region 得到用户快捷菜单
            var shortcuts = new UserShortcut().GetListByUserId(userId);
            if (shortcuts.Count > 0)
            {
                foreach (var shortcut in shortcuts)
                {
                    string params1 = string.Empty;
                    if (!HasUse(shortcut.MenuId, userId, menuUsers, out sourceMember, out params1))
                    {
                        continue;
                    }
                    var menudts = appDt.Select(string.Format("Id='{0}'", shortcut.MenuId.ToString()));
                    if (menudts.Length > 0)
                    {
                        DataRow dr = menudts[0];
                        string icoColor = dr["IcoColor"].ToString();
                        var childs = appDt.Select("ParentID='" + shortcut.MenuId.ToString() + "'");
                        bool hasChilds = false;
                        foreach (var child in childs)
                        {
                            if (HasUse(child["Id"].ToString().ToGuid(), userId, menuUsers, out sourceMember, out params1))
                            {
                                hasChilds = true;
                                break;
                            }
                        }
                        menuSb.Append("<div class=\"menulistdiv\" onclick=\"menuClick(this);\" data-id=\"" + dr["Id"].ToString().ToUpper()
                            + "\" data-title=\"" + dr[titleKey].ToString().Trim() + "\" data-model=\"" + dr["OpenMode"].ToString()
                            + "\" data-width=\"" + dr["Width"].ToString() + "\" data-height=\"" + dr["Height"].ToString()
                            + "\" data-childs=\"" + (hasChilds ? "1" : "0") + "\" data-url=\"" + GetAddress(dr, params1) + "\" data-parent=\"" + Guid.Empty.ToString() + "\" style=\""
                        + (icoColor.IsNullOrEmpty() ? "" : "color:" + icoColor.Trim1() + ";") + "\">");
                        menuSb.Append("<div class=\"menulistdiv1\">");
                        string appIco = dr["Ico"].ToString();
                        if (appIco.IsNullOrEmpty())
                        {
                            menuSb.Append("<i class=\"fa fa-th-list\"></i>");
                        }
                        else if (appIco.IsFontIco())
                        {
                            menuSb.Append("<i class=\"fa " + appIco + "\"></i>");
                        }
                        else
                        {
                            menuSb.Append("<img src=\"" + appIco + "\" style=\"vertical-align:middle\" alt=\"\"/>");
                        }
                        menuSb.Append("<span style=\"vertical-align:middle\">" + dr[titleKey].ToString().Trim1() + "</span>");
                        menuSb.Append("</div>");
                        if (hasChilds)
                        {
                            menuSb.Append("<div class=\"menulistdiv2\"><i class=\"fa fa-angle-left\"></i></div>");
                        }
                        menuSb.Append("</div>");
                    }
                }
            }
            #endregion

            var apps = appDt.Select(string.Format("ParentId='{0}'", root[0]["Id"].ToString()));
            for (int i = 0; i < apps.Length; i++)
            {
                string params1 = string.Empty;
                DataRow dr = apps[i];
                if (!HasUse(dr["Id"].ToString().ToGuid(), userId, menuUsers, out sourceMember, out params1))
                {
                    continue;
                }
                var childs = appDt.Select("ParentId='" + dr["Id"].ToString() + "'");
                bool hasChilds = false;
                foreach (var child in childs)
                {
                    if (HasUse(child["Id"].ToString().ToGuid(), userId, menuUsers, out sourceMember, out params1))
                    {
                        hasChilds = true;
                        break;
                    }
                }

                string icoColor = dr["IcoColor"].ToString();
                menuSb.Append("<div class=\"menulistdiv\" onclick=\"menuClick(this);\" data-id=\"" + dr["Id"].ToString().ToUpper()
                    + "\" data-title=\"" + dr[titleKey].ToString().Trim() + "\" data-model=\"" + dr["OpenMode"].ToString()
                    + "\" data-width=\"" + dr["Width"].ToString() + "\" data-height=\"" + dr["Height"].ToString()
                    + "\" data-childs=\"" + (hasChilds ? "1" : "0") + "\" data-url=\"" + GetAddress(dr, params1) + "\" data-parent=\"" + Guid.Empty.ToString() + "\" style=\""
                        + (icoColor.IsNullOrEmpty() ? "" : "color:" + icoColor.Trim1() + ";") + "\">");
                menuSb.Append("<div class=\"menulistdiv1\">");
                string appIco = dr["Ico"].ToString();
                if (appIco.IsNullOrEmpty())
                {
                    menuSb.Append("<i class=\"fa fa-th-list\"></i>");
                }
                else if (appIco.IsFontIco())
                {
                    menuSb.Append("<i class=\"fa " + appIco + "\"></i>");
                }
                else
                {
                    menuSb.Append("<img src=\"" + appIco + "\" style=\"vertical-align:middle\" alt=\"\"/>");
                }
                menuSb.Append("<span style=\"vertical-align:middle\">" + dr[titleKey].ToString().Trim1() + "</span>");
                menuSb.Append("</div>");
                if (hasChilds)
                {
                    menuSb.Append("<div class=\"menulistdiv2\"><i class=\"fa fa-angle-left\"></i></div>");
                }
                menuSb.Append("</div>");

            }
            return menuSb.ToString();
        }

        /// <summary>
        /// 得到用户下级菜单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="refreshId"></param>
        /// <param name="rootDir">应用根目录</param>
        /// <param name="isrefresh1">0常规菜单 1小菜单刷新</param>
        /// <returns></returns>
        public string GetUserMenuChilds(Guid userId, Guid refreshId, string rootDir = "", string isrefresh1 = "0")
        {
            StringBuilder menuSb = new StringBuilder();
            DataTable appDt1 = menuData.GetMenuAppDataTable();
            var dv = appDt1.DefaultView;
            dv.RowFilter = string.Format("ParentID='{0}'", refreshId);
            dv.Sort = "Sort";
            var appDt = dv.ToTable();
            if (appDt.Rows.Count == 0)
            {
                return "[]";
            }

            //多语言从不同的字段读取标题
            string language = Tools.GetCurrentLanguage();
            string titleKey = language.Equals("en-US") ? "Title_en" : language.Equals("zh") ? "Title_zh" : "Title";

            int count = appDt.Rows.Count;
            List<Model.MenuUser> menuUsers = new MenuUser().GetAll();
            string sourceMember = string.Empty;
            foreach (DataRow dr in appDt.Rows)
            {
                string params1 = string.Empty;
                if (!HasUse(dr["Id"].ToString().ToGuid(), userId, menuUsers, out sourceMember, out params1))
                {
                    continue;
                }
                var childs = appDt1.Select(string.Format("ParentID='{0}'", dr["id"].ToString()));
                bool hasChilds = false;
                foreach (var child in childs)
                {
                    if (HasUse(child["ID"].ToString().ToGuid(), userId, menuUsers, out sourceMember, out params1))
                    {
                        hasChilds = true;
                        break;
                    }
                }
                string icoColor = dr["IcoColor"].ToString();
                menuSb.Append("<div class=\"menulistdiv\" " + ("1" == isrefresh1 ? "data-isrefresh1=\"1\"" : "") + " onclick=\"" + ("1" == isrefresh1 ? "menuClick(this, 1);" : "menuClick(this, 0);") + "\" data-id=\"" + dr["Id"].ToString().ToUpper()
                    + "\" data-title=\"" + dr[titleKey].ToString().Trim() + "\" data-model=\"" + dr["OpenMode"].ToString()
                    + "\" data-width=\"" + dr["Width"].ToString() + "\" data-height=\"" + dr["Height"].ToString()
                    + "\" data-childs=\"" + (hasChilds ? "1" : "0") + "\" data-url=\"" + GetAddress(dr, params1) + "\" data-parent=\"" + refreshId.ToString().ToUpper() + "\" style=\""
                        + (icoColor.IsNullOrEmpty() ? "" : "color:" + icoColor.Trim1() + ";") + "\">");
                menuSb.Append("<div class=\"menulistdiv1\">");
                string appIco = dr["Ico"].ToString();
                if (appIco.IsNullOrEmpty())
                {
                    appIco = dr["Ico"].ToString();
                }
                if (appIco.IsNullOrEmpty())
                {
                    menuSb.Append("<i class=\"fa fa-file-text-o\"></i>");
                }
                else if (appIco.IsFontIco())
                {
                    menuSb.Append("<i class=\"fa " + appIco + "\"></i>");
                }
                else
                {
                    menuSb.Append("<img src=\"" + appIco + "\" style=\"vertical-align:middle\" alt=\"\"/>");
                }
                menuSb.Append("<span style=\"vertical-align:middle\">" + dr[titleKey].ToString().Trim1() + "</span>");
                menuSb.Append("</div>");
                if (hasChilds)
                {
                    menuSb.Append("<div class=\"menulistdiv2\"><i class=\"fa fa-angle-left\"></i></div>");
                }
                menuSb.Append("</div>");
            }
            return menuSb.ToString();
        }

        /// <summary>
        /// 判断一个人员是否有权限使用该菜单
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="userId"></param>
        /// <param name="menuusers"></param>
        /// <param name="source"></param>
        /// <param name="params1">地址参数</param>
        /// <param name="showSource">是否显示菜单来源</param>
        /// <returns></returns>
        public bool HasUse(Guid menuId, Guid userId, List<Model.MenuUser> menuusers, out string source, out string params1, bool showSource = false)
        {
            source = string.Empty;
            params1 = string.Empty;
            var menus = menuusers.FindAll(p => p.MenuId == menuId && p.Users.ContainsIgnoreCase(userId.ToString()));
            if (menus.Count > 0)
            {
                if (showSource)//是否显示菜单来源
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var menu in menus)
                    {
                        sb.Append(menu.Organizes);
                        sb.Append(",");
                        if (!menu.Params.IsNullOrEmpty())
                        {
                            params1 = menu.Params;
                        }
                    }
                    source = new Organize().GetNames(sb.ToString().TrimEnd(','));
                }
                StringBuilder psb = new StringBuilder();//组织单独对菜单使用人员设置的URL参数
                foreach (var m in menus.FindAll(p => !p.Params.IsNullOrEmpty()).GroupBy(p => p.Params))
                {
                    psb.Append(m.Key.Trim());
                    psb.Append("&");
                }
                params1 = psb.ToString().TrimEnd('&');
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断一个人员是否可以使用一个菜单下的某个按钮
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="buttonId"></param>
        /// <param name="userId"></param>
        /// <param name="menuusers"></param>
        /// <returns></returns>
        public bool HasUseButton(Guid menuId, Guid buttonId, Guid userId, List<Model.MenuUser> menuusers)
        {
            if (menuId.IsEmptyGuid() || userId.IsEmptyGuid())
            {
                return false;
            }
            var menus = menuusers.FindAll(p => p.MenuId == menuId && p.Users.ContainsIgnoreCase(userId.ToString()) && p.Users.ContainsIgnoreCase(userId.ToString()));
            if (!menus.Any())
            {
                return false;
            }
            return menus.Exists(p => p.Buttons.ContainsIgnoreCase(buttonId.ToString()));
        }

        /// <summary>
        /// 得到应用地址
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="paramsMenuUsers">用用户设置权限时设置的URL参数</param>
        /// <returns></returns>
        private string GetAddress(DataRow dr, string paramsMenuUsers = "")
        {
            string address = dr["Address"].ToString().Trim();
            string params1 = dr["Params"].ToString().Trim();
            return GetAddress(address, params1, paramsMenuUsers);
        }

        /// <summary>
        /// 得到应用地址
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="params1">参数</param>
        /// <param name="paramsMenuUsers">用用户设置权限时设置的URL参数</param>
        /// <returns></returns>
        public string GetAddress(string address, string params1, string paramsMenuUsers = "")
        {
            StringBuilder sb = new StringBuilder();
            if (params1.IsNullOrWhiteSpace() && paramsMenuUsers.IsNullOrWhiteSpace())
            {
                return address;
            }
            if (!params1.IsNullOrWhiteSpace())
            {
                if (sb.Length > 0)
                {
                    sb.Append('&');
                }
                sb.Append(params1.TrimStart('?').TrimStart('&').TrimEnd('&').TrimEnd('?'));
            }
            if (!paramsMenuUsers.IsNullOrWhiteSpace())
            {
                if (sb.Length > 0)
                {
                    sb.Append('&');
                }
                sb.Append(paramsMenuUsers.TrimStart('?').TrimStart('&').TrimEnd('&').TrimEnd('?'));
            }
            string address1 = address.Contains("?") ? string.Concat(address, "&", sb.ToString()) : string.Concat(address, "?", sb.ToString());
            return address1;
        }

        /// <summary>
        /// 得到菜单授权的TABLE
        /// </summary>
        /// <param name="orgID"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetMenuTreeTableHtml(string orgID, Guid? userId = null)
        {
            DataTable appDt = GetMenuAppDataTable();
            var menuUsers = new MenuUser().GetAll();
            StringBuilder sb = new StringBuilder();
            var root = GetRoot();
            if (null == root)
            {
                return string.Empty;
            }
            GetMenuTreeTableHtml(sb, appDt, root.Id, menuUsers, orgID, userId);
            return sb.ToString();
        }

        private void GetMenuTreeTableHtml(StringBuilder sb, DataTable appDt, Guid parentID, List<Model.MenuUser> menuUsers, string orgId, Guid? userId = null)
        {
            //多语言从不同的字段读取标题
            string language = Tools.GetCurrentLanguage();
            string titleKey = language.Equals("en-US") ? "Title_en" : language.Equals("zh") ? "Title_zh" : "Title";

            DataRow[] drs = appDt.Select("ParentID='" + parentID.ToString() + "'");
            foreach (DataRow dr in drs)
            {
                if (userId != null && userId.HasValue && !userId.Value.IsEmptyGuid())
                {
                    if (!HasUse(dr["Id"].ToString().ToGuid(), userId.Value, menuUsers, out string source, out string params1))
                    {
                        continue;
                    }
                }

                var mu = menuUsers.Find(p => p.MenuId == dr["Id"].ToString().ToGuid() && p.Organizes.EqualsIgnoreCase(orgId));
                string menuchecked = mu != null ? " checked=\"checked\"" : "";

                sb.Append("<tr id=\"" + dr["Id"].ToString().ToUpper() + "\" data-pid=\"" + dr["ParentId"].ToString().ToUpper() + "\">");
                sb.Append("<td>" + dr[titleKey] + "</td>");
                if (!dr["Ico"].ToString().IsNullOrEmpty())
                {
                    sb.Append("<td><input type=\"hidden\" name=\"apptype\" value=\"0\"/>" + (dr["Ico"].ToString().IsFontIco() ? "<i class=\"fa " + dr["Ico"].ToString() + "\" style=\"font-size:14px;\"></i>" : "<img src=\"" + dr["Ico"] + "\" style=\"vertical-align:middle;\"/>") + "</td>");
                }
                else
                {
                    sb.Append("<td></td>");
                }
                sb.Append("<td style=\"text-align:center\"><input type=\"checkbox\" " + menuchecked + " onclick=\"appboxclick(this);\" name=\"menuid\" value=\"" + dr["Id"].ToString().ToUpper() + "\"/></td>");
                sb.Append("<td>");
                bool isAppLibrary = dr["AppLibraryId"].ToString().IsGuid();
                if (isAppLibrary)
                {
                    var buttons = new AppLibraryButton().GetListByApplibraryId(dr["AppLibraryId"].ToString().ToGuid());
                    foreach (var button in buttons.OrderBy(p => p.ShowType).ThenBy(p => p.Sort))
                    {
                        menuchecked = mu != null && mu.Buttons.ContainsIgnoreCase(button.Id.ToString()) 
                            ? " checked=\"checked\"" : "";
                        sb.Append("<input type=\"checkbox\" " + menuchecked + " onclick=\"buttonboxclick(this);\" style=\"vertical-align:middle;\" id=\"button_" + dr["Id"].ToString().ToUpper() + "_" + button.Id + "\" name=\"button_" + dr["Id"].ToString().ToUpper() + "\" value=\"" + button.Id.ToUpperString() + "\"/>");
                        sb.Append("<label style=\"vertical-align:middle;\" for=\"button_" + dr["Id"].ToString().ToUpper() + "_" + button.Id.ToUpperString() + "\">" + button.Name + "</label>");
                    }
                }
                sb.Append("</td>");
                if (isAppLibrary)
                {
                    sb.Append("<td><input type=\"text\" class=\"mytext\" style=\"width:95%\" value=\"" + (mu != null ? mu.Params : "") + "\" name=\"params_" + dr["Id"].ToString().ToUpper() + "\"/></td>");
                }
                else
                {
                    sb.Append("<td>&nbsp;</td>");
                }
                sb.Append("</tr>");
                
                GetMenuTreeTableHtml(sb, appDt, dr["Id"].ToString().ToGuid(), menuUsers, orgId, userId);
            }
        }

        /// <summary>
        /// 得到按钮HTML
        /// </summary>
        /// <param name="menuId">菜单Id</param>
        /// <param name="userId">当前用户Id</param>
        /// <returns><returns>List[0]工具栏按钮 List[1]常规按钮 List[2]列表按钮</returns></returns>
        public List<string> GetButtonHtml(string menuId, Guid userId)
        {
            StringBuilder button_toolbar = new StringBuilder();
            StringBuilder button_normal = new StringBuilder();
            StringBuilder button_list = new StringBuilder();
            if (!menuId.IsGuid(out Guid menuGuid))
            {
                return new List<string>() { "", "", "" };
            }
            var menuModel = Get(menuGuid);
            if (null == menuModel || !menuModel.AppLibraryId.HasValue)
            {
                return new List<string>() { "", "", "" };
            }
            List<Model.MenuUser> menuusers = new MenuUser().GetAll();
            if (!HasUse(menuGuid, userId, menuusers, out string source, out string params1))
            {
                return new List<string>() { "", "", "" };
            }
            var buttons = new AppLibraryButton().GetListByApplibraryId(menuModel.AppLibraryId.Value).OrderBy(p => p.Sort);
            Menu menu = new Menu();
            foreach (var button in buttons)
            {
                //检查权限
                if (true)
                {
                    if (!menu.HasUseButton(menuGuid, button.Id, userId, menuusers))
                    {
                        continue;
                    }
                }

                string funName = "fun_" + Guid.NewGuid().ToString("N");
                string butName = button.Name;
                string ico = button.Ico;
                if (button.ShowType == 2)//工具栏按钮
                {
                    if (ico.IsFontIco())
                    {
                        button_toolbar.Append("<a href=\"javascript:void(0);\" onclick=\"" + funName + "();\">" +
                            "<i class='fa " + ico + "'></i><label>" + butName + "</label></a>");
                    }
                    else
                    {
                        if (!ico.IsNullOrWhiteSpace())//图片图标
                        {
                            button_toolbar.Append("<a href=\"javascript:void(0);\" onclick=\"" + funName + "();\">" +
                                    "<span style=\"background-image:url(" + ico + ");\">" + butName + "</span></a>");
                        }
                        else//没有设置图标
                        {
                            button_toolbar.Append("<a href=\"javascript:void(0);\" onclick=\"" + funName + "();\">" +
                                    "<label>" + butName + "</label></a>");
                        }
                    }
                    button_toolbar.Append("<script type='text/javascript'>function " + funName + "(){" + Wildcard.Filter(button.Events) + "}</script>");
                }
                else if (button.ShowType == 0)//常规按钮
                {
                    button_normal.Append("<button type='button' class='mybutton' style='margin-right:8px;'");
                    button_normal.Append(" onclick=\"" + funName + "();\"");
                    button_normal.Append(">");
                    if (!ico.IsNullOrWhiteSpace())
                    {
                        if (ico.IsFontIco())//如果是字体图标
                        {
                            button_normal.Append("<i class='fa " + ico + "' style='margin-right:3px;'></i>" + butName);
                        }
                        else
                        {
                            button_normal.Append("<img src=\"" + ico + "\" style='margin-right:3px;vertical-align:middle;'/>" + butName);
                        }
                    }
                    else
                    {
                        button_normal.Append(butName);
                    }
                    button_normal.Append("</button>");
                    button_normal.Append("<script type='text/javascript'>function " + funName + "(){" + Wildcard.Filter(button.Events) + "}</script>");
                }
                else if (button.ShowType == 1)//列表按钮 不替换通配符，列表按钮涉及运行时数据，在运行时替换。
                {
                    if (ico.IsFontIco())
                    {
                        button_list.Append("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"" + button.Events + "\">" +
                            "<i class='fa " + ico + "'></i>" + butName + "</a>");
                    }
                    else
                    {
                        if (!ico.IsNullOrWhiteSpace())//图片图标
                        {
                            button_list.Append("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"" + button.Events + "\">" +
                                    "<span style=\"background-image:url(" + ico + ");\">" + butName + "</span></a>");
                        }
                        else//没有设置图标
                        {
                            button_list.Append("<a href=\"javascript:void(0);\" onclick=\"" + button.Events + "\">" +
                                    "<label>" + butName + "</label></a>");
                        }
                    }
                }
            }
            return new List<string>() { button_toolbar.ToString(), button_normal.ToString(), button_list.ToString() };
        }
    }
}
