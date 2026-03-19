namespace Api.Dtos;

public static class MessagesQueryDtoExtensions
{
    public static bool HasNoRangeSet(this MessagesQueryDto query)
    {
        return query.BeginSeq is null || query.TillSeq is null;
    }
}

