using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Postgres.Time;

public sealed class TimeDbContextFactory : IDesignTimeDbContextFactory<TimeDbContext>
{
    public TimeDbContext CreateDbContext(string[] args)
    {
        var cfg = InfrastructureConfigurations.GetConfiguration();

        var cs = cfg.GetConnectionString("Default")
                 ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default");

        Console.WriteLine($"[TimeDbContextFactory]: ConnectionStrings=[{cs}]");
        
            var opts = new DbContextOptionsBuilder<TimeDbContext>()
            .UseNpgsql(cs)
            .Options;

        return new TimeDbContext(opts);
    }
}
