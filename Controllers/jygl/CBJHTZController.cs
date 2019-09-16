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
    [Route("CBJHTZ")]
    public class CBJHTZController : WebApiBaseController
    {
        CBJHTZModule FB = new CBJHTZModule();
        [HttpGet("GetInfo")]
        public IActionResult GetInfo(string XMCODE, string XMMC, int page, int limit)// => Ok(FB.GetInfo(XMBH, XMMC, page, limit));
        {
            Dictionary<string, object> d = new Dictionary<string, object>();
            d["limit"] = limit;
            d["page"] = page;
            d["XMCODE"] = XMCODE;
            d["XMMC"] = XMMC;
            Dictionary<string, object> res = FB.GetInfo(d);
            return Ok(res);
        }
        [HttpPost("CreateInfo")]
        public IActionResult CreateInfo([FromBody]JObject value) => Ok(FB.CreateInfo(value.ToObject<Dictionary<string, object>>()));

        [HttpPost("UpdateInfo")]
        public IActionResult UpdateInfo([FromBody]JObject value) => Ok(FB.UpdateInfo(value.ToObject<Dictionary<string, object>>()));

        //[HttpPost("DeleteInfo")]
        //public IActionResult DeleteInfo([FromBody]JObject value) => Ok(FB.DeleteInfo(value.ToObject<Dictionary<string, object>>()));

    }
}