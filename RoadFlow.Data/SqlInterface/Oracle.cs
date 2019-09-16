using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Linq;
using RoadFlow.Utility;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using RoadFlow.Mapper;

namespace RoadFlow.Data.SqlInterface
{
    public class Oracle : ISql
    {
        private readonly string dbType;
        private readonly string connStr;
        private readonly Model.DbConnection dbConnectionModel;

        public Oracle(Model.DbConnection dbConnection, string dataBaseType)
        {
            dbType = dataBaseType.ToLower();
            if (null != dbConnection)
            {
                connStr = dbConnection.ConnString;
                dbConnectionModel = dbConnection;
            }
        }
        /// <summary>
        /// 得到SQL参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DbParameter GetDbParameter(string name, object value)
        {
            return new OracleParameter(name, Common.GetParameterValue(value));
        }
        /// <summary>
        /// 得到数据库所有表SQL
        /// </summary>
        public string GetDbTablesSql()
        {
            return "select a.TABLE_NAME,b.COMMENTS  from user_tables a,user_tab_comments b WHERE a.TABLE_NAME=b.TABLE_NAME and a.TABLE_NAME not like '%$%' and a.TABLE_NAME not like 'LOGMNR%' and a.TABLE_NAME not like 'HELP%' and a.TABLE_NAME not like 'SPRING%' and a.TABLE_NAME not like 'SQLPLUS%' order by TABLE_NAME";
        }
        /// <summary>
        /// 得到表字段SQL
        /// </summary>
        public string GetTableFieldsSql(string tableName, string dbName)
        {
            return string.Format("SELECT user_tab_columns.COLUMN_NAME as f_name," +
            "user_tab_columns.DATA_TYPE as t_name," +
            "user_tab_columns.CHAR_LENGTH AS length," +
            "CASE user_tab_columns.NULLABLE WHEN 'Y' THEN 1 WHEN 'N' THEN 0 END AS is_null," +
            "user_tab_columns.DATA_DEFAULT AS cdefault," +
            "0 as isidentity,user_tab_columns.DATA_DEFAULT AS defaultvalue,user_col_comments.comments FROM user_tab_columns,user_col_comments " +
            "WHERE user_tab_columns.COLUMN_NAME=user_col_comments.COLUMN_NAME " +
            "and user_tab_columns.TABLE_NAME=user_col_comments.TABLE_NAME " +
            "and UPPER(user_tab_columns.TABLE_NAME)=UPPER('{0}') " +
            "and UPPER(user_col_comments.TABLE_NAME)=UPPER('{0}') " +
            "ORDER BY user_tab_columns.COLUMN_ID", tableName);
        }
        /// <summary>
        /// 得到查询一个字段值SQL
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="primaryKey"></param>
        /// <param name="primaryKeyValue"></param>
        /// <returns></returns>
        public (string sql, DbParameter parameter) GetFieldValueSql(string tableName, string fieldName, string primaryKey, string primaryKeyValue)
        {
            string sql = "SELECT " + fieldName + " FROM " + tableName + " WHERE " + primaryKey + "=:primarykeyvalue";
            DbParameter parameter = new OracleParameter(":primarykeyvalue", primaryKeyValue);
            return (sql, parameter);
        }
        /// <summary>
        /// 得到保存数据SQL
        /// </summary>
        /// <param name="dicts"></param>
        /// <param name="flag">0删除 1新增 2修改</param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetSaveDataSql(Dictionary<string, object> dicts, string tableName, string primaryKey, int flag)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            if (0 == flag)
            {
                sqlBuilder.Append("DELETE FROM " + tableName);
            }
            else if (1 == flag)
            {
                sqlBuilder.Append("INSERT INTO " + tableName + "(");
                foreach (var dict in dicts)
                {
                    sqlBuilder.Append(dict.Key);
                    if (!dict.Key.Equals(dicts.Last().Key))
                    {
                        sqlBuilder.Append(",");
                    }
                }
                sqlBuilder.Append(") VALUES(");
            }
            else if (2 == flag)
            {
                sqlBuilder.Append("UPDATE " + tableName + " SET ");
            }
            foreach (var dict in dicts)
            {
                if (0 == flag)
                {
                    sqlBuilder.Append(" WHERE " + primaryKey + "=:" + primaryKey);
                    parameters.Add(new OracleParameter(":" + primaryKey, Common.GetParameterValue(dicts[primaryKey])));
                }
                else if (1 == flag)
                {
                    parameters.Add(new OracleParameter(":" + dict.Key, Common.GetParameterValue(dict.Value)));
                    sqlBuilder.Append(":" + dict.Key);
                    if (!dict.Key.Equals(dicts.Last().Key))
                    {
                        sqlBuilder.Append(",");
                    }
                }
                else if (2 == flag)
                {
                    if (dict.Key.EqualsIgnoreCase(primaryKey))
                    {
                        continue;
                    }
                    sqlBuilder.Append(dict.Key + "=:" + dict.Key);
                    parameters.Add(new OracleParameter(":" + dict.Key, Common.GetParameterValue(dict.Value)));
                    if (!dict.Key.Equals(dicts.Last().Key))
                    {
                        sqlBuilder.Append(",");
                    }
                }
            }
            if (1 == flag)
            {
                sqlBuilder.Append(")");
            }
            else if (2 == flag)
            {
                sqlBuilder.Append(" WHERE  " + primaryKey + "=:" + primaryKey);
                var primaryDicts = dicts.Where(p => p.Key.EqualsIgnoreCase(primaryKey));
                object o = primaryDicts.Any() ? primaryDicts.First().Value : null;
                parameters.Add(new OracleParameter(":" + primaryKey, Common.GetParameterValue(o)));
            }
            return (sqlBuilder.ToString(), parameters.ToArray());
        }
        /// <summary>
        /// 得到查询自增ID值sql
        /// </summary>
        /// <param name="seqName"></param>
        /// <returns></returns>
        public string GetIdentitySql(string seqName = "")
        {
            return "SELECT " + seqName + ".CURRVAL FROM DUAL";
        }
        /// <summary>
        /// 得到分页sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string GetPaerSql(string sql, int size, int number, out int count, DbParameter[] param = null, string order = "")
        {
            string countString = string.Empty;
            using (var db = null == dbConnectionModel ? new DataContext() : new DataContext(dbConnectionModel.ConnType, dbConnectionModel.ConnString))
            {
                countString = db.ExecuteScalarString(string.Format("SELECT COUNT(*) FROM ({0}) PagerCountTemp", sql), param);
            }
            count = countString.IsInt(out int i) ? i : 0;
            if (count < number * size - size + 1)
            {
                number = 1;
            }
            StringBuilder sql1 = new StringBuilder();
            sql1.Append("SELECT * FROM (");
            if (!sql.ContainsIgnoreCase("ROW_NUMBER()") && !sql.ContainsIgnoreCase("ROWNUM"))
            {
                sql = sql.Insert(sql.IndexOfIgnoreCase("from") - 1, ",ROW_NUMBER() OVER(ORDER BY " + order.FilterSelectSql() + ") AS PagerAutoRowNumber");
            }
            sql1.Append(sql);
            sql1.AppendFormat(") PagerTempTable");
            sql1.AppendFormat(" WHERE PagerAutoRowNumber BETWEEN {0} AND {1}", number * size - size + 1, number * size);
            return sql1.ToString();
        }

        /// <summary>
        /// 得到查询应用程序库SQL
        /// </summary>
        /// <param name="title"></param>
        /// <param name="address"></param>
        /// <param name="typeId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetApplibrarySql(string title, string address, string typeId, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,Title,Address,Type,Title_en,Title_zh FROM RF_AppLibrary WHERE 1=1";
            if (!title.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND (CHARINDEX(:Title,Title)>0 OR CHARINDEX(:Title,Title_en)>0 OR CHARINDEX(:Title,Title_zh)>0)");
                parameters.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (!address.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND CHARINDEX(:Address,Address)>0");
                parameters.Add(new OracleParameter(":Address", address.Trim()));
            }
            if (!typeId.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND Type IN(" + typeId.ToUpper() + ")");
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }
        /// <summary>
        /// 得到查询流程SQL
        /// </summary>
        /// <param name="flowId">可管理的流程ID</param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetFlowSql(List<Guid> flowIdList, string name, string type, string order, int status = -1)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,Name,CreateDate,CreateUser,Status,Note,SystemId FROM RF_Flow WHERE" + (status != -1 ? " Status=" + status.ToString() : " Status!=3");
            if (flowIdList != null && flowIdList.Count > 0)
            {
                whereBuilder.Append(" AND Id IN(" + flowIdList.JoinSqlIn().ToUpper() + ")");
            }
            else
            {
                whereBuilder.Append(" AND 1=0");
            }
            if (!name.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Name,:Name,1,1)>0");
                parameters.Add(new OracleParameter(":Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND FlowType IN(" + type.ToUpper() + ")");
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 得到查询表单SQL
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetFormSql(Guid userId, string name, string type, string order, int status = -1)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,Name,CreateUserName,CreateDate,EditDate,Status FROM RF_Form WHERE INSTR(MANAGEUSER,:MANAGEUSER,1,1)>0 AND " + (status == -1 ? "Status!=2" : "Status=" + status.ToString());
            parameters.Add(new OracleParameter(":MANAGEUSER", userId.ToLowerString()));
            if (!name.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Name,:Name,1,1)>0");
                parameters.Add(new OracleParameter(":Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND FormType IN(" + type.ToUpper() + ")");
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

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
        public (string sql, DbParameter[] parameter) GetLogSql(string title, string type, string userId, string date1, string date2, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,Title,Type,WriteTime,UserName,IPAddress FROM RF_Log WHERE 1=1";
            if (!title.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Title,:Title,1,1)>0");
                parameters.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND Type=:Type");
                parameters.Add(new OracleParameter(":Type", type));
            }
            if (userId.IsGuid(out Guid uid))
            {
                whereBuilder.Append(" AND UserId=:UserId");
                parameters.Add(new OracleParameter(":UserId", userId.ToUpper()));
            }
            if (date1.IsDateTime(out DateTime date11))
            {
                whereBuilder.Append(" AND WriteTime>=:WriteTime");
                parameters.Add(new OracleParameter(":WriteTime", date11.GetDate()));
            }
            if (date2.IsDateTime(out DateTime date22))
            {
                whereBuilder.Append(" AND WriteTime<:WriteTime1");
                parameters.Add(new OracleParameter(":WriteTime1", date22.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }


        /// <summary>
        /// 得到查询待办SQL
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <param name="title">任务标题</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetQueryWaitTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,FlowId,FlowName,StepId,StepName,InstanceId,GroupId,TaskType,Title,SenderId,SenderName,ReceiveTime,CompletedTime,Status,Note FROM RF_FlowTask WHERE ReceiveId=:ReceiveId AND Status IN(0,1)";
            parameters.Add(new OracleParameter(":ReceiveId", userId.ToUpperString()));
            if (!flowId.IsNullOrWhiteSpace())
            {
                if (flowId.IsGuid(out Guid flowGuid))
                {
                    whereBuilder.Append(" AND FlowId=:FlowId");
                    parameters.Add(new OracleParameter(":FlowId", flowGuid));
                }
                else
                {
                    whereBuilder.Append(" AND FlowId IN(" + flowId.ToSqlIn() + ")");
                }
            }
            if (!title.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Title,:Title,1,1)>0");
                parameters.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (startDate.IsDateTime(out DateTime date1))
            {
                whereBuilder.Append(" AND ReceiveTime>=:ReceiveTime");
                parameters.Add(new OracleParameter(":ReceiveTime", date1.GetDate()));
            }
            if (endDate.IsDateTime(out DateTime date2))
            {
                whereBuilder.Append(" AND ReceiveTime<:ReceiveTime1");
                parameters.Add(new OracleParameter(":ReceiveTime1", date2.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 得到查询已办SQL
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <param name="title">任务标题</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetQueryCompletedTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,FlowId,FlowName,StepId,StepName,GroupId,InstanceId,Title,SenderId,SenderName,ReceiveTime,CompletedTime,CompletedTime1,Status,ExecuteType,Note FROM RF_FlowTask WHERE ReceiveId=:ReceiveId AND ExecuteType>1";
            parameters.Add(new OracleParameter(":ReceiveId", userId.ToUpperString()));
            if (flowId.IsGuid(out Guid flowGuid))
            {
                whereBuilder.Append(" AND FlowId=:FlowId");
                parameters.Add(new OracleParameter(":FlowId", flowId.ToUpper()));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Title,:Title,1,1)>0");
                parameters.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (startDate.IsDateTime(out DateTime date1))
            {
                whereBuilder.Append(" AND CompletedTime1>=:CompletedTime1");
                parameters.Add(new OracleParameter(":CompletedTime1", date1.GetDate()));
            }
            if (endDate.IsDateTime(out DateTime date2))
            {
                whereBuilder.Append(" AND CompletedTime1<:CompletedTime11");
                parameters.Add(new OracleParameter(":CompletedTime11", date2.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 得到查询我发起的流程SQL
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <param name="title">任务标题</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetQueryMyStartTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,FlowId,FlowName,StepId,GroupId,InstanceId,Title,ReceiveTime FROM RF_FlowTask WHERE SenderId=:SenderId AND PrevId=:PrevId";
            parameters.Add(new OracleParameter(":SenderId", userId.ToUpperString()));
            parameters.Add(new OracleParameter(":PrevId", Guid.Empty.ToUpperString()));
            if (flowId.IsGuid(out Guid flowGuid))
            {
                whereBuilder.Append(" AND FlowId=:FlowId");
                parameters.Add(new OracleParameter(":FlowId", flowId.ToUpper()));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Title,:Title,1,1)>0");
                parameters.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (startDate.IsDateTime(out DateTime date1))
            {
                whereBuilder.Append(" AND ReceiveTime>=:ReceiveTime");
                parameters.Add(new OracleParameter(":ReceiveTime", date1.GetDate()));
            }
            if (endDate.IsDateTime(out DateTime date2))
            {
                whereBuilder.Append(" AND ReceiveTime<:ReceiveTime1");
                parameters.Add(new OracleParameter(":ReceiveTime1", date2.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 得到查询已委托事项SQL
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <param name="title">任务标题</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetQueryEntrustTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,FlowId,FlowName,StepId,StepName,GroupId,InstanceId,Title,SenderName,ReceiveTime,ReceiveName,CompletedTime1,Status,ExecuteType,Note FROM RF_FlowTask WHERE EntrustUserId=:EntrustUserId";
            parameters.Add(new OracleParameter(":EntrustUserId", userId.ToUpperString()));
            if (flowId.IsGuid(out Guid flowGuid))
            {
                whereBuilder.Append(" AND FlowId=:FlowId");
                parameters.Add(new OracleParameter(":FlowId", flowId.ToUpper()));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Title,:Title,1,1)>0");
                parameters.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (startDate.IsDateTime(out DateTime date1))
            {
                whereBuilder.Append(" AND ReceiveTime>=:ReceiveTime");
                parameters.Add(new OracleParameter(":ReceiveTime", date1.GetDate()));
            }
            if (endDate.IsDateTime(out DateTime date2))
            {
                whereBuilder.Append(" AND ReceiveTime<:ReceiveTime1");
                parameters.Add(new OracleParameter(":ReceiveTime1", date2.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 得到查询实例列表SQL
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="receiveId"></param>
        /// <param name="receiveDate1"></param>
        /// <param name="receiveDate2"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetQueryInstanceSql(string flowId, string title, string receiveId, string receiveDate1, string receiveDate2, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT GroupId,ReceiveTime,ROW_NUMBER() OVER(ORDER BY ReceiveTime DESC) AS PagerAutoRowNumber FROM(SELECT GroupId,MAX(ReceiveTime) ReceiveTime From RF_FlowTask WHERE 1=1 ";
            if (!flowId.IsNullOrWhiteSpace())
            {
                if (flowId.IsGuid(out Guid fid))
                {
                    whereBuilder.Append(" AND FlowId=:FlowId");
                    parameters.Add(new OracleParameter(":FlowId", flowId.ToUpper()));
                }
                else
                {
                    whereBuilder.Append(" AND FlowId IN(" + flowId.ToUpper() + ")");
                }
            }
            else
            {
                whereBuilder.Append(" AND 1=0");
            }
            if (!title.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Title,:Title,1,1)>0");
                parameters.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (receiveId.IsGuid(out Guid rid))
            {
                whereBuilder.Append(" AND ReceiveId=:ReceiveId");
                parameters.Add(new OracleParameter(":ReceiveId", receiveId.ToUpper()));
            }
            if (receiveDate1.IsDateTime(out DateTime date1))
            {
                whereBuilder.Append(" AND ReceiveTime>=:ReceiveTime");
                parameters.Add(new OracleParameter(":ReceiveTime", date1.GetDate()));
            }
            if (receiveDate2.IsDateTime(out DateTime date2))
            {
                whereBuilder.Append(" AND ReceiveTime<:ReceiveTime1");
                parameters.Add(new OracleParameter(":ReceiveTime1", date2.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString() + " GROUP BY GroupId) TaskTemp", parameters.ToArray());
        }

        /// <summary>
        /// 查询一个组中最新的一条任务
        /// </summary>
        /// <param name="gropuId"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetQueryGroupMaxTaskSql(Guid gropuId)
        {
            string sql = "SELECT * FROM RF_FlowTask WHERE GroupId=:GroupId AND ROWNUM=1 ORDER BY Sort DESC";
            OracleParameter[] sqlParameters = new OracleParameter[] { new OracleParameter("@GroupId", gropuId) };
            return (sql, sqlParameters);
        }

        /// <summary>
        /// 得到查询文档列表SQL
        /// </summary>
        /// <param name="title"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetDocSql(Guid userId, string title, string dirId, string date1, string date2, string order, int read)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,DirId,DirName,Title,WriteTime,WriteUserName,ReadCount,DocRank FROM RF_Doc a WHERE EXISTS(SELECT Id FROM RF_DocUser WHERE DocId=a.Id AND UserId=:UserId" + (read >= 0 ? " AND IsRead=:IsRead" : "") + ")";
            parameters.Add(new OracleParameter(":UserId", userId.ToUpperString()));
            if (read >= 0)
            {
                parameters.Add(new OracleParameter(":IsRead", read));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Title,:Title,1,1)>0");
                parameters.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (!dirId.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND DirId IN(" + dirId.ToUpper() + ")");
            }
            if (date1.IsDateTime(out DateTime date11))
            {
                whereBuilder.Append(" AND a.WriteTime>=:WriteTime");
                parameters.Add(new OracleParameter(":WriteTime", date11.GetDate()));
            }
            if (date2.IsDateTime(out DateTime date21))
            {
                whereBuilder.Append(" AND a.WriteTime<:WriteTime1");
                parameters.Add(new OracleParameter(":WriteTime1", date21.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 查询文档阅读情况
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetDocReadUserListSql(string docId, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT DOCID,USERID,ISREAD,READTIME FROM RF_DocUser WHERE DocId=:DocId";
            parameters.Add(new OracleParameter(":DocId", docId.ToUpper()));
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 查询一页委托数据SQL
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetFlowEntrustSql(string userId, string date1, string date2, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,UserId,StartTime,EndTime,FlowId,ToUserId,WriteTime,Note FROM RF_FlowEntrust WHERE 1=1";
            if (userId.IsGuid(out Guid userGuid))
            {
                whereBuilder.Append(" AND UserId=:UserId");
                parameters.Add(new OracleParameter(":UserId", userId.ToUpper()));
            }
            if (date1.IsDateTime(out DateTime date11))
            {
                whereBuilder.Append(" AND StartTime>=:StartTime");
                parameters.Add(new OracleParameter(":StartTime", date11.GetDate()));
            }
            if (date2.IsDateTime(out DateTime date21))
            {
                whereBuilder.Append(" AND StartTime<:StartTime1");
                parameters.Add(new OracleParameter(":StartTime1", date21.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

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
        public (string sql, DbParameter[] parameter) GetFlowArchiveSql(string flowId, string stepName, string title, string date1, string date2, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,FlowId,FlowName,StepId,StepName,TaskId,GroupId,InstanceId,Title,UserName,WriteTime FROM RF_FlowArchive WHERE 1=1";
            if (flowId.IsGuid(out Guid fid))
            {
                whereBuilder.Append(" AND FlowId=:FlowId");
                parameters.Add(new OracleParameter(":FlowId", flowId.ToUpper()));
            }
            if (!stepName.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(StepName,:StepName,1,1)>0");
                parameters.Add(new OracleParameter(":StepName", stepName));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Title,:Title,1,1)>0");
                parameters.Add(new OracleParameter(":Title", title));
            }
            if (date1.IsDateTime(out DateTime dt1))
            {
                whereBuilder.Append(" AND WriteTime>=:WriteTime");
                parameters.Add(new OracleParameter(":WriteTime", dt1));
            }
            if (date2.IsDateTime(out DateTime dt2))
            {
                whereBuilder.Append(" AND WriteTime<:WriteTime1");
                parameters.Add(new OracleParameter(":WriteTime1", dt2.AddDays(1).ToDateString()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }
        /// <summary>
        /// 查询一页程序设计
        /// </summary>
        /// <param name="name"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetProgramSql(string name, string types, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,Name,Type,CreateTime,PublishTime,CreateUserId,Status FROM RF_Program WHERE Status IN(0,1)";
            if (!name.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Name,:Name,1,1)>0");
                parameters.Add(new OracleParameter(":Name", name.Trim()));
            }
            if (!types.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND Type IN(" + types.ToUpper() + ")");
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 查询一页首页设置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetHomeSetSql(string name, string title, string type, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,Type,Name,Title,DataSourceType,DataSource,Ico,BackgroundColor,FontColor,DbConnId,LinkURL,UseOrganizes,UseUsers,Sort,Note FROM RF_HomeSet WHERE 1=1";
            if (!name.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Name,:Name,1,1)>0");
                parameters.Add(new OracleParameter(":Name", name.Trim()));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Title,:Title,1,1)>0");
                parameters.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (type.IsInt(out int t))
            {
                whereBuilder.Append(" AND Type=:Type");
                parameters.Add(new OracleParameter(":Type", t));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 查询一页已发送消息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="oreder"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetMessageSendListSql(string userId, string content, string date1, string date2, string status, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT Id,Contents,SendType,SenderName,SendTime,Files FROM RF_Message WHERE 1=1";
            if ("1".Equals(status))
            {
                whereBuilder.Append(" AND Exists(SELECT Id From RF_MessageUser WHERE RF_MessageUser.MessageId=RF_Message.Id AND IsRead=0 AND UserId=:UserId)");
                parameters.Add(new OracleParameter(":UserId", userId.ToUpper()));
            }
            else if ("2".Equals(status))
            {
                whereBuilder.Append(" AND Exists(SELECT Id From RF_MessageUser WHERE RF_MessageUser.MessageId=RF_Message.Id AND IsRead=1 AND UserId=:UserId)");
                parameters.Add(new OracleParameter(":UserId", userId.ToUpper()));
            }
            else if ("0".Equals(status))
            {
                if (userId.IsGuid(out Guid uid))
                {
                    whereBuilder.Append(" AND SenderId=:SenderId");
                    parameters.Add(new OracleParameter(":SenderId", userId.ToUpper()));
                }
            }
            if (!content.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Contents,:Contents,1,1)>0");
                parameters.Add(new OracleParameter(":Contents", content.Trim()));
            }
            if (date1.IsDateTime(out DateTime dt))
            {
                whereBuilder.Append(" AND SendTime>=:SendTime");
                parameters.Add(new OracleParameter(":SendTime", dt.GetDate()));
            }
            if (date2.IsDateTime(out DateTime dt1))
            {
                whereBuilder.Append(" AND SendTime<:SendTime1");
                parameters.Add(new OracleParameter(":SendTime1", dt1.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 查询一页已发送消息阅读人员
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetMessageReadUserListSql(string messageId, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT MessageId,UserId,IsRead,ReadTime FROM RF_MessageUser WHERE MessageId=:MessageId";
            parameters.Add(new OracleParameter(":MessageId", messageId.ToUpper()));
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 查询一页分享的文件 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetUserFileShareSql(Guid userId, string fileName, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT FileId,ShareUserId,FileName,ShareDate,ExpireDate FROM (SELECT FileId,ShareUserId,MAX(FileName) FileName,MAX(ShareDate) ShareDate,MAX(ExpireDate) ExpireDate FROM RF_UserFileShare group by FileId,ShareUserId HAVING ShareUserId=:ShareUserId) Temp_RF_UserFileShare";
            parameters.Add(new OracleParameter(":ShareUserId", userId.ToUpperString()));
            if (!fileName.IsNullOrEmpty())
            {
                whereBuilder.Append(" WHERE INSTR(FileName,:FileName,1,1)>0");
                parameters.Add(new OracleParameter(":FileName", fileName));
            }
            //whereBuilder.Append(") AS Temp_RF_UserFileShare");
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 查询一页收到的分享文件 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetUserFileShareMySql(Guid userId, string fileName, string order)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT FileId,ShareUserId,FileName,ShareDate,ExpireDate FROM RF_UserFileShare WHERE UserId=:UserId AND ExpireDate>:ExpireDate";
            parameters.Add(new OracleParameter(":UserId", userId.ToUpperString()));
            parameters.Add(new OracleParameter(":ExpireDate", DateExtensions.Now));
            if (!fileName.IsNullOrEmpty())
            {
                whereBuilder.Append(" AND INSTR(FileName,:FileName,1,1)>0");
                parameters.Add(new OracleParameter("@:FileName", fileName));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 查询一页收件箱SQL
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetMailInBoxSql(Guid currentUserId, string subject, string userId, string date1, string date2)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT ID,SUBJECT,SUBJECTCOLOR,USERID,SENDUSERID,SENDDATETIME,CONTENTSID,ISREAD,READDATETIME,OUTBOXID FROM RF_MailInBox WHERE UserId=:UserId";
            parameters.Add(new OracleParameter(":UserId", currentUserId.ToUpperString()));
            if (userId.IsGuid(out Guid guid))
            {
                whereBuilder.Append(" AND SendUserId=:SendUserId");
                parameters.Add(new OracleParameter(":SendUserId", guid.ToUpperString()));
            }
            if (!subject.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" WHERE INSTR(Subject,:Subject,1,1)>0");
                parameters.Add(new OracleParameter(":Subject", subject.Trim()));
            }
            if (date1.IsDateTime(out DateTime dt))
            {
                whereBuilder.Append(" AND SendDateTime>=:SendDateTime");
                parameters.Add(new OracleParameter(":SendDateTime", dt.GetDate()));
            }
            if (date2.IsDateTime(out DateTime dt1))
            {
                whereBuilder.Append(" AND SendDateTime<:SendDateTime1");
                parameters.Add(new OracleParameter(":SendDateTime1", dt1.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 查询一页发件箱SQL
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetMailOutBoxSql(Guid currentUserId, string subject, string date1, string date2, int status = -1)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT ID,SUBJECT,SUBJECTCOLOR,USERID,RECEIVEUSERS,SENDDATETIME,CONTENTSID,STATUS FROM RF_MailOutBox WHERE UserId=:UserId";
            parameters.Add(new OracleParameter(":UserId", currentUserId.ToUpperString()));
            if (status != -1)
            {
                whereBuilder.Append(" AND Status=:Status");
                parameters.Add(new OracleParameter(":Status", status));
            }
            if (!subject.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" WHERE INSTR(Subject,:Subject,1,1)>0");
                parameters.Add(new OracleParameter(":Subject", subject.Trim()));
            }
            if (date1.IsDateTime(out DateTime dt))
            {
                whereBuilder.Append(" AND SendDateTime>=:SendDateTime");
                parameters.Add(new OracleParameter(":SendDateTime", dt.GetDate()));
            }
            if (date2.IsDateTime(out DateTime dt1))
            {
                whereBuilder.Append(" AND SendDateTime<:SendDateTime1");
                parameters.Add(new OracleParameter(":SendDateTime1", dt1.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }
        /// <summary>
        /// 查询一页已删除邮件SQL
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetMailDeletedBoxSql(Guid currentUserId, string subject, string userId, string date1, string date2)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT ID,SUBJECT,SUBJECTCOLOR,USERID,SENDUSERID,SENDDATETIME,CONTENTSID,ISREAD,READDATETIME,OUTBOXID FROM RF_MailDeletedBox WHERE UserId=:UserId";
            parameters.Add(new OracleParameter(":UserId", currentUserId.ToUpperString()));
            if (userId.IsGuid(out Guid guid))
            {
                whereBuilder.Append(" AND SendUserId=:SendUserId");
                parameters.Add(new OracleParameter(":SendUserId", guid.ToUpperString()));
            }
            if (!subject.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" WHERE INSTR(Subject,:Subject,1,1)>0");
                parameters.Add(new OracleParameter(":Subject", subject.Trim()));
            }
            if (date1.IsDateTime(out DateTime dt))
            {
                whereBuilder.Append(" AND SendDateTime>=:SendDateTime");
                parameters.Add(new OracleParameter(":SendDateTime", dt.GetDate()));
            }
            if (date2.IsDateTime(out DateTime dt1))
            {
                whereBuilder.Append(" AND SendDateTime<:SendDateTime1");
                parameters.Add(new OracleParameter(":SendDateTime1", dt1.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 查询一页投票SQL
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetVoteSql(Guid currentUserId, string topic, string date1, string date2)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT ID,TOPIC,CREATEUSERID,CREATETIME,PARTAKEUSERS,RESULTVIEWUSERS,ENDTIME,NOTE,STATUS FROM RF_Vote WHERE CreateUserId=:CreateUserId";
            parameters.Add(new OracleParameter(":CreateUserId", currentUserId.ToUpperString()));
            if (!topic.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Topic,:Topic,1,1)>0");
                parameters.Add(new OracleParameter(":Topic", topic.Trim()));
            }
            if (date1.IsDateTime(out DateTime dt))
            {
                whereBuilder.Append(" AND CreateTime>=:CreateTime");
                parameters.Add(new OracleParameter(":CreateTime", dt.GetDate()));
            }
            if (date2.IsDateTime(out DateTime dt1))
            {
                whereBuilder.Append(" AND CreateTime<:CreateTime1");
                parameters.Add(new OracleParameter(":CreateTime1", dt1.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 查询一页待提交投票SQL
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetWaitSubmitVoteSql(Guid currentUserId, string topic, string date1, string date2)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT ID,TOPIC,CREATEUSERID,CREATETIME,PARTAKEUSERS,RESULTVIEWUSERS,ENDTIME,NOTE,STATUS,PUBLISHTIME FROM RF_Vote WHERE ENDTIME>:EndTime AND EXISTS(SELECT Id FROM RF_VotePartakeUser WHERE UserId=:UserId AND VoteId=RF_Vote.Id AND Status=0)";
            parameters.Add(new OracleParameter(":EndTime", DateExtensions.Now));
            parameters.Add(new OracleParameter(":UserId", currentUserId.ToUpperString()));
            if (!topic.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Topic,:Topic,1,1)>0");
                parameters.Add(new OracleParameter(":Topic", topic.Trim()));
            }
            if (date1.IsDateTime(out DateTime dt))
            {
                whereBuilder.Append(" AND CreateTime>=:CreateTime");
                parameters.Add(new OracleParameter(":CreateTime", dt.GetDate()));
            }
            if (date2.IsDateTime(out DateTime dt1))
            {
                whereBuilder.Append(" AND CreateTime<:CreateTime1");
                parameters.Add(new OracleParameter(":CreateTime1", dt1.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 查询一页待提交投票SQL
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetVoteResultSql(Guid currentUserId, string topic, string date1, string date2)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT ID,TOPIC,CREATEUSERID,CREATETIME,PARTAKEUSERS,RESULTVIEWUSERS,ENDTIME,NOTE,STATUS FROM RF_Vote WHERE EXISTS(SELECT Id FROM RF_VoteResultUser WHERE UserId=:UserId AND VoteId=RF_Vote.Id)";
            parameters.Add(new OracleParameter(":UserId", currentUserId.ToUpperString()));
            if (!topic.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Topic,:Topic,1,1)>0");
                parameters.Add(new OracleParameter(":Topic", topic.Trim()));
            }
            if (date1.IsDateTime(out DateTime dt))
            {
                whereBuilder.Append(" AND CreateTime>=:CreateTime");
                parameters.Add(new OracleParameter(":CreateTime", dt.GetDate()));
            }
            if (date2.IsDateTime(out DateTime dt1))
            {
                whereBuilder.Append(" AND CreateTime<:CreateTime1");
                parameters.Add(new OracleParameter(":CreateTime1", dt1.AddDays(1).GetDate()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 查询一页投票结果SQL
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userId"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public (string sql, DbParameter[] parameter) GetPartakeUserSql(Guid voteId, string name, string org)
        {
            string sql = string.Empty;
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> parameters = new List<DbParameter>();
            sql = "SELECT ID,VOTEID,USERID,USERNAME,USERORGANIZE,STATUS,SUBMITTIME FROM RF_VotePartakeUser WHERE VoteId=:VoteId";
            parameters.Add(new OracleParameter(":VoteId", voteId.ToUpperString()));
            if (!name.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Topic,:UserName,1,1)>0");
                parameters.Add(new OracleParameter(":UserName", name.Trim()));
            }
            if (!org.IsNullOrWhiteSpace())
            {
                whereBuilder.Append(" AND INSTR(Topic,:UserOrganize,1,1)>0");
                parameters.Add(new OracleParameter(":UserOrganize", org.Trim()));
            }
            return (sql + whereBuilder.ToString(), parameters.ToArray());
        }
    }
}
