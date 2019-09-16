using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace RoadFlow.Mapper
{
    public class DataOracle : IData
    {
        /// <summary>
        /// 查询主键记录SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public (string, DbParameter[]) GetFindSql<T>(params object[] objects) where T : class, new()
        {
            Type type = typeof(T);
            var properties = type.GetProperties();
            string tablename = Common.GetTableName(type);
            var keys = Common.GetPrimaryKeyAndTypes(properties);
            List<DbParameter> OracleParameters = new List<DbParameter>();
            StringBuilder whereBuilder = new StringBuilder();
            whereBuilder.Append(" WHERE 1=1");
            int i = 0;
            foreach (var (key, fieldType) in keys)
            {
                whereBuilder.Append(" AND " + key + "=:" + key);
                if (objects.Length > i)
                {
                    object pValue = objects[i];
                    OracleParameters.Add(new OracleParameter(":" + key, Common.GetParameterValue(pValue)));
                }
                i++;
            }
            return ("SELECT * FROM " + tablename + whereBuilder.ToString(), OracleParameters.ToArray());
        }


        /// <summary>
        /// 将SQL转换为查询一条记录SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string GetQueryOneSql(string sql)
        {
            return "SELECT * FROM(" + sql + ") t WHERE ROWNUM<=1";
        }

        /// <summary>
        /// 将datareader转换为List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<T> ReaderToList<T>(DbDataReader reader) where T : class, new()
        {
            List<T> list = new List<T>();
            var properties = typeof(T).GetProperties();
            while (reader.Read())
            {
                T t = new T();
                //object[] values = new object[properties.Length];
                //int length = reader.GetValues(values);
                //int i = 0;
                foreach (var propertie in properties)
                {
                    object value = Common.GetReaderValue(propertie.PropertyType, reader[propertie.Name]);
                    if (value != DBNull.Value)
                    {
                        propertie.SetValue(t, value);
                    }
                }
                list.Add(t);
            }
            reader.Close();
            reader.Dispose();
            return list;
        }

        /// <summary>
        /// 得到插入语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public (string, DbParameter[]) GetInsertSql<T>(T t) where T : class, new()
        {
            Type type = typeof(T);
            var properties = type.GetProperties();
            string tablename = Common.GetTableName(type);
            //如果没有获取到表名，则返回空
            if (string.IsNullOrWhiteSpace(tablename))
            {
                return (string.Empty, new OracleParameter[] { });
            }
            StringBuilder sql = new StringBuilder();
            StringBuilder fields = new StringBuilder();
            List<DbParameter> OracleParameters = new List<DbParameter>();
            foreach (var p in properties)
            {
                sql.Append(":" + p.Name + ",");
                fields.Append(p.Name + ",");
                object v = p.GetValue(t);
                OracleParameters.Add(new OracleParameter(":" + p.Name, Common.GetParameterValue(v)));
            }
            return ("INSERT INTO " + tablename + " (" + fields.ToString().TrimEnd(',') + ") VALUES(" + sql.ToString().TrimEnd(',') + ")", OracleParameters.ToArray());
        }

        /// <summary>
        /// 得到INSERT语句(多条)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public (string, List<DbParameter[]>) GetInsertSql<T>(IEnumerable<T> ts) where T : class, new()
        {
            Type type = typeof(T);
            var properties = type.GetProperties();
            string tablename = Common.GetTableName(type);
            List<DbParameter[]> dbParameters = new List<DbParameter[]>();
            //如果没有获取到表名，则返回空
            if (string.IsNullOrWhiteSpace(tablename))
            {
                return (string.Empty, dbParameters);
            }
            StringBuilder sql = new StringBuilder();
            StringBuilder fields = new StringBuilder();
            foreach (var p in properties)
            {
                sql.Append(":" + p.Name + ",");
                fields.Append(p.Name + ",");
            }
            foreach (var t in ts)
            {
                List<DbParameter> OracleParameters = new List<DbParameter>();
                foreach (var p in properties)
                {
                    object v = p.GetValue(t);
                    OracleParameters.Add(new OracleParameter(":" + p.Name, Common.GetParameterValue(v)));
                }
                dbParameters.Add(OracleParameters.ToArray());
            }
            return ("INSERT INTO " + tablename + " (" + fields.ToString().TrimEnd(',') + ") VALUES(" + sql.ToString().TrimEnd(',') + ")", dbParameters);
        }

        /// <summary>
        /// 得到更新语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public (string, DbParameter[]) GetUpdateSql<T>(T t) where T : class, new()
        {
            Type type = typeof(T);
            var properties = type.GetProperties();
            string tablename = Common.GetTableName(type);
            StringBuilder sql = new StringBuilder();
            List<DbParameter> OracleParameters = new List<DbParameter>();
            List<string> keys = Common.GetPrimaryKeys(properties);
            //如果表名为空或者没有主键则返回空
            if (string.IsNullOrWhiteSpace(tablename) || !keys.Any())
            {
                return (string.Empty, new OracleParameter[] { });
            }
            foreach (var p in properties)
            {
                if (keys.Contains(p.Name))
                {
                    continue;
                }
                sql.Append(p.Name + "=:" + p.Name + ",");
                object v = p.GetValue(t);
                OracleParameters.Add(new OracleParameter(":" + p.Name, Common.GetParameterValue(v)));
            }
            StringBuilder whereBuilder = new StringBuilder();
            whereBuilder.Append(" WHERE 1=1");
            foreach (string key in keys)
            {
                whereBuilder.Append(" AND " + key + "=:" + key);
                foreach (var p in properties)
                {
                    if (key.Equals(p.Name))
                    {
                        object v = p.GetValue(t);
                        OracleParameters.Add(new OracleParameter(":" + p.Name, Common.GetParameterValue(v)));
                        break;
                    }
                }
            }
            return ("UPDATE " + tablename + " SET " + sql.ToString().TrimEnd(',') + whereBuilder.ToString(), OracleParameters.ToArray());
        }

        /// <summary>
        /// 得到更新语句(多条)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public (string, List<DbParameter[]>) GetUpdateSql<T>(IEnumerable<T> ts) where T : class, new()
        {
            Type type = typeof(T);
            var properties = type.GetProperties();
            string tablename = Common.GetTableName(type);
            StringBuilder sql = new StringBuilder();
            List<DbParameter[]> dbParameters = new List<DbParameter[]>();
            List<string> keys = Common.GetPrimaryKeys(properties);
            //如果表名为空或者没有主键则返回空
            if (string.IsNullOrWhiteSpace(tablename) || !keys.Any())
            {
                return (string.Empty, dbParameters);
            }
            foreach (var p in properties)
            {
                if (keys.Contains(p.Name))
                {
                    continue;
                }
                sql.Append(p.Name + "=:" + p.Name + ",");
            }
            StringBuilder whereBuilder = new StringBuilder();
            whereBuilder.Append(" WHERE 1=1");
            foreach (string key in keys)
            {
                whereBuilder.Append(" AND " + key + "=:" + key);
            }
            foreach (var t in ts)
            {
                List<DbParameter> OracleParameters = new List<DbParameter>();
                foreach (var p in properties)
                {
                    if (keys.Contains(p.Name))
                    {
                        continue;
                    }
                    object v = p.GetValue(t);
                    OracleParameters.Add(new OracleParameter(":" + p.Name, Common.GetParameterValue(v)));
                }
                foreach (string key in keys)
                {
                    foreach (var p in properties)
                    {
                        if (key.Equals(p.Name))
                        {
                            object v = p.GetValue(t);
                            OracleParameters.Add(new OracleParameter(":" + p.Name, Common.GetParameterValue(v)));
                            break;
                        }
                    }
                }
                dbParameters.Add(OracleParameters.ToArray());
            }
            return ("UPDATE " + tablename + " SET " + sql.ToString().TrimEnd(',') + whereBuilder.ToString(), dbParameters);
        }

        /// <summary>
        /// 得到删除语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public (string, DbParameter[]) GetRemoveSql<T>(T t) where T : class, new()
        {
            Type type = typeof(T);
            var properties = type.GetProperties();
            string tablename = Common.GetTableName(type);
            var keys = Common.GetPrimaryKeyAndTypes(properties);
            if (string.IsNullOrWhiteSpace(tablename) || !keys.Any())
            {
                return (string.Empty, new OracleParameter[] { });
            }
            List<DbParameter> OracleParameters = new List<DbParameter>();
            StringBuilder whereBuilder = new StringBuilder();
            whereBuilder.Append(" WHERE 1=1");
            foreach (var (key, fieldType) in keys)
            {
                object pValue = null;
                foreach (var p in properties)
                {
                    if (p.Name.Equals(key))
                    {
                        pValue = p.GetValue(t);
                        break;
                    }
                }
                if (pValue == null)
                {
                    continue;
                }
                whereBuilder.Append(" AND " + key + "=:" + key);
                OracleParameters.Add(new OracleParameter(":" + key, Common.GetParameterValue(pValue)));
            }
            return ("DELETE FROM " + tablename + whereBuilder.ToString(), OracleParameters.ToArray());
        }

        /// <summary>
        /// 得到删除语句(多条)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public (string, List<DbParameter[]>) GetRemoveSql<T>(IEnumerable<T> ts) where T : class, new()
        {
            Type type = typeof(T);
            var properties = type.GetProperties();
            string tablename = Common.GetTableName(type);
            var keys = Common.GetPrimaryKeyAndTypes(properties);
            List<DbParameter[]> dbParameters = new List<DbParameter[]>();
            if (string.IsNullOrWhiteSpace(tablename) || !keys.Any())
            {
                return (string.Empty, dbParameters);
            }
            StringBuilder whereBuilder = new StringBuilder();
            whereBuilder.Append(" WHERE 1=1");
            foreach (var (key, fieldType) in keys)
            {
                whereBuilder.Append(" AND " + key + "=:" + key);
            }
            foreach (var t in ts)
            {
                List<DbParameter> OracleParameters = new List<DbParameter>();
                foreach (var (key, fieldType) in keys)
                {
                    object pValue = null;
                    foreach (var p in properties)
                    {
                        if (p.Name.Equals(key))
                        {
                            pValue = p.GetValue(t);
                            break;
                        }
                    }
                    OracleParameters.Add(new OracleParameter(":" + key, Common.GetParameterValue(pValue)));
                }
                dbParameters.Add(OracleParameters.ToArray());
            }
            return ("DELETE FROM " + tablename + whereBuilder.ToString(), dbParameters);
        }

        /// <summary>
        /// 转换SQL（用参数替换掉{0}）
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public (string, DbParameter[]) GetSqlAndParameter(string sql, params object[] objects)
        {
            if (null == objects || objects.Length == 0)
            {
                return (sql, new OracleParameter[] { });
            }
            if (objects is IEnumerable<object>)
            {
                objects = objects.ToArray();
            }
            List<string> list = new List<string>();
            int i = 0;
            List<DbParameter> OracleParameters = new List<DbParameter>();
            foreach (object o in objects)
            {
                var pName = ":p" + i++.ToString();
                list.Add(pName);
                OracleParameters.Add(new OracleParameter(pName, Common.GetParameterValue(o)));
            }
            sql = string.Format(sql, list.ToArray());
            return (sql, OracleParameters.ToArray());
        }

        /// <summary>
        /// 查询自增主键值
        /// </summary>
        /// <returns></returns>
        public string GetIdentitySql(string seqName = "")
        {
            return "SELECT " + seqName + ".CURRVAL FROM DUAL";
        }

        public void BulkCopy<T>(IEnumerable<T> ts, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            
        }
    }
}
