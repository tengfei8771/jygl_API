using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UIDP.BIZModule.Modules;
using UIDP.ODS.jyglDB;
using UIDP.UTILITY;

namespace UIDP.BIZModule.jyglModules
{
    public class CBJHTZModule
    {
        CBJHTZDB db = new CBJHTZDB();

        public Dictionary<string,object> GetInfo(Dictionary<string, object> d)
        {
            //Dictionary<string, object> r = new Dictionary<string, object>();
            //try
            //{
            //    DataTable dt = db.GetInfo(XMBH, XMMC);
            //    if (dt.Rows.Count > 0)
            //    {
            //        r["code"] = 2000;
            //        r["message"] = "success";
            //        r["items"] = KVTool.GetPagedTable(dt, page, limit);
            //        r["total"] = dt.Rows.Count;
            //    }
            //    else
            //    {
            //        r["code"] = 2000;
            //        r["message"] = "success,but no info ";
            //        r["total"] = dt.Rows.Count;
            //    }
            //}
            //catch(Exception e)
            //{
            //    r["code"] = -1;
            //    r["message"] = e.Message;
            //}
            //return r;
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                int limit = d["limit"] == null ? 100 : int.Parse(d["limit"].ToString());
                int page = d["page"] == null ? 1 : int.Parse(d["page"].ToString());

                DataTable dt = db.GetInfo(d);
                r["total"] = dt.Rows.Count;
                r["items"] = KVTool.RowsToListDic(dt, d);
                r["code"] = 2000;
                r["message"] = "查询成功";
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }


        public Dictionary<string,object> CreateInfo(Dictionary<string, object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
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
            catch (Exception e)
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


        //public Dictionary<string, object> DeleteInfo(Dictionary<string, object> d)
        //{
        //    Dictionary<string, object> r = new Dictionary<string, object>();
        //    try
        //    {
        //        string b = db.DeleteInfo(d);
        //        if (b == "")
        //        {
        //            r["code"] = 2000;
        //            r["message"] = "success";
        //        }
        //        else
        //        {
        //            r["code"] = -1;
        //            r["message"] = b;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        r["code"] = -1;
        //        r["message"] = e.Message;
        //    }
        //    return r;
        //}

    }
}
