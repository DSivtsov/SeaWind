using Microsoft.Extensions.Configuration;

namespace Infrastructure.Postgres;

public static class InfrastructureConfigurations
{

    public static IConfigurationRoot GetConfiguration()
    {
        Console.WriteLine($"[AppDbContextFactory]:" +
            $" ASPNETCORE_ENVIRONMENT=[{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}]");

        //Первый шаг найти путь к appsettings.json из проекта Api (SSOT)
        var baseConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var apiJsonPath = baseConfig["Paths:ApiConfigPath"] ?? "../Api";

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        //Использование appsettings.json из проекта Api
        IConfigurationRoot cfg = new ConfigurationBuilder()
            .SetBasePath(Path.GetFullPath(apiJsonPath))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
            // secrets are tied to an assembly; here we use the factory’s assembly
            //.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .AddEnvironmentVariables()
            .Build();

        return cfg;
    }
}