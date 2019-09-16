using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class User
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_user";
        private static List<Model.User> UserList = null;
        /// <summary>
        /// 得到所有用户
        /// </summary>
        /// <returns></returns>
        public List<Model.User> GetAll()
        {
            if (UserList != null)
            {
                return UserList;
            }
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var users = db.QueryAll<Model.User>();
                    Cache.IO.Insert(CACHEKEY, users);
                    UserList = users;
                    return users;
                }
            }
            else
            {
                var users = (List<Model.User>)obj;
                UserList = users;
                return users;
            }
        }
        /// <summary>
        /// 添加一个用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Add(Model.User user, Model.OrganizeUser organizeUser)
        {
            ClearCache();
            new OrganizeUser().ClearCache();//清除人员与机构关系表缓存
            new HomeSet().ClearCache();//清除首页设置缓存
            using (var db = new DataContext())
            {
                db.Add(user);
                db.Add(organizeUser);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="user">用户实体</param>
        public int Update(Model.User user)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(user);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一个用户
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <returns></returns>
        public int Delete(Model.User user, Model.OrganizeUser[] organizeUsers)
        {
            ClearCache();
            new OrganizeUser().ClearCache();//清除人员与机构关系表缓存
            new HomeSet().ClearCache();//清除首页设置缓存
            using (var db = new DataContext())
            {
                db.Remove(user);
                db.RemoveRange(organizeUsers);//删除人员与机构之间的关系
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            UserList = null;
            Cache.IO.Remove(CACHEKEY);
        }
    }
}
