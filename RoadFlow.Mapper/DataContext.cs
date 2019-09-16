using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;

namespace RoadFlow.Mapper
{
    public class DataContext : IDisposable
    {
        /// <summary>
        /// 操作影响的行数
        /// </summary>
        private int InfluenceRows = 0;

        /// <summary>
        /// 数据库类型枚举
        /// </summary>
        public enum DatabaseType { SQLSERVER, MYSQL, ORACLE };

        /// <summary>
        /// 当前数据库类型
        /// </summary>
        public DatabaseType DbType { get; set; }

        /// <summary>
        /// 当前数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 是否带事务
        /// </summary>
        private bool IsTransaction { get; set; }

        /// <summary>
        /// 当前是否是oracle数据库
        /// </summary>
        public bool IsOracle {
            get
            {
                return DbType == DatabaseType.ORACLE;
            }
        }

        /// <summary>
        /// 当前是否是mysql数据库
        /// </summary>
        public bool IsMySql
        {
            get
            {
                return DbType == DatabaseType.MYSQL;
            }
        }

        /// <summary>
        /// 当前是否是SqlServer数据库
        /// </summary>
        public bool IsSqlServer
        {
            get
            {
                return DbType == DatabaseType.SQLSERVER;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="databaseType">数据库类型(mysql,sqlserver,oracle)</param>
        /// <param name="connectionString">数据库类型对应的连接字符串</param>
        /// <param name="isTransaction">是否开户事务</param>
        public DataContext(string databaseType, string connectionString, bool isTransaction = true)
        {
            switch (databaseType.ToLower())
            {
                case "sqlserver":
                    DbType = DatabaseType.SQLSERVER;
                    break;
                case "mysql":
                    DbType = DatabaseType.MYSQL;
                    break;
                case "oracle":
                    DbType = DatabaseType.ORACLE;
                    break;
            }
            ConnectionString = connectionString;
            IsTransaction = isTransaction;
            CreateConnection();
        }

        /// <summary>
        /// 默认构造函数(采用系统配置的数据库类型和连接字符串)
        /// </summary>
        public DataContext()
        {
            InfluenceRows = 0;
            IsTransaction = true;
            switch (Config.DatabaseType)
            {
                case "sqlserver":
                    DbType = DatabaseType.SQLSERVER;
                    ConnectionString = Config.ConnectionString_SqlServer;
                    break;
                case "mysql":
                    DbType = DatabaseType.MYSQL;
                    ConnectionString = Config.ConnectionString_MySql;
                    break;
                case "oracle":
                    DbType = DatabaseType.ORACLE;
                    ConnectionString = Config.ConnectionString_Oracle;
                    break;
            }
            CreateConnection();
        }
        /// <summary>
        /// 当前数据连接
        /// </summary>
        public DbConnection Connection { get; private set; }
        /// <summary>
        /// 得到当前连接
        /// </summary>
        public DbConnection CreateConnection()
        {
            if (null != Connection)
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
                if (null == Transaction)
                {
                    Transaction = Connection.BeginTransaction();
                }
                return Connection;
            }
            switch (DbType)
            {
                case DatabaseType.SQLSERVER:
                    Connection = new SqlConnection(ConnectionString);
                    try
                    {
                        Connection.Open();
                        Transaction = IsTransaction ? Connection.BeginTransaction() : null;
                        return Connection;
                    }
                    catch (SqlException err)
                    {
                        try
                        {
                            Transaction.Dispose();
                            Connection.Dispose();
                        }
                        catch { }
                        throw err;
                    }
                case DatabaseType.MYSQL:
                    Connection = new MySqlConnection(ConnectionString);
                    try
                    {
                        Connection.Open();
                        Transaction = IsTransaction ? Connection.BeginTransaction() : null;
                        return Connection;
                    }
                    catch (MySqlException err)
                    {
                        try
                        {
                            Transaction.Dispose();
                            Connection.Dispose();
                        }
                        catch { }
                        throw err;
                    }
                case DatabaseType.ORACLE:
                    Connection = new OracleConnection(ConnectionString);
                    try
                    {
                        Connection.Open();
                        Transaction = IsTransaction ? Connection.BeginTransaction() : null;
                        return Connection;
                    }
                    catch (OracleException err)
                    {
                        try
                        {
                            Transaction.Dispose();
                            Connection.Dispose();
                        }
                        catch { }
                        throw err;
                    }

            }
            throw new Exception("不支持的数据库类型");
        }

        /// <summary>
        /// 当前dataadapter
        /// </summary>
        public DbDataAdapter CreateAdapter()
        {
            switch (DbType)
            {
                case DatabaseType.SQLSERVER:
                    return new SqlDataAdapter();
                case DatabaseType.MYSQL:
                    return new MySqlDataAdapter();
                case DatabaseType.ORACLE:
                    return new OracleDataAdapter();
            }
            throw new Exception("不支持的数据库类型");
        }

        /// <summary>
        /// 当前连接事务
        /// </summary>
        private DbTransaction Transaction = null;

        /// <summary>
        /// 当前数据类型操作类实例
        /// </summary>
        private IData DataInstance
        {
            get
            {
                switch (DbType)
                {
                    case DatabaseType.SQLSERVER:
                        return new DataSqlServer();
                    case DatabaseType.MYSQL:
                        return new DataMySql();
                    case DatabaseType.ORACLE:
                        return new DataOracle();
                }
                throw new Exception("不支持的数据库类型");
            }
        }

        /// <summary>
        /// 查询所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> QueryAll<T>() where T : class, new()
        {
            string tableName = Common.GetTableName(typeof(T));
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return new List<T>();
            }
            return Query<T>("SELECT * FROM " + tableName);
        }

        /// <summary>
        /// 查询一条记录(根据主键值查询)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Find<T>(params object[] objects) where T : class, new()
        {
            var (sql, parameters) = DataInstance.GetFindSql<T>(objects);
            if (parameters == null || parameters.Length == 0 || string.IsNullOrWhiteSpace(sql))
            {
                return null;
            }
            return QueryOne<T>(sql, parameters);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public List<T> Query<T>(string sql, params object[] objects) where T : class, new()
        {
            List<T> list = new List<T>();
            var (sqlString, parameters) = DataInstance.GetSqlAndParameter(sql, objects);
            using (DbCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sqlString;
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                if (parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                DbDataReader reader = cmd.ExecuteReader();
                cmd.Parameters.Clear();                
                return DataInstance.ReaderToList<T>(reader);
            }
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> Query<T>(string sql, DbParameter[] parameters = null) where T : class, new()
        {
            List<T> list = new List<T>();
            using (DbCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                DbDataReader reader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return DataInstance.ReaderToList<T>(reader);
            }
        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public T QueryOne<T>(string sql, params object[] objects) where T : class, new()
        {
            var list = Query<T>(DataInstance.GetQueryOneSql(sql), objects);
            return list.Any() ? list.First() : null;
        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T QueryOne<T>(string sql, DbParameter[] parameters) where T : class, new()
        {
            var list = Query<T>(sql, parameters);
            return list.Any() ? list.First() : null;
        }

        /// <summary>
        /// 查询DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, params object[] objects)
        {
            var (sqlString, parameters) = DataInstance.GetSqlAndParameter(sql, objects);
            using (DbCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sqlString;
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                if (parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (DbDataAdapter adapter = CreateAdapter())
                {
                    adapter.SelectCommand = cmd;
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    cmd.Parameters.Clear();
                    return dataTable;
                }
            }
        }

        /// <summary>
        /// 查询DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, DbParameter[] parameters = null)
        {
            using (DbCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (DbDataAdapter adapter = CreateAdapter())
                {
                    adapter.SelectCommand = cmd;
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    cmd.Parameters.Clear();
                    return dataTable;
                }
            }
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public int Execute(string sql, params object[] objects)
        {
            var (sqlString, parameters) = DataInstance.GetSqlAndParameter(sql, objects);
            using (DbCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sqlString;
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                if (parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                int i = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                InfluenceRows += i;
                return i;
            }
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int Execute(string sql, DbParameter[] parameters = null)
        {
            using (DbCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                int i = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                InfluenceRows += i;
                return i;
            }
        }

        /// <summary>
        /// 执行SQL返回一行一列数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, DbParameter[] parameters = null)
        {
            using (DbCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                object obj = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return obj;
            }
        }

        /// <summary>
        /// 执行SQL返回一行一列数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, params object[] objects)
        {
            var (sqlString, parameters) = DataInstance.GetSqlAndParameter(sql, objects);
            return ExecuteScalar(sqlString, parameters);
        }

        /// <summary>
        /// 执行SQL返回一行一列字符串
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string ExecuteScalarString(string sql, DbParameter[] parameters = null)
        {
            object obj = ExecuteScalar(sql, parameters);
            return null == obj ? string.Empty : obj.ToString();
        }

        /// <summary>
        /// 执行SQL返回一行一列字符串
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public string ExecuteScalarString(string sql, params object[] objects)
        {
            var (sqlString, parameters) = DataInstance.GetSqlAndParameter(sql, objects);
            return ExecuteScalarString(sqlString, parameters);
        }

        /// <summary>
        /// 得到一个DataTableSchema
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable GetDataTableSchema(string sql, DbParameter[] parameters = null)
        {
            using (DbCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (DbDataAdapter adapter = CreateAdapter())
                {
                    adapter.SelectCommand = cmd;
                    DataTable dataTable = new DataTable();
                    adapter.FillSchema(dataTable, SchemaType.Mapped);
                    cmd.Parameters.Clear();
                    return dataTable;
                }
            }
        }

        /// <summary>
        /// 测试一个SQL语句是否正确（不提交）
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">sql参数</param>
        /// <returns></returns>
        public string TestSQL(string sql, DbParameter[] parameters = null)
        {
            if (!IsTransaction)
            {
                return "事务未开启!";
            }
            using (DbCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Transaction = Transaction;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                try
                {
                    int i = cmd.ExecuteNonQuery();
                    return i.ToString();
                }
                catch (SqlException err)
                {
                    return err.Message;
                }
                finally
                {
                    cmd.Parameters.Clear();
                    Transaction.Rollback();
                }
            }
        }

        /// <summary>
        /// 测试连接是否正常
        /// </summary>
        /// <returns>返回字符串1为正常，其它为错误信息</returns>
        public string TestConnection()
        {
            string errMsg = string.Empty;
            switch (DbType)
            {
                case DatabaseType.SQLSERVER:
                    var sqlConn = new SqlConnection(ConnectionString);
                    try
                    {
                        sqlConn.Open();
                    }
                    catch (SqlException err)
                    {
                        return err.Message;
                    }
                    finally
                    {
                        sqlConn.Close();
                        sqlConn.Dispose();
                    }
                    return "1";
                case DatabaseType.MYSQL:
                    var mySqlConn = new MySqlConnection(ConnectionString);
                    try
                    {
                        mySqlConn.Open();
                    }
                    catch (MySqlException err)
                    {
                        return err.Message;
                    }
                    finally
                    {
                        mySqlConn.Close();
                        mySqlConn.Dispose();
                    }
                    return "1";
                case DatabaseType.ORACLE:
                    var oracleConn = new OracleConnection(ConnectionString);
                    try
                    {
                        oracleConn.Open();
                    }
                    catch (OracleException err)
                    {
                        return err.Message;
                    }
                    finally
                    {
                        oracleConn.Close();
                        oracleConn.Dispose();
                    }
                    return "1";
            }
            return "不支持的数据库类型";
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public int Add<T>(T t) where T : class, new()
        {
            using (DbCommand cmd = Connection.CreateCommand())
            {
                var (sql, parsmeters) = DataInstance.GetInsertSql(t);
                cmd.CommandText = sql;
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                if (parsmeters.Length > 0)
                {
                    cmd.Parameters.AddRange(parsmeters);
                }
                int i = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                InfluenceRows += i;
                return i;
            }
        }

        /// <summary>
        /// 添加多条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public int AddRange<T>(IEnumerable<T> ts) where T : class, new()
        {
            int count = ts.Count();
            if (count > 10 && DbType == DatabaseType.SQLSERVER)//如果同时插入大于10条，采用sqlbulkcopy(目前只实现了SQLSERVER)
            {
                DataInstance.BulkCopy(ts, Connection, Transaction);
                InfluenceRows += count;
                return count;
            }

            int i = 0;
            var (sql, parameters) = DataInstance.GetInsertSql(ts);
            using (DbCommand cmd = Connection.CreateCommand())
            {
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                cmd.CommandText = sql;
                cmd.Prepare();
                foreach (var parray in parameters)
                {
                    if (parray.Length > 0)
                    {
                        cmd.Parameters.AddRange(parray);
                    }
                    i += cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                InfluenceRows += i;
                return i;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public int Update<T>(T t) where T : class, new()
        {
            using (DbCommand cmd = Connection.CreateCommand())
            {
                var (sql, parsmeters) = DataInstance.GetUpdateSql(t);
                cmd.CommandText = sql;
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                if (parsmeters.Length > 0)
                {
                    cmd.Parameters.AddRange(parsmeters);
                }
                int i = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                InfluenceRows += i;
                return i;
            }
        }

        /// <summary>
        /// 更新多条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public int UpdateRange<T>(IEnumerable<T> ts) where T : class, new()
        {
            int i = 0;
            var (sql, parsmeters) = DataInstance.GetUpdateSql(ts);
            using (DbCommand cmd = Connection.CreateCommand())
            {
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                cmd.CommandText = sql;
                cmd.Prepare();
                foreach (var parray in parsmeters)
                {
                    if (parray.Length > 0)
                    {
                        cmd.Parameters.AddRange(parray);
                    }
                    i += cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
            }
            InfluenceRows += i;
            return i;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public int Remove<T>(T t) where T : class, new()
        {
            using (DbCommand cmd = Connection.CreateCommand())
            {
                var (sql, parsmeters) = DataInstance.GetRemoveSql(t);
                cmd.CommandText = sql;
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                if (parsmeters.Length > 0)
                {
                    cmd.Parameters.AddRange(parsmeters);
                }
                int i = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                InfluenceRows += i;
                return i;
            }
        }

        /// <summary>
        /// 删除(多条)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public int RemoveRange<T>(IEnumerable<T> ts) where T : class, new()
        {
            int i = 0;
            var (sql, parsmeters) = DataInstance.GetRemoveSql(ts);
            using (DbCommand cmd = Connection.CreateCommand())
            {
                if (IsTransaction)
                {
                    cmd.Transaction = Transaction;
                }
                cmd.CommandText = sql;
                cmd.Prepare();
                foreach (var parray in parsmeters)
                {
                    if (parray.Length > 0)
                    {
                        cmd.Parameters.AddRange(parray);
                    }
                    i += cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
            }
            InfluenceRows += i;
            return i;
        }

        /// <summary>
        /// 查询自增主键
        /// </summary>
        /// <returns></returns>
        public int GetIdentity(string seqName = "")
        {
            return ExecuteScalarString(DataInstance.GetIdentitySql(seqName)).ToInt(-1);
        }

        /// <summary>
        /// 提交更改
        /// </summary>
        public int SaveChanges()
        {
            int i = InfluenceRows;
            InfluenceRows = 0;
            if (null != Transaction)
            {
                Transaction.Commit();
                return i;
            }
            return 0;
        }

        /// <summary>
        /// 释放连接
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (null != Transaction)
                {
                    Transaction.Dispose();
                }
                Connection.Close();
                Connection.Dispose();
            }
            catch { }
        }

        ~DataContext()
        {
            try
            {
                if (null != Transaction)
                {
                    Transaction.Dispose();
                }
                Connection.Close();
                Connection.Dispose();
            }
            catch { }
        }
    }

    
}
