using System.Text.Json;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles;

public sealed record EntitySeedData(
    string EntityName,
    JsonElement RootElement
);

internal sealed class TableAnalysis
{
    public IReadOnlyDictionary<string, string> PKeys { get; }
    public IReadOnlyDictionary<string, EntitySeedData> EntityData { get; }
    public IReadOnlyCollection<string> ValidationWarnings { get; }

    public bool IsValidationWarningsExist => ValidationWarnings.Count > 0;

    internal TableAnalysis(IReadOnlyDictionary<string, string> dictPKeyGuid,
                        IReadOnlyDictionary<string, EntitySeedData> entityData,
                        IReadOnlyCollection<string> validationWarnings)
    {
        PKeys = dictPKeyGuid;
        EntityData = entityData;
        ValidationWarnings = validationWarnings;
    }
}
