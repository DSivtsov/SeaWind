using Infrastructure.Postgres.Seeding.Shared;
using System.Text.Json;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles
{
    internal class SeedFilesAnalyzer
    {
        private Dictionary<string, string> _dictPKey = new();
        private HashSet<(string fKey, string fileName)> _hashFKeyFileName = new();
        private List<(string pKey, string fileName)> _listDublicatePK = new();
        private List<(string pKey, string fileName)> _listSkippedFixedPK = new();

        /// <summary>
        /// All entity names in this class are stored in lower case (invariant).
        /// </summary>
        private string? _currentEntityNameNormalized;
        private SeedFilesConvention? _seedFilesChecker;
        private List<(string entityName, JsonElement rootElement)> _rootJsonElementsEntities = new ();

        internal TableAnalysis AnalyzeSeedFiles(IEnumerable<string> seedFilePaths)
        {
            foreach (var path in seedFilePaths)
            {
                _currentEntityNameNormalized = SeedFilesConvention.GetEntityNameLowered(path);

                if (_currentEntityNameNormalized is null)
                    throw new InvalidDataException($"Invalid demo seed file name: {path}");

                _seedFilesChecker = new SeedFilesConvention(_currentEntityNameNormalized);

                ParseAndScanSeedFile(path);
            }

            List<string> validationWarnings = DetectedWarnings();

            return new TableAnalysis(_dictPKey, _rootJsonElementsEntities, validationWarnings);
        }

        private void ParseAndScanSeedFile(string pathfile)
        {
            using var fs = File.OpenRead(pathfile);
            using var doc = JsonDocument.Parse(fs);

            JsonElement rootElement = doc.RootElement;

            ParseJsonArray(rootElement);

            _rootJsonElementsEntities.Add((_currentEntityNameNormalized!, rootElement.Clone()));
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
            bool isFixPKeyRealValueDetected = false;
            string PKeyRef = string.Empty;

            foreach (JsonProperty property in jsonObject.EnumerateObject())
            {
                JsonElement jsonValue = property.Value;

                if (jsonValue.ValueKind == JsonValueKind.Object || jsonValue.ValueKind == JsonValueKind.Array)
                    continue;

                string valueOriginal = jsonValue.ToString();
                string valueLowered = valueOriginal.ToLowerInvariant();

                if (isFixPKeyRealValueDetected && property.NameEquals(SeedConst.ColumnIdValue))
                {
                    if (SeedFilesConvention.IsValidPKeyValue(valueLowered))
                        _dictPKey.Add(PKeyRef, valueLowered);
                    else
                        _listSkippedFixedPK.Add((valueOriginal, _currentEntityNameNormalized!));

                    isFixPKeyRealValueDetected = false;
                    PKeyRef = string.Empty;
                    continue;
                }

                var typePKeyRef = _seedFilesChecker?.GetKeyType(valueLowered);

                switch (typePKeyRef)
                {
                    case TypeKey.NotKey:
                        continue;
                    
                    case TypeKey.FixPK:
                        if (_dictPKey.ContainsKey(valueLowered))
                        {
                            _listDublicatePK.Add((valueOriginal, _currentEntityNameNormalized!));
                            continue;
                        }
                        isFixPKeyRealValueDetected = true;
                        PKeyRef = valueLowered;
                        continue;

                    case TypeKey.PK:
                        if (_dictPKey.TryAdd(valueLowered, string.Empty)) continue;

                        _listDublicatePK.Add((valueOriginal, _currentEntityNameNormalized!));
                        continue;

                    case TypeKey.FK:
                        _hashFKeyFileName.Add((valueOriginal, _currentEntityNameNormalized!));
                        continue;
                }
            }
            if (isFixPKeyRealValueDetected)
            {
                throw new InvalidDataException($"[ParseAndCheckObjects]: Required column '{SeedConst.ColumnIdValue}' for fixed PK " +
                    $"is not present in the data file. Entity=[{_currentEntityNameNormalized}]");

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

            foreach ((string pKey, string fileName) item in _listSkippedFixedPK)
            {
                validationWarnings.Add($"Will skip record with invalid value [{item.pKey}] for PK/FK use. Entity=[{item.fileName}].");
            }

            foreach ((string pKey, string fileName) item in _listDublicatePK)
            {
                validationWarnings.Add($"Will skip record with duplicate PKey[{item.pKey}]. Entity=[{item.fileName}]");
            }

            foreach ((string fKey, string fileName) item in _hashFKeyFileName)
            {
                if (!_dictPKey.ContainsKey(item.fKey.ToLowerInvariant()))
                {
                    validationWarnings.Add($"Will skip record with invalid FKey[{item.fKey}]. Entity=[{item.fileName}]");
                }
            }

            return validationWarnings;
        }
    }
}
