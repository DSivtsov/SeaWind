using Api.Identity;
using Application;
using Infrastructure.Postgres;

namespace Api;

public class Program
{
    public static void Main(string[] args)
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
            .AddWorkshopIdentity()      // Подключение ASP.NET Identity + Identity Stores 
            .AddJwtAuth(cfg)            // Подключение JWT-аутентификация
            .AddSwaggerWithJWT();       // Подключение Swagger с поддержкой JWT Bearer-авторизации

        // При запуске в контейнере необходимо указать явное место хранения ключей Data Protection.
        builder.AddStorageForContainers();

        var app = builder.Build();

        app.UsePresentation();

        app.Run();
    }
}
