using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Postgres.Seeding.Shared;

internal class ParseJsonHelper
{
    private static readonly JsonSerializerOptions _opts;

    static ParseJsonHelper()
    {
        _opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            AllowTrailingCommas = true
        };
    }

    public readonly record struct ParseResult<TValue>(bool Ok, TValue? Value, string? Error)
    {
        public static ParseResult<TValue> Fail(string error) => new(false, default, error);
        public static ParseResult<TValue> Success(TValue value) => new(true, value, null);
    }

    public static ParseResult<T> TryParse<T>(string payload)
    {
        try
        {
            var items = JsonSerializer.Deserialize<T>(payload, _opts);
            return items is null
                ? ParseResult<T>.Fail("Empty or null JSON.")
                : ParseResult<T>.Success(items);
        }
        catch (JsonException je)
        {
            // давай полезную диагностику клиенту/логу
            var detail = $"Invalid JSON at '{je.Path}' (line {je.LineNumber}, pos {je.BytePositionInLine}): {je.Message}";
            return ParseResult<T>.Fail(detail);
        }
        catch (Exception)
        {
            // всё прочее — как 500, не светим детали наружу
            return ParseResult<T>.Fail("Unexpected error while parsing JSON.");
        }
    }
}
