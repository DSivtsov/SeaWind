using Api.Dtos;
using Api.Exceptions;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DtoAdmin;
using Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IAdminRepository _adminRepo;
    private readonly IAdminUserService _adminService;

    public UsersController(IAdminRepository adminRepo, IAdminUserService adminService)
    {
        _adminRepo = adminRepo;
        _adminService = adminService;
    }

    /// <summary>
    /// Возвращает информацию о текущем авторизованном пользователе из JWT-токена.
    /// </summary>
    /// <remarks>
    /// Использует данные из клеймов токена (userId, Role и Email).
    /// </remarks>
    /// <returns>
    /// Объект <see cref="UserMeDto"/> с именем и идентификатором пользователя.
    /// </returns>
    /// <response code="200">Информация о текущем пользователе успешно получена.</response>
    [Route("me")]
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(UserMeDto), StatusCodes.Status200OK)]
    public ActionResult<UserMeDto> GetMe()
    {
        // В рамках MVP формат JWT фиксирован,
        // поэтому наличие клеймов токена (userId, Role и Email) считается детерминированным
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvariantViolationException("Missing NameIdentifier claim.");

        string role = User.FindFirstValue(ClaimTypes.Role)
            ?? throw new InvariantViolationException("Missing Role claim.");

        string email = User.FindFirstValue(ClaimTypes.Email)
            ?? throw new InvariantViolationException("Missing Email claim.");

        return Ok(new UserMeDto(userId, role, email));
    }

    /// <summary>
    /// Возвращает список пользователей с их ролями.
    /// </summary>
    /// <remarks>
    /// Поддерживает фильтрацию по имени пользователя и роли.
    /// Поиск выполняется по нормализованным значениям Identity.
    /// </remarks>
    /// <param name="query">
    /// Параметры фильтрации:
    /// имя пользователя (частичное совпадение) и роль.
    /// </param>
    /// <returns>
    /// Список <see cref="UserDto"/> с именем пользователя и его ролью.
    /// </returns>
    /// <response code="200">Список пользователей успешно получен.</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserDto>>> GetUsersWithRoles([FromQuery] UsersQueryDto query)
    {
        if (query.IsEmpty())
        {
            var dtosQueryEmpty = await _adminRepo.GetAllUsersWithRolesAsync();

            return Ok(dtosQueryEmpty);
        }

        var dtos = await _adminRepo.GetAllUsersWithRolesAsync(query.UserName,query.Role);

        return Ok(dtos);
    }

    /// <summary>
    /// Обновляет роль, назначенную указанному пользователю.
    /// </summary>
    /// <remarks>
    /// Заменяет текущую роль пользователя на указанную.
    /// Если у пользователя уже установлена данная роль, изменения не выполняются.
    /// </remarks>
    /// <param name="userId">
    /// Идентификатор пользователя, для которого изменяется роль.
    /// </param>
    /// <param name="req">
    /// Данные для обновления роли (имя роли).
    /// </param>
    /// <returns>
    /// Тело ответа не возвращается.
    /// </returns>
    /// <response code="204">
    /// Роль успешно обновлена либо уже была установлена.
    /// </response>
    /// <response code="400">
    /// Указанная роль не существует.
    /// </response>
    /// <response code="404">
    /// Пользователь не найден.
    /// </response>
    [HttpPut("{userId}/role")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserRole([FromRoute] string userId, [FromBody] UpdateUserRoleRequest req)
    {
        var r = await _adminService.UpdateUserRole(userId, req);

        return r switch
        {
            UpdateUserRoleResult.Updated => NoContent(),
            UpdateUserRoleResult.NoChange => NoContent(),
            UpdateUserRoleResult.RoleNotFound => BadRequest(new { code = "RoleNotFound" }),
            UpdateUserRoleResult.UserNotFound => NotFound(new { code = "UserNotFound" }),
            _ => StatusCode(500)
        };
    }
}
