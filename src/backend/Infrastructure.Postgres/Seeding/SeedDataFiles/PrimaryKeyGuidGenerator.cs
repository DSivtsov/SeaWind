using Infrastructure.Postgres.Seeding.Shared;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles;

internal class PrimaryKeyGuidGenerator
{
    private readonly UUIDMode _modeUUID;

    private Dictionary<string, Guid> _finalGuid = new();

    internal PrimaryKeyGuidGenerator(UUIDMode modeUUID)
    {
        _modeUUID = modeUUID;
    }

    internal Guid GetGuid(string key) => _finalGuid[key];

    internal void Generate(IReadOnlyDictionary<string, Guid> pKeysRaw)
    {
        _finalGuid = _modeUUID switch
        {
            UUIDMode.Real => GenerateGuidReal(pKeysRaw),
            UUIDMode.Stable => GenerateGuidStable(pKeysRaw),
            _ => throw new InvalidDataException($"GeneratorGuid: not support value modeUUID[{_modeUUID}]"),
        };
    }
    private Dictionary<string, Guid> GenerateGuidReal(IReadOnlyDictionary<string, Guid> pKeysRaw)
    {
        return pKeysRaw.ToDictionary(x => x.Key, x => Guid.NewGuid());
    }

    private Dictionary<string, Guid> GenerateGuidStable(IReadOnlyDictionary<string, Guid> pKeysRaw)
    {
        return pKeysRaw.ToDictionary(x => x.Key, x => Uuid5.CreateFast(x.Key));
    }
}
