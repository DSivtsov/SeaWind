using Api.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherExceptionController : ControllerBase
    {
        [Route("Get"), HttpGet]
        public int Get(int id)
        {
            if (id < 0)
                throw new BadRequestException("Значение должно быть больше 0!");

            return id;
        }
    }
}
