using Application.Abstractions.Repositories;
using Infrastructure.Postgres.Main;
using Infrastructure.Postgres.Main.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Postgres;

internal static class AddMainDb
{
    internal static IServiceCollection AddMainDbContext(this IServiceCollection services,
        IConfiguration cfg, string connectionString)
    {
        // DB для MainDb (использует уже существующий connection string из твоей конфигурации)
        services.AddDbContext<MainDbContext>(opt =>
                opt.UseNpgsql(connectionString, npg =>
                {
                    npg.MigrationsAssembly(typeof(MainDbContext).Assembly.FullName);
                    npg.MigrationsHistoryTable("__EFMigrationsHistory", schema: MainDbContext.Schema);
                })
            );

        // Репозитории MainDbContext
        services.AddScoped<ICourseRepository, CourseRepositoryPostgres>();

        return services;
    }
}
