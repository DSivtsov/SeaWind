namespace Application.DtoCourse;

/// <summary>
/// LectureListItemDto для GetAllLecturesByCourseIdAsync
/// </summary>
public record LectureListItemDto(Guid Id, string СourseId, int OrderNo, string Title, string? videoUrl, string? Description);
