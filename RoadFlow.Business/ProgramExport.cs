using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadFlow.Business
{
    public class ProgramExport
    {
        private readonly Data.ProgramExport programExportData;
        public ProgramExport()
        {
            programExportData = new Data.ProgramExport();
        }
        public List<Model.ProgramExport> GetAll(Guid programId)
        {
            return programExportData.GetAll(programId).OrderBy(p => p.Sort).ToList();
        }

        /// <summary>
        /// 查询一个程序设计导出
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.ProgramExport Get(Guid id)
        {
            return programExportData.Get(id);
        }
        /// <summary>
        /// 添加一个程序设计导出
        /// </summary>
        /// <param name="programField">程序设计导出实体</param>
        /// <returns></returns>
        public int Add(Model.ProgramExport programExport)
        {
            return programExportData.Add(programExport);
        }
        /// <summary>
        /// 更新程序设计导出
        /// </summary>
        /// <param name="programField">程序设计导出实体</param>
        public int Update(Model.ProgramExport programExport)
        {
            return programExportData.Update(programExport);
        }
        /// <summary>
        /// 删除程序设计导出
        /// </summary>
        /// <param name="programExports"></param>
        /// <returns></returns>
        public int Delete(Model.ProgramExport[] programExports)
        {
            return programExportData.Delete(programExports);
        }
        /// <summary>
        /// 得到最大序号
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public int GetMaxSort(Guid programId)
        {
            var all = GetAll(programId);
            return all.Count == 0 ? 5 : all.Max(p => p.Sort) + 5;
        }
        /// <summary>
        /// 清空并重新添加
        /// </summary>
        /// <param name="programExports"></param>
        /// <returns></returns>
        public int DeleteAndAdd(Model.ProgramExport[] programExports)
        {
            return programExportData.DeleteAndAdd(programExports);
        }
    }
}
