using Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Infrastructure.Postgres;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var cfg = InfrastructureConfigurations.GetConfiguration();

        var cs = cfg.GetConnectionString("Default")
                 ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default");

        Console.WriteLine($"[AppDbContextFactory]: ConnectionStrings=[{cs}]");
        
            var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(cs, x => x.EnableRetryOnFailure())
            .Options;

        return new AppDbContext(opts);
    }
}
