using Infrastructure.Postgres.Main;
using Infrastructure.Postgres.Main.Seeders;
using Infrastructure.Postgres.Seeding;
using Infrastructure.Postgres.Seeding.ContextDB;
using Infrastructure.Postgres.Seeding.Shared;
using Infrastructure.Postgres.Time;
using Infrastructure.Postgres.Time.Seeders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Postgres;

public interface IRunDbSeeders
{
    Task Run(CancellationToken ct = default);
}
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

        // DI для интерфейса запуска сидирования 
        services.AddScoped<IRunDbSeeders, RunDbSeeders>();
        // DI для generic класса оркестратор порверок и запуска сидирования
        services.AddScoped(typeof(DbRunner<>));
        // DI для generic класса оркестраторов сидеров
        services.AddScoped(typeof(ContextSeeder<>));
        // DI для generic класса оркестраторов клинеров
        services.AddScoped(typeof(ContextCleaner<>));

        // Нужно указать сидеры которые будут загружать данные
        services.AddScoped<ISeeder<TimeDbContext>, TesterSeeder>();
        services.AddScoped<ISeeder<TimeDbContext>, CarsSeeder>();
        services.AddScoped<ISeeder<MainDbContext>, CourseSeeder>();

        return services;
    }

}

// класс для запуска сидирования
internal sealed class RunDbSeeders : IRunDbSeeders
{
    // список оркестраторов для каждого DbContext
    readonly private DbRunner<MainDbContext> _mainDbRunner;
    readonly private DbRunner<TimeDbContext> _timeDbRunner;
    readonly private IOptions<SeedCoreParameters> _coreOpt;
    readonly private ILogger<RunDbSeeders> _logger;

    // DI сам создает необходимые классы при создании объекта "Lazy.DI"
    public RunDbSeeders(DbRunner<MainDbContext> mainDbRunner,
                        DbRunner<TimeDbContext> timeDbRunner,
                        IOptions<SeedCoreParameters> coreOpt,
                        ILogger<RunDbSeeders> logger)
    {
        _mainDbRunner = mainDbRunner;
        _timeDbRunner = timeDbRunner;
        _coreOpt = coreOpt;
        _logger = logger;
    }

    public async Task Run(CancellationToken ct = default)
    {
        _logger.LogInformation("[SEEDING START]");

        SeedCoreParameters coreValues;
        try { coreValues = _coreOpt.Value;}
        catch (OptionsValidationException ex)
        {
            _logger.LogCritical(ex, "Invalid [Seed:CoreOption]. Aborting seeding : {Error}", ex.Message);
            return;
        }

        _logger.LogInformation("Format DemoData files [{FileNameDataTemplate}] [{FileNameSeedTemplate}] [{SuffixSeederClassName}]",
            coreValues.FileNameDataTemplate, coreValues.FileNameSeedTemplate, coreValues.SuffixSeederClassName);

        // Список DbContext для которых будут запускаться Seeders
        await _mainDbRunner.RunAsync(ct);
        await _timeDbRunner.RunAsync(ct);

        _logger.LogInformation("[SEEDING FINISHED]");
    }
}