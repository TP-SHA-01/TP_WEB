using System.Web.Http;

namespace WebApi.Controllers
{
    [RoutePrefix("api/AMSFiling")]
    public class AMSFilingController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get()
        {

           return Json(new { msg = "测试基本的特性路由!" });
        }
    }
}