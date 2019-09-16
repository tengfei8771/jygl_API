using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WZGX.WebAPI.Controllers.jygl
{
    [Produces("application/json")]
    [Route("WorkFlow")]
    public class WorkFlowController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取流程步骤
        /// </summary>
        /// <returns></returns>
        [HttpPost("getStep")]
        public IActionResult getStep()
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            string s = "";
            try
            {
                //string sendUrl = "http://114.115.142.34:8080/RoadFlowCoreApi/FlowTask/ExecuteTask";
                string getStep = "http://114.115.142.34:8080/RoadFlowCoreApi/Flow/GetSendSteps";
                WebRequest req = WebRequest.Create(getStep);
                Dictionary<string, string> postData = new Dictionary<string, string>();
                postData["systemcode"] = "localhost";
                postData["flowid"] = "4447d595-3a2a-4641-8447-c4f012791bae";
                postData["stepid"] = "";
                postData["taskid"] = "";
                postData["userid"] = "EB03262C-AB60-4BC6-A4C0-96E66A4229FE";
                postData["freesend"] = "true";
                string jsonString = JsonConvert.SerializeObject(postData);
                byte[] objectContent = Encoding.UTF8.GetBytes(jsonString);
                req.ContentLength = objectContent.Length;
                req.ContentType = "application/json";
                req.Method = "POST";
                using (var stream = req.GetRequestStream())
                {
                    stream.Write(objectContent, 0, objectContent.Length);
                    stream.Close();
                }
                var resp = req.GetResponse();
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    s = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                r["code"] = 1000;
                r["message"] = ex.Message.ToString();
                return Json(r);
            }
            return Json(s);
        }


        [HttpPost("sendTask")]
        //public IActionResult sendTask([FromBody]JObject value)
        public IActionResult sendTask(string systemcode,string flowid,string taskid,string instanceid,string senderid,string tasktitle,string comment,string type, string steps)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            //Dictionary<string, object> postData = new Dictionary<string, object>();
            Dictionary<string, object> postData = new Dictionary<string, object>();
            //Dictionary<string, object> postData = value.ToObject<Dictionary<string, object>>();
            string s = "";
            try
            {
                string sendUrl = "http://114.115.142.34:8080/RoadFlowCoreApi/FlowTask/ExecuteTask";
               WebRequest req = WebRequest.Create(sendUrl);
                postData["systemcode"] = systemcode;
                postData["flowid"] = flowid;
                postData["taskid"] = taskid;
                postData["instanceid"] = instanceid;
                postData["senderid"] = senderid;
                postData["tasktitle"] = tasktitle;
                postData["comment"] = comment;
                postData["type"] = type;
                postData["steps"] = JsonConvert.DeserializeObject(steps);
                string jsonString = JsonConvert.SerializeObject(postData);
                byte[] objectContent = Encoding.UTF8.GetBytes(jsonString);
                req.ContentLength = objectContent.Length;
                req.ContentType = "application/json";
                req.Method = "POST";
                using (var stream = req.GetRequestStream())
                {
                    stream.Write(objectContent, 0, objectContent.Length);
                    stream.Close();
                }
                var resp = req.GetResponse();
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    s = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                r["code"] = 1000;
                r["message"] = ex.Message.ToString();
                return Json(r);
            }
            return Json(s);
        }
    }
}
