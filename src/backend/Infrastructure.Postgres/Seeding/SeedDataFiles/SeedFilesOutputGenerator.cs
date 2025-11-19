using System.Text.Json;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles
{
    internal class SeedFilesOutputGenerator
    {
        private readonly PrimaryKeyGuidGenerator _finalGuid;
        private readonly IReadOnlySet<(string fKey, string fileName)> _hashFKeyFileName;
        private readonly IReadOnlyCollection<(string entityName, JsonElement rootElement)> _rootJsonElementsEntities;

        private string? _currentEntityName;
        private HelperSeedFilesChecker? _seedFilesChecker;
        private readonly List<(string Name, JsonElement Value)> _buffer = new();
        private Action<JsonElement>? _actionsForJsonObject;
        private readonly HashSet<string> _hashPkKeyExist = new();

        private Utf8JsonWriter? _timeWriter;

        internal SeedFilesOutputGenerator(PrimaryKeyGuidGenerator finalGuid, IReadOnlySet<(string fKey, string fileName)> fKeys,
            IReadOnlyCollection<(string entityName, JsonElement rootElement)> rootJsonElementsEntities)
        {
            _finalGuid = finalGuid ?? throw new ArgumentNullException(nameof(finalGuid));
            _hashFKeyFileName = fKeys ?? throw new ArgumentNullException(nameof(fKeys));
            _rootJsonElementsEntities = rootJsonElementsEntities ?? throw new ArgumentNullException(nameof(rootJsonElementsEntities));
        }

        private static JsonWriterOptions _jsonOptions = new JsonWriterOptions
        {
            Indented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        internal void Generate()
        {
            foreach ((string entityName, JsonElement rootElement) item in _rootJsonElementsEntities)
            {
                _currentEntityName = item.entityName;

                _seedFilesChecker = new HelperSeedFilesChecker(_currentEntityName);

                CreateDataFile(item.rootElement); /// with SaveData
            }
        }

        private string GetNameCurrentDataFile()
        {
            ReadOnlySpan<char> name = _currentEntityName;
            if (name.Length == 0)
                throw new InvalidOperationException("Entity name cannot be empty.");

            string Name = name.Length == 1
                ? char.ToUpperInvariant(name[0]).ToString()
                : string.Concat(char.ToUpperInvariant(name[0]), name[1..].ToString());

            return $"Demo{Name}.json";
        }

        private void CreateDataFile(JsonElement rootElement)
        {
            var currentFileName = GetNameCurrentDataFile();

            using var fs = File.Create(currentFileName);
            using var writer = new Utf8JsonWriter(fs, _jsonOptions);
            _timeWriter = writer;
            writer.WriteStartArray();

            ParseSeedFileAndSaveData(rootElement);

            writer.WriteEndArray();
            writer.Flush();

        }

        private void ParseSeedFileAndSaveData(JsonElement rootElement)
        {
            _actionsForJsonObject = PrepareDataFile;

            ParseJsonArray(rootElement); /// with ParseAndScan
        }

        private bool FillBufferJsonObject(JsonElement jsonObject)
        {
            // цикл по полям объекта
            foreach (JsonProperty property in jsonObject.EnumerateObject())
            {
                JsonElement jsonValue = property.Value;

                if (jsonValue.ValueKind == JsonValueKind.Object || jsonValue.ValueKind == JsonValueKind.Array)
                {
                    _buffer.Add((property.Name, jsonValue));
                    continue;
                }

                string value = jsonValue.ToString();
                var typeKey = _seedFilesChecker?.GetKeyType(value);
                switch (typeKey)
                {
                    case TypeKey.NotKey:
                        _buffer.Add((property.Name, jsonValue));
                        continue;

                    case TypeKey.PK:
                        if (_hashPkKeyExist.Contains(value))
                        {
                            return false;
                        }
                        else
                        {
                            _buffer.Add((property.Name, JsonElementGUID(value)));
                            continue;
                        }

                    case TypeKey.FK:
                        if (_hashFKeyFileName.Contains((value, _currentEntityName!)))
                        {
                            _buffer.Add((property.Name, JsonElementGUID(value)));
                            continue;
                        }
                        else
                            return false;
                }
            }

            return true;
        }

        private void PrepareDataFile(JsonElement jsonObject)
        {
            if (_timeWriter == null)
                throw new NotImplementedException();

            var addCurrentObject = FillBufferJsonObject(jsonObject);

            if (addCurrentObject)
            {
                _timeWriter.WriteStartObject();
                foreach ((string name, JsonElement value) in _buffer)
                {
                    _timeWriter.WritePropertyName(name);
                    value.WriteTo(_timeWriter);
                }
                _timeWriter.WriteEndObject();
            }
            _buffer.Clear();
        }

        private static JsonElement NewJsonElement(Guid value) => JsonSerializer.SerializeToElement(value.ToString("D"));

        private JsonElement JsonElementGUID(string value) => NewJsonElement(_finalGuid.GetGuid(value));

        private void ParseJsonArray(JsonElement rootElement)
        {
            if (rootElement.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException("[ParseJsonArray]: [First element] not is JsonValueKind.Array");

            // цикл по объектам 
            foreach (JsonElement jsonObject in rootElement.EnumerateArray())
            {
                if (jsonObject.ValueKind != JsonValueKind.Object)
                    throw new InvalidDataException("[ParseJsonArray]: In JsonArray not only JsonValueKind.Object");

                if (_actionsForJsonObject == null)
                    throw new InvalidOperationException("[ParseJsonArray]: Not set jsonObjectAction before call");

                _actionsForJsonObject(jsonObject);
            }
        }
    }
}
