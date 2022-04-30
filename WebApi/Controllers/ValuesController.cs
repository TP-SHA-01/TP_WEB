using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.HandleAttribute;

namespace WebApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/<controller>
        [HttpGet]
        [Route("api/Route/Basic")]
        public IHttpActionResult Get()
        {
            return Json(new { msg = "测试基本的特性路由!" });
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}