﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.jyglDB
{
    public class FYBXDB
    {
        DBTool db = new DBTool("");

        public DataTable GetInfo(Dictionary<string, object> d)
        {
            string sql = " SELECT * FROM jy_fybx WHERE IS_DELETE=0";
            if (d.Keys.Contains("BXDH") && d["BXDH"] != null && d["BXDH"].ToString() != "")
            {
                sql += " and BXDH like '%" + d["BXDH"].ToString() + "%'";
            }
            if (d.Keys.Contains("S_BeginDate") && d["S_BeginDate"] != null && d["S_BeginDate"].ToString() != "" && d.Keys.Contains("S_EndDate") && d["S_EndDate"] != null && d["S_EndDate"].ToString() != "")
            {
                DateTime bdate = Convert.ToDateTime(d["S_BeginDate"].ToString());
                DateTime edate = Convert.ToDateTime(d["S_EndDate"].ToString());
                sql += "and  SQSJ BETWEEN '" + bdate.ToString("yyyy-MM-dd") + "' AND '" + edate.ToString("yyyy-MM-dd") + "'";
            }
            return db.GetDataTable(sql);
        }

        public string CreateInfo(Dictionary<string,object> d)
        {
            List<string> sqllist = new List<string>();
            string sql = "INSERT INTO jy_fybx (S_ID,BXDH,DWBM,FYXM,SQSJ,BXSY,BXJEDX,BXJE,YJKJE,XFKJE,FKFS,FJZS,SKDW,KHH,YHZH,SPZT,CJR,CJSJ,IS_DELETE)values(";
            sql += GetSQLStr(Guid.NewGuid().ToString());
            sql += GetSQLStr(d["BXDH"]);
            sql += GetSQLStr(d["DWBM"]);
            sql += GetSQLStr(d["FYXM"]);
            sql += GetSQLStr(d["SQSJ"]);
            sql += GetSQLStr(d["BXSY"]);
            sql += GetSQLStr(d["BXJEDX"]);
            sql += GetSQLStr(d["BXJE"], 1);
            sql += GetSQLStr(d["YJKJE"],1);
            sql += GetSQLStr(d["XFKJE"],1);
            sql += GetSQLStr(d["FKFS"]);
            sql += GetSQLStr(d["FJZS"], 1);
            sql += GetSQLStr(d["SKDW"]);
            sql += GetSQLStr(d["KHH"]);
            sql += GetSQLStr(d["YHZH"]);
            sql += GetSQLStr(0, 1);
            sql += GetSQLStr(d["CJR"]);
            sql += GetSQLStr(DateTime.Now);
            sql += GetSQLStr(0, 1);
            sql = sql.TrimEnd(',');
            sql += ")";
            return db.ExecutByStringResult(sql);
        }

        public string UpdateInfo(Dictionary<string,object> d)
        {
            string sql = " UPDATE jy_fybx SET BXDH=" + GetSQLStr(d["BXDH"]);
            sql += "DWBM=" + GetSQLStr(d["DWBM"]);
            sql += "FYXM=" + GetSQLStr(d["FYXM"]);
            sql += "SQSJ=" + GetSQLStr(d["SQSJ"]);
            sql += "BXSY=" + GetSQLStr(d["BXSY"]);
            sql += "BXJEDX=" + GetSQLStr(d["BXJEDX"]);
            sql += "BXJE=" + GetSQLStr(d["BXJE"], 1);
            sql += "YJKJE=" + GetSQLStr(d["YJKJE"],1);
            sql += "XFKJE=" + GetSQLStr(d["XFKJE"], 1);
            sql += "FKFS=" + GetSQLStr(d["FKFS"]);
            sql += "FJZS=" + GetSQLStr(d["FJZS"], 1);
            sql += "SKDW=" + GetSQLStr(d["SKDW"]);
            sql += "KHH=" + GetSQLStr(d["KHH"]);
            sql += "YHZH=" + GetSQLStr(d["YHZH"]);
            sql += "BJR=" + GetSQLStr(d["BJR"]);
            sql += "BJSJ=" + GetSQLStr(DateTime.Now);
            sql = sql.TrimEnd(',');
            sql += " WHERE S_ID=" + GetSQLStr(d["S_ID"]);
            sql = sql.TrimEnd(',');
            return db.ExecutByStringResult(sql);
        }

        public string DeleteInfo(Dictionary<string,object> d)
        {
            string sql = "UPDATE jy_fybx set IS_DELETE=1 WHERE S_ID=" + GetSQLStr(d["S_ID"]);
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