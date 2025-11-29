using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles
{
    internal sealed class RunnerSeedDataFiles
    {
        private readonly ILogger _logSeeder;

        internal RunnerSeedDataFiles(ILogger logSeeder)
        {
            _logSeeder = logSeeder;
        }

        internal bool Run(string pathBase, UUIDMode modeUUID)
        {
            _logSeeder.LogInformation("Run Prepare DataFiles");

            IEnumerable<string> seedFilePaths;
            try
            {
                var filesLocator = new SeedFilesLocator(pathBase);
                seedFilePaths = filesLocator.LocateFiles();
            }
            catch (Exception ex)
            {
                _logSeeder.LogError("DataFile preparing is stopped : {errMsg}", ex.Message);
                return false;
            }

            TableAnalysis tableAnalysis;
            try
            {
                var dataFilesParser = new SeedFilesAnalyzer();
                tableAnalysis = dataFilesParser.AnalyzeSeedFiles(seedFilePaths);
            }
            catch (Exception ex)
            {
                _logSeeder.LogError("DataFile preparing is stopped :{errMsg}", ex.Message);
                return false;
            }

            if (tableAnalysis.IsValidationWarningsExist)
            {
                LogValidationWarnings(tableAnalysis.ValidationWarnings);
            }

            var pKeysGuid = new PrimaryKeyGuidGenerator(modeUUID);
            pKeysGuid.Generate(tableAnalysis.PKeys);

            var outputSeedDataFiles = new SeedFilesOutputGenerator(pKeysGuid, tableAnalysis.RootJsonElementsEntities,
                pathBase);
            outputSeedDataFiles.Generate();

            return true;
        }

        private void LogValidationWarnings(IReadOnlyCollection<string> validationWarnings)
        {
            foreach (var warning in validationWarnings)
            {
                _logSeeder.LogWarning(warning);
            }
        }
    }
}
