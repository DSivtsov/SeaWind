using Api.Filters;
using Application.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using static Infrastructure.Postgres.IdentityStores;    // for AddPostgresIdentityStores()

namespace Api.Identity;

public static class SetupIdentity
{
    public static IServiceCollection AddWorkshopIdentity(this IServiceCollection services, IWebHostEnvironment env)
    {
        // Регистрирует базовые сервисы ASP.NET Core Identity без UI (UserManager, SignInManager и др.)
        // Используется для управления пользователями (регистрация, вход, смена пароля и т.п.)
        services.AddIdentityCore<AppUser>(opt =>
                {
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
                    opt.ClaimsIdentity.UserNameClaimType = ClaimTypes.Email;
                    opt.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
                    opt.SignIn.RequireConfirmedEmail = env.IsProduction();
                    opt.Lockout.AllowedForNewUsers = true;
                    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    opt.Lockout.MaxFailedAccessAttempts = 5;
                })
                .AddRoles<IdentityRole>()
                .AddPostgresIdentityStores()       // extension method from Infrastructure.Postgres
                .AddUserValidator<EmailFormatValidator>();

        return services;
    }

    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration cfg)
    {
        // JWT (ключ берём из конфигурации/env — без хардкода)
        var jwt = cfg.GetSection("Jwt");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

        // Настраиваем JWT-аутентификацию — сервер будет проверять пользовательские JWT-токены
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", opt =>
                {
                    opt.MapInboundClaims = true;                // оставляем ASP.NET mapping WS-Federation (по умолчанию)
                    opt.TokenValidationParameters = new()
                    {
                        NameClaimType = ClaimTypes.Email,       // чтобы User.Identity.Name = email
                        RoleClaimType = ClaimTypes.Role,        // чтобы [Authorize(Roles="Mentor")] работал
                        ValidateIssuer = true,
                        ValidIssuer = jwt["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = jwt["Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateLifetime = true
                    };
                    // Формируем ошибку в соответствии с единым стандартом ProblemDetails (RFC 7807)
                    opt.TuneCustomJwtBearerException();     
                });

        services.AddAuthorization();

        // Регистрируем в DI, чтобы использовать тот же ключ при создании и подписании
        // JWT-токенов для пользователей
        services.Configure<JwtOptions>(jwt);
        // в простых случаях проверки только через AddJwtBearer, в DI можно не хранить
        //services.AddSingleton<SecurityKey>(key);    
        services.AddSingleton(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
