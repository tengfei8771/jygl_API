using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadFlow.Utility;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class FlowDynamic
    {
        /// <summary>
        /// 查询一个动态流程
        /// </summary>
        /// <param name="StepId">动态步骤ID</param>
        /// <param name="groupId">组ID</param>
        /// <returns></returns>
        public Model.FlowDynamic Get(Guid StepId, Guid groupId)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.FlowDynamic>(StepId, groupId);
            }
        }
        /// <summary>
        /// 添加一个动态流程
        /// </summary>
        /// <param name="flowDynamic">动态流程实体</param>
        /// <returns></returns>
        public int Add(Model.FlowDynamic flowDynamic)
        {
            using (var db = new DataContext())
            {
                db.Add(flowDynamic);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新动态流程
        /// </summary>
        /// <param name="flowDynamic">动态流程实体</param>
        public int Update(Model.FlowDynamic flowDynamic)
        {
            using (var db = new DataContext())
            {
                db.Update(flowDynamic);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除动态流程
        /// </summary>
        /// <param name="flowDynamic">动态流程实体</param>
        /// <returns></returns>
        public int Delete(Model.FlowDynamic flowDynamic)
        {
            using (var db = new DataContext())
            {
                db.Remove(flowDynamic);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除动态流程
        /// </summary>
        /// <param name="stepId">动态步骤ID</param>
        /// <param name="groupId">组ID</param>
        /// <returns></returns>
        public int Delete(Guid stepId, Guid groupId)
        {
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_FlowDynamic WHERE StepId={0} AND GroupId={1}", stepId, groupId);
                return db.SaveChanges();
            }
        }
    }
}
