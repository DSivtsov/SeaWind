using Infrastructure.Postgres.Seeding.Shared;
using System.Diagnostics;
using System.Text.Json;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles
{
    internal class DataFilesCreator
    {
        // Template "Demo{Entity}.seed" without extension
        private const string DEMO_PREFIX = "demo";
        private const string SEED_SUFFIX = ".seed";

        private readonly IEnumerable<string> _seedFilePaths;
        private readonly Dictionary<string, Guid> _dictPKeyGuid;
        private readonly HashSet<(string fKey, string fileName)> _hashFKeyFileName;
        private List<(string pKey, string fileName)> _listDublicatePK;
        private string? _currentEntityName;
        private string? _currentFileName;
        private List<(string entityName, JsonElement rootElement)> _seedEntityAndRootJsonElements = new ();
        private readonly List<(string Name, JsonElement Value)> _buffer = new();
        private Action<JsonElement>? _actionsForJsonObject;
        private HashSet<string> _hashPkKeyExist = new();

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

        public DataFilesCreator(IEnumerable<string> seedFilePaths, Dictionary<string, Guid> pKeyGuid,
            HashSet<(string, string)> hashFKeyFileName, List<(string pKey, string fileName)> listDublicatePK)
        {
            _seedFilePaths = seedFilePaths;
            _dictPKeyGuid = pKeyGuid;
            _hashFKeyFileName = hashFKeyFileName;
            _listDublicatePK = listDublicatePK;
        }


        public void Create()
        {
            Debug.WriteLine($"CurrentDirectory[{Directory.GetCurrentDirectory()}]");
            foreach ((string entityName, JsonElement rootElement) item in _seedEntityAndRootJsonElements)
            {
                _currentEntityName = item.entityName;

                Debug.WriteLine($"entityName[{_currentEntityName}]");

                CreateDataFile(item.rootElement); /// with SaveData
            }
            return;
        }

        public void ReedSeedFiles()
        {

            foreach (var path in _seedFilePaths!)
            {
                _currentFileName = Path.GetFileName(path);
                Debug.WriteLine($"[{path}]");

                if (!GetAndSetCurrentEntityName(path))
                {
                    throw new InvalidDataException($"DataFile preparing is stopped : Not correct seed file {path}");
                }
                Debug.WriteLine($"_currentEntityName=[{_currentEntityName}]");

                Debug.WriteLine($"[{Path.GetFileNameWithoutExtension(path)}]");
                ParseAndScanSeedFile(path);
            }
            return;
        }

        private Utf8JsonWriter? _timeWriter;

        private void CreateDataFile(JsonElement rootElement)
        {
            _currentFileName = GetNameCurrentDataFile();
            Debug.WriteLine($"_currentFileName[{_currentFileName}]");

            var options = new JsonWriterOptions
            {
                Indented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            using var fs = File.Create(_currentFileName);
            using var writer = new Utf8JsonWriter(fs, options);
            _timeWriter = writer;
            writer.WriteStartArray();

            ParseSeedFileAndSaveData(rootElement);

            writer.WriteEndArray();
            writer.Flush();

        }

        private void ParseAndScanSeedFile(string pathfile)
        {
            using var fs = File.OpenRead(pathfile);
            using var doc = JsonDocument.Parse(fs);

            JsonElement rootElement = doc.RootElement;

            _actionsForJsonObject = ParseAndCheckObjects;

            ParseJsonArray(rootElement);

            _seedEntityAndRootJsonElements.Add((_currentEntityName!, rootElement.Clone()));
        }

        private void ParseSeedFileAndSaveData(JsonElement rootElement)
        {
            _actionsForJsonObject = PrepareDataFile;

            ParseJsonArray(rootElement); /// with ParseAndScan
        }


        private void PrepareDataFile(JsonElement jsonObject)
        {
            if (_timeWriter == null)
                throw new NotImplementedException();

            var addCurrentObject = PrepareJsonObject(jsonObject);

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

        private bool PrepareJsonObject(JsonElement jsonObject)
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
                var typeKey = GetKey(value);
                try
                {
                    switch (typeKey)
                    {
                        case TypeKey.NotKey:
                            _buffer.Add((property.Name, jsonValue));
                            continue;
                        case TypeKey.PK:
                            if (_hashPkKeyExist.Contains(value))
                            {
                                Debug.WriteLine($"[getRec]Skiped dublicated [PK] [{value}]");
                                return false;
                            }
                            else
                            {
                                _buffer.Add((property.Name, NewJsonElement(_dictPKeyGuid[value])));
                                continue;
                            }
                        case TypeKey.FK:
                            if (_hashFKeyFileName.Contains((value, _currentEntityName!)))
                            {
                                _buffer.Add((property.Name, NewJsonElement(_dictPKeyGuid[value])));
                                continue;
                            }
                            else
                                return false;
                    }
                }
                catch (Exception)
                {
                    throw new NotSupportedException($"[ParseObject] Switch key '{typeKey}' is not supported.");
                }
            }

            return true;
        }

        private static JsonElement NewJsonElement(Guid value) => JsonSerializer.SerializeToElement(value.ToString("D"));

        private void ParseAndCheckObjects(JsonElement jsonObject)
        {
            foreach (JsonProperty property in jsonObject.EnumerateObject())
            {

                JsonElement jsonValue = property.Value;
                if (jsonValue.ValueKind == JsonValueKind.Object || jsonValue.ValueKind == JsonValueKind.Array)
                {
                    continue;
                    //throw new NotImplementedException("[ParseObject]: value.ValueKind is JsonValueKind.Object или JsonValueKind.Array");
                }

                string value = jsonValue.ToString();

                var typeKey = GetKey(value);

                try
                {
                    switch (typeKey)
                    {
                        case TypeKey.NotKey:
                            continue;
                        case TypeKey.PK:
                            Debug.WriteLine($"[PK] [{value}]");
                            if (_dictPKeyGuid.ContainsKey(value))
                            {
                                _listDublicatePK.Add((value, _currentEntityName!));
                            }
                            else
                            {
                                _dictPKeyGuid.Add(value, Guid.Empty);
                            }
                            break;
                        case TypeKey.FK:
                            Debug.WriteLine($"[FK] [{value}]");
                            _hashFKeyFileName.Add((value, _currentEntityName!));
                            break;
                    }
                }
                catch (Exception)
                {
                    throw new NotSupportedException("[ParseObject]: switch (typeKey) not Supported Exception");
                }


            }
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

                if (_actionsForJsonObject == null)
                    throw new InvalidOperationException("[ParseJsonArray]: Not set jsonObjectAction before call");

                _actionsForJsonObject(jsonObject);
            }
        }

        private enum TypeKey
        {
            NotKey,
            PK,
            FK
        }

        // Template !{entity}.1
        private TypeKey GetKey(string key)
        {
            if (key.Length < 4 || key[0] != '!') return TypeKey.NotKey;

            var dot = key.IndexOf('.');
            if (dot <= 1 || dot >= key.Length - 1) return TypeKey.NotKey;

            ReadOnlySpan<char> s = key.AsSpan();

            var entityName = s.Slice(1, dot - 1);

            if (entityName.AllLetters() && s.Slice(dot + 1).AllDigits())
            {
                return entityName.SequenceEqual(_currentEntityName) ? TypeKey.PK : TypeKey.FK;
            }
            else
                return TypeKey.NotKey;

        }

        private bool GetAndSetCurrentEntityName(string fileName)
        {
            ReadOnlySpan<char> key = Path.GetFileNameWithoutExtension(fileName);

            if (key.Length < DEMO_PREFIX.Length + SEED_SUFFIX.Length + 1)
                return false; // слишком коротко: "demoX.seed" минимум

            key = key.ToString().ToLowerInvariant();

            if (!key.StartsWith(DEMO_PREFIX) || !key.EndsWith(SEED_SUFFIX))
                return false;

            int start = DEMO_PREFIX.Length;
            int end = key.Length - SEED_SUFFIX.Length;
            var entityName = key[start..end].ToString();

            if (string.IsNullOrWhiteSpace(entityName))
                return false;
            else
            {
                _currentEntityName = entityName;
                return true;
            }
        }
    }
}
