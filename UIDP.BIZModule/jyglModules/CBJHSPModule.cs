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

        public Dictionary<string, object> GetInfo(string XMBH, string XMMC, int page, int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetInfo(XMBH, XMMC);
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
    }
}
