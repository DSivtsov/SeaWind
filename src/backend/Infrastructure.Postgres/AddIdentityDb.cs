using Infrastructure.Postgres.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Postgres;

internal static class AddIdentityDb
{
    internal static IServiceCollection AddAppIdentityContext(this IServiceCollection services, IConfiguration cfg,
        string connectionString)
    {
        // DB для Identity (использует уже существующий connection string из твоей конфигурации)
        services.AddDbContext<AppIdentityDbContext>(opt =>
                opt.UseNpgsql(connectionString, npg =>
                {
                    npg.EnableRetryOnFailure();
                    npg.MigrationsAssembly(typeof(AppIdentityDbContext).Assembly.FullName);
                    npg.MigrationsHistoryTable("__EFMigrationsHistory", schema: AppIdentityDbContext.Schema);
                })
            );

        return services;
    }
}

public static class IdentityStores
{
    // принимает IdentityBuilder, чтобы API не знал про AppIdentityDbContext и все связанное с этим DI этого DbContext было здесь
    public static IdentityBuilder AddPostgresIdentityStores(this IdentityBuilder builder)
        => builder.AddEntityFrameworkStores<AppIdentityDbContext>();
}