using Application.Common;
using System.Text.Json;

namespace Application.Models;

public static class ExerciseContentBlockSerializer
{

    public static List<ExerciseContentBlock> ReadBlocks(ExerciseContent content)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));

        var json = content.ContentBlocksJson;
        if (string.IsNullOrWhiteSpace(json))
            return new List<ExerciseContentBlock>();

        return JsonSerializer.Deserialize<List<ExerciseContentBlock>>(json, AppJson.SerializerOpt)
               ?? new List<ExerciseContentBlock>();
    }

    public static ExerciseContent WriteBlocks(ExerciseContent content, List<ExerciseContentBlock> blocks)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));

        string contentBlocksJson =
            JsonSerializer.Serialize(blocks ?? new List<ExerciseContentBlock>(), AppJson.SerializerOpt);

        return new ExerciseContent(content.ExerciseId, content.Details,contentBlocksJson);
    }
}
