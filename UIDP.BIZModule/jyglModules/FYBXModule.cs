﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.ODS.jyglDB;
using UIDP.UTILITY;

namespace UIDP.BIZModule.jyglModules
{
    public class FYBXModule
    {
        FYBXDB db = new FYBXDB();

        public Dictionary<string,object> GetInfo(Dictionary<string, object> d)
        {
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
            catch(Exception e)
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
                string BXDH = CreateBXDH();
                d["BXDH"] = BXDH;
                string b = db.CreateInfo(d);
                if (b == "")
                {
                    r["code"] = 2000;
                    r["message"] = "success";
                    r["BXDH"] = BXDH;
                    r["BXJE"] = d["BXJE"];
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

      
        public string CreateBXDH()
        {
            string timeSpan = "FY"+DateTime.Now.ToString("yyyyMMddhhmmss");
            string RandomStr = "ABCDEFGHIGKLMNOPQRSTUVWXYZ0123456789";
            Random rd = new Random();
            for(int i = 0; i < 4; i++)
            {
                timeSpan += RandomStr[rd.Next(0, RandomStr.Length - 1)];
            }
            return timeSpan;
        }
        /// <summary>
        /// 查询费用审批列表
        /// </summary>
        /// <param name="XMBH"></param>
        /// <param name="XMMC"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetFYSPInfo(string BXDH, string FYXM, int page, int limit, string userid)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetFYSPInfo(BXDH, FYXM, userid);
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
        /// <summary>
        /// 查询费用已办列表
        /// </summary>
        /// <param name="XMBH"></param>
        /// <param name="XMMC"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetFYYBInfo(string BXDH, string FYXM, int page, int limit, string userid)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetFYYBInfo(BXDH, FYXM, userid);
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
