using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.ODS.jyglDB;
using UIDP.UTILITY;

namespace UIDP.BIZModule.jyglModules
{
    public class CBJHSPModule
    {

        CBJHSPDB db = new CBJHSPDB();

        public Dictionary<string, object> GetInfo(string XMBH, string XMMC, int page, int limit,string userid)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetInfo(XMBH, XMMC, userid);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "success";
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                    r["total"] = dt.Rows.Count;
                }
                else
                {
                    r["code"] = 2000;
                    r["message"] = "success,but no info ";
                    r["total"] = dt.Rows.Count;
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }

        public Dictionary<string, object> GetDetailInfo(string XMBH)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetDetailInfo(XMBH);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "success";
                    r["items"] = dt;
                    r["total"] = dt.Rows.Count;
                }
                else
                {
                    r["code"] = 2000;
                    r["message"] = "success,but no info ";
                    r["total"] = dt.Rows.Count;
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }

        public Dictionary<string, object> UpdateSFCW(string XMBH, string sfcw)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string result = db.UpdateSFCW(XMBH, sfcw);
                if (result == "")
                {
                    r["code"] = 2000;
                    r["message"] = "更新数据成功！";
               
                }
                else
                {
                    r["code"] = -1;
                    r["message"] = "更新数据失败！";
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }
    }
}
