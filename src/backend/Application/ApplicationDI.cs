using Application.Abstractions.Services;
using Application.UseCasesTester;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationDI
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITesterService, TesterService>();
        return services;
    }
}
