using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Utility;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Localization;

namespace RoadFlow.Business
{
    public class Message
    {
        private Data.Message messageData;
        public Message()
        {
            messageData = new Data.Message();
        }
        /// <summary>
        /// 添加一个消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public int Add(Model.Message message)
        {
            return messageData.Add(message);
        }

        /// <summary>
        /// 查询一个消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Message Get(Guid id)
        {
            return messageData.Get(id);
        }

        /// <summary>
        /// 添加一批消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public int Add(Model.Message messages, Model.MessageUser[] messageUsers)
        {
            return messageData.Add(messages, messageUsers);
        }

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="receiveUsers">接收人员，如果为空则从message.ReceiverIdString中取</param>
        /// <param name="localizer">语言包</param>
        /// <returns>返回1表示成功，其它为错误信息</returns>
        public string Send(Model.Message message, List<Model.User> receiveUsers = null, IStringLocalizer localizer = null)
        {
            if (null == message)
            {
                return localizer == null ? "消息为空!" : localizer["MessageEmpty"];
            }
            if (receiveUsers == null)
            {
                receiveUsers = new Organize().GetAllUsers(message.ReceiverIdString);
            }
            if (!receiveUsers.Any())
            {
                return localizer == null ? "消息没有接收人!" : localizer["MessageNotReceiver"];
            }
            if (message.ReceiverIdString.IsNullOrEmpty())
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var u in receiveUsers)
                {
                    stringBuilder.Append(Organize.PREFIX_USER);
                    stringBuilder.Append(u.Id);
                    stringBuilder.Append(",");
                }
                message.ReceiverIdString = stringBuilder.ToString().TrimEnd(',');
            }
            List<Model.MessageUser> messageUsers = new List<Model.MessageUser>();
            List<string> userIDList = new List<string>();//记录用户ID，用于发送singalr消息
            string[] sendTypes = message.SendType.Split(',');
            foreach (string sendType in sendTypes)
            {
                if (!sendType.IsInt(out int t))
                {
                    continue;
                }
                switch (t)
                {
                    case 0://站内短信
                        foreach (var user in receiveUsers)
                        {
                            messageUsers.Add(new Model.MessageUser()
                            {
                                MessageId = message.Id,
                                UserId = user.Id,
                                IsRead = 0
                            });
                            userIDList.Add(user.Id.ToString().ToLower());
                        }
                        break;
                    case 1://手机短信
                        StringBuilder mobiles = new StringBuilder();
                        foreach (var user in receiveUsers)
                        {
                            if (!user.Mobile.IsNullOrWhiteSpace())
                            {
                                mobiles.Append(user.Mobile);
                                mobiles.Append(",");
                            }
                        }
                        SMS.SendSMS(message.Contents.RemoveHTML(), mobiles.ToString().TrimEnd(','));
                        break;
                    case 2://微信
                        if (Config.Enterprise_WeiXin_IsUse)
                        {
                            JObject msgJson = new JObject
                            {
                                { "content", message.Contents + (message.SenderName.IsNullOrWhiteSpace() ? "" : "  " + (localizer == null ? "发送人：" : localizer["Sender"]) +message.SenderName) },
                            };
                            EnterpriseWeiXin.Common.SendMessage(receiveUsers, msgJson);
                        }
                        break;
                }
            }
            if (userIDList.Any())//发送singalr消息
            {
                JObject jObject = new JObject
                {
                    { "id", message.Id.ToString() },
                    { "title", localizer == null ? "消息" : localizer["Message"] },
                    { "contents", message.Contents },
                    { "count", 1 }
                };
                new SignalR.SignalRHub().SendMessage(jObject.ToString(Newtonsoft.Json.Formatting.None), userIDList);
            }
            return Add(message, messageUsers.ToArray()) > 0 ? "1" : (localizer == null ? "发送失败!" : localizer["SendFail"]);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="user">接收人员</param>
        /// <param name="content">消息内容</param>
        /// <param name="sendType">消息类型 0,1,2 (0站内消息 1手机短信 2微信)</param>
        /// <param name="sender">发送人</param>
        /// <returns>返回1表示成功，其它为错误信息</returns>
        public string Send(Model.User user, string content, string sendType = "0", Model.User sender = null)
        {
            return Send(new List<Model.User>() { user }, content, sendType, sender);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="userId">接收人员ID</param>
        /// <param name="content">消息内容</param>
        /// <param name="sendType">消息类型 0,1,2 (0站内消息 1手机短信 2微信)</param>
        /// <param name="sender">发送人</param>
        /// <returns>返回1表示成功，其它为错误信息</returns>
        public string Send(Guid userId, string content, string sendType = "0", Model.User sender = null)
        {
            return Send(new List<Model.User>() { new User().Get(userId) }, content, sendType, sender);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="users">接收人员</param>
        /// <param name="content">消息内容</param>
        /// <param name="sendType">消息类型 0,1,2 (0站内消息 1手机短信 2微信)</param>
        /// <param name="sender">发送人</param>
        /// <returns>返回1表示成功，其它为错误信息</returns>
        public string Send(List<Model.User> users, string content, string sendType = "0", Model.User sender = null)
        {
            Model.Message msg = new Model.Message
            {
                Contents = content,
                Id = Guid.NewGuid(),
                SendTime = DateExtensions.Now,
                SendType = sendType,
                SiteMessage = ("," + sendType + ",").Contains(",0,") ? 1 : 0,
                Type = sender == null ? 2 : 1
            };
            if (null != sender)
            {
                msg.SenderId = sender.Id;
                msg.SenderName = sender.Name;
            }
            return Send(msg, users);
        }

        /// <summary>
        /// 查询一页已发送消息
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="contents"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetSendList(out int count, int size, int number, string userId, string contents, string date1, string date2, string status, string order)
        {
            return messageData.GetSendList(out count, size, number, userId, contents, date1, date2, status, order);
        }

        /// <summary>
        /// 得到发送方式字符串
        /// </summary>
        /// <param name="receiveType">发送方式 0站内消息 1手机短信 2微信 </param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        public string GetSendTypeString(string sendType, IStringLocalizer localizer = null)
        {
            if (sendType.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            string[] types = sendType.Split(',');
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string type in types)
            {
                if (!type.IsInt(out int t))
                {
                    continue;
                }
                switch (t)
                {
                    case 0:
                        stringBuilder.Append(localizer == null ? "站内消息、" : localizer["MessageType0"] + "、");
                        break;
                    case 1:
                        stringBuilder.Append(localizer == null ? "手机短信、" : localizer["MessageType1"] + "、");
                        break;
                    case 2:
                        stringBuilder.Append(localizer == null ? "微信、" : localizer["MessageType2"] + "、");
                        break;
                }
            }
            return stringBuilder.ToString().TrimEnd('、');
        }

        /// <summary>
        /// 得到一个用户的未读消息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>(消息实体，未读消息条数)</returns>
        public (Model.Message, int count) GetNoRead(Guid userId)
        {
            return messageData.GetNoRead(userId);
        }
    }
}
