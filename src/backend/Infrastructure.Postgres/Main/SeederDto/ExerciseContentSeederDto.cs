namespace Infrastructure.Postgres.Main.SeederDto;

public class ExerciseContentSeederDto
{
    public Guid ExerciseId { get; set; } = default!;

    public string Details { get; set; } = default!;

    public List<ExerciseContentBlockSeederDto> ContentBlocks { get; set; } = new();
}

public enum KindExerciseContentBlock { Picture, Code }
public enum TypeExerciseContentBlock { CSharp, Json, Text }

public sealed record ExerciseContentBlockSeederDto(
    KindExerciseContentBlock Kind,
    string UrlFile,
    TypeExerciseContentBlock? TypeContent
);
