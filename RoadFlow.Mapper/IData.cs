using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;

namespace RoadFlow.Mapper
{
    interface IData
    {

        /// <summary>
        /// 查询主键记录SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="objects">参数值</param>
        /// <returns></returns>
        (string, DbParameter[]) GetFindSql<T>(params object[] objects) where T : class, new();

        /// <summary>
        /// 将SQL转换为查询一条记录SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string GetQueryOneSql(string sql);

        /// <summary>
        /// 将Reader转换为List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        List<T> ReaderToList<T>(DbDataReader reader) where T : class, new();

        /// <summary>
        /// 得到INSERT语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        (string, DbParameter[]) GetInsertSql<T>(T t) where T : class, new();

        /// <summary>
        /// 得到INSERT语句(多条)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        (string, List<DbParameter[]>) GetInsertSql<T>(IEnumerable<T> ts) where T : class, new();

        /// <summary>
        /// 得到UPDATE语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        (string, DbParameter[]) GetUpdateSql<T>(T t) where T : class, new();

        /// <summary>
        /// 得到UPDATE语句(多条)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        (string, List<DbParameter[]>) GetUpdateSql<T>(IEnumerable<T> ts) where T : class, new();

        /// <summary>
        /// 得到删除语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        (string, DbParameter[]) GetRemoveSql<T>(T t) where T : class, new();

        /// <summary>
        /// 得到删除语句(多条)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        (string, List<DbParameter[]>) GetRemoveSql<T>(IEnumerable<T> ts) where T : class, new();

        /// <summary>
        /// 转换sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        (string, DbParameter[]) GetSqlAndParameter(string sql, params object[] objects);

        /// <summary>
        /// 查询自增主键值 
        /// </summary>
        /// <returns></returns>
        string GetIdentitySql(string seqName = "");

        /// <summary>
        /// SqlBulkCopy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="dbConnection"></param>
        /// <param name="dbTransaction"></param>
        void BulkCopy<T>(IEnumerable<T> ts, DbConnection dbConnection, DbTransaction dbTransaction);
    }
}
