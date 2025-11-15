using Application.AbstractionsTime.Repositories;
using Infrastructure.Postgres.Time;
using Infrastructure.Postgres.Time.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Postgres
{
    internal static class AddTimeDb
    {
        internal static IServiceCollection AddTimeDbContext(this IServiceCollection services, string connectionString)
        {
            // Таблица "__EFMigrationsHistory" будет в схеме public
            services.AddDbContext<TimeDbContext>(o => o.UseNpgsql(connectionString));

            // Репозитории TimeDbContext
            services.AddScoped<ITesterRepository, TesterRepositoryPostgres>();

            return services;
        }
    }
}