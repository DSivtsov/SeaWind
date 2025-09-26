using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

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
