using Application.Abstractions.Services;
using Application.Dto.Exercise;
using Application.DtoCourse;
using Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class ExercisesController : ControllerBase
{
    private readonly IExerciseService _service;

    public ExercisesController(IExerciseService service)
    {
        _service = service;
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
    public async Task<ActionResult<ExerciseDto>> GetExerciseById([FromRoute] Guid exerciseId)
    {
        var dto = await _service.GetExerciseByIdAsync(exerciseId);

        if (dto is null) return NotFound();

        return Ok(dto);
    }


    [HttpGet("{exerciseId:guid}/content")]
    [Authorize]
    [ProducesResponseType(typeof(ExerciseContentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExerciseContentDto>> GetExerciseContentByExerciseId(
        [FromRoute] Guid exerciseId,
        [FromServices] IExerciseService service)
    {
        var dto = await _service.GetExerciseContentByExerciseId(exerciseId);

        if (dto is null) return NotFound();

        return Ok(dto);
    }
}
