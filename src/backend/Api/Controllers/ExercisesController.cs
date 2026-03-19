using Api.Dtos;
using Application.Abstractions.Services;
using Application.Abstractions.Services.ChatExercise;
using Application.Common.Enums;
using Application.Common.Exceptions;
using Application.Dto.Exercise;
using Application.DtoCourse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class ExercisesController : ControllerBase
{
    private readonly IExerciseService _exerciseService;
    private readonly IThreadService _exerciseChatService;
    private readonly ICurrentUserService _currentUserService;

    public ExercisesController(IExerciseService exerciseService, IThreadService exerciseChatService,
        ICurrentUserService currentUserService)
    {
        _exerciseService = exerciseService;
        _exerciseChatService = exerciseChatService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Получить данные упражнения по его Id
    /// </summary>
    /// <param name="exerciseId">Id упражнения</param>
    /// <returns>
    /// Возвращает данные упраженения.
    /// Если упраженение не найдено возвращает <see cref="ActionResult"/> с кодом 404.
    /// </returns>
    [HttpGet("{exerciseId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExerciseDto>> GetExercise([FromRoute] Guid exerciseId)
    {
        var dto = await _exerciseService.GetExerciseAsync(exerciseId);

        if (dto is null)
            throw new NotFoundException($"Упражнение [{exerciseId}] не найдено.");

        return Ok(dto);
    }

    /// <summary>
    /// Получить дополнительные данные упражнения по его Id
    /// </summary>
    /// <param name="exerciseId">Идентификатор упражнения</param>
    /// <returns>
    /// Возвращает <see cref="ExerciseContentDto"/> — детальное описание
    /// и набор блоков дополнительного контента упражнения.
    /// Если дополнительные данные упражнения не найдены возвращает <see cref="ActionResult"/> с кодом 404.
    /// </returns>
    [HttpGet("{exerciseId:guid}/content")]
    [Authorize]
    [ProducesResponseType(typeof(ExerciseContentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExerciseContentDto>> GetExerciseContent([FromRoute] Guid exerciseId)
    {
        var dto = await _exerciseService.GetExerciseContentAsync(exerciseId);

        if (dto is null)
            throw new NotFoundException($"Дополнительные данные упражнения [{exerciseId}] не найдены.");

        return Ok(dto);
    }

    /// <summary>
    /// Возвращает данные чата для упражнения.
    /// Если запись StudentExercise и чат-тред для данного пользователя
    /// и упражнения ещё не существуют, они будут созданы.
    /// </summary>
    /// <param name="exerciseId">Идентификатор упражнения.</param>
    /// <param name="query">
    /// Индификатор студента:
    /// Необходимо указать для ментора, индификатор студента чат которого он хочет открыть.
    /// </param>
    /// <param name="ct">Токен отмены HTTP-запроса.</param>
    /// <returns>
    /// DTO с данными чата (threadId, статус упражнения, оценка, lockSeq).
    /// </returns>
    /// <exception cref="InvariantViolationException">
    /// Выбрасывается, если в токене пользователя отсутствует claim NameIdentifier.
    /// </exception>
    [HttpGet("{exerciseId:guid}/chat-thread")]
    [Authorize]
    [ProducesResponseType(typeof(ExerciseChatDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExerciseChatDto>> GetOrCreateExerciseChatData(
        [FromRoute] Guid exerciseId,
        [FromQuery] ExerciseChatQueryDto query,
        CancellationToken ct)
    {
        var currentUser = _currentUserService.GetUserInfo();
        var userRole = Enum.Parse<ExerciseChatRole>(currentUser.Role);

        if (userRole == ExerciseChatRole.Student)
        {
            var dto = await _exerciseChatService.GetOrCreateExerciseChatAsync(exerciseId, currentUser.UserId, ct);
            return Ok(dto);
        }

        if (userRole == ExerciseChatRole.Mentor)
        {
            if (query.StudentId is null)
                throw new ValidationException("Необходимо указать studentId для открытия dashboard от имени ментора.");

            var dto = await _exerciseChatService.GetOrOpenExerciseChatForMentorAsync(exerciseId, currentUser.UserId,
                query.StudentId, ct);

            return Ok(dto);
        }

        throw new InvariantViolationException("Недопустимый вариант вызова.");
    }
}
