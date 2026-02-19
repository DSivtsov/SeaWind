using Infrastructure.Postgres.Identity;
using Infrastructure.Postgres.Main;
using Infrastructure.Postgres.Main.Seeders;
using Infrastructure.Postgres.Seeding;
using Infrastructure.Postgres.Seeding.ContextDB;
using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Postgres;

public static class AddDbSeeders
{
    public static IServiceCollection DbSeedersDI(this IServiceCollection services, ConfigurationManager cfg)
    {
        // DI регистрация и получение критично важных параметров, определяющих
        // формат имени файла данных
        services.AddOptions<SeedCoreParameters>()
            .Bind(cfg.GetSection("Seed:CoreOption"))
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.FileNameDataTemplate), "[Seed:CoreOption:FileNameDataTemplate] must be set")
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.FileNameSeedTemplate), "[Seed:CoreOption:FileNameSeedTemplate] must be set")
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.SuffixSeederClassName), "[Seed:CoreOption:SuffixSeederClassName] must be set");

        // регистрация сервисов необходимых для формирования имени файла данных и пути к нему
        services.AddSingleton<ISeedCoreOptions, DefaultSeedCoreOptions>();
        services.AddSingleton<IPathFile, PathFile>();

        // регистратор сервиса проверки наличия и создания администратора
        services.AddScoped<EnsureSuperAdmin>();

        // регистратор сервиса анализа престов сидирования
        services.AddSingleton<SeedPresetAnalyzer>();

        // DI для интерфейса запуска сидирования 
        services.AddScoped<IMainRunnerSeeding, MainRunner>();

        // DI для generic класса оркестраторов сидеров
        services.AddScoped(typeof(Seeder<>));
        // DI для generic класса оркестраторов клинеров
        services.AddScoped(typeof(Cleaner<>));

        // Нужно указать DbContext в который будут загружаться данные
        services.AddScoped<IDbContextRunner, RunnerContextDB<MainDbContext>>();
        services.AddScoped<IDbContextRunner, RunnerContextDB<AppIdentityDbContext>>();

        // Нужно указать сидеры которые будут загружать данные
        services.AddScoped<ISeeder<MainDbContext>, CourseSeeder>();
        services.AddScoped<ISeeder<MainDbContext>, LectureSeeder>();
        services.AddScoped<ISeeder<AppIdentityDbContext>, AppUserSeeder>();
        services.AddScoped<ISeeder<AppIdentityDbContext>, AspNetUserRolesSeeder>();

        return services;
    }
}
