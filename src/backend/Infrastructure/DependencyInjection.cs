using Application.Abstractions.Repositories;
using Infrastructure.InMemory.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITesterRepository, TesterRepository>();
        return services;
    }
}
