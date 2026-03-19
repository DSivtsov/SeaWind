using Application.UseCases;

namespace Api.Services;

public sealed class CleanupHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CleanupHostedService> _logger;

    public CleanupHostedService(IServiceScopeFactory scopeFactory,
        ILogger<CleanupHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            bool isNothingToClean = false;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var cleaner = scope.ServiceProvider.GetRequiredService<CleanMongoService>();

                _logger.LogInformation("[CleanMongoService]: Started [OrphanThreadsAsync]");

                isNothingToClean = await cleaner.OrphanThreadsAsync(
                    utcNow: DateTime.UtcNow,
                    ttl: TimeSpan.FromMinutes(60),//TimeSpan.FromHours(24),
                    limitBatch: 50,
                    batchSecDelay: 5,
                    stoppingToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during orphan threads cleanup");
            }

            var nextHoursDelay = isNothingToClean ? 24 : 1;
            _logger.LogInformation($"[CleanMongoService]: Finished [OrphanThreadsAsync] next run in [{nextHoursDelay}] hours");

            await Task.Delay(TimeSpan.FromHours(nextHoursDelay), stoppingToken);
        }
    }
}
