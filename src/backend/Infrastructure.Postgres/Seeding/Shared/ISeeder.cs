using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Seeding.Shared;

internal interface ISeeder<TContext> where TContext : DbContext
{
    /// <summary>
    /// приоритет вызова данного Seeder
    /// </summary>
    SeedOrder Order { get; }

    string Name { get; }

    Task<(bool ok, string? errorMsg)> LoadAndValidateAsync(string pathDemoDataFile, CancellationToken ct = default);

    Task<(bool ok, string? errorMsg)> SeedAsync(TContext db, SeedInsertMode mode, CancellationToken ct = default);

    Task<(bool ok, string? errorMsg)> RemoveRecordsAsync(TContext dbContext, CancellationToken ct = default);

    bool HasDemoData { get; }
}
