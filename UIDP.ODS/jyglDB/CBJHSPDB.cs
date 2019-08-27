using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.jyglDB
{
    public class CBJHSPDB
    {
        DBTool db = new DBTool("");

        public DataTable GetInfo(string XMBH, string XMMC)
        {
            string sql = " SELECT * FROM jy_cbjh WHERE IS_DELETE=0";
            if (!string.IsNullOrEmpty(XMBH))
            {
                sql += " AND XMBH LIKE'" + XMBH + "%'";
            }
            if (!string.IsNullOrEmpty(XMMC))
            {
                sql += " AND XMBH='" + XMMC + "'";
            }
            //sql += " LIMIT " + (page - 1) * limit + "," + limit;
            return db.GetDataTable(sql);
        }
    }
}
