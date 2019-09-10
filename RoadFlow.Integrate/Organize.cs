using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UIDP.ODS;
namespace RoadFlow.Integrate
{
    public class Organize
    {
        /// <summary>
        /// 得到系统所有用户
        /// </summary>
        /// <returns></returns>
        UserDB userdb = new UserDB();
        OrgDB orgdb = new OrgDB();
        RoleDB roledb = new RoleDB();
        public List<Model.User> GetAllUser()
        {
            List<Model.User> userList = new List<Model.User>();
            userList = GetUserItem(userdb.fetchUserList()).OrderBy(o => o.Name).ToList();
            return userList;
            //return new Data.User().GetAll();
        }
        public List<Model.User> GetUserItem(DataTable dtUser)
        {
            List<Model.User> userList = new List<Model.User>();
            try
            {
                foreach (DataRow dr in dtUser.Rows)
                {
                    Model.User objUser = new Model.User();
                    objUser.Id = new Guid(dr["USER_ID"].ToString());
                    objUser.Name = dr["USER_NAME"].ToString();
                    objUser.Account = dr["USER_DOMAIN"].ToString();
                    objUser.Password = dr["USER_PASS"].ToString();
                    objUser.Sex = int.Parse(dr["USER_SEX"].ToString());
                    objUser.Status = int.Parse(dr["FLAG"].ToString());
                    userList.Add(objUser);
                }
                return userList;
            }
            catch
            {
            }
            return userList;
        }
        /// <summary>
        /// 得到系统所有组织架构
        /// </summary>
        /// <returns></returns>
        public List<Model.Organize> GetAllOrganize()
        {
            List<Model.Organize> orgList = new List<Model.Organize>();
            orgList = GetOrgItem(orgdb.syncOrgList()).ToList();
            return orgList;
            //return new Data.Organize().GetAll();
        }
        public List<Model.Organize> GetOrgItem(DataTable dtOrg)
        {
            List<Model.Organize> orgList = new List<Model.Organize>();
            try
            {
                foreach (DataRow dr in dtOrg.Rows)
                {
                    Model.Organize objOrg = new Model.Organize();
                    objOrg.Id = new Guid(dr["ORG_ID"].ToString());
                    objOrg.ParentId = new Guid(dr["ORG_ID_UPPER"].ToString());
                    objOrg.Name = dr["ORG_NAME"].ToString();
                    objOrg.Type = 1;
                    objOrg.Note = dr["REMARK"].ToString();
                    objOrg.Status = int.Parse(dr["ISINVALID"].ToString());
                    orgList.Add(objOrg);
                }
                return orgList;
            }
            catch
            {
            }
            return orgList;
        }
        /// <summary>
        /// 得到所有组织架构与人员关系
        /// </summary>
        /// <returns></returns>
        public List<Model.OrganizeUser> GetAllOrganizeUser()
        {
            List<Model.OrganizeUser> orgUserList = new List<Model.OrganizeUser>();
            orgUserList = GetOrgUserItem(orgdb.getOrgUser()).ToList();
            return orgUserList;
            //return new Data.OrganizeUser().GetAll();
        }
        public List<Model.OrganizeUser> GetOrgUserItem(DataTable dtOrg)
        {
            List<Model.OrganizeUser> orgUserList = new List<Model.OrganizeUser>();
            try
            {
                foreach (DataRow dr in dtOrg.Rows)
                {
                    Model.OrganizeUser objOrgUser = new Model.OrganizeUser();
                    objOrgUser.Id = new Guid(Guid.NewGuid().ToString());
                    objOrgUser.OrganizeId = new Guid(dr["ORG_ID"].ToString());
                    objOrgUser.UserId = new Guid(dr["USER_ID"].ToString());
                    objOrgUser.IsMain = 1;
                    orgUserList.Add(objOrgUser);
                }
                return orgUserList;
            }
            catch
            {
            }
            return orgUserList;
        }
        /// <summary>
        /// 得到所有工作组/角色组
        /// </summary>
        /// <returns></returns>
        public List<Model.WorkGroup> GetAllWorkGroup()
        {
            List<Model.WorkGroup> workGroupList = new List<Model.WorkGroup>();
            workGroupList = GetWorkGroupItem(roledb.GetRoles(), roledb.GetRoleUserOrg()).ToList();
            return workGroupList;
            //return new Data.WorkGroup().GetAll();
        }
        public List<Model.WorkGroup> GetWorkGroupItem(DataTable dtRole, DataTable dtRoleUserOrg)
        {
            List<Model.WorkGroup> orgWorkGroupList = new List<Model.WorkGroup>();
            try
            {
                Model.WorkGroup objWorkGroup = new Model.WorkGroup();
                foreach (DataRow dr in dtRole.Rows)
                {
                    objWorkGroup.Id = new Guid(dr["GROUP_ID"].ToString());
                    objWorkGroup.Name = dr["GROUP_NAME"].ToString();
                    StringBuilder sb = new StringBuilder();
                    foreach (DataRow drUserOrg in dtRoleUserOrg.Select("GROUP_ID ='" + dr["GROUP_ID"].ToString() + "'"))
                    {
                        sb.Append(drUserOrg["ORG_ID"].ToString() + ",u_" + drUserOrg["USER_ID"].ToString() + ",");
                    }
                    string str = sb.ToString();
                    str = str.Substring(0, str.Length - 1);
                    objWorkGroup.Members = str;
                    objWorkGroup.Note = dr["REMARK"].ToString();
                    orgWorkGroupList.Add(objWorkGroup);
                }
                return orgWorkGroupList;
            }
            catch
            {
            }
            return orgWorkGroupList;
        }
    }
}
