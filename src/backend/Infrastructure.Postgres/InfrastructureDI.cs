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
        
        services.AddTimeDbContext(cs);

        services.AddAppIdentityContext(cfg, cs);

        services.AddMainDbContext(cfg, cs);

        return services;
    }
}
