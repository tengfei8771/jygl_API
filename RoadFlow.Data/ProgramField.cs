using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class ProgramField
    {
        public List<Model.ProgramField> GetAll(Guid programId)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.ProgramField>("SELECT * FROM RF_ProgramField WHERE ProgramId={0}", programId);
            }
        }

        /// <summary>
        /// 查询一个程序设计字段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.ProgramField Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.ProgramField>(id);
            }
        }
        /// <summary>
        /// 添加一个程序设计字段
        /// </summary>
        /// <param name="programField">程序设计字段实体</param>
        /// <returns></returns>
        public int Add(Model.ProgramField programField)
        {
            using (var db = new DataContext())
            {
                db.Add(programField);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新程序设计字段
        /// </summary>
        /// <param name="programField">程序设计字段实体</param>
        public int Update(Model.ProgramField programField)
        {
            using (var db = new DataContext())
            {
                db.Update(programField);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除程序设计字段
        /// </summary>
        /// <param name="programFields"></param>
        /// <returns></returns>
        public int Delete(Model.ProgramField[] programFields)
        {
            using (var db = new DataContext())
            {
                db.RemoveRange(programFields);
                return db.SaveChanges();
            }
        }
    }
}
