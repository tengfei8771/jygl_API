using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadFlow.Business
{
    public class ProgramQuery
    {
        private readonly Data.ProgramQuery programQueryData;
        public ProgramQuery()
        {
            programQueryData = new Data.ProgramQuery();
        }
        public List<Model.ProgramQuery> GetAll(Guid programId)
        {
            return programQueryData.GetAll(programId).OrderBy(p => p.Sort).ToList();
        }
        /// <summary>
        /// 查询一个程序设计查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.ProgramQuery Get(Guid id)
        {
            return programQueryData.Get(id);
        }
        /// <summary>
        /// 添加一个程序设计查询
        /// </summary>
        /// <param name="programField">程序设计查询实体</param>
        /// <returns></returns>
        public int Add(Model.ProgramQuery programQuery)
        {
            return programQueryData.Add(programQuery);
        }
        /// <summary>
        /// 更新程序设计查询
        /// </summary>
        /// <param name="programField">程序设计查询实体</param>
        public int Update(Model.ProgramQuery programQuery)
        {
            return programQueryData.Update(programQuery);
        }
        /// <summary>
        /// 删除程序设计查询
        /// </summary>
        /// <param name="programFields"></param>
        /// <returns></returns>
        public int Delete(Model.ProgramQuery[] programQueries)
        {
            return programQueryData.Delete(programQueries);
        }
        /// <summary>
        /// 得到最大编号
        /// </summary>
        /// <returns></returns>
        public int GetMaxSort(Guid programId)
        {
            var all = GetAll(programId);
            return all.Count == 0 ? 5 : all.Max(p => p.Sort) + 5;
        }
    }
}
