using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using System.Data.Common;
using RoadFlow.Utility;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class DbConnection
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_dbconnection";
        /// <summary>
        /// 得到所有连接
        /// </summary>
        /// <returns></returns>
        public List<Model.DbConnection> GetAll()
        {
            object obj = Cache.IO.Get(CACHEKEY);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    var dbConnections = db.QueryAll<Model.DbConnection>().OrderBy(p => p.Sort).ToList();
                    Cache.IO.Insert(CACHEKEY, dbConnections);
                    return dbConnections;
                }
            }
            else
            {
                return (List<Model.DbConnection>)obj;
            }
        }
        /// <summary>
        /// 添加一个连接
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <returns></returns>
        public int Add(Model.DbConnection dbConnection)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Add(dbConnection);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新连接
        /// </summary>
        /// <param name="dictionary">连接实体</param>
        public int Update(Model.DbConnection dbConnection)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.Update(dbConnection);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除一批连接
        /// </summary>
        /// <param name="dbConnections">连接实体数组</param>
        /// <returns></returns>
        public int Delete(Model.DbConnection[] dbConnections)
        {
            ClearCache();
            using (var db = new DataContext())
            {
                db.RemoveRange(dbConnections);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            Cache.IO.Remove(CACHEKEY);
        }

        /// <summary>
        /// 测试一个连接是否正常
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <returns>返回字符串1为正常，其它为错误信息</returns>
        public string TesetConnection(Model.DbConnection dbConnection)
        {
            using (var db = new DataContext(dbConnection.ConnType, dbConnection.ConnString))
            {
                return db.TestConnection();
            }
        }

        /// <summary>
        /// 查询一个连接所有表
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <returns>表名，表说明</returns>
        public DataTable GetTables(Model.DbConnection dbConnection)
        {
            using (var db = new DataContext(dbConnection.ConnType, dbConnection.ConnString))
            {
                return db.GetDataTable(new DbconnnectionSql(dbConnection).SqlInstance.GetDbTablesSql());
            }
        }

        /// <summary>
        /// 得到一个表所有字段
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="table">表名</param>
        /// <param name="dbName">数据库名(MYSQL时需要,防止同一个连接中不同的数据库中有相同的表名的情况。其它数据库为空)</param>
        /// <returns>返回datatable 列:f_name,t_name,length,is_null,cdefault,isidentity,defaultvalue,comments</returns>
        public DataTable GetTableFields(Model.DbConnection dbConnection, string table, string dbName)
        {
            using (var db = new DataContext(dbConnection.ConnType, dbConnection.ConnString))
            {
                return db.GetDataTable(new DbconnnectionSql(dbConnection).SqlInstance.GetTableFieldsSql(table, dbName));
            }
        }

        /// <summary>
        /// 执行多条SQL
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <param name="tuples">sql语句, sql参数</param>
        /// <returns>返回数字表示成功（数字是受影响的行数），其它为错误信息</returns>
        public string ExecuteSQL(Model.DbConnection dbConnection, List<(string sql, DbParameter[] parameters)> tuples)
        {
            using (var db = new DataContext(dbConnection.ConnType, dbConnection.ConnString))
            {
                try
                {
                    foreach (var (sql, parameter) in tuples)
                    {
                        db.Execute(sql, parameter);
                    }
                    return db.SaveChanges().ToString();
                }
                catch (Exception err)
                {
                    return err.Message;
                }
            }
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <param name="sql">sql</param>
        /// <param name="paramObj">参数值</param>
        /// <returns>返回数字表示成功（数字是受影响的行数），其它为错误信息</returns>
        public string ExecuteSQL(Model.DbConnection dbConnection, string sql, IEnumerable<object> paramObjs = null)
        {
            using (var db = new DataContext(dbConnection.ConnType, dbConnection.ConnString))
            {
                try
                {
                    int i = db.Execute(sql, paramObjs);
                    db.SaveChanges();
                    return i.ToString();
                }
                catch (Exception err)
                {
                    return err.Message;
                }
            }
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <param name="sql">sql</param>
        /// <param name="paramObj">参数值</param>
        /// <returns>返回数字表示成功（数字是受影响的行数），其它为错误信息</returns>
        public string ExecuteSQL(Model.DbConnection dbConnection, string sql, params object[] paramObjs)
        {
            using (var db = new DataContext(dbConnection.ConnType, dbConnection.ConnString))
            {
                try
                {
                    int i = db.Execute(sql, paramObjs);
                    db.SaveChanges();
                    return i.ToString();
                }
                catch (Exception err)
                {
                    return err.Message;
                }
            }
        }

        /// <summary>
        /// 执行SQL列表
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <param name="ts">sql列表</param>
        /// <returns>返回数字表示成功（数字是受影响的行数），其它为错误信息</returns>
        public string ExecuteSQL(Model.DbConnection dbConnection, List<(string, IEnumerable<object> paramObjs)> ts)
        {
            using (var db = new DataContext(dbConnection.ConnType, dbConnection.ConnString))
            {
                try
                {
                    int i = 0;
                    foreach (var (sql, paramObjs) in ts)
                    {
                        i += db.Execute(sql, paramObjs.ToArray());
                    }
                    db.SaveChanges();
                    return i.ToString();
                }
                catch (Exception err)
                {
                    return err.Message;
                }
            }
        }

        /// <summary>
        /// 测试一个SQL语句是否正确（不提交）
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">sql参数</param>
        /// <returns></returns>
        public string TestSQL(Model.DbConnection dbConnection, string sql, DbParameter[] parameters)
        {
            using (var db = new DataContext(dbConnection.ConnType, dbConnection.ConnString))
            {
                return db.TestSQL(sql, parameters);
            }
        }

        /// <summary>
        /// 得到DATATABLE
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns></returns>
        public DataTable GetDataTable(Model.DbConnection dbConnection, string sql, params object[] objects)
        {
            using (var db = new DataContext(dbConnection.ConnType, dbConnection.ConnString))
            {
                return db.GetDataTable(sql, objects);
            }
        }

        /// <summary>
        /// 得到DATATABLE
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns></returns>
        public DataTable GetDataTable(Model.DbConnection dbConnection, string sql, DbParameter[] parameters = null)
        {
            using (var db = new DataContext(dbConnection.ConnType, dbConnection.ConnString))
            {
                return db.GetDataTable(sql, parameters);
            }
        }

        /// <summary>
        /// 得到DATATABLE
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <param name="tableName">表名</param>
        /// <param name="primaryKey">主键</param>
        /// <param name="primaryKeyValue">主键值</param>
        /// <returns></returns>
        public DataTable GetDataTable(Model.DbConnection dbConnection, string tableName, string primaryKey, string primaryKeyValue)
        {
            var sql = new DbconnnectionSql(dbConnection).SqlInstance.GetFieldValueSql(tableName, "*", primaryKey, primaryKeyValue);
            return GetDataTable(dbConnection, sql.sql, new DbParameter[] { sql.parameter });
        }

        /// <summary>
        /// 得到一个字段的值
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">要查询的字段</param>
        /// <param name="primaryKey">主键</param>
        /// <param name="primaryKeyValue">主键值</param>
        /// <returns></returns>
        public string GetFieldValue(Model.DbConnection dbConnection, string tableName, string fieldName, string primaryKey, string primaryKeyValue)
        {
            var sql = new DbconnnectionSql(dbConnection).SqlInstance.GetFieldValueSql(tableName, fieldName, primaryKey, primaryKeyValue);
            DataTable dataTable = GetDataTable(dbConnection, sql.sql, new DbParameter[] { sql.parameter });
            return dataTable.Rows.Count > 0 ? dataTable.Rows[0][0].ToString() : string.Empty;
        }

        /// <summary>
        /// 得到一个字段的值
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public string GetFieldValue(Model.DbConnection dbConnection, string sql)
        {
            DataTable dataTable = GetDataTable(dbConnection, sql.FilterSelectSql());
            return dataTable.Rows.Count > 0 ? dataTable.Rows[0][0].ToString() : string.Empty;
        }

        /// <summary>
        /// 得到一个字段的值
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <param name="sql">SQL</param>
        /// <param name="objs">参数值</param>
        /// <returns></returns>
        public string GetFieldValue(Model.DbConnection dbConnection, string sql, params object[] objs)
        {
            DataTable dataTable = GetDataTable(dbConnection, sql.FilterSelectSql(), objs);
            return dataTable.Rows.Count > 0 ? dataTable.Rows[0][0].ToString() : string.Empty;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="tuples"></param>
        /// <returns>返回数字表示成功（数字是受影响的行数或者自增主键值），其它为错误信息</returns>
        public string SaveData(Model.DbConnection dbConnection, List<(Dictionary<string, object> dicts, string tableName, string primaryKey, int flag)> tuples, bool isIdentity = false, string seqName = "")
        {
            DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(dbConnection);
            List<(string sql, DbParameter[] parameters)> sqlList = new List<(string sql, DbParameter[] parameters)>();
            foreach (var (dicts, tableName, primaryKey, flag) in tuples)
            {
                var (sql, paramArray) = dbconnnectionSql.SqlInstance.GetSaveDataSql(dicts, tableName, primaryKey, flag);
                sqlList.Add((sql, paramArray));
            }
            using (var db = new DataContext(dbConnection.ConnType, dbConnection.ConnString))
            {
                int i = 0;
                foreach (var (sql, parameter) in sqlList)
                {
                    i += db.Execute(sql, parameter);
                }
                if (isIdentity)
                {
                    i = db.GetIdentity(seqName);
                }
                db.SaveChanges();
                return i.ToString();
            }
        }

        /// <summary>
        /// 得到DataTableSchema
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <param name="sql">SQL</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataTable GetDataTableSchema(Model.DbConnection dbConnection, string sql, DbParameter[] parameters)
        {
            using (var db = new DataContext(dbConnection.ConnType, dbConnection.ConnString))
            {
                return db.GetDataTableSchema(sql, parameters);
            }
        }

    }
}
