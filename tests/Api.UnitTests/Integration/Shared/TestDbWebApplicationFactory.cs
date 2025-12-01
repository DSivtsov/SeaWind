using Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

public class TestDbWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string TEST_DB = "Host=localhost;Port=44432;Database=workshopcode_test;Username=appuser_test;Password=apppass_test";

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("WC_USE_TEST_SETTINGS", "true");

        Environment.SetEnvironmentVariable("ConnectionStrings__Default", TEST_DB);

        return base.CreateHost(builder);
    }
}