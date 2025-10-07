using Application.Abstractions.Repositories;
using Infrastructure.InMemory.Repositories;
using Infrastructure.Postgres;
using Infrastructure.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureDI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        //appsettings.json из проекта Api использовать InMemoryRepository или RepositoryPostgres
        var useInMemory = cfg.GetValue<bool>("UseInMemoryRepository");
        Console.WriteLine($"[useInMemory]={useInMemory}");

        if (useInMemory)
        {
            services.AddSingleton<ITesterRepository, TesterRepositoryInMemory>();
        }
        else
        {

            var cs = cfg.GetConnectionString("Default")
                ?? throw new InvalidOperationException("ConnectionStrings:Default is missing.");

            Console.WriteLine($"[AddInfrastructure]: ConnectionStrings=[{cs}]");

            services.AddDbContext<AppDbContext>(o => o.UseNpgsql(cs));

            // Репозитории
            services.AddScoped<ITesterRepository, TesterRepositoryPostgres>();

        }

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddSingleton<ITesterRepository, TesterRepositoryInMemory>();
        return services;
    }
}
