using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Utility;

namespace RoadFlow.Business
{
    public class WorkGroup
    {
        private readonly Data.WorkGroup workGroupData;
        public WorkGroup()
        {
            workGroupData = new Data.WorkGroup();
        }
        /// <summary>
        /// 得到所有工作组
        /// </summary>
        /// <returns></returns>
        public List<Model.WorkGroup> GetAll()
        {
            return new Integrate.Organize().GetAllWorkGroup().OrderBy(p=>p.Sort).ToList();
        }
        /// <summary>
        /// 根据ID得到一个工作组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.WorkGroup Get(Guid id)
        {
            return GetAll().Find(p => p.Id == id);
        }
        /// <summary>
        /// 添加一个工作组
        /// </summary>
        /// <param name="workGroup">工作组实体</param>
        /// <returns></returns>
        public int Add(Model.WorkGroup workGroup)
        {
            int i = workGroupData.Add(workGroup);
            //更新菜单
            new MenuUser().UpdateAllUseUserAsync();
            return i;
        }
        /// <summary>
        /// 更新工作组
        /// </summary>
        /// <param name="workGroup">工作组实体</param>
        public int Update(Model.WorkGroup workGroup)
        {
            int i = workGroupData.Update(workGroup);
            //更新菜单
            new MenuUser().UpdateAllUseUserAsync();
            return i;
        }
        /// <summary>
        /// 更新工作组
        /// </summary>
        /// <param name="workGroups">工作组实体数组</param>
        public int Update(Model.WorkGroup[] workGroups)
        {
            int i = workGroupData.Update(workGroups);
            //更新菜单
            new MenuUser().UpdateAllUseUserAsync();
            return i;
        }
        /// <summary>
        /// 删除一个工作组
        /// </summary>
        /// <param name="workGroup">工作组实体</param>
        /// <returns></returns>
        public int Delete(Model.WorkGroup workGroup)
        {
            int i = workGroupData.Delete(workGroup);
            //更新菜单
            new MenuUser().UpdateAllUseUserAsync();
            return i;
        }
        /// <summary>
        /// 得到一个工作组下所有人员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Model.User> GetAllUsers(Guid id)
        {
            var model = Get(id);
            if (null == model || model.Members.IsNullOrWhiteSpace())
            {
                return new List<Model.User>();
            }
            return new Organize().GetAllUsers(model.Members);
        }
        /// <summary>
        /// 根据ID得到名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetName(Guid id)
        {
            var group = Get(id);
            return null == group ? string.Empty : group.Name;
        }
        /// <summary>
        /// 得到工作组最大排序
        /// </summary>
        /// <returns></returns>
        public int GetMaxSort()
        {
            var all = GetAll();
            return all.Count == 0 ? 5 : all.Max(p => p.Sort) + 5;
        }
    }
}
