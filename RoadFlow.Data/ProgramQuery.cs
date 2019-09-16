using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class ProgramQuery
    {
        public List<Model.ProgramQuery> GetAll(Guid programId)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.ProgramQuery>("SELECT * FROM RF_ProgramQuery WHERE ProgramId={0}", programId);
            }
        }

        /// <summary>
        /// 查询一个程序设计查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.ProgramQuery Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.ProgramQuery>(id);
            }
        }
        /// <summary>
        /// 添加一个程序设计查询
        /// </summary>
        /// <param name="programField">程序设计查询实体</param>
        /// <returns></returns>
        public int Add(Model.ProgramQuery programQuery)
        {
            using (var db = new DataContext())
            {
                db.Add(programQuery);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新程序设计查询
        /// </summary>
        /// <param name="programField">程序设计查询实体</param>
        public int Update(Model.ProgramQuery programQuery)
        {
            using (var db = new DataContext())
            {
                db.Update(programQuery);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除程序设计查询
        /// </summary>
        /// <param name="programFields"></param>
        /// <returns></returns>
        public int Delete(Model.ProgramQuery[] programQueries)
        {
            using (var db = new DataContext())
            {
                db.RemoveRange(programQueries);
                return db.SaveChanges();
            }
        }
    }
}
