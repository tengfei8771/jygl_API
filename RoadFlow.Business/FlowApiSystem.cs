using System;
using System.Collections.Generic;
using System.Linq;
using RoadFlow.Utility;
using System.Text;

namespace RoadFlow.Business
{
    public class FlowApiSystem
    {
        private readonly Data.FlowApiSystem flowApiSystemData;
        public FlowApiSystem()
        {
            flowApiSystemData = new Data.FlowApiSystem();
        }
        /// <summary>
        /// 得到所有系统
        /// </summary>
        /// <returns></returns>
        public List<Model.FlowApiSystem> GetAll()
        {
            return flowApiSystemData.GetAll();
        }
        /// <summary>
        /// 根据ID得到实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.FlowApiSystem Get(Guid id)
        {
            List<Model.FlowApiSystem> flowApiSystems = GetAll();
            return flowApiSystems.Find(p => p.Id == id);
        }
        /// <summary>
        /// 根据系统标识得到实体
        /// </summary>
        /// <param name="systemCode"></param>
        /// <returns></returns>
        public Model.FlowApiSystem Get(string systemCode)
        {
            if (systemCode.IsNullOrEmpty())
            {
                return null;
            }
            List<Model.FlowApiSystem> flowApiSystems = GetAll();
            return flowApiSystems.Find(p => p.SystemCode.EqualsIgnoreCase(systemCode));
        }
        /// <summary>
        /// 根据系统代码得到ID
        /// </summary>
        /// <param name="systemCode"></param>
        /// <returns></returns>
        public Guid GetIdBySystemCode(string systemCode)
        {
            var model = Get(systemCode);
            return null == model ? Guid.Empty : model.Id;
        }
        /// <summary>
        /// 添加系统
        /// </summary>
        /// <param name="appLibrary"></param>
        /// <returns></returns>
        public int Add(Model.FlowApiSystem appLibrary)
        {
            return flowApiSystemData.Add(appLibrary);
        }
        /// <summary>
        /// 更新系统
        /// </summary>
        /// <param name="appLibrary"></param>
        /// <returns></returns>
        public int Update(Model.FlowApiSystem appLibrary)
        {
            return flowApiSystemData.Update(appLibrary);
        }
        /// <summary>
        /// 删除一批系统
        /// </summary>
        /// <param name="flowApiSystems"></param>
        /// <returns></returns>
        public int Delete(Model.FlowApiSystem[] flowApiSystems)
        {
            return flowApiSystemData.Delete(flowApiSystems);
        }
        /// <summary>
        /// 得到最大序号
        /// </summary>
        /// <returns></returns>
        public int GetMaxSort()
        {
            var all = GetAll();
            return all.Count == 0 ? 5 : all.Max(p => p.Sort) + 5;
        }

        /// <summary>
        /// 根据ID得到系统名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetName(Guid id)
        {
            if (id.IsEmptyGuid())
            {
                return string.Empty;
            }
            var sys = Get(id);
            return null == sys ? string.Empty : sys.Name;
        }

        /// <summary>
        /// 验证系统标识是否重复
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool ValidateSystemCode(Guid id, string code)
        {
            var all = GetAll();
            if (id.IsEmptyGuid())
            {
                return !all.Exists(p => p.SystemCode.EqualsIgnoreCase(code));
            }
            else
            {
                return !all.Exists(p => p.Id != id && p.SystemCode.EqualsIgnoreCase(code));
            }
        }
        /// <summary>
        /// 得到所有系统下拉选择
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetAllOptions(string value = "")
        {
            var all = GetAll();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var sys in all)
            {
                stringBuilder.Append("<option" + (!value.IsNullOrEmpty() && value.EqualsIgnoreCase(sys.Id.ToString()) ? " selected='selected'" : "") + " value='" + sys.Id.ToString() + "'>" + sys.Name + "(" + sys.SystemCode + ")</option>");
            }
            return stringBuilder.ToString();
        }
    }
}
