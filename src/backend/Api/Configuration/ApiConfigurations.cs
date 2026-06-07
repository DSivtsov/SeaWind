namespace Api.Configuration;

public static class ApiConfigurations
{
    public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder builder)
    {
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
