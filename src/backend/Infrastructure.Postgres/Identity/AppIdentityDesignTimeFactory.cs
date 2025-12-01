using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Postgres.Identity
{
    internal class AppIdentityDesignTimeFactory : IDesignTimeDbContextFactory<AppIdentityDbContext>
    {
        public AppIdentityDbContext CreateDbContext(string[] args)
        {
            var cfg = InfrastructureConfigurations.GetConfiguration();

            var cs = cfg.GetConnectionString("Default")
                     ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default");

            Console.WriteLine($"[AppIdentityDbContext]: ConnectionStrings=[{cs}]");

            var opts = new DbContextOptionsBuilder<AppIdentityDbContext>()
                    .UseNpgsql(cs, npg =>
                        {
                            npg.MigrationsAssembly(typeof(AppIdentityDbContext).Assembly.FullName);
                            npg.MigrationsHistoryTable("__EFMigrationsHistory", schema: "identity");
                        })
                    .Options;

            return new AppIdentityDbContext(opts);
        }
    }
}
