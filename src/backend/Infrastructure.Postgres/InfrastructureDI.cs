using Application.Abstractions.Repositories;
using Infrastructure.Postgres.Time;
using Infrastructure.Postgres.Time.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Postgres;

public static class InfrastructureDI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        var cs = cfg.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is missing.");

        Console.WriteLine($"[AddInfrastructure]: ConnectionStrings=[{cs}]");

        services.AddDbContext<TimeDbContext>(o => o.UseNpgsql(cs));

        // Репозитории MainDbContext
        services.AddScoped<ITesterRepository, TesterRepositoryPostgres>();

        return services;
    }
}
