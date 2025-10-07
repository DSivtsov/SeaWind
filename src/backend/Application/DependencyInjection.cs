using Application.Abstractions.Services;
using Application.UseCasesTester;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ITesterService, TesterService>();
        return services;
    }
}
