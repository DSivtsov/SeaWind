using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DtoAdmin;

namespace Application.UseCases;

public enum UpdateUserRoleResult
{
    Updated,
    UserNotFound,
    RoleNotFound,
    NoChange
}

public class AdminUserService : IAdminUserService
{
    private readonly IAdminRepository _adminRepo;

    public AdminUserService(IAdminRepository adminRepo)
    {
        _adminRepo = adminRepo;
    }

    public async Task<UpdateUserRoleResult> UpdateUserRole(string userId, UpdateUserRoleRequest req)
    {
        var roleId = await _adminRepo.GetRoleIDAsync(req.Role);

        if (roleId == null) return UpdateUserRoleResult.RoleNotFound;

        return await _adminRepo.UpdateUserRoleAsync(userId, roleId);
    }
}
