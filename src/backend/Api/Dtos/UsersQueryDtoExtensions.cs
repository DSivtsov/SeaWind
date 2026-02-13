namespace Api.Dtos;

public static class UsersQueryDtoExtensions
{
    public static bool IsEmpty(this UsersQueryDto query)
    {
        return query.UserName is null && query.Role is null;
    }
}

