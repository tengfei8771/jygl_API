using System;
using System.Collections.Generic;
using System.Data.Common;

namespace RoadFlow.Data.SqlInterface
{
    public interface ISql
    {

        /// <summary>
        /// 得到SQL参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        DbParameter GetDbParameter(string name, object value);

        /// <summary>
        /// 得到数据库所有表SQL
        /// </summary>
        /// <returns></returns>
        string GetDbTablesSql();

        /// <summary>
        /// 得到表字段SQL
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbName">数据库名(MYSQL时需要,防止同一个连接中不同的数据库中有相同的表名的情况。其它数据库为空)</param>
        /// <returns></returns>
        string GetTableFieldsSql(string tableName, string dbName);

        /// <summary>
        /// 得到查询一个字段值SQL
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="primaryKey"></param>
        /// <param name="primaryKeyValue"></param>
        /// <returns></returns>
        (string sql, DbParameter parameter) GetFieldValueSql(string tableName, string fieldName, string primaryKey, string primaryKeyValue);

        /// <summary>
        /// 得到保存数据SQL
        /// </summary>
        /// <param name="dicts"></param>
        /// <param name="flag">0删除 1新增 2修改</param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetSaveDataSql(Dictionary<string, object> dicts, string tableName, String primaryKey, int flag);

        /// <summary>
        /// 得到查询自增ID值sql
        /// </summary>
        /// <param name="seqName"></param>
        /// <returns></returns>
        string GetIdentitySql(string seqName = "");

        /// <summary>
        /// 得到分页sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string GetPaerSql(string sql, int size, int number, out int count, DbParameter[] param = null, string order = "");

        /// <summary>
        /// 得到查询应用程序库SQL
        /// </summary>
        /// <param name="title"></param>
        /// <param name="address"></param>
        /// <param name="typeId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetApplibrarySql(string title, string address, string typeId, string order);

        /// <summary>
        /// 得到查询流程SQL
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="order"></param>
        /// <param name="status">状态-1表示查询未删除的流程</param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetFlowSql(List<Guid> flowIdList, string name, string type, string order, int status = -1);

        /// <summary>
        /// 得到查询表单SQL
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetFormSql(Guid userId, string name, string type, string order, int status = -1);

        /// <summary>
        /// 得到查询日志sql
        /// </summary>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetLogSql(string title, string type, string userId, string date1, string date2, string order);

        /// <summary>
        /// 得到查询待办SQL
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <param name="title">任务标题</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetQueryWaitTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order);

        /// <summary>
        /// 得到查询已办SQL
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <param name="title">任务标题</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetQueryCompletedTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order);

        /// <summary>
        /// 得到查询我发起的流程SQL
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <param name="title">任务标题</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetQueryMyStartTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order);

        /// <summary>
        /// 得到查询已委托事项SQL
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <param name="title">任务标题</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetQueryEntrustTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order);

        /// <summary>
        /// 得到查询实例列表SQL
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="receiveId"></param>
        /// <param name="receiveDate1"></param>
        /// <param name="receiveDate2"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetQueryInstanceSql(string flowId, string title, string receiveId, string receiveDate1, string receiveDate2, string order);

        /// <summary>
        /// 查询一个组中最新的一条任务
        /// </summary>
        /// <param name="gropuId"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetQueryGroupMaxTaskSql(Guid gropuId);

        /// <summary>
        /// 得到查询文档列表SQL
        /// </summary>
        /// <param name="title"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetDocSql(Guid userId, string title, string dirId, string date1, string date2, string order, int read);

        /// <summary>
        /// 查询文档阅读情况
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetDocReadUserListSql(string docId, string order);

        /// <summary>
        /// 查询一页委托数据SQL
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetFlowEntrustSql(string userId, string date1, string date2, string order);

        /// <summary>
        /// 查询一页流程归档SQL
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="stepName"></param>
        /// <param name="title"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetFlowArchiveSql(string flowId, string stepName, string title, string date1, string date2, string order);

        /// <summary>
        /// 查询一页程序设计
        /// </summary>
        /// <param name="name"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetProgramSql(string name, string types, string order);

        /// <summary>
        /// 查询一页首页设置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetHomeSetSql(string name, string title, string type, string oreder);

        /// <summary>
        /// 查询一页已发送消息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="status"></param>
        /// <param name="oreder"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetMessageSendListSql(string userId, string content, string date1, string date2, string status, string order);

        /// <summary>
        /// 查询一页已发送消息阅读人员
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetMessageReadUserListSql(string messageId, string order);

        /// <summary>
        /// 查询一页分享的文件 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetUserFileShareSql(Guid userId, string fileName, string order);

        /// <summary>
        /// 查询一页收到的分享文件 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetUserFileShareMySql(Guid userId, string fileName, string order);

        /// <summary>
        /// 查询一页收件箱SQL
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetMailInBoxSql(Guid currentUserId, string subject, string userId, string date1, string date2);

        /// <summary>
        /// 查询一页发件箱SQL
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetMailOutBoxSql(Guid currentUserId, string subject, string date1, string date2, int status);

        /// <summary>
        /// 查询一页已删除邮件SQL
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetMailDeletedBoxSql(Guid currentUserId, string subject, string userId, string date1, string date2);

        /// <summary>
        /// 查询一页投票SQL
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        (string sql, DbParameter[] parameter) GetVoteSql(Guid currentUserId, string topic, string date1, string date2);

        /// <summary>
        /// 查询一页待提交投票SQL
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        (string sql, DbParameter[] parameter) GetWaitSubmitVoteSql(Guid currentUserId, string topic, string date1, string date2);

        /// <summary>
        /// 查询一页投票结果SQL
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        (string sql, DbParameter[] parameter) GetVoteResultSql(Guid currentUserId, string topic, string date1, string date2);

        /// <summary>
        /// 查询一页参与人员数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="org"></param>
        /// <returns></returns>
        (string sql, DbParameter[] parameter) GetPartakeUserSql(Guid voteId, string name, string org);
    }
}
