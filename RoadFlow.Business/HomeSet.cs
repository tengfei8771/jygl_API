using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RoadFlow.Utility;

namespace RoadFlow.Business
{
    public class HomeSet
    {
        private Data.HomeSet homeSetData;
        public HomeSet()
        {
            homeSetData = new Data.HomeSet();
        }
        /// <summary>
        /// 得到所有设置
        /// </summary>
        /// <returns></returns>
        public List<Model.HomeSet> GetAll()
        {
            object obj = Cache.IO.Get("roadflow_cache_homeset");
            if (null == obj)
            {
                Organize organize = new Organize();
                var homeSets = homeSetData.GetAll();
                foreach (var model in homeSets)
                {
                    if (!model.UseOrganizes.IsNullOrWhiteSpace())
                    {
                        model.UseUsers = organize.GetAllUsersId(model.UseOrganizes);
                    }
                }
                Cache.IO.Insert("roadflow_cache_homeset", homeSets);
                return homeSets;
            }
            else
            {
                return (List<Model.HomeSet>)obj;
            }
        }
        /// <summary>
        /// 查询一个设置
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public Model.HomeSet Get(Guid id)
        {
            return GetAll().Find(p => p.Id == id);
        }
        /// <summary>
        /// 添加一个设置
        /// </summary>
        /// <param name="homeSet"></param>
        /// <returns></returns>
        public int Add(Model.HomeSet homeSet)
        {
            return homeSetData.Add(homeSet);
        }
        /// <summary>
        /// 更新设置
        /// </summary>
        /// <param name="homeSet">设置实体</param>
        public int Update(Model.HomeSet homeSet)
        {
            return homeSetData.Update(homeSet);
        }
        /// <summary>
        /// 删除一个设置
        /// </summary>
        /// <param name="homeSet">设置实体</param>
        /// <returns></returns>
        public int Delete(Model.HomeSet homeSet)
        {
            return homeSetData.Delete(homeSet);
        }
        /// <summary>
        /// 删除一批设置
        /// </summary>
        /// <param name="homeSets">设置实体</param>
        /// <returns></returns>
        public int Delete(Model.HomeSet[] homeSets)
        {
            return homeSetData.Delete(homeSets);
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
        /// 查询一页设置
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerData(out int count, int size, int number, string name, string title, string type, string order)
        {
            return homeSetData.GetPagerData(out count, size, number, name, title, type, order);
        }
        /// <summary>
        /// 查询一个用户可以使用的设置
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Model.HomeSet> GetListByUserId(Guid userId)
        {
            Organize organize = new Organize();
            var all = GetAll();
            return all.FindAll(p => p.UseOrganizes.IsNullOrWhiteSpace() 
            || p.UseUsers.ContainsIgnoreCase(userId.ToString())).OrderBy(p=>p.Sort).ToList();
        }

        /// <summary>
        /// 得到数据源显示字符串
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public string GetDataSourceString(Model.HomeSet item)
        {
            if (item == null)
            {
                return string.Empty;
            }
            switch (item.DataSourceType)
            {
                case 0:
                    if (item.DbConnId.HasValue)
                    {
                        return GetSqlString(item.DataSource, item.Type, item.DbConnId.Value);
                    }
                    else
                    {
                        return string.Empty;
                    }
                case 1:
                    return GetCsharpMethodString(item.DataSource);
                case 2:
                    return GetUrlString(item.DataSource);
            }
            return string.Empty;
        }
        /// <summary>
        /// 得到sql字符串
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetSqlString(string sql, int type, Guid dbconnId)
        {
            switch (type)
            {
                case 0:
                    return new DbConnection().GetFieldValue(dbconnId, Wildcard.Filter(sql.FilterSelectSql()));
                case 1:
                case 2:
                    try
                    {
                        DataTable dt = new DbConnection().GetDataTable(dbconnId, Wildcard.Filter(sql.FilterSelectSql()));
                        if(dt.Rows.Count == 0)
                        {
                            return string.Empty;
                        }
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"hometable\"><thead><tr>");
                        foreach (DataColumn dc in dt.Columns)
                        {
                            sb.Append("<th>" + dc.ColumnName + "</th>");
                        }
                        sb.Append("</tr></thead><tbody>");
                        foreach (DataRow dr in dt.Rows)
                        {
                            sb.Append("<tr>");
                            foreach (DataColumn dc in dt.Columns)
                            {
                                sb.Append("<td>" + dr[dc.ColumnName].ToString() + "</td>");
                            }
                            sb.Append("</tr>");
                        }
                        sb.Append("</tbody></table>");
                        return sb.ToString();
                    }
                    catch
                    {
                        return string.Empty;
                    }
            }
            return string.Empty;
        }

        /// <summary>
        /// 得到c#方法字符串
        /// </summary>
        /// <param name="method"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetCsharpMethodString(string method, params object[] param)
        {
            var (obj, err) = Tools.ExecuteMethod(method, param);
            return obj == null ? "" : obj.ToString();
        }

        /// <summary>
        /// 得到URL输出字符串
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetUrlString(string url)
        {
            return HttpHelper.HttpGet(url);
        }
    }
}
