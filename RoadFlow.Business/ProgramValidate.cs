using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RoadFlow.Business
{
    public class ProgramValidate
    {
        private readonly Data.ProgramValidate programValidateData;
        public ProgramValidate()
        {
            programValidateData = new Data.ProgramValidate();
        }
        public List<Model.ProgramValidate> GetAll(Guid programId)
        {
            return programValidateData.GetAll(programId);
        }
        /// <summary>
        /// 添加一批程序设计验证
        /// </summary>
        /// <param name="ProgramValidates">程序设计验证</param>
        /// <returns></returns>
        public int Add(Model.ProgramValidate[] ProgramValidates)
        {
            return programValidateData.Add(ProgramValidates);
        }
       
    }
}
