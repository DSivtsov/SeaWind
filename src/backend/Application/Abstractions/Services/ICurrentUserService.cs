using Application.Dto.CurrentUser;

namespace Application.Abstractions.Services;

public interface ICurrentUserService
{
    CurrentUserInfo GetUserInfo();
}
