using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using UIDP.BIZModule.jyglModules;
using WZGX.WebAPI.Controllers;

namespace JYGL.WebAPI.Controllers.jygl
{
    [Produces("application/json")]
    [Route("FYBX")]
    public class FYBXController : WebApiBaseController
    {
        FYBXModule FB = new FYBXModule();
        [HttpGet("GetInfo")]
        public IActionResult GetInfo(string BXDH, string S_BeginDate,string S_EndDate, int page, int limit,string userid)// => Ok(FB.GetInfo(XMBH, XMMC, page, limit));
        {
            Dictionary<string, object> d = new Dictionary<string, object>();
            d["limit"] = limit;
            d["page"] = page;
            d["BXDH"] = BXDH;
            d["S_BeginDate"] = S_BeginDate;
            d["S_EndDate"] = S_EndDate;
            d["userid"] = userid;
            Dictionary<string, object> res = FB.GetInfo(d);
            return Ok(res);
        }
        [HttpPost("CreateInfo")]
        public IActionResult CreateInfo([FromBody]JObject value) => Ok(FB.CreateInfo(value.ToObject<Dictionary<string, object>>()));

        [HttpPost("UpdateInfo")]
        public IActionResult UpdateInfo([FromBody]JObject value) => Ok(FB.UpdateInfo(value.ToObject<Dictionary<string, object>>()));

        [HttpPost("DeleteInfo")]
        public IActionResult DeleteInfo([FromBody]JObject value) => Ok(FB.DeleteInfo(value.ToObject<Dictionary<string, object>>()));
        /// <summary>
        /// 费用审批查询列表
        /// </summary>
        /// <param name="XMBH"></param>
        /// <param name="XMMC"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet("GetFYSPInfo")]
        public IActionResult GetFYSPInfo(string BXDH, string FYXM, int page, int limit, string userid) => Ok(FB.GetFYSPInfo(BXDH, FYXM, page, limit, userid));
    }
}