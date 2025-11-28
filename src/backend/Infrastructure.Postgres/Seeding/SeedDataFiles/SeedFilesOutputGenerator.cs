using System.Text.Json;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles
{
    internal class SeedFilesOutputGenerator
    {
        private readonly PrimaryKeyGuidGenerator _finalGuid;
        private readonly IReadOnlyCollection<(string entityName, JsonElement rootElement)> _rootJsonElementsEntities;
        private readonly string _pathBase;
        private readonly HashSet<string> _existPKeys = new();

        internal SeedFilesOutputGenerator(PrimaryKeyGuidGenerator finalGuid,
            IReadOnlyCollection<(string entityName, JsonElement rootElement)> rootJsonElementsEntities,
                string pathBase)
        {
            _finalGuid = finalGuid ?? throw new ArgumentNullException(nameof(finalGuid));
            _rootJsonElementsEntities = rootJsonElementsEntities ?? throw new ArgumentNullException(nameof(rootJsonElementsEntities));
            _pathBase = pathBase;
        }

        internal void Generate()
        {
            foreach ((string entityName, JsonElement rootElement) item in _rootJsonElementsEntities)
            {
                CreateDataFileForEntity(item.entityName, item.rootElement);
            }
        }

        private void CreateDataFileForEntity(string entityName, JsonElement rootElement)
        {
            var currentFileName = GetNameCurrentDataFile(entityName);

            using var currentWriter = new DataFileWriter(Path.Combine(_pathBase, currentFileName));

            currentWriter.BeginWriteFile();

            WriteJsonElement(entityName, currentWriter, rootElement);

            currentWriter.EndWriteFile();
        }

        private void WriteJsonElement(string entityName, DataFileWriter currentWriter, JsonElement rootElement)
        {
            if (rootElement.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException("[ParseJsonArray]: [First element] not is JsonValueKind.Array");

            // цикл по объектам 
            foreach (JsonElement jsonObject in rootElement.EnumerateArray())
            {
                if (jsonObject.ValueKind != JsonValueKind.Object)
                    throw new InvalidDataException("[ParseJsonArray]: In JsonArray not only JsonValueKind.Object");

                (bool isCorrectRec, List<(string Name, JsonElement Value)>? buffer) rez = GenerateJsonObject(entityName, jsonObject);

                if (rez.isCorrectRec)
                    currentWriter.WriteJsonObject(rez.buffer!);
            }
        }

        private (bool isCorrectRec, List<(string Name, JsonElement Value)>? buffer) GenerateJsonObject(string entityName,
            JsonElement jsonObject)
        {
            var seedFilesChecker = new HelperSeedFilesChecker(entityName);
            List<(string Name, JsonElement Value)> buffer = new();

            // цикл по полям объекта
            foreach (JsonProperty property in jsonObject.EnumerateObject())
            {
                JsonElement jsonValue = property.Value;

                if (jsonValue.ValueKind == JsonValueKind.Object || jsonValue.ValueKind == JsonValueKind.Array)
                {
                    buffer.Add((property.Name, jsonValue));
                    continue;
                }

                string value = jsonValue.ToString();
                var typeKey = seedFilesChecker?.GetKeyType(value);

                switch (typeKey)
                {
                    case TypeKey.NotKey:
                        buffer.Add((property.Name, jsonValue));
                        continue;

                    case TypeKey.PK:
                        if (_existPKeys.Contains(value))
                        {
                            // skip rec with dublicate PKeys
                            return (false, null);
                        }
                        else
                        {
                            _existPKeys.Add(value);
                            buffer.Add((property.Name, NewJsonElement(_finalGuid.GetGuid(value))));
                            continue;
                        }

                    case TypeKey.FK:
                        if (_finalGuid.TryGetGuid(value, out Guid guid))
                        {
                            buffer.Add((property.Name, NewJsonElement(guid)));
                            continue;
                        }
                        else
                        {
                            // skip rec with FK which not exist in dict PKeys
                            return (false, null);
                        }
                }
            }

            return (true, buffer);
        }

        private string GetNameCurrentDataFile(ReadOnlySpan<char> name)
        {
            if (name.Length == 0)
                throw new InvalidOperationException("Entity name cannot be empty.");

            string Name = name.Length == 1
                ? char.ToUpperInvariant(name[0]).ToString()
                : string.Concat(char.ToUpperInvariant(name[0]), name[1..].ToString());

            return $"Demo{Name}.json";
        }

        private static JsonElement NewJsonElement(Guid value) => JsonSerializer.SerializeToElement(value.ToString("D"));
    }
}