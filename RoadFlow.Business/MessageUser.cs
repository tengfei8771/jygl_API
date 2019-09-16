using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Business
{
    public class MessageUser
    {
        private readonly Data.MessageUser messageUserData;
        public MessageUser()
        {
            messageUserData = new Data.MessageUser();
        }
        
        /// <summary>
        /// 更新一个消息为已读
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int UpdateIsRead(Guid messageId, Guid userId)
        {
            return messageUserData.UpdateIsRead(messageId, userId);
        }

        /// <summary>
        /// 查询一页已发送消息阅读人员
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="messageId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetReadUserList(out int count, int size, int number, string messageId, string order)
        {
            return messageUserData.GetReadUserList(out count, size, number, messageId, order);
        }
    }
}
