using Api.Services;
using Application.Abstractions.Services;

namespace Api;

public static class ApiServicesDI
{
    public static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<CleanupHostedService>();
        services.AddScoped<AttachmentUploadService>();
        services.AddScoped<ICurrentUserService, HttpCurrentUserInfo>();

        return services;
    }
}
