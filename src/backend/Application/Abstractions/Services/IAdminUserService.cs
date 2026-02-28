using Application.DtoAdmin;
using Application.UseCases;

namespace Application.Abstractions.Services;

public interface IAdminUserService
{
    Task<UpdateUserRoleResult> UpdateUserRole (string userId, UpdateUserRoleRequest req);
}
