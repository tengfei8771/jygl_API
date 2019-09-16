using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Utility;
using Microsoft.Extensions.Localization;

namespace RoadFlow.Business
{
    /// <summary>
    /// 问卷调查业务类
    /// </summary>
    public class Vote
    {
        private readonly Data.Vote voteData;
        private readonly Data.VoteItem voteItemData;
        private readonly Data.VoteItemOption voteItemOptionData;
        private readonly Data.VotePartakeUser votePartakeUserData;
        private readonly Data.VoteResult voteResultData;
        private readonly Data.VoteResultUser voteResultUserData;

        public Vote()
        {
            voteData = new Data.Vote();
            voteItemData = new Data.VoteItem();
            voteItemOptionData = new Data.VoteItemOption();
            votePartakeUserData = new Data.VotePartakeUser();
            voteResultData = new Data.VoteResult();
            voteResultUserData = new Data.VoteResultUser();
        }

        /// <summary>
        /// 查询一个投票主题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Vote GetVote(Guid id)
        {
            return voteData.Get(id);
        }

        /// <summary>
        /// 更新投票主题
        /// </summary>
        /// <param name="vote"></param>
        /// <returns></returns>
        public int UpdateVote(Model.Vote vote)
        {
            return voteData.Update(vote);
        }

        /// <summary>
        /// 添加投票主题
        /// </summary>
        /// <param name="vote"></param>
        /// <returns></returns>
        public int AddVote(Model.Vote vote)
        {
            return voteData.Add(vote);
        }

        /// <summary>
        /// 删除投票
        /// </summary>
        /// <param name="voteId"></param>
        /// <param name="isDeleteAll">是否删除该投票的相关所有数据</param>
        /// <returns></returns>
        public int DeleteVote(Guid voteId, bool isDeleteAll = true)
        {
            return voteData.Delete(voteId, isDeleteAll);
        }

        /// <summary>
        /// 得到一个问卷的结果
        /// </summary>
        /// <param name="voteId"></param>
        /// <returns></returns>
        public List<Model.VoteResult> GetVoteResults(Guid voteId)
        {
            return voteResultData.GetVoteResults(voteId);
        }

        /// <summary>
        /// 得到一个问卷的用户
        /// </summary>
        /// <param name="voteId"></param>
        /// <returns></returns>
        public List<Model.VoteResultUser> GetVoteResultUsers(Guid voteId)
        {
            return voteResultUserData.GetVoteResultUsers(voteId);
        }

        /// <summary>
        /// 查询一页投票数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="topic"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetVotePagerList(out int count, int size, int number, Guid currentUserId, string topic, string date1, string date2, string order)
        {
            return voteData.GetPagerList(out count, size, number, currentUserId, topic, date1, date2, order);
        }

        /// <summary>
        /// 查询一页待提交投票数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="topic"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetWaitSubmitPagerList(out int count, int size, int number, Guid currentUserId, string topic, string date1, string date2, string order)
        {
            return voteData.GetWaitSubmitPagerList(out count, size, number, currentUserId, topic, date1, date2, order);
        }

        /// <summary>
        /// 查询一页结果投票数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="topic"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetResultPagerList(out int count, int size, int number, Guid currentUserId, string topic, string date1, string date2, string order)
        {
            return voteData.GetResultPagerList(out count, size, number, currentUserId, topic, date1, date2, order);
        }

        /// <summary>
        /// 查询一页参与人员数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="topic"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPartakeUserPagerList(out int count, int size, int number, Guid voteId, string name, string org, string order)
        {
            return voteData.GetPartakeUserPagerList(out count, size, number, voteId, name, org, order);
        }

        /// <summary>
        /// 根据问卷ID得到选题
        /// </summary>
        /// <param name="voteId"></param>
        /// <returns></returns>
        public List<Model.VoteItem> GetVoteItems(Guid voteId)
        {
            return voteItemData.GetItems(voteId);
        }

        /// <summary>
        /// 得到选题的选项
        /// </summary>
        /// <param name="itemId">选题ID</param>
        /// <returns></returns>
        public List<Model.VoteItemOption> GetItemOptions(Guid itemId)
        {
            return voteItemOptionData.GetItemOptions(itemId);
        }

        /// <summary>
        /// 得到整个问卷的选项
        /// </summary>
        /// <param name="voteId">问卷ID</param>
        /// <returns></returns>
        public List<Model.VoteItemOption> GetVoteItemOptions(Guid voteId)
        {
            return voteItemOptionData.GetVoteItemOptions(voteId);
        }

        /// <summary>
        /// 得到问卷选项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.VoteItem GetVoteItem(Guid id)
        {
            return voteItemData.Get(id);
        }

        /// <summary>
        /// 得到选项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.VoteItemOption GetVoteItemOption(Guid id)
        {
            return voteItemOptionData.Get(id);
        }

        /// <summary>
        /// 更新选题
        /// </summary>
        /// <param name="voteItem"></param>
        /// <returns></returns>
        public int UpdateVoteItem(Model.VoteItem voteItem)
        {
            return voteItemData.Update(voteItem);
        }

        /// <summary>
        /// 添加选题
        /// </summary>
        /// <param name="voteItem"></param>
        /// <returns></returns>
        public int AddVoteItem(Model.VoteItem voteItem)
        {
            return voteItemData.Add(voteItem);
        }

        /// <summary>
        /// 删除选题
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public int DeleteVoteItem(Guid itemId)
        {
            return voteItemData.Delete(itemId);
        }

        /// <summary>
        /// 更新选题的选项
        /// </summary>
        /// <param name="voteItemOption"></param>
        /// <returns></returns>
        public int UpdateVoteItemOption(Model.VoteItemOption voteItemOption)
        {
            return voteItemOptionData.Update(voteItemOption);
        }

        /// <summary>
        /// 添加选题的选项
        /// </summary>
        /// <param name="voteItemOption"></param>
        /// <returns></returns>
        public int AddVoteItemOption(Model.VoteItemOption voteItemOption)
        {
            return voteItemOptionData.Add(voteItemOption);
        }

        /// <summary>
        /// 得到选题最大排序
        /// </summary>
        /// <param name="voteId"></param>
        /// <returns></returns>
        public int GetVoteItemMaxSort(Guid voteId)
        {
            var items = GetVoteItems(voteId);
            return items.Count == 0 ? 5 : items.Max(p => p.Sort) + 5;
        }

        /// <summary>
        /// 添加结果
        /// </summary>
        /// <param name="voteItems"></param>
        /// <returns></returns>
        public int AddVoteResults(List<Model.VoteResult> voteItems)
        {
            return voteResultData.AddRange(voteItems);
        }

        /// <summary>
        /// 查询一个问卷的参与人员
        /// </summary>
        /// <param name="voteID"></param>
        /// <param name="status">-1查询所有 0未提交 1已提交</param>
        /// <returns></returns>
        public List<Model.VotePartakeUser> GetPartakeUsers(Guid voteID, int status = -1)
        {
            return votePartakeUserData.GetPartakeUsers(voteID, status);
        }

        /// <summary>
        /// 发布一个问卷
        /// </summary>
        /// <param name="voteId"></param>
        /// <param name="localizer">语言包</param>
        /// <returns>返回“1”发布成功，其它为错误信息</returns>
        public string Publish(Guid voteId, IStringLocalizer localizer = null)
        {
            var voteModel = GetVote(voteId);
            if (null == voteModel)
            {
                return localizer == null ? "未找到要发布的问卷!" : localizer["NotFoundPublishVote"];
            }
            if (voteModel.Status != 0)
            {
                return localizer == null ? "该问卷已发布!" : localizer["VoteIsPublish"];
            }
            if (voteModel.PartakeUsers.IsNullOrWhiteSpace())
            {
                return localizer == null ? "该问卷没有设置要参与的人员!" : localizer["VoteNotSetUser"];
            }
            Organize organize = new Organize();
            var users = organize.GetAllUsers(voteModel.PartakeUsers);
            if (users.Count == 0)
            {
                return localizer == null ? "该问卷没有要参与的人员!" : localizer["VoteNotSetUser"];
            }
            if (GetVoteItems(voteId).Count == 0)
            {
                return localizer == null ? "该问卷未设置选题!" : localizer["VoteNotTopic"];
            }
            if (voteModel.ResultViewUsers.IsNullOrWhiteSpace())
            {
                return localizer == null ? "该问卷没有设置结果查看人员!" : localizer["VoteNotSetResustViewUser"];
            }
            var resultUsers = organize.GetAllUsers(voteModel.ResultViewUsers);
            if (resultUsers.Count == 0)
            {
                return localizer == null ? "该问卷没有结果查看人员!" : localizer["VoteNotSetResustViewUser"];
            }
            User buser = new User();
            List<Model.VotePartakeUser> votePartakeUsers = new List<Model.VotePartakeUser>();
            foreach (var user in users)
            {
                votePartakeUsers.Add(new Model.VotePartakeUser()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    UserName = user.Name,
                    UserOrganize = buser.GetOrganizeMainShowHtml(user.Id, false),
                    VoteId = voteId,
                    Status = 0
                });
            }
            List<Model.VoteResultUser> voteResultUsers = new List<Model.VoteResultUser>();
            foreach (var user in resultUsers)
            {
                voteResultUsers.Add(new Model.VoteResultUser()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    VoteId = voteId
                });
            }
            //if (!voteResultUsers.Exists(p => p.Id == voteModel.CreateUserId))//如果结果查询人员没有选择自己加上自己
            //{
            //    voteResultUsers.Add(new Model.VoteResultUser()
            //    {
            //        Id = Guid.NewGuid(),
            //        UserId = voteModel.CreateUserId,
            //        VoteId = voteId
            //    });
            //}
            return votePartakeUserData.Add(votePartakeUsers, voteResultUsers) > 0 ? "1" : (localizer == null ? "发布失败!" : localizer["PublishFail"]);
        }

        /// <summary>
        /// 取消发布一个问卷
        /// </summary>
        /// <param name="voteId"></param>
        /// <param name="localizer">语言包</param>
        /// <returns>返回“1”发布成功，其它为错误信息</returns>
        public string UnPublish(Guid voteId, IStringLocalizer localizer = null)
        {
            var voteModel = GetVote(voteId);
            if (voteModel == null)
            {
                return localizer == null ? "未找到该问卷!" : localizer["NotFoundVote"];
            }
            if (voteModel.Status > 1)
            {
                return localizer == null ? "该问卷已有提交结果,不能取消!" : localizer["VoteCannotCancel"];
            }
            return votePartakeUserData.DeleteByVoteId(voteId) > 0 ? "1" : (localizer == null ? "取消发布失败!" : localizer["CancelPublishFail"]);
        }
    }
}
