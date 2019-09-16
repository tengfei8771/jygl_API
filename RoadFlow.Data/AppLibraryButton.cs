using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class AppLibraryButton
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_applibrarybutton";
        /// <summary>
        /// 得到所有应用按钮
        /// </summary>
        /// <returns></returns>
        public List<Model.AppLibraryButton> GetAll()
        {
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var appLibraryButtons = db.QueryAll<Model.AppLibraryButton>();
                    Cache.IO.Insert(CACHEKEY, appLibraryButtons);
                    return appLibraryButtons;
                }
            }
            else
            {
                return (List<Model.AppLibraryButton>)obj;
            }
        }
        /// <summary>
        /// 添加一个应用按钮
        /// </summary>
        /// <param name="appLibraryButton"></param>
        /// <returns></returns>
        public int Add(Model.AppLibraryButton appLibraryButton)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(appLibraryButton);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新应用按钮
        /// </summary>
        /// <param name="appLibraryButton">应用按钮实体</param>
        public int Update(Model.AppLibraryButton appLibraryButton)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(appLibraryButton);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新一批应用按钮
        /// </summary>
        /// <param name="tuples">Tuple(实体,状态0删除，1修改，2新增)</param>
        /// <returns></returns>
        public int Update(List<Tuple<Model.AppLibraryButton, int>> tuples)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                foreach (var tuple in tuples)
                {
                    if (0 == tuple.Item2)
                    {
                        db.Remove(tuple.Item1);
                    }
                    else if (1 == tuple.Item2)
                    {
                        db.Update(tuple.Item1);
                    }
                    else if (2 == tuple.Item2)
                    {
                        db.Add(tuple.Item1);
                    }
                }
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一个应用按钮
        /// </summary>
        /// <param name="appLibraryButton">应用按钮实体</param>
        /// <returns></returns>
        public int Delete(Model.AppLibraryButton appLibraryButton)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Remove(appLibraryButton);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一批应用按钮
        /// </summary>
        /// <param name="appLibraryButtons">应用按钮实体</param>
        /// <returns></returns>
        public int Delete(Model.AppLibraryButton[] appLibraryButtons)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.RemoveRange(appLibraryButtons);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            Cache.IO.Remove(CACHEKEY);
        }
    }
}
