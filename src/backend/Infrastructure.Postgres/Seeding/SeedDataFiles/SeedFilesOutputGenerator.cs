using Infrastructure.Postgres.Seeding.Shared;
using System.Text.Json;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles
{
    internal class SeedFilesOutputGenerator
    {
        private readonly PrimaryKeyValuesUpdater _pKeysUpdater;
        private readonly IReadOnlyDictionary<string, EntitySeedData> _entityData;
        private readonly string _pathBase;
        private readonly HashSet<string> _existPKeyRef = new();
        private string? _currentEntityKey;

        internal SeedFilesOutputGenerator(PrimaryKeyValuesUpdater pKeysUpdater,
            IReadOnlyDictionary<string, EntitySeedData> entityData,
                string pathBase)
        {
            _pKeysUpdater = pKeysUpdater ?? throw new ArgumentNullException(nameof(pKeysUpdater));
            _entityData = entityData ?? throw new ArgumentNullException(nameof(entityData));
            _pathBase = pathBase;
        }

        internal void Generate()
        {
            foreach (KeyValuePair<string, EntitySeedData> item in _entityData)
            {
                _currentEntityKey = item.Key;
                CreateDataFileForEntity(item.Value);
            }
        }

        private void CreateDataFileForEntity(EntitySeedData entityData)
        {
            var currentFileName = SeedFilesConvention.GenerateDataFileName(entityData.EntityName);

            using var currentWriter = new DataFileWriter(Path.Combine(_pathBase, currentFileName));

            currentWriter.BeginWriteFile();

            WriteJsonElement(currentWriter, entityData.RootElement);

            currentWriter.EndWriteFile();
        }

        private void WriteJsonElement(DataFileWriter currentWriter, JsonElement rootElement)
        {
            if (rootElement.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException("[WriteJsonElement]: [First element] not is JsonValueKind.Array");

            // цикл по объектам 
            foreach (JsonElement jsonObject in rootElement.EnumerateArray())
            {
                if (jsonObject.ValueKind != JsonValueKind.Object)
                    throw new InvalidDataException("[WriteJsonElement]: In JsonArray not only JsonValueKind.Object");

                (bool isCorrectRec, List<(string Name, JsonElement Value)>? buffer) rez = GenerateJsonObject(jsonObject);

                if (rez.isCorrectRec)
                    currentWriter.WriteJsonObject(rez.buffer!);
            }
        }

        private (bool isCorrectRec, List<(string Name, JsonElement Value)>? buffer) GenerateJsonObject(JsonElement jsonObject)
        {
            var seedFilesChecker = new SeedFilesConvention(_currentEntityKey!);
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

                //skip temporary column with IDValue from final file
                if (property.NameEquals(SeedConst.ColumnIdValue)) continue;

                string refPKeyLowered = jsonValue.ToString().ToLowerInvariant();
                var typeKey = seedFilesChecker?.GetKeyType(refPKeyLowered);

                switch (typeKey)
                {
                    case TypeKey.NotKey:
                        buffer.Add((property.Name, jsonValue));
                        continue;

                    case TypeKey.PK:
                    case TypeKey.FixPK:
                        // skip rec with dublicate PKeys
                        if (_existPKeyRef.Contains(refPKeyLowered)) return (false, null);

                        // skip rec with invalid PKey Value
                        if (!_pKeysUpdater.TryGetPKeyRelValue(refPKeyLowered, out string? PKeyRealValue)) return (false, null);

                        _existPKeyRef.Add(refPKeyLowered);
                        buffer.Add((property.Name, JsonSerializer.SerializeToElement(PKeyRealValue)));
                        continue;

                    case TypeKey.FK:
                        // skip rec with FK which not exist in dict PKeys
                        if (!_pKeysUpdater.TryGetPKeyRelValue(refPKeyLowered, out string? fkRealValue)) return (false, null);

                        buffer.Add((property.Name, JsonSerializer.SerializeToElement(fkRealValue)));
                        continue;
                }
            }
            return (true, buffer);
        }
    }
}
