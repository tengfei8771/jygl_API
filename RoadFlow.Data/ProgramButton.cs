using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class ProgramButton
    {
        public List<Model.ProgramButton> GetAll(Guid programId)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.ProgramButton>("SELECT * FROM RF_ProgramButton WHERE ProgramId={0}", programId);
            }
        }

        /// <summary>
        /// 查询一个程序设计按钮
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.ProgramButton Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.ProgramButton>(id);
            }
        }
        /// <summary>
        /// 添加一个程序设计按钮
        /// </summary>
        /// <param name="programButton">程序设计按钮实体</param>
        /// <returns></returns>
        public int Add(Model.ProgramButton programButton)
        {
            using (var db = new DataContext())
            {
                db.Add(programButton);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新程序设计按钮
        /// </summary>
        /// <param name="programButton">程序设计按钮实体</param>
        public int Update(Model.ProgramButton programButton)
        {
            using (var db = new DataContext())
            {
                db.Update(programButton);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除程序设计按钮
        /// </summary>
        /// <param name="programButtons"></param>
        /// <returns></returns>
        public int Delete(Model.ProgramButton[] programButtons)
        {
            using (var db = new DataContext())
            {
                db.RemoveRange(programButtons);
                return db.SaveChanges();
            }
        }
    }
}
