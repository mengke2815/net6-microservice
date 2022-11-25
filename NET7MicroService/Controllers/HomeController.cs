using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace NET7MicroService.Controllers;

/// <summary>
/// 微服务控制器
/// </summary>
//[Authorize]
[ApiController]
[Route("home")]
public class HomeController : ControllerBase
{
    /// <summary>
    /// GET
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Get()
    {
        var username = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Name)?.Value;
        return Ok($"{username} 已登录：这里是NET7MicroService...");
    }
}