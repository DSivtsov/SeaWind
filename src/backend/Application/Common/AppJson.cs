using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Common;


public static class AppJson
{
    // Единственный общий набор правил JSON для всего решения
    public static readonly JsonSerializerOptions SerializerOpt = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        var opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            AllowTrailingCommas = true
        };

        opts.Converters.Add(new JsonStringEnumConverter());

        return opts;
    }
}
