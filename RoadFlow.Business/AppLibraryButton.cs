using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RoadFlow.Business
{
    public class AppLibraryButton
    {
        private readonly Data.AppLibraryButton appLibraryButtonData;
        public AppLibraryButton()
        {
            appLibraryButtonData = new Data.AppLibraryButton();
        }
        /// <summary>
        /// 得到所有程序按钮
        /// </summary>
        /// <returns></returns>
        public List<Model.AppLibraryButton> GetAll()
        {
            return appLibraryButtonData.GetAll();
        }
        /// <summary>
        /// 添加一个程序按钮
        /// </summary>
        /// <param name="appLibraryButton"></param>
        /// <returns></returns>
        public int Add(Model.AppLibraryButton appLibraryButton)
        {
            return appLibraryButtonData.Add(appLibraryButton);
        }
        /// <summary>
        /// 更新一个按钮
        /// </summary>
        /// <param name="appLibraryButton"></param>
        /// <returns></returns>
        public int Update(Model.AppLibraryButton appLibraryButton)
        {
            return appLibraryButtonData.Update(appLibraryButton);
        }
        /// <summary>
        /// 删除一个按钮
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(Guid id)
        {
            return appLibraryButtonData.Delete(Get(id));
        }
        /// <summary>
        /// 删除一个应用所有按钮
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteByApplibraryId(Guid applibraryId)
        {
            var apps = GetListByApplibraryId(applibraryId);
            return appLibraryButtonData.Delete(apps.ToArray());
        }
        /// <summary>
        /// 根据ID得到按钮实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.AppLibraryButton Get(Guid id)
        {
            return GetAll().Find(p => p.Id == id);
        }
        /// <summary>
        /// 得到一个应用程序的按钮
        /// </summary>
        /// <param name="applibraryId"></param>
        /// <returns></returns>
        public List<Model.AppLibraryButton> GetListByApplibraryId(Guid applibraryId)
        {
            return GetAll().FindAll(p => p.AppLibraryId == applibraryId).OrderBy(p => p.Sort).ToList();
        }
        /// <summary>
        /// 更新一批应用按钮
        /// </summary>
        /// <param name="tuples">Tuple(实体,状态0删除，1修改，2新增)</param>
        /// <returns></returns>
        public int Update(List<Tuple<Model.AppLibraryButton, int>> tuples)
        {
            return appLibraryButtonData.Update(tuples);
        }
    }
}
