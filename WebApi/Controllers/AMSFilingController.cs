using System.Collections.Generic;
using System.Web.Http;

namespace WebApi.Controllers
{
    [RoutePrefix("api/AMSFiling")]
    public class AMSFilingController : ApiController
    {
        private readonly IAMSFilingRpt _filingRpt;
        public AMSFilingController(IAMSFilingRpt filingRpt)
        {
            _filingRpt = filingRpt;
        }

        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get()
        {
            string retMsg = string.Empty;
            IEnumerable<string> list = _filingRpt.GetValue();
            foreach (var item in list)
            {
                retMsg += string.Join(",", item);
            }
            return Json(new { msg = "测试基本的特性路由!" , reson = retMsg });
        }
    }
}