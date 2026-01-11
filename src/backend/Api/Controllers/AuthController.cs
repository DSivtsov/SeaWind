using Api.Dtos;
using Api.Exceptions;
using Api.Identity;
using Infrastructure.Postgres.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ITokenService _svc;

    /// <summary>
    /// Конфигурация приложения (используется для параметров токена).
    /// </summary>
    private readonly IConfiguration _cfg;

    /// <summary>
    /// Менеджер пользователей ASP.NET Identity.
    /// </summary>
    private readonly UserManager<AppUser> _userManager;

    // static hasher + предсгенерированный фейковый хэш для защиты от тайминговых атак
    private static readonly PasswordHasher<AppUser> passwordHasher = new();
    private static readonly string fakeHashedPassword = passwordHasher.HashPassword(new AppUser(),
        "FakePasswordWorkshopCode#2025");

    public AuthController(ITokenService svc, UserManager<AppUser> userManager, IConfiguration cfg)
    {
        _svc = svc;
        _userManager = userManager;
        _cfg = cfg;
    }

    /// <summary>
    /// Регистрирует нового пользователя в системе.
    /// </summary>
    /// <param name="req">Registration data (email and password).</param>
    /// <returns>
    /// Возвращает <see cref="ActionResult"/> с кодом 200 при успешной регистрации
    /// или ошибку 400/409 при некорректных данных.
    /// </returns>
    /// <response code="200">Регистрация прошла успешно.</response>
    /// <response code="400">
    /// Ошибка регистрации — например, пароль не соответствует требованиям.
    /// </response>
    /// <response code="409">
    /// Конфликт: пользователь с таким email или именем уже существует.
    /// </response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ActionResult),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Register(AuthCredentialsDto req)
    {
        var user = new AppUser { UserName = req.Email, Email = req.Email };
        var res = await _userManager.CreateAsync(user, req.Password);

        if (!res.Succeeded)
        {
            bool conflict = res.Errors.Any(e =>
                e.Code == nameof(IdentityErrorDescriber.DuplicateEmail) ||
                e.Code == nameof(IdentityErrorDescriber.DuplicateUserName));

            if (conflict)
                throw new ConflictException("Учётная запись с такими данными уже существует.");

            if (res.Errors.Any(e => e.Code == "InvalidEmail"))
                throw new BadRequestException("Email имеет некорректный формат.");

            if (res.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.PasswordTooShort)))
                throw new BadRequestException("Пароль должен быть не менее 6 символов.");

            if (res.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.PasswordRequiresLower)))
                throw new BadRequestException("Пароль должен содержать хотя бы одну строчную букву (a-z)");

            if (res.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.PasswordRequiresUpper)))
                throw new BadRequestException("Пароль должен содержать хотя бы одну заглавную букву (A-Z)");

            if (res.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.PasswordRequiresDigit)))
                throw new BadRequestException("Пароль должен содержать хотя бы одну цифру");

            throw new BadRequestException("Ошибка регистрации.");
        }

        return Ok();
    }

    /// <summary>
    /// Авторизация пользователя по email и паролю.
    /// </summary>
    /// <param name="req">Registration data (email and password).</param>
    /// <returns>
    /// Возвращает <see cref="AuthTokenResponseDto"/> с токеном доступа при успешной авторизации
    /// или ошибку 401, если email или пароль неверны.
    /// </returns>
    /// <response code="200">Возвращает JWT-токен доступа.</response>
    /// <response code="401">Неверный email или пароль.</response>
    /// <response code="400">Не указан email или пароль.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthTokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthTokenResponseDto>> LoginWithAccount(AuthCredentialsDto req)
    {
        var email = req.Email;
        var password = req.Password;

        // Не информировать об отсутствии такого email
        if (string.IsNullOrWhiteSpace(email))
            throw new UnauthorizedException("Неверный email или пароль.");

        var user = await _userManager.FindByEmailAsync(email.Trim());

        // Защита от user‑enumeration и тайминговых атак:
        // проверяем фейковый хэш, если пользователя нет
        if (user is null)
        {
            passwordHasher.VerifyHashedPassword(new AppUser(), fakeHashedPassword, password);
            throw new UnauthorizedException("Неверный email или пароль.");
        }

        // Проверка блокировки аккаунта
        if (_userManager.SupportsUserLockout &&  await _userManager.IsLockedOutAsync(user))
            throw new UnauthorizedException("Аккаунт временно заблокирован.");

        // Проверка пароля
        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            if (_userManager.SupportsUserLockout)
                await _userManager.AccessFailedAsync(user).ConfigureAwait(false);
            throw new UnauthorizedException("Неверный email или пароль.");
        }

        // Успешный вход — сбрасываем счётчик неудач
        if (_userManager.SupportsUserLockout)
            await _userManager.ResetAccessFailedCountAsync(user).ConfigureAwait(false);

        // Опционально: требовать подтверждённый email (если включено в опциях)
        if (_userManager.Options.SignIn.RequireConfirmedEmail && !await _userManager.IsEmailConfirmedAsync(user))
            throw new UnauthorizedException("Требуется подтверждение email.");

        var token = _svc.Create(user);
        return Ok(new AuthTokenResponseDto(token));
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
    [Authorize]
    [ProducesResponseType(typeof(UserClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public ActionResult<UserClaimDto> Get()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var name = User.Identity?.Name
            ?? User.FindFirst(ClaimTypes.Email)?.Value;             // emailaddress есть всегда в текущем JWT

        return Ok(new UserClaimDto(name ?? "Empty", id ?? "Error"));
    }

    [HttpGet("test500")]
    public IActionResult Test500()
    {
        throw new Exception("Просто тест 500.");
    }
}
