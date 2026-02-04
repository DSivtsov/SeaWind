using Application.DtoAdmin;
using Application.UseCasesAdmin;

namespace Application.Abstractions.Services;

public interface IAdminUserService
{
    Task<UpdateUserRoleResult> UpdateUserRole (string userId, UpdateUserRoleRequest req);
}
