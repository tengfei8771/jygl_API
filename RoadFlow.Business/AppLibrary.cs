using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using RoadFlow.Utility;
using System.Linq;

namespace RoadFlow.Business
{
    public class AppLibrary
    {
        private readonly Data.AppLibrary appLibraryData;
        public AppLibrary()
        {
            appLibraryData = new Data.AppLibrary();
        }
        /// <summary>
        /// 得到所有应用
        /// </summary>
        /// <returns></returns>
        public List<Model.AppLibrary> GetAll()
        {
            return appLibraryData.GetAll();
        }
        /// <summary>
        /// 添加应用
        /// </summary>
        /// <param name="appLibrary"></param>
        /// <returns></returns>
        public int Add(Model.AppLibrary appLibrary)
        {
            return appLibraryData.Add(appLibrary);
        }
        /// <summary>
        /// 更新应用
        /// </summary>
        /// <param name="appLibrary"></param>
        /// <returns></returns>
        public int Update(Model.AppLibrary appLibrary)
        {
            return appLibraryData.Update(appLibrary);
        }
        /// <summary>
        /// 删除应用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(Guid id)
        {
            var app = Get(id);
            return null == app ? 0 : appLibraryData.Delete(app);
        }
        /// <summary>
        /// 删除一批应用
        /// </summary>
        /// <param name="idString">逗号隔开的多个ID字符串</param>
        /// <returns></returns>
        public List<Model.AppLibrary> Delete(string idString)
        {
            List<Model.AppLibrary> appLibraries = new List<Model.AppLibrary>();
            string[] ids = idString.Split(',');
            var all = GetAll();
            foreach (string id in ids)
            {
                if (id.IsGuid(out Guid guid))
                {
                    appLibraries.Add(all.Find(p => p.Id == guid));
                }
            }
            appLibraryData.Delete(appLibraries.ToArray());
            return appLibraries;
        }
        /// <summary>
        /// 根据ID得到实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.AppLibrary Get(Guid id)
        {
            List<Model.AppLibrary> appLibraries = GetAll();
            return appLibraries.Find(p => p.Id == id);
        }
        /// <summary>
        /// 根据Code得到实体
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Model.AppLibrary GetByCode(string code)
        {
            List<Model.AppLibrary> appLibraries = GetAll();
            return appLibraries.Find(p => p.Code.EqualsIgnoreCase(code));
        }
        /// <summary>
        /// 得到类别下拉选项
        /// </summary>
        /// <param name="value">默认值</param>
        /// <returns></returns>
        public string GetTypeOptions(string value = "")
        {
            return new Dictionary().GetOptionsByCode("system_applibrarytype", Dictionary.ValueField.Id, value);
        }
        /// <summary>
        /// 得到一个类别的应用
        /// </summary>
        /// <param name="typeId">类别ID</param>
        /// <returns></returns>
        public List<Model.AppLibrary> GetListByType(Guid typeId)
        {
            List<Model.AppLibrary> appLibraries = new List<Model.AppLibrary>();
            var all = GetAll();
            var typeIds = new Dictionary().GetAllChildsId(typeId);
            foreach (var id in typeIds)
            {
                appLibraries.AddRange(all.FindAll(p => p.Type == id));
            }
            return appLibraries.OrderBy(p => p.Title).ToList();
        }
        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerList(out int count, int size, int number, string title, string address, string typeId, string order)
        {
            return appLibraryData.GetPagerList(out count, size, number, title, address, typeId, order);
        }
        /// <summary>
        /// 得到导出的JSON字符串
        /// </summary>
        /// <param name="ids">逗号分开的多个ID</param>
        /// <returns></returns>
        public string GetExportString(string ids)
        {
            if (ids.IsNullOrWhiteSpace())
            {
                return "[]";
            }
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (string id in ids.Split(','))
            {
                if (!id.IsGuid(out Guid guid))
                {
                    continue;
                }
                var appLibraryModel = Get(guid);
                if (null == appLibraryModel)
                {
                    continue;
                }
                jArray.Add(Newtonsoft.Json.Linq.JObject.FromObject(appLibraryModel));
            }
            return jArray.ToString();
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="json"></param>
        /// <returns>返回"1"表示成功，其它为错误信息</returns>
        public string Import(string json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return "要导入的json为空!";
            }
            Newtonsoft.Json.Linq.JArray jArray = null;
            try
            {
                jArray = Newtonsoft.Json.Linq.JArray.Parse(json);
            }
            catch
            {
                jArray = null;
            }
            if (null == jArray)
            {
                return "json解析错误!";
            }
            foreach (Newtonsoft.Json.Linq.JObject jObject in jArray)
            {
                Model.AppLibrary appLibraryModel = jObject.ToObject<Model.AppLibrary>();
                if (null == appLibraryModel)
                {
                    continue;
                }
                if (Get(appLibraryModel.Id) != null)
                {
                    Update(appLibraryModel);
                }
                else
                {
                    Add(appLibraryModel);
                }
            }
            return "1";
        }
    }
}
