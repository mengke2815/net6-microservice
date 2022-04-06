using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NET6MicroService.Controllers
{
    /// <summary>
    /// 微服务控制器
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("home")]
    public class HomeController : Controller
    {
        /// <summary>
        /// GET
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("这里是NET6MicroService...");
        }
    }
}