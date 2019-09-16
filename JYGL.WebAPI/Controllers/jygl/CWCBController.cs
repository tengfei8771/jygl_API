using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UIDP.BIZModule.jyglModules;
using WZGX.WebAPI.Controllers;

namespace JYGL.WebAPI.Controllers.jygl
{
    [Produces("application/json")]
    [Route("CWCB")]
    public class CWCBController : WebApiBaseController
    {
        CWCBModule CWCB = new CWCBModule();
        [HttpGet("GetInfo")]
        public IActionResult GetInfo(string XMBH, string XMMC, int page, int limit) => Ok(CWCB.GetInfo(XMBH, XMMC, page, limit));

        [HttpGet("GetDetailInfo")]
        public IActionResult GetDetailInfo(string XMBH) => Ok(CWCB.GetDetailInfo(XMBH));
    }
}