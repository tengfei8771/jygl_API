using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RoadFlow.Business
{
    public class OrganizeUser
    {
        private readonly Data.OrganizeUser organizeUserData;
        public OrganizeUser()
        {
            organizeUserData = new Data.OrganizeUser();
        }
        /// <summary>
        /// 查询所有组织机构与人员关系
        /// </summary>
        /// <returns></returns>
        public List<Model.OrganizeUser> GetAll()
        {
            return new Integrate.Organize().GetAllOrganizeUser();
        }
        /// <summary>
        /// 根据ID查询一个实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.OrganizeUser Get(Guid id)
        {
            return GetAll().Find(p => p.Id == id);
        }
        /// <summary>
        /// 添加一个实体
        /// </summary>
        /// <param name="organizeUser"></param>
        /// <returns></returns>
        public int Add(Model.OrganizeUser organizeUser)
        {
            return organizeUserData.Add(organizeUser);
        }
        /// <summary>
        /// 更新一批组织人员
        /// </summary>
        /// <param name="organizeUsers">组织人员实体数组</param>
        public int Update(Model.OrganizeUser[] organizeUsers)
        {
            return organizeUserData.Update(organizeUsers);
        }
        /// <summary>
        /// 更新一批组织人员
        /// </summary>
        /// <param name="tuples">要更新的列表，，int 0删除 1新增 2修改</param>
        public int Update(List<Tuple<Model.OrganizeUser, int>> tuples)
        {
            return organizeUserData.Update(tuples);
        }
        /// <summary>
        /// 查询一个人员的所有关系
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Model.OrganizeUser> GetListByUserId(Guid userId)
        {
            return GetAll().FindAll(p => p.UserId == userId).OrderByDescending(p => p.IsMain).ToList();
        }
        /// <summary>
        /// 得到一个人员的主要关系
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Model.OrganizeUser GetMainByUserId(Guid userId)
        {
            return GetAll().Find(p => p.UserId == userId && p.IsMain == 1);
        }
        /// <summary>
        /// 查询一个组织机构的所有关系
        /// </summary>
        /// <param name="organizeId"></param>
        /// <returns></returns>
        public List<Model.OrganizeUser> GetListByOrganizeId(Guid organizeId)
        {
            return GetAll().FindAll(p => p.OrganizeId == organizeId).OrderBy(p => p.Sort).ToList();
        }
        /// <summary>
        /// 判断一个组织架构下是否有人员
        /// </summary>
        /// <param name="organizeId"></param>
        /// <returns></returns>
        public bool HasUser(Guid organizeId)
        {
            return GetAll().Exists(p => p.OrganizeId == organizeId);
        }
        /// <summary>
        /// 得到一个架构下最大排序
        /// </summary>
        /// <param name="organizeId"></param>
        /// <returns></returns>
        public int GetMaxSort(Guid organizeId)
        {
            var orgUsers = GetListByOrganizeId(organizeId);
            return orgUsers.Count == 0 ? 5 : orgUsers.Max(p => p.Sort) + 5;
        }
    }
}
