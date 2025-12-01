using Api.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Api.Identity;

internal static class SwaggerSupportIdentity
{
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
            // Swagger-mapping для корректного отображения ProblemDetails и ValidationProblemDetails
            swgOpt.MapTypeProblemDetails();
            swgOpt.MapTypeValidationProblemDetails();
        });

        return services;
    }
}