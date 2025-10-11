
namespace Api;

public static class ApiConfigurations
{

    public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder build)
    {
        build.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{build.Environment.EnvironmentName}.json", optional: true)
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables();

        return build;
    }
}