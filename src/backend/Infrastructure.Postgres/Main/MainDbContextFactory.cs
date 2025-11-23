using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Postgres.Main;

public sealed class MainDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
{
    public MainDbContext CreateDbContext(string[] args)
    {
        var cfg = InfrastructureConfigurations.GetConfiguration();

        var cs = cfg.GetConnectionString("Default")
                 ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default");

        Console.WriteLine($"[MainDbContextFactory]: ConnectionStrings=[{cs}]");
        
            var opts = new DbContextOptionsBuilder<MainDbContext>()
                        .UseNpgsql(cs, npg =>
                        {
                            npg.MigrationsAssembly(typeof(MainDbContext).Assembly.FullName);
                            npg.MigrationsHistoryTable("__EFMigrationsHistory",
                                schema: MainDbContext.Schema);
                        })
                        .Options;

        return new MainDbContext(opts);
    }
}
