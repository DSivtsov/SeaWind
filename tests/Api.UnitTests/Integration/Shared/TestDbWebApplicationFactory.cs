using Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

public class TestDbWebApplicationFactory : WebApplicationFactory<Program>
{

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("WC_USE_TEST_SETTINGS", "true");

        return base.CreateHost(builder);
    }
}