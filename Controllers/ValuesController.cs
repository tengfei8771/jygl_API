using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoadFlow.Integrate;
namespace WZGX.WebAPI.Controllers
{
    [Route("[controller]")]
    public class ValuesController : Controller
    {

        Organize test = new Organize();
        [HttpGet("GetAllUser")]
        public IActionResult GetAllUser()
        {
            return Json(test.GetAllUser());
        }
        [HttpGet("GetAllOrganize")]
        public IActionResult GetAllOrganize()
        {
            return Json(test.GetAllOrganize());
        }


        [HttpGet("GetAllOrganizeUser")]
        public IActionResult GetAllOrganizeUser()
        {
            return Json(test.GetAllOrganizeUser());
        }
        [HttpGet("GetAllWorkGroup")]
        public IActionResult GetAllWorkGroup()
        {
            return Json(test.GetAllWorkGroup());
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
			
        }
        

      
        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
