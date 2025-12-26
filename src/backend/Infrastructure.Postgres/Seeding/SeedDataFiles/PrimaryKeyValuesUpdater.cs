using Infrastructure.Postgres.Seeding.Shared;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles;

internal class PrimaryKeyValuesUpdater
{
    private readonly UUIDMode _modeUUID;

    private Dictionary<string, string> _dictUpdPKeys = new();

    internal PrimaryKeyValuesUpdater(UUIDMode modeUUID)
    {
        _modeUUID = modeUUID;
    }

    internal string GetPKeyRelValue(string pkRef) => _dictUpdPKeys[pkRef];

    internal bool TryGetPKeyRelValue(string pkRef, out string? pkRealValue) => _dictUpdPKeys.TryGetValue(pkRef, out pkRealValue);

    internal void UpdatePKeyValues(IReadOnlyDictionary<string, string> pKeysRaw)
    {
        Func<string, string> GenerateGuid = _modeUUID switch
        {
            UUIDMode.Real => (_) => Guid.NewGuid().ToString("D"),
            UUIDMode.Stable => (key) => Uuid5.CreateFast(key).ToString("D"),
            _ => throw new InvalidDataException($"[UpdatePKeyValues]: not support modeUUID[{_modeUUID}]"),
        };

        foreach (var (pkRef, pkRealValue) in pKeysRaw)
        {
            if (pkRealValue != string.Empty)
            {
                _dictUpdPKeys.Add(pkRef, pkRealValue);
            }
            else
            {
                _dictUpdPKeys.Add(pkRef, GenerateGuid(pkRef));
            }
        }
    }
}
