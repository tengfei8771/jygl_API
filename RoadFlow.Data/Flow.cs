using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Utility;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class Flow
    {
        private const string CACHEKEY = "roadflow_cache_flow";
        /// <summary>
        /// 得到所有流程
        /// </summary>
        /// <returns></returns>
        public List<Model.Flow> GetAll()
        {
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var flows = db.QueryAll<Model.Flow>();
                    Cache.IO.Insert(CACHEKEY, flows);
                    return flows;
                }
            }
            else
            {
                return (List<Model.Flow>)obj;
            }
        }
        /// <summary>
        /// 查询一个流程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Flow Get(Guid id)
        {
            return GetAll().Find(p => p.Id == id);
        }
        /// <summary>
        /// 添加一个流程
        /// </summary>
        /// <param name="flow">流程实体</param>
        /// <returns></returns>
        public int Add(Model.Flow flow)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(flow);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新流程
        /// </summary>
        /// <param name="flow">流程实体</param>
        public int Update(Model.Flow flow)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(flow);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 安装流程
        /// </summary>
        /// <param name="flow">流程实体</param>
        /// <param name="appLibrary">应用程序库实体</param>
        public int Install(Model.Flow flow)
        {
            using (var db = new DataContext())
            {
                #region 加入到应用程序库
                AppLibrary appLibrary = new AppLibrary();
                var applibraryModel = appLibrary.GetAll().Find(p => p.Code.EqualsIgnoreCase(flow.Id.ToString()));
                bool isAdd = false;
                if (null == applibraryModel)
                {
                    isAdd = true;
                    applibraryModel = new Model.AppLibrary
                    {
                        Id = Guid.NewGuid()
                    };
                }
                applibraryModel.Title = flow.Name;
                applibraryModel.Address = "/RoadFlowCore/FlowRun/Index?flowid=" + flow.Id.ToString();
                applibraryModel.Code = flow.Id.ToString();
                applibraryModel.OpenMode = 0;
                applibraryModel.Type = flow.FlowType;
                applibraryModel.Note = flow.Note;

                //多语言设定流程名称(这里暂时设定一样，没有区分语言)
                applibraryModel.Title_en = flow.Name;
                applibraryModel.Title_zh = flow.Name;

                if (isAdd)
                {
                    db.Add(applibraryModel);
                }
                else
                {
                    db.Update(applibraryModel);
                }
                #endregion

                db.Update(flow);
                ClearCache();
                appLibrary.ClearCache();
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="flow">流程实体</param>
        /// <returns></returns>
        public int Delete(Model.Flow flow)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Remove(flow);
                return db.SaveChanges();
            }
        }

        public void ClearCache()
        {
            Cache.IO.Remove(CACHEKEY);
        }

        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="flowId">可管理的流程ID</param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerList(out int count, int size, int number, List<Guid> flowIdList, string name, string type, string order, int status = -1)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetFlowSql(flowIdList, name, type, order, status);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }

        
    }
}
