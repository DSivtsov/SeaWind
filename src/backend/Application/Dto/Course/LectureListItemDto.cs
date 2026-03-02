namespace Application.DtoCourse;

/// <summary>
/// LectureListItemDto для GetAllLecturesByCourseIdAsync
/// </summary>
public record LectureListItemDto(Guid Id, int OrderNo, string Title, string? videoUrl, string? Description);
