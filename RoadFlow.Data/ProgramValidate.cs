using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class ProgramValidate
    {
        public List<Model.ProgramValidate> GetAll(Guid programId)
        {
            using (var db = new DataContext())
            {
                return db.Query<Model.ProgramValidate>("SELECT * FROM RF_ProgramValidate WHERE ProgramId={0}", programId);
            }
        }
        
        /// <summary>
        /// 添加一个程序设计验证
        /// </summary>
        /// <param name="programValidates">程序设计验证实体</param>
        /// <returns></returns>
        public int Add(Model.ProgramValidate[] programValidates)
        {
            if (programValidates.Length == 0)
            {
                return 0;
            }
            using (var db = new DataContext())
            {
                db.Execute("DELETE FROM RF_ProgramValidate WHERE ProgramId='"+ programValidates.First().ProgramId + "'");
                db.AddRange(programValidates);
                return db.SaveChanges();
            }
        }
       
    }
}
