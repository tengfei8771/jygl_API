using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoadFlow.Utility;
using Microsoft.AspNetCore.Http;

namespace RoadFlow.Business.SignalR
{
    public class SignalRHub : Hub
    {
        private static IHubContext<SignalRHub> _hubContext;
        public static void Configure(IHubContext<SignalRHub> accessor)
        {
            _hubContext = accessor;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userIds">接收消息的用户ID</param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task SendMessage(string message, List<string> userIds, string userName = "")
        {
            return _hubContext.Clients.Groups(userIds).SendAsync("SendMessage", userName.IsNullOrWhiteSpace() ? "系统" : userName, message);
        }
        public override async Task OnConnectedAsync()
        {
            string userId = User.CurrentUserId.ToString().ToLower();
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userId = User.CurrentUserId.ToString().ToLower();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
