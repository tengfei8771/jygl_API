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

        public DataTable GetInfo(string XMBH,string XMMC,string userid,int type)
        {
            string sql = " SELECT a.*,b.Name as PC ,c.Name as LB,d.ORG_NAME CBDWMC FROM jy_cbjh a left join tax_dictionary b on a.XMPC=b.Code left join tax_dictionary c on c.Code=a.XMLB left join ts_uidp_org d on a.CBDW=d.ORG_CODE WHERE a.IS_DELETE=0 ";
            if (!string.IsNullOrEmpty(XMBH))
            {
                sql += " AND a.XMCODE LIKE'" + XMBH + "%'";
            } 
            if (!string.IsNullOrEmpty(XMMC))
            {
                sql += " AND a.XMMC='" + XMMC + "'";
            }
            if (type == 0)
            {
                sql += " AND CJR='" + userid + "'";
            }
            //sql += " LIMIT " + (page - 1) * limit + "," + limit;
            ///待添加功能，只有流程审批完成后才能选择项目，sql+=" PROCESS_STATE=2"
            return db.GetDataTable(sql);
        }

        public string CreateInfo(Dictionary<string,object> d,List<Dictionary<string,object>> list,string XMBH)
        {
            List<string> sqllist = new List<string>();
            string sql = "INSERT INTO jy_cbjh (XMBH,XMMC,CBDW,JSNR,JHZJE,TZHJHZJE,LSJE,BNJE,WLJE,XMPC,CJR,CJSJ,IS_DELETE,CZWZ,WZJHJE,XMLB,HASINCOME,XMCODE,JHND,PROCESS_STATE)values(";
            sql += GetSQLStr(XMBH);
            sql += GetSQLStr(d["XMMC"]);
            sql += GetSQLStr(d["CBDW"]);
            sql += GetSQLStr(d["JSNR"]);
            sql += GetSQLStr(d["JHZJE"],1);
            sql += GetSQLStr(d["JHZJE"], 1);
            sql += GetSQLStr(d["LSJE"], 1);
            sql += GetSQLStr(d["BNJE"], 1);
            sql += GetSQLStr(d["WLJE"], 1);
            sql += GetSQLStr(d["XMPC"]);
            sql += GetSQLStr(d["CJR"]);
            sql += GetSQLStr(DateTime.Now);
            sql += GetSQLStr(0,1);
            sql += GetSQLStr(d["CZWZ"],1);
            sql += GetSQLStr(d["WZJHJE"], 1);
            //sql += GetSQLStr(d["SFCW"], 1);
            sql += GetSQLStr(d["XMLB"]);
            sql += GetSQLStr(d["HASINCOME"],1);
            sql += GetSQLStr(d["XMCODE"]);
            sql += GetSQLStr(d["JHND"]);
            sql += GetSQLStr(0,1);
            sql = sql.TrimEnd(',');
            sql += ")";
            sqllist.Add(sql);
            int i;
            if(int.TryParse(d["CZWZ"].ToString(),out i))
            {
                if (i == 1)
                {
                    foreach (Dictionary<string, object> dic in list)
                    {
                        string sql1 = "INSERT INTO jy_cbwz (WZID,XMBH,WZMC,WZSL,WZLX,WZSM,IS_DELETE) VALUES(";
                        sql1 += GetSQLStr(Guid.NewGuid());
                        sql1 += GetSQLStr(XMBH);
                        sql1 += GetSQLStr(dic["WZMC"]);
                        sql1 += GetSQLStr(dic["WZSL"], 1);
                        sql1 += GetSQLStr(dic["WZLX"]);
                        sql1 += GetSQLStr(dic["WZSM"]);
                        sql1 += GetSQLStr(0, 1);
                        sql1 = sql1.TrimEnd(',');
                        sql1 += ")";
                        sqllist.Add(sql1);                       
                    }
                }
            }            
            return db.Executs(sqllist);
        }

        public string UpdateInfo(Dictionary<string,object> d,List<Dictionary<string,object>> list)
        {
            List<string> sqllist = new List<string>();
            string sql = " UPDATE jy_cbjh SET XMMC=" + GetSQLStr(d["XMMC"]);
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
            sql += "WZJHJE=" + GetSQLStr(d["WZJHJE"], 1);
            sql += "SFCW=" + GetSQLStr(d["SFCW"],1);
            sql += " XMLB=" + GetSQLStr(d["XMLB"]);
            sql += "HASINCOME=" + GetSQLStr(d["HASINCOME"], 1);
            sql += " XMCODE=" + GetSQLStr(d["XMCODE"]);
            sql += " JHND=" + GetSQLStr(d["JHND"]);
            sql = sql.TrimEnd(',');
            sql += " WHERE XMBH=" + GetSQLStr(d["XMBH"]);
            sql = sql.TrimEnd(',');
            sqllist.Add(sql);
            foreach(Dictionary<string,object> dic in list)
            {
                if (!dic.ContainsKey("WZID"))
                {
                    string sql1 = "INSERT INTO jy_cbwz (WZID,XMBH,WZMC,WZSL,WZLX,WZSM,IS_DELETE) VALUES(";
                    sql1 += GetSQLStr(Guid.NewGuid());
                    sql1 += GetSQLStr(d["XMBH"]);
                    sql1 += GetSQLStr(dic["WZMC"]);
                    sql1 += GetSQLStr(dic["WZSL"], 1);
                    sql1 += GetSQLStr(dic["WZLX"]);
                    sql1 += GetSQLStr(dic["WZSM"]);
                    sql1 += GetSQLStr(0, 1);
                    sql1 = sql1.TrimEnd(',');
                    sql1 += ")";
                    sqllist.Add(sql1);
                }
                else
                {
                    string sql2 = "UPDATE jy_cbwz set";
                    sql2 += " WZMC=" + GetSQLStr(dic["WZMC"]);
                    sql2 += " WZSL=" + GetSQLStr(dic["WZSL"], 1);
                    sql2 += " WZLX=" + GetSQLStr(dic["WZLX"]);
                    sql2 += " WZSM=" + GetSQLStr(dic["WZSM"]);
                    sql2 = sql2.TrimEnd(',');
                    sql2 += " WHERE WZID='" + dic["WZID"] + "'";
                    sqllist.Add(sql2);
                }
            }
            return db.Executs(sqllist);
        }

        public string DeleteInfo(Dictionary<string,object> d)
        {
            string sql = "UPDATE jy_cbjh set IS_DELETE=1 WHERE XMBH=" + GetSQLStr(d["XMBH"]);
            sql = sql.TrimEnd(',');
            return db.ExecutByStringResult(sql);
        }

        public DataTable GetDetailInfo(string XMBH)
        {
            string sql = "select * from jy_cbwz where XMBH='" + XMBH + "' AND IS_DELETE=0";
            return db.GetDataTable(sql);
        }

        public string DeleteDetailInfo(string WZID)
        {
            string sql = "UPDATE jy_cbwz set IS_DELETE=1 WHERE WZID='" + WZID + "'";
            return db.ExecutByStringResult(sql);
        }

        public DataTable GetNodedt()
        {
            string sql = "select * from tax_dictionary";
            return db.GetDataTable(sql);

        }

        public DataTable GetYearProject()
        {
            string sql = "select XMMC,XMCODE from jy_cbjh where YEAR(JHND)='" + DateTime.Now.Year + "'";
            return db.GetDataTable(sql);
        }

        //根据报销金额更新

        public string UpdateAddCBJHJE(string id,decimal BXJE)
        {
            string sql = "UPDATE jy_cbjh set YBXJE=CASE WHEN YBXJE IS NULL THEN " + BXJE + " ELSE YBXJE+" + BXJE + " END WHERE XMBH='" + id + "'";
            return db.ExecutByStringResult(sql);
        }

        public string UpdateDesCBJHJE(string id, decimal BXJE)
        {
            string sql = "UPDATE jy_cbjh set YBXJE=CASE WHEN YBXJE IS NULL THEN " + BXJE + " ELSE YBXJE-" + BXJE + " END WHERE XMBH='" + id + "'";
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
