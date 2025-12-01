using Infrastructure.Postgres.Seeding.Shared;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles;

internal class HelperSeedFilesChecker
{
    // Template "Demo{Entity}.seed" without extension
    private const string DEMO_PREFIX = "demo";
    private const string SEED_SUFFIX = ".seed";

    public string? CurrentEntitytName { get; internal set; }
    
    public HelperSeedFilesChecker(string currentEntitytName)
    {
        CurrentEntitytName = currentEntitytName;
    }

    public static string? GetEntityName(string path)
    {
        ReadOnlySpan<char> key = Path.GetFileNameWithoutExtension(path);

        if (key.Length < DEMO_PREFIX.Length + SEED_SUFFIX.Length + 1)
            return null;

        key = key.ToString().ToLowerInvariant();

        if (!key.StartsWith(DEMO_PREFIX) || !key.EndsWith(SEED_SUFFIX))
            return null;

        int start = DEMO_PREFIX.Length;
        int end = key.Length - SEED_SUFFIX.Length;
        var entityName = key[start..end].ToString();

        return string.IsNullOrWhiteSpace(entityName) ? null : entityName;
    }

    // Template !{entity}.1
    public TypeKey GetKeyType(string key)
    {
        if (key.Length < 4 || key[0] != '!') return TypeKey.NotKey;

        var dot = key.IndexOf('.');
        if (dot <= 1 || dot >= key.Length - 1) return TypeKey.NotKey;

        ReadOnlySpan<char> s = key.AsSpan();

        var entityName = s.Slice(1, dot - 1);

        if (entityName.AllLetters() && s.Slice(dot + 1).AllDigits())
        {
            return entityName.SequenceEqual(CurrentEntitytName) ? TypeKey.PK : TypeKey.FK;
        }
        else
            return TypeKey.NotKey;

    }
}