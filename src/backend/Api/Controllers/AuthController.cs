using Api.Exceptions;
using Api.Identity;
using Api.Models;
using Infrastructure.Postgres.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public sealed class AuthController : ControllerBase
{
    private readonly ITokenService _svc;
    public AuthController(ITokenService svc) => _svc = svc;

    /// <summary>
    /// Регистрирует нового пользователя в системе.
    /// </summary>
    /// <param name="um">Менеджер пользователей ASP.NET Identity.</param>
    /// <param name="email">Email, который будет использоваться как логин.</param>
    /// <param name="password">Пароль пользователя.</param>
    /// <returns>
    /// Возвращает <see cref="IResult"/> с кодом 200 при успешной регистрации
    /// или ошибку 400/409 при некорректных данных.
    /// </returns>
    /// <response code="200">Регистрация прошла успешно.</response>
    /// <response code="400">
    /// Ошибка регистрации — например, пароль не соответствует требованиям.
    /// Возвращается объект <see cref="ResponseDtoBase"/> с деталями ошибки.
    /// </response>
    /// <response code="409">
    /// Конфликт: пользователь с таким email или именем уже существует.
    /// Возвращается объект <see cref="ResponseDtoBase"/> с сообщением о конфликте.
    /// </response>
    /// <remarks>
    /// Ошибки Identity возвращаются в формате <see cref="IdentityErrorResponse"/>,
    /// где каждая ошибка содержит <c>Code</c> и <c>Description</c>.  
    /// Примеры возможных кодов ошибок:
    /// <list type="bullet">
    /// <item><description><c>DuplicateEmail</c> — указанный email уже зарегистрирован.</description></item>
    /// <item><description><c>DuplicateUserName</c> — имя пользователя уже занято.</description></item>
    /// <item><description><c>PasswordTooShort</c> — пароль слишком короткий.</description></item>
    /// </list>
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDtoBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDtoBase), StatusCodes.Status409Conflict)]
    public async Task<IResult> Register(UserManager<AppUser> um, string email, string password)
    {
        var user = new AppUser { UserName = email, Email = email };
        var res = await um.CreateAsync(user, password);

        if (!res.Succeeded)
        {
            bool conflict = res.Errors.Any(e =>
                e.Code == nameof(IdentityErrorDescriber.DuplicateEmail) ||
                e.Code == nameof(IdentityErrorDescriber.DuplicateUserName));

            if (conflict)
                throw new ConflictException("Пользователь с таким email или именем уже существует.");

            throw new BadRequestException("Ошибка регистрации.");
        }

        return Results.Ok();
    }

    public record IdentityErrorDto(string Code, string Description);

    public record IdentityErrorResponse(List<IdentityErrorDto> Errors);


    /// <summary>
    /// Авторизация пользователя по email и паролю.
    /// </summary>
    /// <param name="um">Менеджер пользователей ASP.NET Identity.</param>
    /// <param name="cfg">Конфигурация приложения (используется для параметров токена).</param>
    /// <param name="email">Email пользователя.</param>
    /// <param name="password">Пароль пользователя.</param>
    /// <returns>
    /// Возвращает <see cref="IResult"/> с токеном доступа при успешной авторизации
    /// или ошибку 401, если email или пароль неверны.
    /// </returns>
    /// <response code="200">Возвращает JWT-токен доступа.</response>
    /// <response code="401">Неверный email или пароль.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDtoBase), StatusCodes.Status401Unauthorized)]
    public async Task<IResult> LoginWithAccount(UserManager<AppUser> um, IConfiguration cfg,
        string email, string password)
    {
        var user = await um.FindByEmailAsync(email);

        if (user is null || !await um.CheckPasswordAsync(user, password))
            throw new UnauthorizedException("Неверный email или пароль.");

        var token = _svc.Create(user);

        return Results.Ok(new { access_token = token });
    }

    /// <summary>
    /// Возвращает информацию о текущем авторизованном пользователе из JWT-токена.
    /// </summary>
    /// <remarks>
    /// Использует данные из клеймов токена (NameIdentifier и Email).
    /// </remarks>
    /// <returns>
    /// Объект <see cref="UserClaimDto"/> с именем и идентификатором пользователя.
    /// </returns>
    /// <response code="200">Информация о текущем пользователе успешно получена.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [Route("/api/me")]
    [HttpGet]
    [ProducesResponseType(typeof(UserClaimDto), StatusCodes.Status200OK)]
    [Authorize]
    public ActionResult<UserClaimDto> Get()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var name = User.Identity?.Name
            ?? User.FindFirst(ClaimTypes.Email)?.Value;             // emailaddress есть всегда в текущем JWT

        return Ok(new UserClaimDto(name ?? "Empty", id ?? "Error"));
    }

    public record UserClaimDto(string name, string id);
}