using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.ODS.jyglDB;
using UIDP.UTILITY;

namespace UIDP.BIZModule.CangChu
{
    public class CBJHSQModule
    {
        CBJHSQDB db = new CBJHSQDB();

        public Dictionary<string,object> GetInfo(string XMBH, string XMMC, int page, int limit)
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
            catch(Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }


        public Dictionary<string,object> CreateInfo(Dictionary<string,object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                d["XMBH"] = CreateXMBH();
                string b = db.CreateInfo(d);
                if (b == "")
                {
                    r["code"] = 2000;
                    r["message"] = "success";
                }
                else
                {
                    r["code"] = -1;
                    r["message"] = b;
                }
            }
            catch(Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }



        public Dictionary<string, object> UpdateInfo(Dictionary<string, object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.UpdateInfo(d);
                if (b == "")
                {
                    r["code"] = 2000;
                    r["message"] = "success";
                }
                else
                {
                    r["code"] = -1;
                    r["message"] = b;
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }


        public Dictionary<string, object> DeleteInfo(Dictionary<string, object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.DeleteInfo(d);
                if (b == "")
                {
                    r["code"] = 2000;
                    r["message"] = "success";
                }
                else
                {
                    r["code"] = -1;
                    r["message"] = b;
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }

        public string CreateXMBH()
        {
            string timeSpan = DateTime.Now.ToString("yyyyMMdd");
            string RandomStr = "ABCDEFGHIGKLMNOPQRSTUVWXYZ0123456789";
            Random rd = new Random();
            for(int i = 0; i < 3; i++)
            {
                timeSpan += RandomStr[rd.Next(0, RandomStr.Length - 1)];
            }
            return timeSpan;
        }
    }
}
