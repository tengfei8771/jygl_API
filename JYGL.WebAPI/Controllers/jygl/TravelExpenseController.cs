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
    [Route("TravelExpense")]
    public class TravelExpenseController : WebApiBaseController
    {
        TravelExpenseModule TE = new TravelExpenseModule();
        [HttpGet("GetInfo")]
        public IActionResult GetInfo(string CLBH, int page, int limit,string userid) => Ok(TE.GetInfo(CLBH,page, limit,userid));
        [HttpGet("GetCLXCInfo")]
        public IActionResult GetCLXCInfo(string CLBH) => Ok(TE.GetCLXCInfo(CLBH));
        [HttpPost("CreateInfo")]
        //public IActionResult CreateInfo([FromBody]JObject value,[FromBody]JArray list) => Ok(CB.CreateInfo(value.ToObject<Dictionary<string, object>>(), list.ToObject<List<Dictionary<string, object>>>()));
        public IActionResult CreateInfo([FromBody]JObject value) => Ok(TE.CreateInfo(value.ToObject<Dictionary<string, object>>()));
        [HttpPost("UpdateInfo")]
        public IActionResult UpdateInfo([FromBody]JObject value) => Ok(TE.UpdateInfo(value.ToObject<Dictionary<string, object>>()));
        [HttpGet("DeleteInfo")]
        public IActionResult DeleteInfo(string CLBH) => Ok(TE.DeleteInfo(CLBH));

        [HttpGet("DeleteXCInfo")]
        public IActionResult DeleteXCInfo(string XCID) => Ok(TE.DeleteXCInfo(XCID));
    }
}