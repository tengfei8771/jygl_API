﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.jyglDB
{
    public class TravelExpenseDB
    {
        DBTool db = new DBTool("");
        public DataTable GetInfo(string CLBH)
        {
            string sql = " select * from jy_clbx where IS_DELETE=0";
            if (!String.IsNullOrEmpty(CLBH))
            {
                sql = " and CLBH='" + CLBH + "'";
            }
            return db.GetDataTable(sql);
        }

        public string CreateInfo(Dictionary<string,object> d)
        {
            List<string> sqllist = new List<string>();
            string sql = " INSERT INTO jy_clbx (CLBH,DWBM,CCXM,CCSY,CCKSSJ,CCJSSJ,CCTS,HJJE,HJDX,YJCLF,YTBJE,REMARK,SKRXM,CJR,CJSJ,IS_DELETE) VALUES (";
            sql += GetSQLStr(d["CLBH"]);
            sql += GetSQLStr(d["DWBM"]);
            sql += GetSQLStr(d["CCXM"]);
            sql += GetSQLStr(d["CCSY"]);
            sql += GetSQLStr(d["CCKSSJ"]);
            sql += GetSQLStr(d["CCJSSJ"]);
            sql += GetSQLStr(d["CCTS"],1);
            sql += GetSQLStr(d["HJJE"],1);
            sql += GetSQLStr(d["HJDX"]);
            sql += GetSQLStr(d["YJCLF"],1);
            sql += GetSQLStr(d["YTBJE"], 1);
            sql += GetSQLStr(d["REMARK"]);
            sql += GetSQLStr(d["SKRXM"]);
            sql += GetSQLStr(d["userId"]);
            sql += GetSQLStr(DateTime.Now);
            sql += GetSQLStr("0",1);
            sql = sql.TrimEnd(',');
            sql += ")";
            sqllist.Add(sql);
            List<Dictionary<string, object>> diclist = JArray.Parse(d["XCList"].ToString()).ToObject<List<Dictionary<string, object>>>();
            foreach (Dictionary<string, object> dd in diclist)
            {
                if ((dd["CFRQ"] == null || dd["CFRQ"].ToString() == "") && (d["DDRQ"] == null || d["DDRQ"].ToString() == ""))
                {
                    continue;
                }
                else
                {
                    string sql1 = "INSERT INTO jy_clxc (XCID,CLBH,CFRQ,DDRQ,CCDD,CQTS,CQBZ,BZJE,BFB,TS,HCFY,FCLB,FCXB,FCJE,ZFLB,ZFJE,CJR,CJSJ,IS_DELETE,CFDD) VALUES(";
                    sql1 += GetSQLStr(Guid.NewGuid());
                    sql1 += GetSQLStr(d["CLBH"]);
                    sql1 += GetSQLStr(dd["CFRQ"]);
                    sql1 += GetSQLStr(dd["DDRQ"]);
                    sql1 += GetSQLStr(dd["CCDD"]);
                    sql1 += GetSQLStr(dd["CQTS"], 1);
                    sql1 += GetSQLStr(dd["CQBZ"], 1);
                    sql1 += GetSQLStr(dd["BZJE"], 1);
                    sql1 += GetSQLStr(dd["BFB"]);
                    sql1 += GetSQLStr(dd["TS"], 1);
                    sql1 += GetSQLStr(dd["HCFY"], 1);
                    sql1 += GetSQLStr(dd["FCLB"]);
                    sql1 += GetSQLStr(dd["FCXB"]);
                    sql1 += GetSQLStr(dd["FCJE"], 1);
                    sql1 += GetSQLStr(dd["ZFLB"]);
                    sql1 += GetSQLStr(dd["ZFJE"], 1);
                    sql1 += GetSQLStr(d["userId"]);
                    sql1 += GetSQLStr(DateTime.Now);
                    sql1 += GetSQLStr(0, 1);
                    sql1 += GetSQLStr(dd["CFDD"]);
                    sql1 = sql1.TrimEnd(',');
                    sql1 += ")";
                    sqllist.Add(sql1);
                }
            }
            return db.Executs(sqllist);
        }

        public string UpdateInfo(Dictionary<string,object> d)
        {
            string sql = "UPDATE jy_clbx SET DWBM=" + GetSQLStr(d["DWBM"]);
            sql += "DWBM=" + GetSQLStr(d["DWBM"]);
            sql += "CCXM=" + GetSQLStr(d["CCXM"]);
            sql += "CCSY=" + GetSQLStr(d["CCSY"]);
            sql += "CCKSSJ=" + GetSQLStr(d["CCKSSJ"]);
            sql += "CCJSSJ=" + GetSQLStr(d["CCJSSJ"]);
            sql += "CCTS=" + GetSQLStr(d["CCTS"],1);
            sql += "HJJE=" + GetSQLStr(d["HJJE"],1);
            sql += "HJDX=" + GetSQLStr(d["HJDX"]);
            sql += "YJCLF=" + GetSQLStr(d["YJCLF"],1);
            sql += "YTBJE=" + GetSQLStr(d["YTBJE"],1);
            sql += "REMARK=" + GetSQLStr(d["REMARK"]);
            sql += "SKRXM=" + GetSQLStr(d["SKRXM"]);
            sql += "CJR=" + GetSQLStr(d["CJR"]);
            sql += "CJSJ=" + GetSQLStr(d["CJSJ"]);
            sql += "BJR=" + GetSQLStr(d["BJR"]);
            sql += "BJSJ=" + GetSQLStr(d["BJSJ"]);
            sql += "IS_DELETE=" + GetSQLStr(d["IS_DELETE"],1);
            sql = sql.TrimEnd(',');
            sql += "AND CLBH='" + d["CLBH"] + "'";
            return db.ExecutByStringResult(sql);
        }

        public string DeleteInfo(string CLBH)
        {
            string sql = " UPDATE jy_clbx set IS_DELETE=1 where CLBH='" + CLBH + "'";
            return db.ExecutByStringResult(sql);
        }


        public string GetSQLStr(object s, int flag = 0)
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
