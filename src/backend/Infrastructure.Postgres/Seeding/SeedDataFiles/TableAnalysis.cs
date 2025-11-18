using System.Text.Json;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles;

internal sealed class TableAnalysis
{
    public IReadOnlyDictionary<string, Guid> PKeys { get; }
    public IReadOnlySet<(string fKey, string fileName)> FKeys { get; }
    public IReadOnlyCollection<(string entityName, JsonElement rootElement)> RootJsonElementsEntities { get; }
    public IReadOnlyCollection<string> ValidationWarnings { get; }

    public bool IsValidationWarningsExist => ValidationWarnings.Count > 0;

    internal TableAnalysis(IReadOnlyDictionary<string, Guid> dictPKeyGuid,
                        IReadOnlySet<(string fKey, string fileName)> hashFKeyFileName,
                        IReadOnlyCollection<(string entityName, JsonElement rootElement)> rootJsonElementsEntities,
                        IReadOnlyCollection<string> validationWarnings)
    {
        PKeys = dictPKeyGuid;
        FKeys = hashFKeyFileName;
        RootJsonElementsEntities = rootJsonElementsEntities;
        ValidationWarnings = validationWarnings;
    }
}
