using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class OrganizeUser
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_organizeuser";
        private static List<Model.OrganizeUser> OrganizeUserList = null;
        /// <summary>
        /// 得到所有组织人员
        /// </summary>
        /// <returns></returns>
        public List<Model.OrganizeUser> GetAll()
        {
            if (OrganizeUserList != null)
            {
                return OrganizeUserList;
            }
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var organizeUsers = db.QueryAll<Model.OrganizeUser>();
                    Cache.IO.Insert(CACHEKEY, organizeUsers);
                    OrganizeUserList = organizeUsers;
                    return organizeUsers;
                }
            }
            else
            {
                var organizeUsers = (List<Model.OrganizeUser>)obj;
                OrganizeUserList = organizeUsers;
                return organizeUsers;
            }
        }
        /// <summary>
        /// 添加一个组织人员
        /// </summary>
        /// <param name="organizeUser"></param>
        /// <returns></returns>
        public int Add(Model.OrganizeUser organizeUser)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(organizeUser);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新组织人员
        /// </summary>
        /// <param name="organizeUser">组织人员实体</param>
        public int Update(Model.OrganizeUser organizeUser)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(organizeUser);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新一批组织人员
        /// </summary>
        /// <param name="organizeUsers">组织人员实体数组</param>
        public int Update(Model.OrganizeUser[] organizeUsers)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.UpdateRange(organizeUsers);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新一批组织人员
        /// </summary>
        /// <param name="tuples">要更新的列表，，int 0删除 1新增 2修改</param>
        public int Update(List<Tuple<Model.OrganizeUser, int>> tuples)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                foreach (var tuple in tuples)
                {
                    if (tuple.Item2 == 0)
                    {
                        db.Remove(tuple.Item1);
                    }
                    else if (tuple.Item2 == 1)
                    {
                        db.Add(tuple.Item1);
                    }
                    else if (tuple.Item2 == 2)
                    {
                        db.Update(tuple.Item1);
                    }
                }
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一个组织人员
        /// </summary>
        /// <param name="organizeUser">组织人员实体</param>
        /// <returns></returns>
        public int Delete(Model.OrganizeUser organizeUser)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Remove(organizeUser);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            OrganizeUserList = null;
            Cache.IO.Remove(CACHEKEY);
        }
        
    }
}
