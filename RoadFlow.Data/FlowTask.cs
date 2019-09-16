using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using RoadFlow.Utility;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class FlowTask
    {
        /// <summary>
        /// 查询一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.FlowTask Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.FlowTask>(id);
            }
        }
        /// <summary>
        /// 根据子流程组ID查询主流程任务
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<Model.FlowTask> GetListBySubFlowGroupId(Guid groupId)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.FlowTask>("SELECT * FROM RF_FlowTask WHERE SubFlowGroupId LIKE '%" + groupId.ToString().ToUpper() + "%'");
            }
        }
        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="flowTask">任务实体</param>
        /// <returns></returns>
        public int Add(Model.FlowTask flowTask)
        {
            using (var db = new DataContext())
            {
                db.Add(flowTask);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="flowTaskModel">任务实体</param>
        /// <returns></returns>
        public int Update(Model.FlowTask flowTaskModel)
        {
            using (var db = new DataContext())
            {
                db.Update(flowTaskModel);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 更新一组实例
        /// </summary>
        /// <param name="flowTaskModels">任务实体</param>
        /// <returns></returns>
        public int Update(Model.FlowTask[] flowTaskModels)
        {
            using (var db = new DataContext())
            {
                db.UpdateRange(flowTaskModels);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除一组流程实例
        /// </summary>
        /// <param name="groupId">组ID</param>
        /// <returns></returns>
        public int DeleteByGroupId(Guid groupId)
        {
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_FlowTask WHERE GroupId={0}", groupId);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除一个流程的实例
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public int DeleteByFlowId(Guid flowId)
        {
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_FlowTask WHERE FlowId={0}", flowId);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 根据组ID查询列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<Model.FlowTask> GetListByGroupId(Guid groupId)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.FlowTask>("SELECT * FROM RF_FlowTask WHERE GroupId={0}", groupId);
            }
        }

        /// <summary>
        /// 根据组ID查询最新的一条
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Model.FlowTask GetMaxByGroupId(Guid groupId)
        {
            DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Config.DatabaseType);
            var (sql, parameter) = dbconnnectionSql.SqlInstance.GetQueryGroupMaxTaskSql(groupId);
            using (var db = new DataContext())
            {
                return db.QueryOne<Model.FlowTask>(sql, parameter);
            }
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="removeTasks">要删除的列表</param>
        /// <param name="updateTasks">要更新的列表</param>
        /// <param name="addTasks">要添加的列表</param>
        /// <param name="executeSqls">要执行的sql列表(sql,参数,0提交退回前 1提交退回后)</param>
        /// <returns></returns>
        public int Update(List<Model.FlowTask> removeTasks, List<Model.FlowTask> updateTasks, List<Model.FlowTask> addTasks, List<(string, object[], int)> executeSqls)
        {
            using (var db = new DataContext())
            {
                if (null != executeSqls && executeSqls.Any())//执行提交退回前SQL
                {
                    foreach (var (sql, objs, afterOrBefore) in executeSqls)
                    {
                        if (afterOrBefore == 0)
                        {
                            db.Execute(sql, objs);
                        }
                    }
                }
                if (null != removeTasks && removeTasks.Any())
                {
                    db.RemoveRange(removeTasks);
                }
                if (null != updateTasks && updateTasks.Any())
                {
                    db.UpdateRange(updateTasks);
                }
                if (null != addTasks && addTasks.Any())
                {
                    db.AddRange(addTasks);
                }
                if (null != executeSqls && executeSqls.Any())//执行提交退回后SQL
                {
                    foreach (var (sql, objs, afterOrBefore) in executeSqls)
                    {
                        if (afterOrBefore == 1)
                        {
                            db.Execute(sql, objs);
                        }
                    }
                }
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 查询待办事项
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable GetWaitList(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetQueryWaitTaskSql(userId, flowId, title, startDate, endDate, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }

        /// <summary>
        /// 查询已办事项
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable GetCompletedList(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetQueryCompletedTaskSql(userId, flowId, title, startDate, endDate, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }

        /// <summary>
        /// 查询我发起的流程
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable GetMyStartList(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetQueryMyStartTaskSql(userId, flowId, title, startDate, endDate, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }

        /// <summary>
        /// 查询已委托事项
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable GetEntrustList(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetQueryEntrustTaskSql(userId, flowId, title, startDate, endDate, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }

        /// <summary>
        /// 查询实例列表
        /// </summary>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="receiveId"></param>
        /// <param name="receiveDate1"></param>
        /// <param name="receiveDate2"></param>
        /// <param name="order"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DataTable GetInstanceList(int size, int number, string flowId, string title, string receiveId, string receiveDate1, string receiveDate2, string order, out int count)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetQueryInstanceSql(flowId, title, receiveId, receiveDate1, receiveDate2, order);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }

        /// <summary>
        /// 查询超时的任务
        /// </summary>
        /// <returns></returns>
        public DataTable GetExpireTasks()
        {
            using (var db = new DataContext())
            {
                string sql = "SELECT Id FROM RF_FlowTask WHERE IsAutoSubmit=1 AND Status IN(0,1) AND CompletedTime<{0}";
                return db.GetDataTable(sql, DateExtensions.Now);
            }
        }

        /// <summary>
        /// 查询要提醒的任务
        /// </summary>
        /// <returns></returns>
        public DataTable GetRemindTasks()
        {
            using (var db = new DataContext())
            {
                string sql = "SELECT Id,Title,ReceiveId,ReceiveName,CompletedTime FROM RF_FlowTask WHERE Status IN(0,1) AND RemindTime<{0}";
                return db.GetDataTable(sql, DateExtensions.Now);
            }
        }
        /// <summary>
        /// 更新提醒时间
        /// </summary>
        /// <returns></returns>
        public int UpdateRemind(Guid taskId, DateTime remindTime)
        {
            using (var db = new DataContext())
            {
                string sql = "UPDATE RF_FlowTask SET RemindTime={0} WHERE Id={1}";
                int i = db.Execute(sql, remindTime, taskId);
                db.SaveChanges();
                return i;
            }
        }
    }
}
