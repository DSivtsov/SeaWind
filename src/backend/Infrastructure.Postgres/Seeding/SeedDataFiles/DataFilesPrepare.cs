using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles
{
    internal sealed class DataFilesPrepare
    {
        private const string SEED_TEMPLATE = "*.seed.json";
        private const string ROOT_FOLDER = "./";
        private readonly ILogger _logSeeder;
        private readonly string _pathBase;
        private readonly UUIDMode _modeUUID;
        private IEnumerable<string>? _seedFilePaths;

        private Dictionary<string, Guid> _dictPKeyGuid = new();
        private HashSet<(string fKey, string fileName)> _hashFKeyFileName = new();
        private List<(string pKey, string fileName)> _listDublicatePK = new();

        public DataFilesPrepare(ILogger logSeeder, string pathBase, UUIDMode modeUUID = UUIDMode.Real)
        {
            _logSeeder = logSeeder;
            _pathBase = pathBase;
            _modeUUID = modeUUID;
        }

        internal async Task<bool> Run(CancellationToken ct = default)
        {
            _logSeeder.LogInformation("Run Prepare  DataFiles");

            try { Directory.SetCurrentDirectory(_pathBase); }
            catch (DirectoryNotFoundException ex)
            {
                _logSeeder.LogError("DataFile preparing is stopped : Directory for seed files not found {pathBase}", ex.Message);
                return false;
            }

            _seedFilePaths = GetListSeedFilePaths();
            if (_seedFilePaths == null)
            {
                _logSeeder.LogError("DataFile preparing is stopped : No seed files in {pathBase}", _pathBase);
                return false;
            }

            DataFilesCreator dataFilesParser = new DataFilesCreator(_seedFilePaths, _dictPKeyGuid, _hashFKeyFileName, _listDublicatePK);
            try
            {
                dataFilesParser.ReedSeedFiles();
            }
            catch (Exception ex)
            {
                _logSeeder.LogError("DataFile preparing is stopped :{errMsg}", ex.Message);
                return false;
            }

            ValidateFkeys();

            switch (_modeUUID)
            {
                case UUIDMode.Real:
                    GenerateGuidReal();
                    break;
                case UUIDMode.Stable:
                    GenerateGuidStable();
                    break;
            }

            Debug.WriteLine($"\n==============================\n");

            dataFilesParser.Create();

            await Task.CompletedTask;
            
            return true;
        }

        private void GenerateGuidStable()
        {
            Debug.WriteLine($"\n==============================\n");
            foreach (var item in _dictPKeyGuid)
            {
                _dictPKeyGuid[item.Key] = Uuid5.Create(Uuid5.SeedNamespace,item.Key);
                Debug.WriteLine($"PKey[{item.Key}] Value[{_dictPKeyGuid[item.Key]}]");
            }
        }

        private void GenerateGuidReal()
        {
            Debug.WriteLine($"\n==============================\n");
            foreach (var item in _dictPKeyGuid)
            {
                _dictPKeyGuid[item.Key] = Guid.NewGuid();
                Debug.WriteLine($"PKey[{item.Key}] Value[{_dictPKeyGuid[item.Key]}]");
            }
        }

        private void ValidateFkeys()
        {
            Debug.WriteLine($"\n==============================\n");
            
            if (_listDublicatePK.Count != 0)
            {
                foreach ((string pKey, string fileName) item in _listDublicatePK)
                {
                    _logSeeder.LogWarning($"Skipped record with duplicate PKey[{item.pKey}] in file [{item.fileName}]");
                }
            }

            Debug.WriteLine($"--------------------------------");
            List<(string fKey, string fileName)> recToDel = new();
            foreach ((string fKey, string fileName) item in _hashFKeyFileName)
            {
                if (!_dictPKeyGuid.ContainsKey(item.fKey))
                {
                    _logSeeder.LogWarning($"Skipped record with invalid FKey[{item.fKey}] in file [{item.fileName}]");
                    recToDel.Add(item);
                }
            }
            foreach (var rec in recToDel)
                _hashFKeyFileName.Remove(rec);
        }

        internal IEnumerable<string> GetListSeedFilePaths() =>
            Directory.EnumerateFiles(ROOT_FOLDER, SEED_TEMPLATE)
            .OrderBy(fileName => fileName, StringComparer.Ordinal)
            .Select(x => x);

    }
}
