using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadFlow.Mvc.Common
{
    /// <summary>
    /// 定时任务类
    /// </summary>
    public class TimedJob : Job
    {
        private readonly Business.FlowTask flowTask;
        public TimedJob()
        {
            flowTask = new Business.FlowTask();
        }
        /// <summary>
        /// 自动提交流程任务
        /// Begin 起始时间；Interval执行时间间隔，单位是毫秒，建议使用以下格式，此处为1分钟；
        /// SkipWhileExecuting是否等待上一个执行完成，true为等待；
        /// </summary>
        [Invoke(IsEnabled = false, Begin = "2018-08-26 00:00", Interval = 2000 * 60, SkipWhileExecuting = true)]
        public void AutoSubmitFlowTask()
        {
            flowTask.AutoSubmitExpireTask();
        }

        /// <summary>
        /// 检查到期任务提醒
        /// </summary>
        [Invoke(IsEnabled = false, Begin = "2018-08-26 00:00", Interval = 3000 * 60, SkipWhileExecuting = true)]
        public void RemindTask()
        {
            flowTask.RemindTask();
        }

        /// <summary>
        /// 检查在线用户
        /// </summary>
        [Invoke(IsEnabled = false, Begin = "2018-08-26 00:00", Interval = 1000 * 60 * 3, SkipWhileExecuting = true)]
        public void CheckOnlineUser()
        {
            if (null == Business.OnlineUser.OnlineUsers || !Business.OnlineUser.OnlineUsers.Any())
            {
                return;
            }
            var expireUsers = Business.OnlineUser.OnlineUsers.Values.Where(p => p.LastTime.AddMinutes(Utility.Config.SessionTimeout <= 0 ? 20 : Utility.Config.SessionTimeout) < Current.DateTime);
            List<(Guid, int)> removeUsers = new List<(Guid, int)>();
            foreach(var user in expireUsers)
            {
                removeUsers.Add((user.UserId, user.LoginType));
            }
            foreach(var (userId, loginType) in removeUsers)
            {
                Business.OnlineUser.Remove(userId, loginType);
            }
        }

    }
}
