using System;
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
            string sql = " SELECT a.*,b.Name as FKFSName FROM jy_fybx a left join tax_dictionary b on a.FKFS=b.Code  WHERE IS_DELETE=0";
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
            sql += " AND CJR='" + d["userid"] + "'";
            return db.GetDataTable(sql);
        }

        public string CreateInfo(Dictionary<string,object> d)
        {
            List<string> sqllist = new List<string>();
            string sql = "INSERT INTO jy_fybx (S_ID,BXDH,DWBM,S_OrgCode,FYXM,XMBH,SQSJ,BXSY,BXJEDX,BXJE,YJKJE,XFKJE,FKFS,FJZS,SKDW,KHH,YHZH,SPZT,CJR,CJSJ,IS_DELETE,PROCESS_STATE)values(";
            sql += GetSQLStr(Guid.NewGuid().ToString());
            sql += GetSQLStr(d["BXDH"]);
            sql += GetSQLStr(d["DWBM"]);
            sql += GetSQLStr(d["S_OrgCode"]);
            sql += GetSQLStr(d["FYXM"]);
            sql += GetSQLStr(d["XMBH"]);
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
            sql += GetSQLStr(0, 1);
            sql = sql.TrimEnd(',');
            sql += ")";
            return db.ExecutByStringResult(sql);
        }

        public string UpdateInfo(Dictionary<string,object> d)
        {
            string sql = " UPDATE jy_fybx SET BXDH=" + GetSQLStr(d["BXDH"]);
            sql += "DWBM=" + GetSQLStr(d["DWBM"]);
            sql += "S_OrgCode=" + GetSQLStr(d["S_OrgCode"]);
            sql += "XMBH=" + GetSQLStr(d["XMBH"]);
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
        /// <summary>
        /// 查询费用审批列表
        /// </summary>
        /// <param name="XMBH"></param>
        /// <param name="XMMC"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public DataTable GetFYSPInfo(string BXDH, string FYXM, string userid)
        {
            string sql = @"SELECT a.*,c.Id,c.FlowId,c.FlowName,c.StepId,c.StepName,c.InstanceId,c.GroupId,c.TaskType,c.Title,c.SenderId,c.SenderName,c.ReceiveTime,c.CompletedTime,c.Status,c.Note,d.TZHJHZJE 
                        FROM jy_fybx a 
                         join RF_FlowTask c on a.BXDH=c.InstanceId and LEFT(a.BXDH,2)='FY'
                        join jy_cbjh d on d.XMBH=a.XMBH
                        WHERE a.IS_DELETE=0 and c.Status IN(0,1) ";
            if (!string.IsNullOrEmpty(userid))
            {
                sql += " AND c.ReceiveId ='" + userid.ToUpper() + "'";
            }
            if (!string.IsNullOrEmpty(BXDH))
            {
                sql += " AND a.BXDH LIKE'" + BXDH + "%'";
            }
            if (!string.IsNullOrEmpty(FYXM))
            {
                sql += " AND a.FYXM='" + FYXM + "'";
            }
            return db.GetDataTable(sql);
        }
    }
}
