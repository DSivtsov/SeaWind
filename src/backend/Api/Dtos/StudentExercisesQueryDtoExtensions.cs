namespace Api.Dtos;

public static class StudentExercisesQueryDtoExtensions
{
    public static bool IsEmpty(this StudentExercisesQueryDto query)
    {
        return query.Email is null && query.OnlyOnCheck is null;
    }
}

