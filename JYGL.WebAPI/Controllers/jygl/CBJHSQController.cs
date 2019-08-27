﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using UIDP.BIZModule.CangChu;

namespace JYGL.WebAPI.Controllers.jygl
{
    [Produces("application/json")]
    [Route("CBJHSQ")]
    public class CBJHSQController : Controller
    {
        CBJHSQModule CB = new CBJHSQModule();
        [HttpGet("GetInfo")]
        public IActionResult GetInfo(string XMBH, string XMMC, int page, int limit) => Ok(CB.GetInfo(XMBH, XMMC, page, limit));
        [HttpPost("CreateInfo")]
        //public IActionResult CreateInfo([FromBody]JObject value,[FromBody]JArray list) => Ok(CB.CreateInfo(value.ToObject<Dictionary<string, object>>(), list.ToObject<List<Dictionary<string, object>>>()));
        public IActionResult CreateInfo([FromBody]JArray list) => Ok(CB.CreateInfo(list.ToObject<List<Dictionary<string, object>>>()));
        [HttpPost("UpdateInfo")]
        public IActionResult UpdateInfo([FromBody]JArray list) => Ok(CB.UpdateInfo(list.ToObject<List<Dictionary<string, object>>>()));
        [HttpPost("DeleteInfo")]
        public IActionResult DeleteInfo([FromBody]JObject value) => Ok(CB.DeleteInfo(value.ToObject<Dictionary<string, object>>()));
        [HttpGet("GetDetailInfo")]
        public IActionResult GetDetailInfo(string XMBH) => Ok(CB.GetDetailInfo(XMBH));

        [HttpGet("DeleteDetailInfo")]
        public IActionResult DeleteDetailInfo(string WZID) => Ok(CB.DeleteDetailInfo(WZID));
    }
}