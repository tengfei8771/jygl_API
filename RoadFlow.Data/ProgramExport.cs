using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class ProgramExport
    {
        public List<Model.ProgramExport> GetAll(Guid programId)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.ProgramExport>("SELECT * FROM RF_ProgramExport WHERE ProgramId={0}", programId);
            }
        }

        /// <summary>
        /// 查询一个程序设计导出
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.ProgramExport Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.ProgramExport>(id);
            }
        }
        /// <summary>
        /// 添加一个程序设计导出
        /// </summary>
        /// <param name="programField">程序设计导出实体</param>
        /// <returns></returns>
        public int Add(Model.ProgramExport programExport)
        {
            using (var db = new DataContext())
            {
                db.Add(programExport);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新程序设计导出
        /// </summary>
        /// <param name="programField">程序设计导出实体</param>
        public int Update(Model.ProgramExport programExport)
        {
            using (var db = new DataContext())
            {
                db.Update(programExport);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除程序设计导出
        /// </summary>
        /// <param name="programExports"></param>
        /// <returns></returns>
        public int Delete(Model.ProgramExport[] programExports)
        {
            using (var db = new DataContext())
            {
                db.RemoveRange(programExports);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 清空并重新添加
        /// </summary>
        /// <param name="programExports"></param>
        /// <returns></returns>
        public int DeleteAndAdd(Model.ProgramExport[] programExports)
        {
            if (programExports.Length == 0)
            {
                return 0;
            }
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_ProgramExport WHERE ProgramId='"+ programExports.First().ProgramId + "'");
                db.AddRange(programExports);
                return db.SaveChanges();
            }
        }
    }
}
