using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.jyglDB
{
    public class CBJHTZDB
    {
        DBTool db = new DBTool("");

        public DataTable GetInfo(Dictionary<string, object> d)
        {
            string sql = " SELECT a.S_ID,a.XMBH,a.TZQJE,a.TZJE,a.TZSM,a.TZSJ,a.CJR,a.CJSJ,b.XMMC,b.CBDW,b.TZHJHZJE,u.USER_NAME CJRName  FROM jy_cbjh_adjust a left join jy_cbjh b on a.XMBH=b.XMBH left join ts_uidp_userinfo u on a.CJR=u.USER_ID  WHERE IS_DELETE=0";
            if (d.Keys.Contains("XMCODE") && d["XMCODE"] != null && d["XMCODE"].ToString() != "")
            {
                sql += " and XMCODE like '%" + d["XMCODE"].ToString() + "%'";
            }
            if (d.Keys.Contains("XMMC") && d["XMMC"] != null && d["XMMC"].ToString() != "")
            {
                sql += " and XMMC like '%" + d["XMMC"].ToString() + "%'";
            }
            //if (d.Keys.Contains("S_BeginDate") && d["S_BeginDate"] != null && d["S_BeginDate"].ToString() != "" && d.Keys.Contains("S_EndDate") && d["S_EndDate"] != null && d["S_EndDate"].ToString() != "")
            //{
            //    DateTime bdate = Convert.ToDateTime(d["S_BeginDate"].ToString());
            //    DateTime edate = Convert.ToDateTime(d["S_EndDate"].ToString());
            //    sql += "and  SQSJ BETWEEN '" + bdate.ToString("yyyy-MM-dd") + "' AND '" + edate.ToString("yyyy-MM-dd") + "'";
            //}
            return db.GetDataTable(sql);
        }

        public string CreateInfo(Dictionary<string,object> d)
        {
            List<string> sqllist = new List<string>();
            string sql = "INSERT INTO jy_cbjh_adjust (S_ID,XMBH,XMMC,TZQJE,TZJE,TZSM,TZSJ,CJR,CJSJ)values(";
            sql += GetSQLStr(Guid.NewGuid().ToString());
            sql += GetSQLStr(d["XMBH"]);
            sql += GetSQLStr(d["XMMC"]);
            sql += GetSQLStr(d["TZQJE"]);
            sql += GetSQLStr(d["TZJE"]);
            sql += GetSQLStr(d["TZSM"]);
            sql += GetSQLStr(d["TZSJ"]);
            sql += GetSQLStr(d["CJR"]);
            sql += GetSQLStr(DateTime.Now);
            sql = sql.TrimEnd(',');
            sql += ")";
            string updatesql = "update jy_cbjh set TZHJHZJE=TZHJHZJE+"+ d["TZJE"].ToString() + " where XMBH='" + d["XMBH"].ToString() + "'";
            sqllist.Add(sql);
            sqllist.Add(updatesql);
            return db.Executs(sqllist);
            //return db.ExecutByStringResult(sql);
        }

        public string UpdateInfo(Dictionary<string,object> d)
        {
            string sql = " UPDATE jy_cbjh_adjust SET XMBH=" + GetSQLStr(d["XMBH"]);
            sql += "XMMC=" + GetSQLStr(d["XMMC"]);
            sql += "TZQJE=" + GetSQLStr(d["TZQJE"]);
            sql += "TZJE=" + GetSQLStr(d["TZJE"]);
            sql += "TZSM=" + GetSQLStr(d["TZSM"]);
            sql += "TZSJ=" + GetSQLStr(d["TZSJ"]);
            //sql += "BXSY=" + GetSQLStr(d["BXSY"]);
            //sql += "BXJEDX=" + GetSQLStr(d["BXJEDX"]);
            //sql += "BXJE=" + GetSQLStr(d["BXJE"], 1);
            //sql += "YJKJE=" + GetSQLStr(d["YJKJE"],1);
            //sql += "XFKJE=" + GetSQLStr(d["XFKJE"], 1);
            //sql += "FKFS=" + GetSQLStr(d["FKFS"]);
            //sql += "FJZS=" + GetSQLStr(d["FJZS"], 1);
            //sql += "SKDW=" + GetSQLStr(d["SKDW"]);
            //sql += "KHH=" + GetSQLStr(d["KHH"]);
            //sql += "YHZH=" + GetSQLStr(d["YHZH"]);
            //sql += "BJR=" + GetSQLStr(d["BJR"]);
            //sql += "BJSJ=" + GetSQLStr(DateTime.Now);
            sql = sql.TrimEnd(',');
            sql += " WHERE S_ID=" + GetSQLStr(d["S_ID"]);
            sql = sql.TrimEnd(',');
            return db.ExecutByStringResult(sql);
        }

        //public string DeleteInfo(Dictionary<string,object> d)
        //{
        //    string sql = "UPDATE jy_fybx set IS_DELETE=1 WHERE S_ID=" + GetSQLStr(d["S_ID"]);
        //    sql = sql.TrimEnd(',');
        //    return db.ExecutByStringResult(sql);
        //}


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
