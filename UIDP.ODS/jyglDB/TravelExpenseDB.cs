using Newtonsoft.Json.Linq;
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
        public DataTable GetInfo(string CLBH,string userid)
        {
            string sql = " select a.*,c.SFCW,c.TZHJHZJE,c.YBXJE from jy_clbx a join jy_cbjh c on a.XMBH=c.XMBH  where a.IS_DELETE=0";
            if (!String.IsNullOrEmpty(CLBH))
            {
                sql += " and a.CLBH='" + CLBH + "'";
            }
            sql += " and a.CJR='" + userid + "'";
            return db.GetDataTable(sql);
        }


        public DataSet GetCLXCInfo(string CLBH)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string sql = "SELECT * from jy_clbx where IS_DELETE=0 AND CLBH='" + CLBH + "'";
            string sql1 = " SELECT * from jy_clxc where IS_DELETE=0 AND CLBH='" + CLBH + "'";
            dic.Add("XLBX", sql);
            dic.Add("CLXC", sql1);
            return db.GetDataSet(dic);
        }

        public string CreateInfo(Dictionary<string,object> d)
        {
            List<string> sqllist = new List<string>();
            string sql = " INSERT INTO jy_clbx (CLBH,DWBM,CCXM,CCSY,CCKSSJ,CCJSSJ,CCTS,HJJE,HJDX,YJCLF,YTBJE,REMARK,SKRXM,CJR,CJSJ,IS_DELETE,XMBH,XMMC,DWMC,PROCESS_STATE) VALUES (";
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
            sql += GetSQLStr(d["XMBH"]);
            sql += GetSQLStr(d["XMMC"]);
            sql += GetSQLStr(d["DWMC"]);
            sql += GetSQLStr(0, 1);
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
            List<string> sqllist = new List<string>();
            string sql = "UPDATE jy_clbx SET DWBM=" + GetSQLStr(d["DWBM"]);
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
            sql += "BJR=" + GetSQLStr(d["userId"]);
            sql += "BJSJ=" + GetSQLStr(DateTime.Now);
            sql += "IS_DELETE=" + GetSQLStr(d["IS_DELETE"],1);
            sql += "XMBH=" + GetSQLStr(d["XMBH"]);
            sql += "XMMC=" + GetSQLStr(d["XMMC"]);
            sql += "DWMC=" + GetSQLStr(d["DWMC"]);
            sql = sql.TrimEnd(',');
            sql += " WHERE CLBH='" + d["CLBH"] + "'";
            sqllist.Add(sql);
            List<Dictionary<string, object>> diclist = JArray.Parse(d["XCList"].ToString()).ToObject<List<Dictionary<string, object>>>();
            foreach(Dictionary<string,object> dd in diclist)
            {
                if (!dd.ContainsKey("XCID"))
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
                else
                {
                    string sql2 = "UPDATE jy_clxc SET CFRQ=" + GetSQLStr(dd["CFRQ"]);
                    sql2 += "DDRQ=" + GetSQLStr(dd["DDRQ"]);
                    sql2 += "CCDD=" + GetSQLStr(dd["CCDD"]);
                    sql2 += "CQTS=" + GetSQLStr(dd["CQTS"],1);
                    sql2 += "CQBZ=" + GetSQLStr(dd["CQBZ"],1);
                    sql2 += "BZJE=" + GetSQLStr(dd["BZJE"],1);
                    sql2 += "BFB=" + GetSQLStr(dd["BFB"]);
                    sql2 += "TS=" + GetSQLStr(dd["TS"],1);
                    sql2 += "HCFY=" + GetSQLStr(dd["HCFY"],1);
                    sql2 += "FCLB=" + GetSQLStr(dd["FCLB"]);
                    sql2 += "FCXB=" + GetSQLStr(dd["FCXB"]);
                    sql2 += "FCJE=" + GetSQLStr(dd["FCJE"],1);
                    sql2 += "ZFLB=" + GetSQLStr(dd["ZFLB"]);
                    sql2 += "ZFJE=" + GetSQLStr(dd["ZFJE"],1);
                    sql2 += "BJR=" + GetSQLStr(d["userId"]);
                    sql2 += "BJSJ="+ GetSQLStr(DateTime.Now);
                    sql2 += "CFDD=" + GetSQLStr(dd["CFDD"]);
                    sql2 = sql2.TrimEnd(',');
                    sql2 += " WHERE XCID='" + dd["XCID"] + "'";
                    sqllist.Add(sql2);
                }
            }
            return db.Executs(sqllist);
        }



        public string DeleteInfo(string CLBH)
        {
            string sql = " UPDATE jy_clbx set IS_DELETE=1 where CLBH='" + CLBH + "'";
            return db.ExecutByStringResult(sql);
        }

        public string DeleteXCInfo(string XCID)
        {
            string sql = " UPDATE jy_clxc set IS_DELETE=1 WHERE XCID='" + XCID + "'";
            return db.ExecutByStringResult(sql);
        }

        public DataTable GetSPInfo(string CLBH, string userid)
        {
            string sql = "select a.*,b.Id,b.FlowId,b.FlowName,b.StepId,b.StepName,b.InstanceId,b.GroupId,b.TaskType,b.Title,b.SenderId,b.SenderName,b.ReceiveTime," +
                "b.CompletedTime,b.Status,b.Note, d.TZHJHZJE,d.YBXJE,d.SFCW from jy_clbx a left join RF_FlowTask b on a.CLBH=b.InstanceId join jy_cbjh d on d.XMBH=a.XMBH where 1=1 and a.IS_DELETE=0 AND b.ReceiveId='{0}'" +
                "and left(b.InstanceId,2)='CL'";
            sql = string.Format(sql, userid.ToUpper());
            if (!String.IsNullOrEmpty(CLBH))
            {
                sql += " AND a.CLBH='" + CLBH + "'";
            }
            sql += " AND b.Status IN(0,1)";
            return db.GetDataTable(sql);
        }
        public DataTable GetYBInfo(string CLBH, string userid)
        {
            string sql = "select a.*,b.Id,b.FlowId,b.FlowName,b.StepId,b.StepName,b.InstanceId,b.GroupId,b.TaskType,b.Title,b.SenderId,b.SenderName,b.ReceiveTime," +
                "b.CompletedTime,b.Status,b.Note, d.TZHJHZJE,d.YBXJE,d.SFCW from jy_clbx a left join RF_FlowTask b on a.CLBH=b.InstanceId join jy_cbjh d on d.XMBH=a.XMBH where 1=1 and a.IS_DELETE=0 AND b.ReceiveId='{0}'" +
                "and left(b.InstanceId,2)='CL'";
            sql = string.Format(sql, userid.ToUpper());
            if (!String.IsNullOrEmpty(CLBH))
            {
                sql += " AND a.CLBH='" + CLBH + "'";
            }
            sql += " AND b.ExecuteType>1 ";
            return db.GetDataTable(sql);
        }
        public DataTable GetSPXCInfo(string CLBH)
        {
            string sql = " SELECT * from jy_clxc where IS_DELETE=0 AND CLBH='" + CLBH + "'";
            return db.GetDataTable(sql);
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
