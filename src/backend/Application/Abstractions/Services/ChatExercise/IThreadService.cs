using Api.Dtos;
using Application.Common.Enums;
using Application.Dto.CurrentUser;
using Application.Dto.Exercise;

namespace Application.Abstractions.Services.ChatExercise
{
    public interface IThreadService
    {
        Task<ChangeStatusThreadResponse> ChangeStatusThreadAsync(string threadId, CurrentUserInfo currentUser,
            StatusStudentExercise newStatus, int? mark, CancellationToken ct);

        Task CheckThreadAccessAsync(string threadId, CurrentUserInfo currentUser, CancellationToken ct);

        Task<ExerciseChatDto> GetOrCreateExerciseChatAsync(Guid exerciseId, string userId,
            CancellationToken ct);

        Task<ExerciseChatDto> GetOrOpenExerciseChatForMentorAsync(Guid exerciseId, string mentorId,
            string studentId, CancellationToken ct);
    }
}
