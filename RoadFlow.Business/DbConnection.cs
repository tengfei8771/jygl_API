using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Utility;
using System.Data;

namespace RoadFlow.Business
{
    public class DbConnection
    {
        private readonly Data.DbConnection dbConnectionData;
        public DbConnection()
        {
            dbConnectionData = new Data.DbConnection();
        }
        /// <summary>
        /// 数据连接类型枚举
        /// </summary>
        public enum ConnType
        {
            SqlServer,
            MySql,
            Oracle
        }
        /// <summary>
        /// 得到所有连接
        /// </summary>
        /// <returns></returns>
        public List<Model.DbConnection> GetAll()
        {
            return dbConnectionData.GetAll();
        }
        /// <summary>
        /// 查询一个连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.DbConnection Get(Guid id)
        {
            return GetAll().Find(p => p.Id == id);
        }
        /// <summary>
        /// 添加一个连接
        /// </summary>
        /// <param name="dbConnection">连接实体</param>
        /// <returns></returns>
        public int Add(Model.DbConnection dbConnection)
        {
            return dbConnectionData.Add(dbConnection);
        }
        /// <summary>
        /// 更新连接
        /// </summary>
        /// <param name="dictionary">连接实体</param>
        public int Update(Model.DbConnection dbConnection)
        {
            return dbConnectionData.Update(dbConnection);
        }
        /// <summary>
        /// 删除一批连接
        /// </summary>
        /// <param name="dbConnections">连接实体数组</param>
        /// <returns></returns>
        public int Delete(Model.DbConnection[] dbConnections)
        {
            return dbConnectionData.Delete(dbConnections);
        }
        /// <summary>
        /// 得到连接最大排序号
        /// </summary>
        /// <returns></returns>
        public int GetMaxSort()
        {
            var all = GetAll();
            return all.Count == 0 ? 5 : all.Max(p => p.Sort) + 5;
        }

        /// <summary>
        /// 得到连接类别下拉项
        /// </summary>
        /// <returns></returns>
        public string GetConnTypeOptions(string value = "")
        {
            StringBuilder options = new StringBuilder();
            var array = Enum.GetValues(typeof(ConnType));
            foreach (var arr in array)
            {
                options.AppendFormat("<option value=\"{0}\" {1}>{0}</option>", arr, arr.ToString().EqualsIgnoreCase(value) ? "selected=\"selected\"" : "");
            }
            return options.ToString();
        }

        /// <summary>
        /// 得到所有连接下拉选项
        /// </summary>
        /// <param name="value">默认值</param>
        /// <returns></returns>
        public string GetOptions(string value = "")
        {
            StringBuilder options = new StringBuilder();
            var array = GetAll();
            foreach (var arr in array)
            {
                options.AppendFormat("<option value=\"{0}\" {1}>{2}</option>", arr.Id, arr.Id.ToString().EqualsIgnoreCase(value) ? "selected=\"selected\"" : "", arr.Name);
            }
            return options.ToString();
        }

        /// <summary>
        /// 将字符串连接类型转换为枚举
        /// </summary>
        /// <param name="connType"></param>
        /// <returns></returns>
        public ConnType ParseConnType(string connType)
        {
            return (ConnType)Enum.Parse(typeof(ConnType), connType, true);
        }

        /// <summary>
        /// 测试一个连接
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <returns>返回"1"表示正常，其它为错误信息</returns>
        public string TestConnection(Guid id)
        {
            var conn = Get(id);
            if (null == conn)
            {
                return "未找到连接";
            }
            return dbConnectionData.TesetConnection(conn);
        }

        /// <summary>
        /// 得到一个连接所有表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetTables(Guid id)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var conn = Get(id);
            if (null == conn)
            {
                return dict;
            }
            DataTable dt = dbConnectionData.GetTables(conn);
            foreach (DataRow dr in dt.Rows)
            {
                dict.Add(dr[0].ToString(), dr[1].ToString());
            }
            return dict;
        }

        /// <summary>
        /// 得到一个连接所有表选项
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <param name="value">默认值</param>
        /// <returns></returns>
        public string GetTableOptions(Guid id, string value = "")
        {
            Dictionary<string, string> dict = GetTables(id);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var table in dict)
            {
                stringBuilder.AppendFormat("<option value=\"{0}\" {1}>{2}</option>", table.Key, table.Key.EqualsIgnoreCase(value) ? "selected=\"selected\"" : "", table.Key + (table.Value.IsNullOrWhiteSpace() ? "" : "(" + table.Value + ")"));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 得到一个表所有字段
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<Model.TableField> GetTableFields(Guid id, string tableName)
        {
            var conn = Get(id);
            if (null == conn)
            {
                return new List<Model.TableField>();
            }
            return GetTableFields(conn, tableName);
        }

        /// <summary>
        /// 得到一个表所有字段
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<Model.TableField> GetTableFields(Model.DbConnection conn, string tableName)
        {
            List<Model.TableField> tableFields = new List<Model.TableField>();
            string dbName = string.Empty;
            if (conn.ConnType.EqualsIgnoreCase("mysql"))
            {
                string[] connStringArray = conn.ConnString.Split(';');
                foreach(string connString in connStringArray)
                {
                    string[] connArray = connString.Split('=');
                    if (connArray.Length > 1 && (connArray[0].Trim().Equals("database") || connArray[0].Trim().Equals("db") || connArray[0].Trim().Equals("Initial Catalog")))
                    {
                        dbName = connArray[1].Trim();
                    }
                }
            }
            var dt = dbConnectionData.GetTableFields(conn, tableName, dbName);
            foreach (DataRow dr in dt.Rows)
            {
                Model.TableField tableField = new Model.TableField()
                {
                    FieldName = dr["f_name"].ToString(),
                    Type = dr["t_name"].ToString(),
                    Size = dr["length"].ToString().ToInt(),
                    IsNull = "1".Equals(dr["is_null"].ToString()),
                    IsDefault = dr["cdefault"].ToString().ToInt() != 0,
                    IsIdentity = dr["isidentity"].ToString().ToInt() == 1,
                    DefaultValue = dr["defaultvalue"].ToString(),
                    Comment = dr["comments"].ToString()
                };
                tableFields.Add(tableField);
            }
            return tableFields;
        }

        /// <summary>
        /// 得到一个表所有字段下拉选项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GetTableFieldOptions(Guid id, string tableName, string fieldName = "")
        {
            var fields = GetTableFields(id, tableName);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var field in fields)
            {
                stringBuilder.AppendFormat("<option value=\"{0}\" {1}>{2}</option>", field.FieldName, field.FieldName.EqualsIgnoreCase(fieldName) ? "selected=\"selected\"" : "", field.FieldName + (field.Comment.IsNullOrWhiteSpace() ? "" : "(" + field.Comment + ")"));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 执行一个SQL语句
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <param name="sql">要执行的sql</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回数字表示成功（数字是受影响的行数），其它为错误信息</returns>
        public string ExecuteSQL(Guid id, string sql, System.Data.Common.DbParameter[] parameters = null)
        {
            List<(string sql, System.Data.Common.DbParameter[] parameters)> tuples = new List<(string sql, System.Data.Common.DbParameter[] parameters)>()
            {
                (sql, parameters)
            };
            return ExecuteSQL(id, tuples);
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <param name="sql">sql</param>
        /// <param name="paramObjs">参数值</param>
        /// <returns>返回数字表示成功（数字是受影响的行数），其它为错误信息</returns>
        public string ExecuteSQL(Guid id, string sql, IEnumerable<object> paramObjs)
        {
            var conn = Get(id);
            if (null == conn)
            {
                return "未找到连接实体";
            }
            return dbConnectionData.ExecuteSQL(conn, sql, paramObjs);
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <param name="sql">sql</param>
        /// <param name="paramObjs">参数值</param>
        /// <returns>返回数字表示成功（数字是受影响的行数），其它为错误信息</returns>
        public string ExecuteSQL(Guid id, string sql, params object[] paramsObj)
        {
            var conn = Get(id);
            if (null == conn)
            {
                return "未找到连接实体";
            }
            return dbConnectionData.ExecuteSQL(conn, sql, paramsObj);
        }

        /// <summary>
        /// 执行SQL(调用EF的executecommand,不用指明参数类型)
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <param name="sql">sql</param>
        /// <param name="paramObjs">参数值</param>
        /// <returns>返回数字表示成功（数字是受影响的行数），其它为错误信息</returns>
        public string ExecuteSQL(Guid id, List<(string, IEnumerable<object>)> ps)
        {
            var conn = Get(id);
            if (null == conn)
            {
                return "未找到连接实体";
            }
            return dbConnectionData.ExecuteSQL(conn, ps);
        }

        /// <summary>
        /// 执行多条SQL
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <param name="tuples">sql语句, sql参数</param>
        /// <returns>返回数字表示成功（数字是受影响的行数），其它为错误信息</returns>
        public string ExecuteSQL(Guid id, List<(string sql, System.Data.Common.DbParameter[] parameters)> tuples)
        {
            var conn = Get(id);
            if (null == conn)
            {
                return "未找到连接实体";
            }
            return dbConnectionData.ExecuteSQL(conn, tuples);
        }

        /// <summary>
        /// 测试一个SQL语句是否正确
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">sql参数</param>
        /// <returns></returns>
        public string TestSQL(Guid id, string sql, System.Data.Common.DbParameter[] parameters = null)
        {
            var conn = Get(id);
            if (null == conn)
            {
                return "未找到连接实体";
            }
            return dbConnectionData.TestSQL(conn, Wildcard.Filter(sql), parameters);
        }

        /// <summary>
        /// 得到DATATABLE
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns></returns>
        public DataTable GetDataTable(Guid id, string sql, params object[] objects)
        {
            return GetDataTable(Get(id), sql, objects);
        }

        /// <summary>
        /// 得到DATATABLE
        /// </summary>
        /// <param name="dbConnectionModel">连接实体</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns></returns>
        public DataTable GetDataTable(Model.DbConnection dbConnectionModel, string sql, params object[] objects)
        {
            return dbConnectionData.GetDataTable(dbConnectionModel, sql, objects);
        }

        /// <summary>
        /// 得到DATATABLE
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns></returns>
        public DataTable GetDataTable(Guid id, string sql, System.Data.Common.DbParameter[] parameters = null)
        {
            return GetDataTable(Get(id), sql, parameters);
        }

        /// <summary>
        /// 得到DATATABLE
        /// </summary>
        /// <param name="dbConnectionModel">连接实体</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns></returns>
        public DataTable GetDataTable(Model.DbConnection dbConnectionModel, string sql, System.Data.Common.DbParameter[] parameters = null)
        {
            if (null == dbConnectionModel)
            {
                return new DataTable();
            }
            return dbConnectionData.GetDataTable(dbConnectionModel, sql, parameters);
        }

        /// <summary>
        /// 得到DATATABLE
        /// </summary>
        /// <param name="dbConnectionModel"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKey"></param>
        /// <param name="primaryKeyValue"></param>
        /// <param name="order">排序 f1 desc</param>
        /// <returns></returns>
        public DataTable GetDataTable(Model.DbConnection dbConnectionModel, string tableName, string primaryKey, string primaryKeyValue, string order = "")
        {
            if (null == dbConnectionModel || primaryKeyValue.IsNullOrWhiteSpace()
                || tableName.IsNullOrWhiteSpace() || primaryKey.IsNullOrWhiteSpace())
            {
                return new DataTable();
            }
            DataTable dt = dbConnectionData.GetDataTable(dbConnectionModel, tableName, primaryKey, primaryKeyValue);
            if (!order.IsNullOrWhiteSpace())
            {
                dt.DefaultView.Sort = order;
                return dt.DefaultView.ToTable();
            }
            else
            {
                return dt;
            }
        }

        /// <summary>
        /// 得到一个字段的值
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">要查询的字段</param>
        /// <param name="primaryKey">主键</param>
        /// <param name="primaryKeyValue">主键值</param>
        /// <returns></returns>
        public string GetFieldValue(Guid id, string tableName, string fieldName, string primaryKey, string primaryKeyValue)
        {
            var conn = Get(id);
            if (null == conn)
            {
                return string.Empty;
            }
            return dbConnectionData.GetFieldValue(conn, tableName, fieldName, primaryKey, primaryKeyValue);
        }

        /// <summary>
        /// 得到一个字段的值
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public string GetFieldValue(Guid id, string sql)
        {
            var conn = Get(id);
            if (null == conn)
            {
                return string.Empty;
            }
            return dbConnectionData.GetFieldValue(conn, sql);
        }

        /// <summary>
        /// 得到一个字段的值
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public string GetFieldValue(Guid id, string sql, params object[] objs)
        {
            var conn = Get(id);
            if (null == conn)
            {
                return string.Empty;
            }
            return dbConnectionData.GetFieldValue(conn, sql, objs);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="tuples"></param>
        /// <returns>返回数字表示成功（数字是受影响的行数或者自增主键值），其它为错误信息</returns>
        public string SaveData(Model.DbConnection dbConnection, List<(Dictionary<string, object> dicts, string tableName, string primaryKey, int flag)> tuples, bool isIdentity = false, string seqName = "")
        {
            return tuples.Count == 0 ? "0" : dbConnectionData.SaveData(dbConnection, tuples, isIdentity, seqName);
        }

        /// <summary>
        /// 根据sql得到查询字段
        /// </summary>
        /// <param name="connId">连接ID</param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<string> GetFieldsBySql(Guid connId, string sql, System.Data.Common.DbParameter[] parameters = null)
        {
            sql = Wildcard.Filter(sql).FilterSelectSql();
            DataTable dt = dbConnectionData.GetDataTableSchema(Get(connId), sql, parameters);
            List<string> fields = new List<string>();
            foreach (DataColumn column in dt.Columns)
            {
                fields.Add(column.ColumnName);
            }
            return fields;
        }
    }
}
