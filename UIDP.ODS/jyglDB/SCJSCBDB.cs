using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.jyglDB
{
    public class SCJSCBDB
    {
        DBTool db = new DBTool("");

        public DataTable GetInfo(string XMBH, string XMMC)
        {
            string sql = " SELECT a.*,b.Name as PC,c.Name as LB,d.ORG_NAME CBDWMC FROM jy_cbjh a left join tax_dictionary b on a.XMPC=b.Code left join tax_dictionary c on c.Code=a.XMLB left join ts_uidp_org d on a.CBDW=d.ORG_CODE WHERE a.IS_DELETE=0 AND a.SFCW=0 ";
            if (!string.IsNullOrEmpty(XMBH))
            {
                sql += " AND a.XMBH LIKE'" + XMBH + "%'";
            }
            if (!string.IsNullOrEmpty(XMMC))
            {
                sql += " AND a.XMMC='" + XMMC + "'";
            }
            //sql += " LIMIT " + (page - 1) * limit + "," + limit;
            return db.GetDataTable(sql);
        }

        public DataTable GetDetailInfo(string XMBH)
        {
            string sql = "select * from jy_cbwz where XMBH='" + XMBH + "' AND IS_DELETE=0 ";
            return db.GetDataTable(sql);
        }
    }
}
