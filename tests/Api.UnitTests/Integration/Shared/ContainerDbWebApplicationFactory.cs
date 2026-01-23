using Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

public class ContainerDbWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    public ContainerDbWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("WC_USE_TEST_SETTINGS", "true");

        Environment.SetEnvironmentVariable("WC_TEST_EMPTY_DB", "true");

        Environment.SetEnvironmentVariable("ConnectionStrings__Default",_connectionString);

        return base.CreateHost(builder);
    }
}
