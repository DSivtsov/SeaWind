using Api.Dtos;
using Api.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    /// <summary>
    /// Возвращает информацию о текущем авторизованном пользователе из JWT-токена.
    /// </summary>
    /// <remarks>
    /// Использует данные из клеймов токена (Role и Email).
    /// </remarks>
    /// <returns>
    /// Объект <see cref="UserMeDto"/> с именем и идентификатором пользователя.
    /// </returns>
    /// <response code="200">Информация о текущем пользователе успешно получена.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [Route("me")]
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(UserMeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<UserMeDto> Get()
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;          // role есть всегда в текущем JWT
        var email = User.FindFirst(ClaimTypes.Email)?.Value;         // emailaddress есть всегда в текущем JWT

        // В рамках MVP формат JWT фиксирован,
        // поэтому наличие клеймов токена (Role и Email) считается детерминированным
        if (role == null || email == null)
            throw new InvariantViolationException("Ошибка получения данных.");

        return Ok(new UserMeDto(role, email));
    }
}
