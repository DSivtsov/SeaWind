using Api.Exceptions;
using Application.Abstractions.Services;
using Application.DtoCourse;
using Application.DtoTime.Tester;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;
    public CoursesController(ICourseService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить все доступные курсы
    /// </summary>
    /// <returns>
    /// Возвращает коллекцию курсов. Если курсы не найдены возвращает пустую коллекцию
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CourseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll()
    {
        IEnumerable<CourseDto>? dtos = await _service.GetAllAsync();

        if (dtos is null)
        {
            return Ok(Array.Empty<CourseDto>());
        }

        Response.Headers["Cache-Control"] = "public, max-age=60";

        return Ok(dtos);
    }

    /// <summary>
    /// Получить курс по его Id</summary>
    /// <param name="courseId">Id курса</param>
    /// <returns>
    /// Возвращает курс.
    /// Если курсы не найден возвращает <see cref="ActionResult"/> с кодом 404
    /// </returns>
    [HttpGet("{courseId}")]
    [Authorize]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> GetByIdAsync([FromRoute] string courseId)
    {
        var dto = await _service.GetByIdAsync(courseId);

        if (dto is null) return NotFound();

        return Ok(dto);
    }
}
