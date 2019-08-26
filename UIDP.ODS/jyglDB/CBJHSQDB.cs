using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.jyglDB
{
    public class CBJHSQDB
    {
        DBTool db = new DBTool("");

        public DataTable GetInfo(string XMBH,string XMMC)
        {
            string sql = " SELECT * FROM jy_srxm WHERE IS_DELETE=0";
            if (!string.IsNullOrEmpty(XMBH))
            {
                sql += " AND XMBH='" + XMBH + "'";
            } 
            if (!string.IsNullOrEmpty(XMMC))
            {
                sql += " AND XMBH='" + XMMC + "'";
            }
            //sql += " LIMIT " + (page - 1) * limit + "," + limit;
            return db.GetDataTable(sql);
        }

        public string CreateInfo(Dictionary<string,object> d)
        {
            string sql = "INSERT INTO jy_srxm (XMBH,XMMC,CBDW,JSNR,JHZJE,LSJE,BNJE,WLJE,XMPC,CJR,CJSJ,IS_DELETE,CZWZ,SFCW)values(";
            sql += GetSQLStr(d["XMBH"]);
            sql += GetSQLStr(d["XMMC"]);
            sql += GetSQLStr(d["CBDW"]);
            sql += GetSQLStr(d["JSNR"]);
            sql += GetSQLStr(d["JHZJE"],1);
            sql += GetSQLStr(d["LSJE"], 1);
            sql += GetSQLStr(d["BNJE"], 1);
            sql += GetSQLStr(d["WLJE"], 1);
            sql += GetSQLStr(d["XMPC"]);
            sql += GetSQLStr(d["CJR"]);
            sql += GetSQLStr(DateTime.Now);
            sql += GetSQLStr(0,1);
            sql += GetSQLStr(d["CZWZ"],1);
            sql += GetSQLStr(d["SFCW"], 1);
            sql = sql.TrimEnd(',');
            sql += ")";
            return db.ExecutByStringResult(sql);
        }

        public string UpdateInfo(Dictionary<string,object> d)
        {
            string sql = " UPDATE jy_srxm SET XMMC=" + GetSQLStr(d["XMMC"]);
            sql += "CBDW=" + GetSQLStr(d["CBDW"]);
            sql += "JSNR=" + GetSQLStr(d["JSNR"]);
            sql += "JHZJE=" + GetSQLStr(d["JHZJE"], 1);
            sql += "LSJE=" + GetSQLStr(d["LSJE"], 1);
            sql += "BNJE=" + GetSQLStr(d["BNJE"], 1);
            sql += "WLJE=" + GetSQLStr(d["WLJE"], 1);
            sql += "XMPC=" + GetSQLStr(d["XMPC"]);
            sql += "CJR=" + GetSQLStr(d["CJR"]);
            sql += "CJSJ=" + GetSQLStr(d["CJSJ"]);
            sql += "IS_DELETE=" + GetSQLStr(d["IS_DELETE"],1);
            sql += "CZWZ=" + GetSQLStr(d["CZWZ"],1);
            sql += "SFCW=" + GetSQLStr(d["SFCW"],1);
            sql = sql.TrimEnd(',');
            sql += " WHERE XMBH=" + GetSQLStr(d["XMBH"]);
            sql = sql.TrimEnd(',');
            return db.ExecutByStringResult(sql);
        }

        public string DeleteInfo(Dictionary<string,object> d)
        {
            string sql = "UPDATE jy_srxm set IS_DELETE=1 WHERE XMBH=" + GetSQLStr(d["XMBH"]);
            sql = sql.TrimEnd(',');
            return db.ExecutByStringResult(sql);
        }


        public string GetSQLStr(object s,int flag=0)
        {
            string sql = string.Empty;
            if (s == null || s.ToString() == "")
            {
                return "null,";
            }
            else
            {
                if (flag == 0)
                {
                    return "'" + s + "',";
                }
                else
                {
                    return s + ",";
                }
            }
        }
    }
}
