using Api.Dtos;
using Application.Abstractions.Repositories;
using Application.Abstractions.Repositories.ChatExercise;
using Application.Abstractions.Services.ChatExercise;
using Application.Common.Enums;
using Application.Common.Exceptions;
using Application.Dto.CurrentUser;
using Application.Dto.Exercise;
using Application.Models;

namespace Application.UseCases.ChatExercise;

public sealed class ThreadService : IThreadService
{
    private readonly IThreadRepository _threadRepository;
    private readonly IStudentExerciseRepository _studentExerciseRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ILockService _lockService;

    public ThreadService(IThreadRepository threadRepository, IStudentExerciseRepository studentExerciseRepository,
        IExerciseRepository exerciseRepository, ILockService lockService)
    {
        _threadRepository = threadRepository;
        _studentExerciseRepository = studentExerciseRepository;
        _exerciseRepository = exerciseRepository;
        _lockService = lockService;
    }

    public async Task CheckThreadAccessAsync(string threadId, CurrentUserInfo currentUser, CancellationToken ct)
    {
        if (!Enum.TryParse<ExerciseChatRole>(currentUser.Role, out var chatRole))
            throw new ArgumentException();

        if (chatRole == ExerciseChatRole.Mentor)
            return;

        bool isUserOwnerThread = await _studentExerciseRepository.IsUserThreadOwner(currentUser.UserId, threadId, ct);

        if (!isUserOwnerThread)
            throw new ArgumentException();
    }

    private async Task<ExerciseChatDto> GetExerciseChatData(StudentExerciseDto exerciseDto, CancellationToken ct)
    {
        var threadLocksDto = await _threadRepository.GetThreadLocksDataAsync(exerciseDto.ThreadId, ct);

        return new ExerciseChatDto(exerciseDto, threadLocksDto);
    }

    public async Task<ExerciseChatDto> GetOrCreateExerciseChatAsync(Guid exerciseId, string userId, CancellationToken ct)
    {
        var exerciseDto = await _studentExerciseRepository.GetStudentExerciseAsync(exerciseId, userId, ct);

        if (exerciseDto is null)
        {
            var courseId = await _exerciseRepository.GetCourseIdByExerciseIdAsync(exerciseId, ct);
            var candidateThreadId = await _threadRepository.CreateNewThreadAsync(ct);

            StudentExercise newEntity = new(userId, exerciseId, courseId, candidateThreadId);

            exerciseDto = await _studentExerciseRepository.TryInsertNewStudentExerciseOrReadExistingAsync(newEntity, ct);
        }

        return await GetExerciseChatData(exerciseDto, ct);
    }


    public async Task<ExerciseChatDto> GetOrOpenExerciseChatForMentorAsync(Guid exerciseId, string mentorId,
        string studentId, CancellationToken ct)
    {
        var exerciseDto = await _studentExerciseRepository.GetStudentExerciseForMentorAsync(exerciseId,
            mentorId, studentId, ct);

        if (exerciseDto is not null)
            return await GetExerciseChatData(exerciseDto, ct);

        var existingStudentExercise = await _studentExerciseRepository.GetExistingStudentExerciseIdAsync(exerciseId,
            studentId, ct);

        if (existingStudentExercise == Guid.Empty)
            throw new NotFoundException($"Упражнение c id [{exerciseId}] для студента [{studentId}] еще не создано.");

        var mentorWasAssigned = await _studentExerciseRepository.TryAssignMentorToStudentExerciseAsync(existingStudentExercise,
            mentorId, ct);

        if (!mentorWasAssigned)
            throw new ConflictException($"Другой ментор проверяет упраженение [{exerciseId}] для студента [{studentId}].");

        var exerciseDtoForNewMentor = await _studentExerciseRepository.GetStudentExerciseForMentorAsync(exerciseId,
            mentorId, studentId, ct);

        if (exerciseDtoForNewMentor is null)
            throw new InvariantViolationException(
                $"StudentExercise [{exerciseId}] for student [{studentId}] was assigned to mentor [{mentorId}]," +
                $" but cannot be read back.");

        return await GetExerciseChatData(exerciseDtoForNewMentor, ct);
    }

    public async Task<ChangeStatusThreadResponse> ChangeStatusThreadAsync(string threadId, CurrentUserInfo currentUser,
        StatusStudentExercise newStatus, int? mark, CancellationToken ct)
    {
        var chatRole = Enum.Parse<ExerciseChatRole>(currentUser.Role);
        var lockOwnerId = Guid.NewGuid().ToString();
        var locked = await _lockService.AcquireLockAsync(threadId, lockOwnerId, ct);
        if (!locked)
            throw new ConflictException($"Чат {threadId} пока занят другими операциями записи.");

        try
        {
            switch ((chatRole, newStatus))
            {
                case (ExerciseChatRole.Student, newStatus: StatusStudentExercise.OnMentor):
                case (ExerciseChatRole.Mentor, newStatus: StatusStudentExercise.OnStudent)
                        when mark is null or (>= 0 and <= 2):
                    await _studentExerciseRepository.UpdateStatusAsync(threadId, chatRole, mark, newStatus, ct);
                    break;
                default:
                    throw new ValidationException("Неподдерживаемые вариант смены статуса упражнения.");
            }

            return await _threadRepository.UpdateThreadLockSeq(threadId, chatRole, ct);
        }
        finally
        {
            await _lockService.ReleaseLockAsync(threadId, lockOwnerId, CancellationToken.None);
        }
    }
}
