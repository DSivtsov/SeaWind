using System.Text.Json;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles
{
    internal class SeedFilesAnalyzer
    {
        private Dictionary<string, Guid> _dictPKeyGuid = new();
        private HashSet<(string fKey, string fileName)> _hashFKeyFileName = new();
        private List<(string pKey, string fileName)> _listDublicatePK = new();

        private string? _currentEntityName;
        private HelperSeedFilesChecker? _seedFilesChecker;
        private List<(string entityName, JsonElement rootElement)> _rootJsonElementsEntities = new ();

        internal TableAnalysis AnalyzeSeedFiles(IEnumerable<string> seedFilePaths)
        {
            foreach (var path in seedFilePaths)
            {
                _currentEntityName = HelperSeedFilesChecker.GetEntityName(path);

                if (_currentEntityName is null)
                    throw new InvalidDataException($"Invalid demo seed file name: {path}");

                _seedFilesChecker = new HelperSeedFilesChecker(_currentEntityName);

                ParseAndScanSeedFile(path);
            }

            List<string> validationWarnings = DetectedWarnings();

            return new TableAnalysis(_dictPKeyGuid, _rootJsonElementsEntities, validationWarnings);
        }

        private void ParseAndScanSeedFile(string pathfile)
        {
            using var fs = File.OpenRead(pathfile);
            using var doc = JsonDocument.Parse(fs);

            JsonElement rootElement = doc.RootElement;

            ParseJsonArray(rootElement);

            _rootJsonElementsEntities.Add((_currentEntityName!, rootElement.Clone()));
        }

        private void ParseJsonArray(JsonElement rootElement)
        {
            if (rootElement.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException("[ParseJsonArray]: [First element] not is JsonValueKind.Array");

            // цикл по объектам 
            foreach (JsonElement jsonObject in rootElement.EnumerateArray())
            {
                if (jsonObject.ValueKind != JsonValueKind.Object)
                    throw new InvalidDataException("[ParseJsonArray]: In JsonArray not only JsonValueKind.Object");

                ParseAndCheckObjects(jsonObject);
            }
        }

        private void ParseAndCheckObjects(JsonElement jsonObject)
        {
            foreach (JsonProperty property in jsonObject.EnumerateObject())
            {
                JsonElement jsonValue = property.Value;

                if (jsonValue.ValueKind == JsonValueKind.Object || jsonValue.ValueKind == JsonValueKind.Array)
                    continue;

                string value = jsonValue.ToString();

                var typeKey = _seedFilesChecker?.GetKeyType(value);

                switch (typeKey)
                {
                    case TypeKey.NotKey:
                        continue;

                    case TypeKey.PK:
                        if (_dictPKeyGuid.TryAdd(value, Guid.Empty)) continue;

                        _listDublicatePK.Add((value, _currentEntityName!));
                        continue;

                    case TypeKey.FK:
                        _hashFKeyFileName.Add((value, _currentEntityName!));
                        continue;
                }
            }
        }

        /// <summary>
        /// Анализирует собранные данные и формирует список предупреждений (Warnings):
        /// дублирующиеся PKey и FKey, для которых отсутствуют соответствующие PKey.
        /// <para/>
        /// Эти проблемы не приводят к исключению — они будут автоматически
        /// устранены при генерации выходных файлов в
        /// SeedFilesOutputGenerator.GenerateJsonObject().
        /// </summary>
        /// <returns>Список текстовых предупреждений.</returns>
        private List<string> DetectedWarnings()
        {
            List<string> validationWarnings = new List<string>();

            if (_listDublicatePK.Count != 0)
            {
                foreach ((string pKey, string fileName) item in _listDublicatePK)
                {
                    validationWarnings.Add($"Will Skipped record with duplicate PKey[{item.pKey}] in file [{item.fileName}]");
                }
            }

            foreach ((string fKey, string fileName) item in _hashFKeyFileName)
            {
                if (!_dictPKeyGuid.ContainsKey(item.fKey))
                {
                    validationWarnings.Add($"Will Skipped record with invalid FKey[{item.fKey}] in file [{item.fileName}]");
                }
            }

            return validationWarnings;
        }
    }
}
