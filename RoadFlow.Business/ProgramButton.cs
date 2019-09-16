using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadFlow.Business
{
    public class ProgramButton
    {
        private readonly Data.ProgramButton programButtonData;
        public ProgramButton()
        {
            programButtonData = new Data.ProgramButton();
        }
        public List<Model.ProgramButton> GetAll(Guid programId)
        {
            return programButtonData.GetAll(programId).OrderBy(p => p.Sort).ToList();
        }

        /// <summary>
        /// 查询一个程序设计按钮
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.ProgramButton Get(Guid id)
        {
            return programButtonData.Get(id);
        }
        /// <summary>
        /// 添加一个程序设计按钮
        /// </summary>
        /// <param name="programButton">程序设计按钮实体</param>
        /// <returns></returns>
        public int Add(Model.ProgramButton programButton)
        {
            return programButtonData.Add(programButton);
        }
        /// <summary>
        /// 更新程序设计按钮
        /// </summary>
        /// <param name="programButton">程序设计按钮实体</param>
        public int Update(Model.ProgramButton programButton)
        {
            return programButtonData.Update(programButton);
        }
        /// <summary>
        /// 删除程序设计按钮
        /// </summary>
        /// <param name="programButtons"></param>
        /// <returns></returns>
        public int Delete(Model.ProgramButton[] programButtons)
        {
            return programButtonData.Delete(programButtons);
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
    }
}
