namespace Api;

public static class ApiConfigurations
{

    public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables();

        // при спец-флаге загружаем Test-оверрайды
        if (Environment.GetEnvironmentVariable("WC_USE_TEST_SETTINGS") == "true")
        {
            builder.Configuration
                .AddJsonFile("appsettings.IntegrationTests.json", optional: true, reloadOnChange: true);
        }

/*        string cs = builder.Configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is missing.");

        Debug.WriteLine($"ConnectionStrings[{cs}]");*/

        return builder;
    }
}