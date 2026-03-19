using Application.Common.Exceptions;
using Application.Abstractions.Services;
using Application.Dto.CurrentUser;
using System.Security.Claims;

namespace Api.Services;

public sealed class HttpCurrentUserInfo : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUserInfo(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUserInfo GetUserInfo()
    {
        // В рамках MVP формат JWT фиксирован,
        // поэтому наличие клеймов токена (userId, Role и Email) считается детерминированным
        var user = _httpContextAccessor.HttpContext?.User
            ?? throw new InvariantViolationException("Missing HttpContext user.");

        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvariantViolationException("Missing NameIdentifier claim.");

        string role = user.FindFirstValue(ClaimTypes.Role)
            ?? throw new InvariantViolationException("Missing Role claim.");

        string email = user.FindFirstValue(ClaimTypes.Email)
            ?? throw new InvariantViolationException("Missing Email claim.");

        return new CurrentUserInfo(userId, role, email);
    }
}
