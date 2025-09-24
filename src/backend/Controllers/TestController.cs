using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [Route("Get"), HttpGet]
    public async Task<string> GetAsync()
    {
        return "OK";
    }
}
