using Infrastructure.Postgres.Seeding.Shared;
using System.Text.RegularExpressions;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles;

internal class SeedFilesConvention
{
    // Template input file "Demo{Entity}.seed" without extension
    private const string DEMO_PREFIX = "demo";
    private const string SEED_SUFFIX = ".seed";

    // Template output file "Demo{Entity}.seed" with extension
    private const string OUTPUT_FILE_TEMPLATE = "Demo{0}.json";

    /// <summary>
    /// Allowed key format:
    /// - lowercase letters (a–z)
    /// - digits (0–9)
    /// - optional single hyphens between segments
    ///
    /// Examples:
    ///   valid:   course-basic, csharp1, level-2-advanced
    ///   invalid: Course, _test, test--key, -start, end-
    /// </summary>
    private static readonly Regex KeyFormatRegex = new Regex("^[a-z0-9]+(-[a-z0-9]+)*$", RegexOptions.Compiled);

    /// <summary>
    /// In this class, the entity name is always stored in lower case (invariant).
    /// </summary>
    private string _currentEntityNameLowered;
    
    public SeedFilesConvention(string currentEntitytNameLowered)
    {
        _currentEntityNameLowered = currentEntitytNameLowered;
    }

    /// <summary>
    /// Extracts entity name from seed file path.
    /// </summary>
    /// <param name="path">Seed file path.</param>
    /// <returns>
    /// Entity name if successfully extracted; otherwise, null.
    /// </returns>
    public static string? GetEntityName(string path)
    {
        ReadOnlySpan<char> key = Path.GetFileNameWithoutExtension(path);

        if (key.Length < DEMO_PREFIX.Length + SEED_SUFFIX.Length + 1)
            return null;

        if (!key.StartsWith(DEMO_PREFIX, StringComparison.OrdinalIgnoreCase) ||
            !key.EndsWith(SEED_SUFFIX, StringComparison.OrdinalIgnoreCase))
            return null;

        int start = DEMO_PREFIX.Length;
        int end = key.Length - SEED_SUFFIX.Length;

        var entityName = key[start..end];

        return entityName.Length == 0 ? null : entityName.ToString();
    }

    public static string GenerateDataFileName(ReadOnlySpan<char> name)
    {
        if (name.Length == 0)
            throw new InvalidOperationException("[GetNameCurrentDataFile]: Entity name cannot be empty.");

        return string.Format(OUTPUT_FILE_TEMPLATE, name.ToString());
    }

    // Template !{entity}.n & !{entity}.IDn
    public TypeKey GetKeyType(string keyLowered)
    {
        if (keyLowered.Length < 4 || keyLowered[0] != '!') return TypeKey.NotKey;

        var dot = keyLowered.IndexOf('.');
        if (dot <= 1 || dot >= keyLowered.Length - 1) return TypeKey.NotKey;

        ReadOnlySpan<char> s = keyLowered.AsSpan();

        var entityNameLowered = s.Slice(1, dot - 1);
        var tail = s.Slice(dot + 1);

        // !{entity}.{digits}
        if (entityNameLowered.AllLetters() && tail.AllDigits())
        {
            return entityNameLowered.SequenceEqual(_currentEntityNameLowered) ? TypeKey.PK : TypeKey.FK;
        }

        const string LOWERED_PK_SYMBOL = "id";
        // !{entity}.ID{digits} or !{entity}.id{digits}
        if (entityNameLowered.AllLetters() &&
            tail.Length > 2 &&
            tail.StartsWith(LOWERED_PK_SYMBOL.AsSpan()) &&
            tail.Slice(2).AllDigits())
        {
            return entityNameLowered.SequenceEqual(_currentEntityNameLowered) ? TypeKey.FixPK : TypeKey.FK;
        }

        return TypeKey.NotKey;
    }

    /// <summary>
    /// Checks whether the provided value is a valid string key
    /// suitable for use as PK/FK in seed files.
    /// </summary>
    /// <param name="keyValue">Raw key value from seed data.</param>
    /// <returns>
    /// True if the key matches the required format; otherwise false.
    /// </returns>
    public static bool IsValidPKeyValue(string? keyValue)
    {
        return !string.IsNullOrWhiteSpace(keyValue) && KeyFormatRegex.IsMatch(keyValue);
    }
}
