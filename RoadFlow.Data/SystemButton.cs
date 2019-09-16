using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class SystemButton
    {
        /// <summary>
        /// 得到所有按钮
        /// </summary>
        /// <returns></returns>
        public List<Model.SystemButton> GetAll()
        {
            using (var db = new DataContext())
            {
                return db.QueryAll<Model.SystemButton>().OrderBy(p => p.Sort).ToList();
            }
        }
        /// <summary>
        /// 查询一个按钮
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.SystemButton Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.SystemButton>(id);
            }
        }
        /// <summary>
        /// 添加一个按钮
        /// </summary>
        /// <param name="systemButton">字典实体</param>
        /// <returns></returns>
        public int Add(Model.SystemButton systemButton)
        {
            using (var db = new DataContext())
            {
                db.Add(systemButton);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="systemButton">字典实体</param>
        public int Update(Model.SystemButton systemButton)
        {
            using (var db = new DataContext())
            {
                db.Update(systemButton);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新一批按钮
        /// </summary>
        /// <param name="systemButtons">字典实体数组</param>
        public int Update(Model.SystemButton[] systemButtons)
        {
            using (var db = new DataContext())
            {
                db.UpdateRange(systemButtons);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一个按钮
        /// </summary>
        /// <param name="systemButton">字典实体</param>
        /// <returns></returns>
        public int Delete(Model.SystemButton systemButton)
        {
            using (var db = new DataContext())
            {
                db.Remove(systemButton);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一批按钮
        /// </summary>
        /// <param name="systemButtons">字典实体数组</param>
        /// <returns></returns>
        public int Delete(Model.SystemButton[] systemButtons)
        {
            using (var db = new DataContext())
            {
                db.RemoveRange(systemButtons);
                return db.SaveChanges();
            }
        }
       
    }
}
