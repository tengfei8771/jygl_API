using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;
using System.Data.Common;
using System.Data;
using System.Linq;

namespace RoadFlow.Data
{
    /// <summary>
    /// 数据连接SQL类
    /// </summary>
    public class DbconnnectionSql
    {
        private readonly string dbType;
        private readonly string connStr;
        private readonly Model.DbConnection dbConnectionModel;
        
        public DbconnnectionSql(string dataBaseType)
        {
            dbType = dataBaseType.ToLower();
            connStr = string.Empty;
            dbConnectionModel = null;
            
        }

        public DbconnnectionSql(Model.DbConnection dbConnection)
        {
            dbType = dbConnection.ConnType.ToLower();
            connStr = dbConnection.ConnString;
            dbConnectionModel = dbConnection;
        }

        public SqlInterface.ISql SqlInstance
        {
            get
            {
                switch (dbType)
                {
                    case "sqlserver":
                        return new SqlInterface.SqlServer(dbConnectionModel, dbType);
                    case "mysql":
                        return new SqlInterface.MySql(dbConnectionModel, dbType);
                    case "oracle":
                        return new SqlInterface.Oracle(dbConnectionModel, dbType);
                }
                throw new Exception("不支持的数据库类型");
            }
        }
    }
}
