using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using static Infrastructure.Postgres.IdentityStores;    // for AddPostgresIdentityStores()

namespace Api.Identity;

public static class SetupIdentity
{
    public static IServiceCollection AddWorkshopIdentity(this IServiceCollection services)
    {
        // Регистрирует базовые сервисы ASP.NET Core Identity без UI (UserManager, SignInManager и др.)
        // Используется для управления пользователями (регистрация, вход, смена пароля и т.п.)
        services.AddIdentityCore<Infrastructure.Postgres.Identity.AppUser>(opt =>
                {
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = false;
                    opt.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
                    opt.ClaimsIdentity.UserNameClaimType = ClaimTypes.Email;
                    opt.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
                })
                .AddRoles<IdentityRole>()
                .AddPostgresIdentityStores();       // extension method from Infrastructure.Postgres

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

    public static IServiceCollection AddSwaggerWithJWT(this IServiceCollection services)
    {
        // Swagger with JWT
        // со специальной схемой ("bearer"):
        // - "Bearer " - вставлять в поле не надо
        // - вставляешь ТОЛЬКО token (без "Bearer ")
        services.AddSwaggerGen(swgOpt =>
        {
            swgOpt.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Вставь JWT-токен. Префикс 'Bearer' добавится автоматически.",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            swgOpt.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
            swgOpt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });

            // Подключение XML-комментариев в SwaggerGen
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            swgOpt.IncludeXmlComments(xmlPath);
        });

        return services;
    }
}
