using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Seeding.ContextDB;

internal sealed class Options<TContext> where TContext : DbContext
{
    private const string OPTION_PATH_BASE = "PathBase";
    private const string OPTION_PATH_VERSION = "PathVersion";
    private const string OPTION_INSERT_MODE = "InsertMode";
    private const string OPTION_UUID = "UUID";
    private const string OPTION_EXISTEN_DATA = "ExistenData";
    private const string OPTION_AUTO_MIGRATE = "AutoMigrate";
    private readonly ILogger _log;
    private readonly IConfiguration _cfg;
    private readonly string _prefix;

    public Options(ILogger logSeeder, IConfiguration cfg, string prefix)
    {
        _log = logSeeder;
        _cfg = cfg;
        _prefix = prefix;
    }

    internal OptionsResult CheckAndGet()
    {
        try
        {
            var (existenData, mode, pathBase, pathVersion, autoMigrate, UUIDmode) = GetSeedingOption();
            _log.LogInformation("Seeding options for {Ctx}: ExistenData={ExistenData}, Mode={Mode}," +
                " Base Path=[{PathBase}] Version Path=[{PathVersion}] AutoMigrate[{AutoMigrate}] UUIDmode[{UUIDmode}]",
                typeof(TContext).Name, existenData, mode, pathBase,
                string.IsNullOrWhiteSpace(pathVersion)? "None" : pathVersion, autoMigrate, UUIDmode);
            return new(true, existenData, mode, pathBase, pathVersion, autoMigrate, UUIDmode);
        }
        catch (DirectoryNotFoundException ex)
        {
            _log.LogError(ex, "Path not found for {Ctx}", typeof(TContext).Name);
            return new(false, default, default, "", "", false, default, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _log.LogError(ex, "Invalid seeding options for {Ctx}", typeof(TContext).Name);
            return new(false, default, default, "", "", false, default, ex.Message);
        }
    }

    private (ExistenData existenData, SeedInsertMode mode, string pathBase, string pathVersion,
        bool autoMigrate, UUIDMode UUIDmode) GetSeedingOption()
    {
        var pathBaseRel = _cfg[_prefix + OPTION_PATH_BASE];
        //var pathVersionRel = _cfg[_prefix + OPTION_PATH_VERSION];
        var insertModeText = _cfg[_prefix + OPTION_INSERT_MODE];
        var existenDataText = _cfg[_prefix + OPTION_EXISTEN_DATA];
        var autoMigrateText = _cfg[_prefix + OPTION_AUTO_MIGRATE];
        var UUIDText = _cfg[_prefix + OPTION_UUID];

        if (string.IsNullOrWhiteSpace(pathBaseRel))
            throw new InvalidOperationException($"[{_prefix}{OPTION_PATH_BASE}] is required");

        if (!Enum.TryParse(insertModeText, ignoreCase: true, out SeedInsertMode mode))
            throw new InvalidOperationException($"[{_prefix}{OPTION_INSERT_MODE}] value is invalid: '{insertModeText}'" +
                $" (use InsertOnly|InsertOrUpdate)");

        if (!Enum.TryParse(existenDataText, ignoreCase: true, out ExistenData existenData))
            throw new InvalidOperationException($"[{_prefix}{OPTION_EXISTEN_DATA}] value is invalid: '{existenDataText}'" +
                $" (use Fresh|NotDel)");

        if (!Boolean.TryParse(autoMigrateText, out bool autoMigrate))
            throw new InvalidOperationException($"[{_prefix}{OPTION_AUTO_MIGRATE}] value is invalid: '{autoMigrateText}'" +
                $" (use true|false)");

        if (!Enum.TryParse(UUIDText, ignoreCase: true, out UUIDMode UUIDmode))
            throw new InvalidOperationException($"[{_prefix}{OPTION_UUID}] value is invalid: '{UUIDText}'" +
                $" (use Stable|Real)");

        var baseDir = AppContext.BaseDirectory;

        var pathBase = Path.Combine(baseDir, pathBaseRel);
        if (!Directory.Exists(pathBase))
            throw new DirectoryNotFoundException($"{_prefix}Base Path not found: {pathBase}");

        // OPTION_PATH_VERSION turn off pathVersion = string.Empty always  
        string pathVersion = string.Empty;
        //if (!string.IsNullOrWhiteSpace(pathVersionRel))
        //{
        //    pathVersion = Path.Combine(baseDir, pathVersionRel);
        //    if (!Directory.Exists(pathVersion))
        //        pathVersion = string.Empty;
        //}

        return (existenData, mode, pathBase, pathVersion, autoMigrate, UUIDmode);
    }
}