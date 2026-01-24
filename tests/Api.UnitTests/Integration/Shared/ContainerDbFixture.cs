using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Infrastructure.Postgres.Main;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Api.UnitTests.Integration.Shared;

public class ContainerDbFixture : IAsyncLifetime
{
    private const string FILENAME_DB_LOG = "postgres_log.txt";
    private const int TIME_SEC_STOP_FROZZEN_CONTAINER = 15;
    private HttpClient? _client;
    private WebApplicationFactory<Program>? _factory;

    public HttpClient? Client => _client;
    public WebApplicationFactory<Program>? Factory => _factory;

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
        _client = _factory.CreateClient();
    }

    public async Task ResetMainDbAsync()
    {
        using var scope = _factory!.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

        var conn = db.Database.GetDbConnection();
        await conn.OpenAsync();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
        DO $$
        DECLARE
            r RECORD;
        BEGIN
            FOR r IN
                SELECT tablename
                FROM pg_tables
                WHERE schemaname = 'main'
            LOOP
                EXECUTE format('TRUNCATE TABLE main.%I RESTART IDENTITY CASCADE', r.tablename);
            END LOOP;
        END
        $$;
        """;

        await cmd.ExecuteNonQueryAsync();
    }


    public async Task DisposeAsync()
    {
        _client?.Dispose();
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
