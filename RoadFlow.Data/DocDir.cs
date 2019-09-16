using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Utility;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class DocDir
    {
        private const string CACHEKEY = "roadflow_cache_docdir";
        /// <summary>
        /// 得到所有目录
        /// </summary>
        /// <returns></returns>
        public List<Model.DocDir> GetAll()
        {
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var docDirs = db.QueryAll<Model.DocDir>().OrderBy(p => p.Sort).ToList();
                    Cache.IO.Insert(CACHEKEY, docDirs);
                    return docDirs;
                }
            }
            else
            {
                return (List<Model.DocDir>)obj;
            }
        }
        /// <summary>
        /// 添加一个目录
        /// </summary>
        /// <param name="docDir">目录实体</param>
        /// <returns></returns>
        public int Add(Model.DocDir docDir)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(docDir);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新目录
        /// </summary>
        /// <param name="docDir">目录实体</param>
        public int Update(Model.DocDir docDir)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(docDir);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="docDirs">目录实体</param>
        /// <returns></returns>
        public int Delete(Model.DocDir[] docDirs)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.RemoveRange(docDirs);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 判断一个栏目下是否有文档
        /// </summary>
        /// <param name="dirIds"></param>
        /// <returns></returns>
        public bool HasDoc(List<Guid> dirIds)
        {
            using (var db = new DataContext())
            {
                var dt = db.GetDataTable("SELECT COUNT(Id) FROM RF_Doc WHERE DirId IN(" + dirIds.JoinSqlIn().ToUpper() + ")");
                return dt.Rows[0][0].ToString().ToInt() > 0;
            }
        }
        /// <summary>
        /// 得到根栏目
        /// </summary>
        /// <returns></returns>
        public Model.DocDir GetRoot()
        {
            var all = GetAll();
            return all.Count == 0 ? null : all.Find(p => p.ParentId == Guid.Empty);
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
