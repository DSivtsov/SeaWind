using System.Text.Json;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles;

internal sealed class TableAnalysis
{
    public IReadOnlyDictionary<string, string> PKeys { get; }
    public IReadOnlyCollection<(string entityName, JsonElement rootElement)> RootJsonElementsEntities { get; }
    public IReadOnlyCollection<string> ValidationWarnings { get; }

    public bool IsValidationWarningsExist => ValidationWarnings.Count > 0;

    internal TableAnalysis(IReadOnlyDictionary<string, string> dictPKeyGuid,
                        IReadOnlyCollection<(string entityName, JsonElement rootElement)> rootJsonElementsEntities,
                        IReadOnlyCollection<string> validationWarnings)
    {
        PKeys = dictPKeyGuid;
        RootJsonElementsEntities = rootJsonElementsEntities;
        ValidationWarnings = validationWarnings;
    }
}
