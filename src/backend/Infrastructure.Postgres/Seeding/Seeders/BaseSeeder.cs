using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Seeding.Seeders;

internal abstract class BaseSeeder<TContext, TEntity, TDto> : ISeeder<TContext> where TContext : DbContext
{
    public abstract SeedOrder Order { get; }

    /// <summary>
    /// Поиск демо данных по ключевым полям в БД  
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="recDemo"></param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>TEntity или null</returns>
    protected abstract Task<TEntity?> TryFindEntity(TContext dbContext, TDto recDemo, CancellationToken ct);
    
    /// <summary>
    /// Добавить в БД запись из демо данных
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="recDemo"></param>
    protected abstract void AddNewEntity(TContext dbContext, TDto recDemo);

    /// <summary>
    /// Обновить поля записи в БД демо данными
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="recDemo"></param>
    protected abstract void UpdateEntity(TEntity entity, TDto recDemo);

    /// <summary>
    /// Удалить все записи из таблицы
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    protected abstract Task RemoveEntities(TContext dbContext, CancellationToken ct);

    public string Name => GetType().Name;

    public bool HasDemoData => _demoData is { Count: > 0 };

    protected IReadOnlyList<TDto>? _demoData = null;

    public async Task<(bool ok, string? errorMsg)> LoadAndValidateAsync(string pathDemoDataFile, CancellationToken ct = default)
    {
        string payload;
        try
        {
            payload = await File.ReadAllTextAsync(pathDemoDataFile, ct);
        }
        catch (UnauthorizedAccessException ex)
        {
            var errInfo = $"Access denied to file [{pathDemoDataFile}]: {ex.Message}";
            return (false, errInfo);
        }
        catch (IOException ex)
        {
            var errInfo = $"Error reading file [{pathDemoDataFile}]: {ex.Message}";
            return (false, errInfo);
        }

        if (string.IsNullOrWhiteSpace(payload))
            return (false, $"File [{pathDemoDataFile}] is empty.");

        var parseResult = ParseJsonHelper.TryParseTesters<List<TDto>>(payload);

        if (!parseResult.Ok)
        {
            return (false, parseResult.Error);
        }

        if (parseResult.Value is not { Count: > 0 })
            return (false, "No items found.");

        _demoData = parseResult.Value;

        return (true, null);
    }

    public async Task<(bool, string?)> SeedAsync(TContext dbContext, SeedMode mode, CancellationToken ct = default)
    {
        if (_demoData is null)
            throw new SeederDataException("SeedAsync called without prior successful LoadAndValidateAsync.");

        foreach (var recDemo in _demoData)
        {
            var entity = await TryFindEntity(dbContext, recDemo, ct);

            if (entity is null)
            {
                AddNewEntity(dbContext, recDemo);
            }
            else
            {
                if (mode == SeedMode.InsertOnly) continue;
                UpdateEntity(entity, recDemo);
            }
        }

        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return (false, $"Concurrency conflict: {ex.Message}");
        }
        catch (DbUpdateException ex)
        {
            return (false, $"DB update failed: {ex.InnerException?.Message ?? ex.Message}");
        }
        catch (TimeoutException ex)
        {
            return (false, $"Database timeout: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (false, $"Unexpected error: {ex.Message}");
        }

        return (true, null);
    }

    public async Task<(bool ok, string? errorMsg)> RemoveRecordsAsync(TContext dbContext, CancellationToken ct = default)
    {
        try
        {
            await RemoveEntities(dbContext, ct);

            if (dbContext.ChangeTracker.HasChanges())
                await dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            return (false, $"Unexpected error: {ex.Message}");
        }

        return (true, null);
    }

}
