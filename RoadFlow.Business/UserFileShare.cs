using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;

namespace RoadFlow.Business
{
    public class UserFileShare
    {
        private Data.UserFileShare userFileShareData;
        public UserFileShare()
        {
            userFileShareData = new Data.UserFileShare();
        }
        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="userFileShare"></param>
        /// <returns></returns>
        public int Add(Model.UserFileShare userFileShare)
        {
            return userFileShareData.Add(userFileShare);
        }

        /// <summary>
        /// 添加多条
        /// </summary>
        /// <param name="userFileShares"></param>
        /// <returns></returns>
        public int Add(IEnumerable<Model.UserFileShare> userFileShares)
        {
            return userFileShareData.Add(userFileShares);
        }

        /// <summary>
        /// 删除一个文件的分享记录
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public int DeleteByFileId(string fileId)
        {
            return userFileShareData.DeleteByFileId(fileId);
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Model.UserFileShare Get(string fileId, Guid userId)
        {
            return userFileShareData.Get(fileId, userId);
        }

        /// <summary>
        /// 分享目录
        /// </summary>
        /// <param name="dirs">分享的目录或文件</param>
        /// <param name="members">分享给谁</param>
        /// <param name="shareUerId">分享人员ID</param>
        /// <param name="expireDate">过期时间</param>
        /// <returns></returns>
        public int Share(string dirs, string members, Guid shareUerId, DateTime expireDate)
        {
            if (dirs.IsNullOrWhiteSpace())
            {
                return 0;
            }
            string[] dirArray = dirs.Split(',');
            var users = new Organize().GetAllUsers(members);
            DateTime shareDateTime = DateExtensions.Now;
            int i = 0;
            foreach (string dir in dirArray)
            {
                if (dir.IsNullOrWhiteSpace())
                {
                    continue;
                }
                List<Model.UserFileShare> userFileShares = new List<Model.UserFileShare>();
                foreach (var user in users)
                {
                    if(user.Id == shareUerId)
                    {
                        continue;//自己分享给自己不加记录
                    }
                    Model.UserFileShare userFileShare = new Model.UserFileShare
                    {
                        FileId = dir,
                        ShareDate = shareDateTime,
                        FileName = System.IO.Path.GetFileName(dir.DESDecrypt()),
                        ShareUserId = shareUerId,
                        UserId = user.Id,
                        ExpireDate = expireDate,
                        IsView = 0
                    };
                    userFileShares.Add(userFileShare);
                }
                i += userFileShareData.Share(userFileShares, dir);
            }
            return i;
        }

        /// <summary>
        /// 查询一页我分享的数据
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable GetMySharePagerList(out int count, int size, int number, Guid userId, string fileName, string order)
        {
            return userFileShareData.GetMySharePagerList(out count, size, number, userId, fileName, order);
        }

        /// <summary>
        /// 查询我分享的文件的接收人员
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable GetMyShareUsers(string fileId, Guid userId)
        {
            return userFileShareData.GetMyShareUsers(fileId, userId);
        }

        /// <summary>
        /// 查询一页我收到的数据
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable GetShareMyPagerList(out int count, int size, int number, Guid userId, string fileName, string order)
        {
            return userFileShareData.GetShareMyPagerList(out count, size, number, userId, fileName, order);
        }

        /// <summary>
        /// 判断一个用户是否可以访问该文件夹或文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="userId"></param>
        /// <param name="fileId1">如果第一个fileId不存在，则从fileId1中找，分享文件夹时的情况</param>
        /// <returns></returns>
        public bool IsAccess(string fileId, Guid userId, string fileId1 = "")
        {
            var share = Get(fileId, userId);
            if (null == share && !fileId1.IsNullOrWhiteSpace())
            {
                share = Get(fileId1, userId);
            }
            return share != null && share.ExpireDate > DateExtensions.Now;
        }
    }
}
