﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UIDP.BIZModule.jyglModules;

namespace JYGL.WebAPI.Controllers.jygl
{
    [Produces("application/json")]
    [Route("CBWZSP")]
    public class CBWZSPController : Controller
    {
        CBJHSPModule SP = new CBJHSPModule();
        [HttpGet("GetInfo")]
        public IActionResult GetInfo(string XMBH, string XMMC, int page, int limit) => Ok(SP.GetInfo(XMBH, XMMC, page, limit));

        [HttpGet("GetDetailInfo")]
        public IActionResult GetDetailInfo(string XMBH) => Ok(SP.GetDetailInfo(XMBH));
    }
}