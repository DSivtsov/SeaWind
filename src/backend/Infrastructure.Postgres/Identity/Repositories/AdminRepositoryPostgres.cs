using Application.Abstractions.Repositories;
using Application.DtoAdmin;
using Application.Models;
using Application.UseCasesAdmin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Identity.Repositories;

public class AdminRepositoryPostgres : IAdminRepository
{
    private readonly AppIdentityDbContext _identityDbContext;
    private readonly UserManager<AppUser> _userManager;

    public AdminRepositoryPostgres(AppIdentityDbContext identityDbContext, UserManager<AppUser> userManager)
    {
        _identityDbContext = identityDbContext;
        _userManager = userManager;
    }

    public async Task<List<UserDto>> GetAllUsersWithRolesAsync()
    {
        var userDtos = await _identityDbContext.UserRoles.AsNoTracking()
            .Join(_identityDbContext.Users,
            ur => ur.UserId,
            u => u.Id,
            (ur, u) => new { u.Id, email = u.UserName, roleId = ur.RoleId })
            .Join(_identityDbContext.Roles,
            q => q.roleId,
            roles => roles.Id,
            (q,roles) => new UserDto(q.Id, q.email!, roles.Name!))
            .ToListAsync();

        return userDtos;
    }

    public async Task<List<UserDto>> GetAllUsersWithRolesAsync(string? userName, string? role)
    {
        var rolesIdSelected = _identityDbContext.Roles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(role))
        {
            var normRole = _userManager.NormalizeName(role);
            rolesIdSelected = rolesIdSelected.Where(rec => rec.NormalizedName == normRole);
        }    

        var usersRoleSelected = _identityDbContext.UserRoles.AsNoTracking()
            .Join(rolesIdSelected,
            userRole => userRole.RoleId,
            roleIdSel => roleIdSel.Id,
            (userRole, rolesIdSelected) => new { userRole.UserId, RoleName = rolesIdSelected.Name });

        var userDtosTemp = _identityDbContext.Users.AsNoTracking()
            .Join(usersRoleSelected,
            users => users.Id,
            userRole => userRole.UserId,
            (users, userRole) => new {users.Id, users.UserName, users.NormalizedUserName, userRole.RoleName});

        if (!string.IsNullOrWhiteSpace(userName))
        {
            var normUserName = _userManager.NormalizeName(userName);
            userDtosTemp = userDtosTemp.Where(userDto => userDto.NormalizedUserName!.Contains(normUserName));
        }    

        var userWithRolesFiltred = await userDtosTemp.Select(rec => new UserDto(rec.Id, rec.UserName!, rec.RoleName!)).ToListAsync();

        return userWithRolesFiltred;
    }

    public async Task<string?> GetRoleIDAsync(string roleName)
    {
        var normRoleName = _userManager.NormalizeName(roleName);

        var rolesIdSelected = await _identityDbContext.Roles.AsNoTracking()
            .Where(r => r.NormalizedName == normRoleName)
            .Select(r => r.Id)
            .SingleOrDefaultAsync();

        return rolesIdSelected;
    }

    public async Task<UpdateUserRoleResult> UpdateUserRoleAsync(string userId, string newRoleId)
    {
        var existingUserRole = await _identityDbContext.UserRoles
            .FirstOrDefaultAsync(r => r.UserId == userId);

        if (existingUserRole is null) return UpdateUserRoleResult.UserNotFound;

        if (existingUserRole.RoleId == newRoleId) return UpdateUserRoleResult.NoChange;

        _identityDbContext.UserRoles.Remove(entity: existingUserRole);

        _identityDbContext.UserRoles.Add(new IdentityUserRole<string>
        {
            UserId = userId,
            RoleId = newRoleId
        });

        await _identityDbContext.SaveChangesAsync();
        return UpdateUserRoleResult.Updated;
    }
}
