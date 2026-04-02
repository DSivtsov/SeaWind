using Api.Configuration;
using Api.Filters;
using Api.Identity;
using Api.Services;
using Api.Trace;
using Application;
using Infrastructure.Mongo;
using Infrastructure.Postgres;
using Infrastructure.Postgres.Seeding;

namespace Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //нужно добавить перед добавлением сервисов
        builder.AddConfiguration();

        var cfg = builder.Configuration;
        var services = builder.Services;

        services.AddAppSettingsOptions(cfg);

        services
            .AddApplication()
            .AddInfrastructure(cfg)
            .AddMongoInfrastructure(cfg)
            .AddPresentation(cfg, builder.Environment);

        // Настраиваем JWT аутентификацию и авторизацию
        services
            .AddWorkshopIdentity(builder.Environment)      // Подключение ASP.NET Identity + Identity Stores 
            .AddJwtAuth(cfg);                              // Подключение JWT-аутентификация

        if (Environment.GetEnvironmentVariable("WC_USE_TEST_SETTINGS") != "true")
        {
            services.AddSwaggerWithJWT();       // Подключение Swagger с поддержкой JWT Bearer-авторизации
        }

        // При запуске в контейнере необходимо указать явное место хранения ключей Data Protection.
        services.AddStorageForContainers(cfg);

        // Настройка централизованного формата для всех ошибок
        services.AddCustomException();

        // Добавить сервис "X-Correlation-Id"
        services.AddTransient<CorrelationIdMiddleware>();

        // Подключения Seeder сервисов 
        services.DbSeedersDI(cfg);

        // Регистрация HostedService для фоновых задач приложения
        services.AddHostedServices();

        // Доступ к текущему HttpContext (используется для получения текущего пользователя из JWT)
        services.AddHttpContextAccessor();

        var app = builder.Build();

        // Запуск Seeder для закрузку демо-данных для окружения DEV
        if (app.Environment.IsDevelopment())
        {
            // await using — синтаксис для асинхронного освобождения (IAsyncDisposable).
            await using var scope = app.Services.CreateAsyncScope();
            var mainRunner = scope.ServiceProvider.GetRequiredService<IMainRunnerSeeding>();

            await mainRunner.Run(app.Lifetime.ApplicationStopping);
        }

        // Использовать сервис "X-Correlation-Id" в pipeline HTTP request
        app.UseMiddleware<CorrelationIdMiddleware>();

        app.UsePresentation();

        app.Run();
    }
}
