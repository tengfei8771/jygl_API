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

        public DataTable GetInfo(string XMBH, string XMMC,string userid)
        {
            //string sql = " SELECT a.*,b.Name as PC FROM jy_cbjh a left join tax_dictionary b on a.XMPC=b.Code WHERE a.IS_DELETE=0";
            string sql = @"SELECT a.*,b.Name as PC,c.Id,c.FlowId,c.FlowName,c.StepId,c.StepName,c.InstanceId,c.GroupId,c.TaskType,c.Title,c.SenderId,c.SenderName,c.ReceiveTime,c.CompletedTime,c.Status,c.Note 
FROM jy_cbjh a left join tax_dictionary b on a.XMPC=b.Code 
left join RF_FlowTask c on a.XMBH=c.InstanceId and LEFT(a.XMBH,2)='CB'
WHERE a.IS_DELETE=0 and c.Status IN(0,1) ";
            if (!string.IsNullOrEmpty(userid))
            {
                sql += " AND c.ReceiveId ='" + userid.ToUpper() + "'";
            }
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
            string sql = "select * from jy_cbwz where XMBH='" + XMBH + "' AND IS_DELETE=0";
            return db.GetDataTable(sql);
        }
    }
}
