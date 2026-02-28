using Application.DtoAdmin;
using Application.UseCases;

namespace Application.Abstractions.Repositories;

public interface IAdminRepository
{
    Task<List<UserDto>> GetAllUsersWithRolesAsync();
    Task<List<UserDto>> GetAllUsersWithRolesAsync(string? userName, string? role);

    Task<UpdateUserRoleResult> UpdateUserRoleAsync(string userId, string roleId);

    Task<string?> GetRoleIDAsync(string roleName);
}
