using Newtonsoft.Json.Linq;
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
    public class CBJHSQModule
    {
        CBJHSQDB db = new CBJHSQDB();

        public Dictionary<string,object> GetInfo(string XMBH, string XMMC, int page, int limit,string userid,int type)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetInfo(XMBH, XMMC, userid,type);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "操作成功！";
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                    r["total"] = dt.Rows.Count;
                }
                else
                {
                    r["code"] = 2000;
                    r["message"] = "暂无数据！";
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
        public Dictionary<string, object> GetYBInfo(string XMBH, string XMMC, int page, int limit, string userid)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetYBInfo(XMBH, XMMC, userid);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "操作成功！";
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                    r["total"] = dt.Rows.Count;
                }
                else
                {
                    r["code"] = 2000;
                    r["message"] = "暂无数据！";
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
        
        public Dictionary<string,object> CreateInfo(List<Dictionary<string,object>> list)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string XMBH = CreateXMBH();
                Dictionary<string, object> d = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                if (!d.Keys.Contains("WZJHJE") || d["WZJHJE"] == null || d["WZJHJE"].ToString() == "")
                {
                    d["WZJHJE"] = "0";
                }
                string b = db.CreateInfo(d,list,XMBH);
                if (b == "")
                {
                    r["code"] = 2000;
                    r["message"] = "操作成功！";
                    r["XMBH"] = XMBH;
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



        public Dictionary<string, object> UpdateInfo(List<Dictionary<string, object>> list)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                Dictionary<string, object> d = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                string b = db.UpdateInfo(d,list);
                if (b == "")
                {
                    r["code"] = 2000;
                    r["message"] = "操作成功！";
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
                    r["message"] = "操作成功！";
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

        public Dictionary<string, object> GetDetailInfo(string XMBH)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetDetailInfo(XMBH);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "操作成功！";
                    r["items"] = dt;
                    r["total"] = dt.Rows.Count;
                }
                else
                {
                    r["code"] = 2000;
                    r["message"] = "暂无数据！ ";
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

        public Dictionary<string, object> DeleteDetailInfo(string WZID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.DeleteDetailInfo(WZID);
                if (b == "")
                {
                    r["code"] = 2000;
                    r["message"] = "操作成功！";
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

        public Dictionary<string,object> GetTreeOptions(string code)
        {
            Dictionary<string, object> d = new Dictionary<string, object>();
            try
            {
                List<ConfigNode> list = CreateNode(code);
                d["code"] = 2000;
                d["items"] = list;
                d["message"] = "操作成功！";
            }
            catch(Exception e)
            {
                d["message"] = e.Message;
                d["code"] = -1;
            }
            return d;
        }

        public Dictionary<string,object> GetYearProject(string userid)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetYearProject(userid);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "操作成功！";
                    r["items"] = dt;
                    r["total"] = dt.Rows.Count;
                }
                else
                {
                    r["code"] = 2000;
                    r["message"] = "暂无数据！";
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

        public List<ConfigNode> CreateNode(string code)
        {
            DataTable dt = db.GetNodedt();
            List<ConfigNode> list = new List<ConfigNode>();
            foreach(DataRow dr in dt.Select("Code='"+code+"'"))
            {
                ConfigNode node = new ConfigNode();
                node.S_Id = dr["S_Id"].ToString();
                node.ParentCode = dr["ParentCode"].ToString();
                node.Code = dr["Code"].ToString();
                node.EnglishCode = dr["EnglishCode"].ToString();
                node.Name = dr["Name"].ToString();
                node.SortNo = dr["SortNo"].ToString();
                node.children = new List<ConfigNode>();
                CreateChildrenNode(node, dt);
                node.children = node.children.OrderBy(t => t.SortNo).ToList();
                list.Add(node);
            }
            return list;
        }

        public void CreateChildrenNode(ConfigNode node, DataTable dt)
        {
            foreach(DataRow dr in dt.Select("ParentCode='"+node.Code+"'"))
            {
                ConfigNode childrenNode = new ConfigNode();
                childrenNode.S_Id = dr["S_Id"].ToString();
                childrenNode.S_Id = dr["S_Id"].ToString();
                childrenNode.ParentCode = dr["ParentCode"].ToString();
                childrenNode.Code = dr["Code"].ToString();
                childrenNode.EnglishCode = dr["EnglishCode"].ToString();
                childrenNode.Name = dr["Name"].ToString();
                childrenNode.SortNo = dr["SortNo"].ToString();
                childrenNode.children = new List<ConfigNode>();
                CreateChildrenNode(childrenNode, dt);
                childrenNode.children = childrenNode.children.OrderBy(t => t.SortNo).ToList();
                node.children.Add(childrenNode);
            }
        }

        public Dictionary<string,object> UpdateAddCBJHJE(JObject obj)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            string id = obj.Value<string>("XMBH");
            decimal JE = JE = obj.Value<decimal>("BXJE");
            try
            {
                string b = db.UpdateAddCBJHJE(id,JE);
                if (b == "")
                {
                    res["code"] = 2000;
                    res["message"] = "操作成功！";
                }
                else
                {
                    res["code"] = -1;
                    res["message"] = "操作失败！";
                }
            }
            catch(Exception e)
            {
                res["message"] = e.Message;
                res["code"] = -1;
            }
            return res;
        }


        public Dictionary<string, object> UpdateDesCBJHJE(JObject obj)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            string id = obj.Value<string>("XMBH");
            decimal JE = JE = obj.Value<decimal>("BXJE");
            try
            {
                string b = db.UpdateDesCBJHJE(id, JE);
                if (b == "")
                {
                    res["code"] = 2000;
                    res["message"] = "操作成功！";
                }
                else
                {
                    res["code"] = -1;
                    res["message"] = "操作失败！";
                }
            }
            catch (Exception e)
            {
                res["message"] = e.Message;
                res["code"] = -1;
            }
            return res;
        }





        public string CreateXMBH()
        {
            string timeSpan = "CB"+DateTime.Now.ToString("yyyyMMddhhmmss");
            string RandomStr = "ABCDEFGHIGKLMNOPQRSTUVWXYZ0123456789";
            Random rd = new Random();
            for(int i = 0; i < 4; i++)
            {
                timeSpan += RandomStr[rd.Next(0, RandomStr.Length - 1)];
            }
            return timeSpan;
        }
    }
}
