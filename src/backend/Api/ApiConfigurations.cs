namespace Api;

public static class ApiConfigurations
{
    public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables();

        // при спец-флаге загружаем Test-оверрайды
        if (Environment.GetEnvironmentVariable("WC_USE_TEST_SETTINGS") == "true")
        {
            if (Environment.GetEnvironmentVariable("WC_TEST_EMPTY_DB") == "true")
            {
                builder.Configuration.AddJsonFile("appsettings.IntegrationTestsEmptyDb.json", optional: true, reloadOnChange: true); 
            }
            else
            {
                builder.Configuration.AddJsonFile("appsettings.IntegrationTests.json", optional: true, reloadOnChange: true);
            }
        }

        return builder;
    }
}
