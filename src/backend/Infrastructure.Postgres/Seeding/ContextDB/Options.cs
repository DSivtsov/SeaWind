using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Seeding.ContextDB;

internal sealed class Options<TContext> where TContext : DbContext
{
    private const string OPTION_PATH_BASE = "PathBase";
    private const string OPTION_PATH_VERSION = "PathVersion";
    private const string OPTION_MODE = "Mode";
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
            var (existenData, mode, pathBase, pathVersion, autoMigrate) = GetSeedingOption();
            _log.LogInformation("Seeding options for {Ctx}: ExistenData={ExistenData}, Mode={Mode}," +
                " Base Path=[{PathBase}] Version Path=[{PathVersion}] AutoMigrate[{AutoMigrate}]",
                typeof(TContext).Name, existenData, mode, pathBase,
                string.IsNullOrWhiteSpace(pathVersion)? "None" : pathVersion, autoMigrate);
            return new(true, existenData, mode, pathBase, pathVersion, autoMigrate);
        }
        catch (DirectoryNotFoundException ex)
        {
            _log.LogError(ex, "Path not found for {Ctx}", typeof(TContext).Name);
            return new(false, default, default, "", "", false, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _log.LogError(ex, "Invalid seeding options for {Ctx}", typeof(TContext).Name);
            return new(false, default, default, "", "", false, ex.Message);
        }
    }

    private (ExistenData existenData, SeedMode mode, string pathBase, string pathVersion,
        bool autoMigrate) GetSeedingOption()
    {
        var pathBaseRel = _cfg[_prefix + OPTION_PATH_BASE];
        //var pathVersionRel = _cfg[_prefix + OPTION_PATH_VERSION];
        var modeText = _cfg[_prefix + OPTION_MODE];
        var existenDataText = _cfg[_prefix + OPTION_EXISTEN_DATA];
        var autoMigrateText = _cfg[_prefix + OPTION_AUTO_MIGRATE];

        if (string.IsNullOrWhiteSpace(pathBaseRel))
            throw new InvalidOperationException($"{_prefix}Path is required");

        if (!Enum.TryParse(modeText, ignoreCase: true, out SeedMode mode))
            throw new InvalidOperationException($"{_prefix}Mode is invalid: '{modeText}'" +
                $" (use InsertOnly|InsertOrUpdate)");

        if (!Enum.TryParse(existenDataText, ignoreCase: true, out ExistenData existenData))
            throw new InvalidOperationException($"{_prefix}ExistenData value is invalid: '{existenDataText}'" +
                $" (use Fresh|NotDel)");

        if (!Boolean.TryParse(autoMigrateText, out bool autoMigrate))
            throw new InvalidOperationException($"{_prefix}AutoMigrate value is invalid: '{autoMigrateText}'" +
                $" (use true|false)");

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

        return (existenData, mode, pathBase, pathVersion, autoMigrate);
    }
}