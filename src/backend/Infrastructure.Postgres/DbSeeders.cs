using Infrastructure.Postgres.Main;
using Infrastructure.Postgres.Main.Seeders;
using Infrastructure.Postgres.Seeding;
using Infrastructure.Postgres.Seeding.ContextDB;
using Infrastructure.Postgres.Seeding.Shared;
using Infrastructure.Postgres.Time;
using Infrastructure.Postgres.Time.Seeders;
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
        services.AddScoped<IDbContextRunner, RunnerContextDB<TimeDbContext>>();

        // Нужно указать сидеры которые будут загружать данные
        services.AddScoped<ISeeder<TimeDbContext>, TesterSeeder>();
        services.AddScoped<ISeeder<TimeDbContext>, CarSeeder>();
        services.AddScoped<ISeeder<MainDbContext>, CourseSeeder>();

        return services;
    }
}