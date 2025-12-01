using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Api.UnitTests.Integration.Shared;

public class ContainerDbFixture : IAsyncLifetime
{
    private const string FILENAME_DB_LOG = "postgres_log.txt";
    private const int TIME_SEC_STOP_FROZZEN_CONTAINER = 15;
    public HttpClient? Client;
    private WebApplicationFactory<Program>? _factory;

    // 3. защита от зависаний
    private static readonly Action<IWaitStrategy> _waitStrategy = strategy
        => strategy.WithTimeout(TimeSpan.FromSeconds(TIME_SEC_STOP_FROZZEN_CONTAINER));

    public PostgreSqlContainer Container { get; }

    public ContainerDbFixture()
    {
        var initDir = Path.Combine(AppContext.BaseDirectory, "TestcontainerInitDb");

        Container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("workshopcode_test")
            .WithUsername("appuser_test")
            .WithPassword("apppass_test")
                .WithEnvironment("POSTGRES_DB", "workshopcode_test")
                .WithEnvironment("POSTGRES_USER", "postgres")
                .WithEnvironment("POSTGRES_PASSWORD", "postgres")
                .WithEnvironment("APP_DB", "workshopcode_test")
                .WithEnvironment("APP_USER", "appuser_test")
                .WithEnvironment("APP_PASSWORD", "apppass_test")
                .WithBindMount(initDir, "/docker-entrypoint-initdb.d")
                .WithTmpfsMount("/var/lib/postgresql/data")
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilMessageIsLogged("===INIT END===", _waitStrategy)   // 1. init-скрипт полностью завершён
                    .UntilDatabaseIsAvailable(NpgsqlFactory.Instance, _waitStrategy)) // 2. Postgres реально отвечает как БД
                    //.WithReuse(true)
                .Build();
    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();

        _factory = new ContainerDbWebApplicationFactory(Container.GetConnectionString());
        Client = _factory.CreateClient();
    }
    
    public async Task DisposeAsync()
    {
        Client?.Dispose();
        _factory?.Dispose();

        if (Container is not null)
        {
            var pathLog = Path.Combine(AppContext.BaseDirectory, FILENAME_DB_LOG);
            var logs = await Container.GetLogsAsync();
            File.WriteAllText(pathLog, logs.Stdout + logs.Stderr);

            await Container.DisposeAsync();
        }
    }
}
