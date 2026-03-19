using Api.Dtos;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Common.Enums;
using Application.Common.Exceptions;
using Application.Dto.ChatExercise;
using Application.Dto.Exercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class MentorController : ControllerBase
{
    private readonly IStudentExerciseRepository _studentExerciseRepository;
    private readonly ICurrentUserService _userService;

    public MentorController(IStudentExerciseRepository studentExerciseRepository, ICurrentUserService userService)
    {
        _studentExerciseRepository = studentExerciseRepository;
        _userService = userService;
    }

    /// <summary>
    /// Возвращает inbox ментора со списком упражнений студентов,
    /// содержащих чат для проверки.
    /// Поддерживается фильтрация по email студента и флагу OnlyOnCheck.
    /// </summary>
    /// <param name="query">Параметры фильтрации списка.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список элементов inbox ментора.</returns>
    [HttpGet("exercise-chats/inbox")]
    [Authorize]
    [ProducesResponseType(typeof(ExerciseChatDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MentorExerciseChatInboxResponse>>> GetMentorExerciseChatInbox(
        [FromQuery] StudentExercisesQueryDto query,
        CancellationToken ct)
    {
        var currentUser = _userService.GetUserInfo();
        var chatRole = Enum.Parse<ExerciseChatRole>(currentUser.Role);

        if (chatRole != ExerciseChatRole.Mentor)
            throw new ForbiddenException("Данный запрос может делать только ментор.");

        List<MentorExerciseChatInboxResponse> dto;
        if (query.IsEmpty())
        {
            dto = await _studentExerciseRepository.GetMentorExerciseChatInbox(ct);
        }
        else
        {
            dto = await _studentExerciseRepository.GetMentorExerciseChatInbox(query.Email, query.OnlyOnCheck, ct);
        }
        return Ok(dto);
    }
}
