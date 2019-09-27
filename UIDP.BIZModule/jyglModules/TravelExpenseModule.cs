using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.BIZModule.jyglModel;
using UIDP.ODS.jyglDB;
using UIDP.UTILITY;

namespace UIDP.BIZModule.jyglModules
{
    public class TravelExpenseModule
    {
        TravelExpenseDB db = new TravelExpenseDB();
        public Dictionary<string, object> GetInfo(string CLBH,int page,int limit,string userid)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetInfo(CLBH,userid);
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

        public Dictionary<string, object> GetCLXCInfo(string CLBH)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataSet ds = db.GetCLXCInfo(CLBH);
                CLBXModel model = GetUpdateModel(ds.Tables[0], ds.Tables[1]);
                r["code"] = 2000;
                r["items"] = model;
                r["message"] = "success";
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }

        public CLBXModel GetUpdateModel(DataTable dt,DataTable dt1)
        {
            CLBXModel model = new CLBXModel();
            foreach (DataRow dr in dt.Rows)
            {
                if (!IsNull(dr["CLBH"]))
                {
                    model.CLBH = dr["CLBH"].ToString();
                }
                if (!IsNull(dr["DWBM"]))
                {
                    model.DWBM = dr["DWBM"].ToString();
                }
                if (!IsNull(dr["CCXM"] ))
                {
                    model.CCXM = dr["CCXM"].ToString();
                }
                if (!IsNull(dr["CCSY"]))
                {
                    model.CCSY = dr["CCSY"].ToString();
                }
                if (!IsNull(dr["CCKSSJ"]))
                {
                    model.CCKSSJ = Convert.ToDateTime(dr["CCKSSJ"]);
                }
                if (!IsNull(dr["CCJSSJ"]))
                {
                    model.CCJSSJ = Convert.ToDateTime(dr["CCJSSJ"]);
                }
                if (!IsNull(dr["CCTS"]))
                {
                    model.CCTS = Convert.ToDecimal(dr["CCTS"]);
                }
                if (!IsNull(dr["HJJE"]))
                {
                    model.HJJE = Convert.ToDecimal(dr["HJJE"]);
                }
                if (!IsNull(dr["HJDX"]))
                {
                    model.HJDX = dr["HJDX"].ToString();
                } 
                
                if (!IsNull(dr["YJCLF"]))
                {
                    model.YJCLF = Convert.ToDecimal(dr["YJCLF"]);
                }
                if (!IsNull(dr["YTBJE"]))
                {
                    model.YTBJE = Convert.ToDecimal(dr["YTBJE"]);
                }
                if (!IsNull(dr["REMARK"]))
                {
                    model.REMARK = dr["REMARK"].ToString();
                }
                if (!IsNull(dr["SKRXM"]))
                {
                    model.SKRXM = dr["SKRXM"].ToString();
                }
                if (!IsNull(dr["CJR"]))
                {
                    model.CJR = dr["CJR"].ToString();
                }
                if (!IsNull(dr["CJSJ"]))
                {
                    model.CJSJ = Convert.ToDateTime(dr["CJSJ"]);
                }
                if (!IsNull(dr["BJR"]))
                {
                    model.BJR = dr["BJR"].ToString();
                }
                
                if (!IsNull(dr["BJSJ"]))
                {
                    model.BJSJ = Convert.ToDateTime(dr["BJSJ"]);
                }
                if (!IsNull(dr["IS_DELETE"]))
                {
                    model.IS_DELETE = Convert.ToInt32(dr["IS_DELETE"]);
                }
                if (!IsNull(dr["XMBH"]))
                {
                    model.XMBH = dr["XMBH"].ToString();
                }
                if (!IsNull(dr["XMMC"]))
                {
                    model.XMMC = dr["XMMC"].ToString();
                }
                if (!IsNull(dr["DWMC"]))
                {
                    model.DWMC = dr["DWMC"].ToString();
                }

                model.XCList = new List<CLXCModel>();
                GetCLXCList(model, dt1);
            }
            return model;
        }

        public void GetCLXCList(CLBXModel model,DataTable dt)
        {
            foreach(DataRow dr in dt.Rows)
            {
                CLXCModel XVXC = new CLXCModel();  
                if (!IsNull(dr["XCID"]))
                {
                    XVXC.XCID = dr["XCID"].ToString();
                }
                if (!IsNull(dr["CLBH"]))
                {
                    XVXC.CLBH = dr["CLBH"].ToString();
                }
                if (!IsNull(dr["CFRQ"]))
                {
                    XVXC.CFRQ = Convert.ToDateTime(dr["CFRQ"]);
                }
                if (!IsNull(dr["DDRQ"]))
                {
                    XVXC.DDRQ = Convert.ToDateTime(dr["DDRQ"]);
                }
                if (!IsNull(dr["CCDD"]))
                {
                    XVXC.CCDD = dr["CCDD"].ToString();
                }
                
                if (!IsNull(dr["CQTS"]))
                {
                    XVXC.CQTS = Convert.ToDecimal(dr["CQTS"]);
                }
                if (!IsNull(dr["CQBZ"]))
                {
                    XVXC.CQTS = Convert.ToDecimal(dr["CQBZ"]);
                }
                if (!IsNull(dr["BZJE"]))
                {
                    XVXC.BZJE = Convert.ToDecimal(dr["BZJE"]);
                }
                if (!IsNull(dr["BFB"]))
                {
                    XVXC.BFB = dr["BFB"].ToString();
                }
  
                if (!IsNull(dr["TS"]))
                {
                    XVXC.TS = Convert.ToInt32(dr["TS"]);
                }
                if (!IsNull(dr["HCFY"]))
                {
                    XVXC.HCFY = Convert.ToDecimal(dr["HCFY"]);
                }
                if (!IsNull(dr["HCFY"]))
                {
                    XVXC.HCFY = Convert.ToDecimal(dr["HCFY"]);
                }
                if (!IsNull(dr["FCLB"]))
                {
                    XVXC.FCLB = dr["FCLB"].ToString();
                }
                if (!IsNull(dr["FCXB"]))
                {
                    XVXC.FCXB = dr["FCXB"].ToString();
                }              
                if (!IsNull(dr["FCJE"]))
                {
                    XVXC.FCJE = Convert.ToDecimal(dr["FCJE"]);
                }
                if (!IsNull(dr["ZFLB"]))
                {
                    XVXC.ZFLB = dr["ZFLB"].ToString();
                }
                if (!IsNull(dr["ZFJE"]))
                {
                    XVXC.ZFJE = Convert.ToDecimal(dr["ZFJE"]);
                }
                if (!IsNull(dr["CJR"]))
                {
                    XVXC.CJR = dr["CJR"].ToString();
                }
                if (!IsNull(dr["CJSJ"]))
                {
                    XVXC.CJSJ = Convert.ToDateTime(dr["CJSJ"]);
                }
                if (!IsNull(dr["BJR"]))
                {
                    XVXC.BJR = dr["BJR"].ToString();
                }
                if (!IsNull(dr["BJSJ"]))
                {
                    XVXC.BJSJ = Convert.ToDateTime(dr["BJSJ"]);
                }
                if (!IsNull(dr["IS_DELETE"]))
                {
                    XVXC.IS_DELETE = Convert.ToInt32(dr["IS_DELETE"]);
                }
                if (!IsNull(dr["CFDD"]))
                {
                    XVXC.CFDD = dr["CFDD"].ToString();
                }             
                model.XCList.Add(XVXC);
            }        
        }


        public Dictionary<string, object> CreateInfo(Dictionary<string,object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            d.Add("CLBH", CreateXMBH());
            //if (d.ContainsValue("")||d.ContainsValue(null))
            //{
            //    r["code"] = -1;
            //    r["message"] = "您的表单有未填项！";
            //    return r;
            //}
            try
            {
                string b = db.CreateInfo(d);
                if (b == "")
                {
                    r["code"] = 2000;
                    r["message"] = "success";
                    r["CLBH"] = d["CLBH"];
                    //r["XMBH"] = d["XMBH"];
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


        public Dictionary<string, object> DeleteInfo(string CLBH)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.DeleteInfo(CLBH);
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

        public Dictionary<string, object> DeleteXCInfo(string XCID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.DeleteXCInfo(XCID);
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
        public Dictionary<string, object> GetSPInfo(string CLBH, int page, int limit, string userid)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetSPInfo(CLBH, userid);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                    r["total"] = dt.Rows.Count;

                }
                else
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
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
        public Dictionary<string, object> GetYBInfo(string CLBH, int page, int limit, string userid)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetYBInfo(CLBH, userid);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                    r["total"] = dt.Rows.Count;

                }
                else
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
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
        public Dictionary<string, object> GetSPXCInfo(string CLBH)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetSPXCInfo(CLBH);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
                    r["items"] = dt;
                    r["total"] = dt.Rows.Count;

                }
                else
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
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


        public string CreateXMBH()
        {
            string timeSpan = "CL"+ DateTime.Now.ToString("yyyyMMddhhmmss");
            string RandomStr = "ABCDEFGHIGKLMNOPQRSTUVWXYZ0123456789";
            Random rd = new Random();
            for (int i = 0; i < 4; i++)
            {
                timeSpan += RandomStr[rd.Next(0, RandomStr.Length - 1)];
            }
            return timeSpan;
        }

        public bool IsNull(object t)
        {
            if (t == null || t.ToString() == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
