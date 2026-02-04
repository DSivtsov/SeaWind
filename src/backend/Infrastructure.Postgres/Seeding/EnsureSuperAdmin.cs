using Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Seeding;

internal sealed class EnsureSuperAdmin
{
    private const string _sectionSuperAdmin = "Seed:SuperAdmin";
    private const string _enableSuperAdmin = "Seed:SuperAdmin:Enabled";
    private const string _emailSuperAdmin = "Seed:SuperAdmin:Email";
    private const string _pwdKeyUserSecret = "SuperAdmin:Password";
    private const string ADMIN_ROLE = "Admin";
    private readonly ILogger _log;
    private readonly IConfiguration _cfg;
    private readonly UserManager<AppUser> _userManager;

    public EnsureSuperAdmin(ILogger<EnsureSuperAdmin> log, IConfiguration cfg, UserManager<AppUser> userManager)
    {
        _log = log;
        _cfg = cfg;
        _userManager = userManager;
    }

    internal async Task<bool> CreateSuperAdminAsync()
    {
        var existSection = _cfg.GetSection(_sectionSuperAdmin).Exists();

        if (!existSection || !String.Equals(_cfg[_enableSuperAdmin], "true", StringComparison.OrdinalIgnoreCase))
        {
            _log.LogInformation("SuperAdmin Seeding Skipped");
            return false;
        }

        var email = _cfg[_emailSuperAdmin];
        var pwd = _cfg[_pwdKeyUserSecret];

        if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(pwd))
        {
            _log.LogError("SuperAdmin Seeding Abort. Email or Password IsNullOrEmpty");
            return false;
        }

        var userFound = await _userManager.FindByEmailAsync(email);

        if (userFound != null)
        {
            _log.LogError($"SuperAdmin Seeding Abort. User with Email [{email}] already exist");
            return false;
        }

        var userNew = new AppUser { UserName = email, Email = email };
        var res = await _userManager.CreateAsync(userNew, pwd);

        if (!res.Succeeded)
        {
            _log.LogError("SuperAdmin Seeding Abort. Incorrect email address or password does not meet requirements.");
            return false;
        }

        res = await _userManager.AddToRoleAsync(userNew, ADMIN_ROLE);

        // В рамках MVP роли фиксированы и создаются через миграции (HasData),
        // поэтому AddToRoleAsync здесь считается детерминированным
        if (!res.Succeeded)
        {
            _log.LogError("SuperAdmin Seeding Abort. Ошибка регистрации.");
            return false;
        }

        _log.LogInformation($"SuperAdmin Seeding Finish: Admin created {email}");
        return true;
    }
}
