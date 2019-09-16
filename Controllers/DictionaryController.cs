using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using UIDP.BIZModule;
using UIDP.BIZModule.Modules;

namespace JYGL.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("Dictionary")]
    public class DictionaryController : Controller
    {
        TaxConfigModule TC = new TaxConfigModule();
        [HttpGet("getLeftTree")]
        public dynamic getLeftTree() => Ok(TC.getLeftTree());
        [HttpPost("editNode")]
        public IActionResult editNode([FromBody]JObject value) => Ok(TC.editNode(value.ToObject<Dictionary<string, object>>()));

        [HttpPost("createNode")]
        public IActionResult createNode([FromBody]JObject value) => Ok(TC.createNode(value.ToObject<Dictionary<string, object>>()));

        [HttpPost("delNode")]
        public IActionResult delNode([FromBody]JObject value) => Ok(TC.delNode(value.ToObject<Dictionary<string, object>>()));

        [HttpGet("GetOptions")]
        public IActionResult GetOptions(string ParentCode) => Ok(TC.GetOptions(ParentCode));
    }
}