using Api.Filters;
using Api.Identity;
using Api.Trace;
using Application;
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

        builder.Services
            .AddApplication()
            .AddInfrastructure(cfg)
            .AddPresentation(cfg, builder.Environment);

        // Настраиваем JWT аутентификацию и авторизацию
        builder.Services
            .AddWorkshopIdentity(builder.Environment)      // Подключение ASP.NET Identity + Identity Stores 
            .AddJwtAuth(cfg);                              // Подключение JWT-аутентификация

        if (Environment.GetEnvironmentVariable("WC_USE_TEST_SETTINGS") != "true")
        {
            builder.Services.AddSwaggerWithJWT();       // Подключение Swagger с поддержкой JWT Bearer-авторизации
        }

        // При запуске в контейнере необходимо указать явное место хранения ключей Data Protection.
        builder.AddStorageForContainers();

        // Настройка централизованного формата для всех ошибок
        builder.Services.AddCustomException();

        // Добавить сервис "X-Correlation-Id"
        builder.Services.AddTransient<CorrelationIdMiddleware>();

        // Подключения Seeder сервисов 
        builder.Services.DbSeedersDI(cfg);

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
