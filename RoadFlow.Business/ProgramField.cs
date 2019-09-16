using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadFlow.Utility;

namespace RoadFlow.Business
{
    public class ProgramField
    {
        private readonly Data.ProgramField programFieldData;
        public ProgramField()
        {
            programFieldData = new Data.ProgramField();
        }
        public List<Model.ProgramField> GetAll(Guid programId)
        {
            return programFieldData.GetAll(programId).OrderBy(p => p.Sort).ToList();
        }

        /// <summary>
        /// 查询一个程序设计字段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.ProgramField Get(Guid id)
        {
            return programFieldData.Get(id);
        }
        /// <summary>
        /// 添加一个程序设计字段
        /// </summary>
        /// <param name="programField">程序设计字段实体</param>
        /// <returns></returns>
        public int Add(Model.ProgramField programField)
        {
            return programFieldData.Add(programField);
        }
        /// <summary>
        /// 更新程序设计字段
        /// </summary>
        /// <param name="programField">程序设计字段实体</param>
        public int Update(Model.ProgramField programField)
        {
            return programFieldData.Update(programField);
        }
        /// <summary>
        /// 删除程序设计字段
        /// </summary>
        /// <param name="programFields"></param>
        /// <returns></returns>
        public int Delete(Model.ProgramField[] programFields)
        {
            return programFieldData.Delete(programFields);
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
        /// 得到字段下拉选项
        /// </summary>
        /// <param name="connId">连接id</param>
        /// <param name="sql"></param>
        /// <param name="value"></param>
        /// <param name="removeValue">要排除的字段列表</param>
        /// <returns></returns>
        public string GetFieldOptions(Guid connId, string sql, string value, List<string> removeValue = null)
        {
            var fields = new DbConnection().GetFieldsBySql(connId, sql);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string field in fields)
            {
                if (null != removeValue && !value.EqualsIgnoreCase(field) && removeValue.ContainsIgnoreCase(field))
                {
                    continue;
                }
                stringBuilder.Append("<option value=\"" + field + "\"" + (field.EqualsIgnoreCase(value) ? " selected=\"selected\"" : "")
                    + ">" + field + "</option>");
            }
            return stringBuilder.ToString();
        }
    }
}
