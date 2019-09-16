using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Business
{
    public class Mail
    {
        private readonly Data.MailInBox mailInBoxData = new Data.MailInBox();
        private readonly Data.MailOutBox mailOutBoxData = new Data.MailOutBox();
        private readonly Data.MailDeletedBox mailDeletedBoxData = new Data.MailDeletedBox();
        private readonly Data.MailContent mailContentData = new Data.MailContent();

        #region 收件箱
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.MailInBox GetMailInBox(Guid id)
        {
            return mailInBoxData.Get(id);
        }

        /// <summary>
        /// 判断一个发件是否所有人都未读(如果所有人未读则可以撤回)
        /// </summary>
        /// <param name="outBoxId"></param>
        /// <returns></returns>
        public bool MailInBoxAllNoRead(Guid outBoxId)
        {
            return mailInBoxData.AllNoRead(outBoxId);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="mailInBox"></param>
        /// <returns></returns>
        public int AddMailInBox(Model.MailInBox mailInBox)
        {
            return mailInBoxData.Add(mailInBox);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="mailInBox"></param>
        /// <returns></returns>
        public int UpdateMailInBox(Model.MailInBox mailInBox)
        {
            return mailInBoxData.Update(mailInBox);
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id">收件箱ID</param>
        /// <param name="status">状态</param>
        /// <param name="isUpdateDate">是否更新阅读时间</param>
        /// <returns></returns>
        public int UpdateIsRead(Guid id, int status, bool isUpdateDate = false)
        {
            return mailInBoxData.UpdateIsRead(id, status, isUpdateDate);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="mailInBox"></param>
        /// <returns></returns>
        public int DeleteMailInBox(Model.MailInBox mailInBox)
        {
            return mailInBoxData.Delete(mailInBox);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">收件箱ID</param>
        /// <param name="status">删除方式 0转到已删除 1彻底删除</param>
        /// <returns></returns>
        public int DeleteMailInBox(Guid id, int status)
        {
            return mailInBoxData.Delete(id, status);
        }
        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetMailInBoxPagerList(out int count, int size, int number, Guid currentUserId, string subject, string userId, string date1, string date2, string order)
        {
            return mailInBoxData.GetPagerList(out count, size, number, currentUserId, subject, userId, date1, date2, order);
        }
        #endregion

        #region 发件箱
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.MailOutBox GetMailOutBox(Guid id)
        {
            return mailOutBoxData.Get(id);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="mailOutBox"></param>
        /// <returns></returns>
        public int AddMailOutBox(Model.MailOutBox mailOutBox)
        {
            return mailOutBoxData.Add(mailOutBox);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="mailOutBox"></param>
        /// <returns></returns>
        public int UpdateMailOutBox(Model.MailOutBox mailOutBox)
        {
            return mailOutBoxData.Update(mailOutBox);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="mailOutBox"></param>
        /// <returns></returns>
        public int DeleteMailOutBox(Model.MailOutBox mailOutBox)
        {
            return mailOutBoxData.Delete(mailOutBox);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="mailOutBox"></param>
        /// <returns></returns>
        public int DeleteMailOutBox(Guid id)
        {
            return mailOutBoxData.Delete(id);
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailOutBox"></param>
        /// <param name="mailContent"></param>
        /// <param name="isAdd">是否新增</param>
        /// <returns></returns>
        public int Send(Model.MailOutBox mailOutBox, Model.MailContent mailContent, bool isAdd)
        {
            Organize organize = new Organize();
            return mailOutBoxData.Send(mailOutBox, mailContent, organize.GetAllUsers(mailOutBox.ReceiveUsers),
                organize.GetAllUsers(mailOutBox.CarbonCopy), organize.GetAllUsers(mailOutBox.SecretCopy), isAdd);
        }
        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetMailOutBoxPagerList(out int count, int size, int number, Guid currentUserId, string subject, string date1, string date2, string order, int status)
        {
            return mailOutBoxData.GetPagerList(out count, size, number, currentUserId, subject, date1, date2, order, status);
        }
        /// <summary>
        /// 判断一个邮件是否可以撤回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsWithdraw(Guid id)
        {
            return MailInBoxAllNoRead(id);
        }
        /// <summary>
        /// 撤回邮件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Withdraw(Guid id)
        {
            return mailOutBoxData.Withdraw(id);
        }
        #endregion

        #region 已删除
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.MailDeletedBox GetMailDeletedBox(Guid id)
        {
            return mailDeletedBoxData.Get(id);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="mailInBox"></param>
        /// <returns></returns>
        public int AddMailDeletedBox(Model.MailDeletedBox mailDeletedBox)
        {
            return mailDeletedBoxData.Add(mailDeletedBox);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="mailDeletedBox"></param>
        /// <returns></returns>
        public int UpdateMailDeletedBox(Model.MailDeletedBox mailDeletedBox)
        {
            return mailDeletedBoxData.Update(mailDeletedBox);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="mailDeletedBox"></param>
        /// <returns></returns>
        public int DeleteMailDeletedBox(Model.MailDeletedBox mailDeletedBox)
        {
            return mailDeletedBoxData.Delete(mailDeletedBox);
        }
        /// <summary>
        /// 还原一个已删除邮件
        /// </summary>
        /// <param name="id">已删除邮件ID</param>
        /// <returns></returns>
        public int RecoveryMailDeletedBox(Guid id)
        {
            var mailDeletedBox = GetMailDeletedBox(id);
            if (null == mailDeletedBox)
            {
                return 0;
            }
            return mailDeletedBoxData.Recovery(mailDeletedBox);
        }
        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetMailDeletedBoxPagerList(out int count, int size, int number, Guid currentUserId, string subject, string userId, string date1, string date2, string order)
        {
            return mailDeletedBoxData.GetPagerList(out count, size, number, currentUserId, subject, userId, date1, date2, order);
        }
        #endregion

        #region 邮件内容
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.MailContent GetMailContent(Guid id)
        {
            return mailContentData.Get(id);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="mailContent"></param>
        /// <returns></returns>
        public int AddMailContent(Model.MailContent mailContent)
        {
            return mailContentData.Add(mailContent);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="mailContent"></param>
        /// <returns></returns>
        public int UpdateMailContent(Model.MailContent mailContent)
        {
            return mailContentData.Update(mailContent);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="mailContent"></param>
        /// <returns></returns>
        public int DeleteMailContent(Model.MailContent mailContent)
        {
            return mailContentData.Delete(mailContent);
        }
        #endregion

    }
}
