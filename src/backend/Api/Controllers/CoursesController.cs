using Application.Abstractions.Services;
using Application.DtoCourse;
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
    /// Возвращает коллекцию курсов. Если курсы не найдены возвращает пустую коллекцию.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CourseDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAllCoursesAsync()
    {
        IEnumerable<CourseDto> dtos = await _service.GetAllCoursesAsync();

        //Response.Headers["Cache-Control"] = "public, max-age=60";

        return Ok(dtos);
    }

    /// <summary>
    /// Получить курс по его Id
    /// </summary>
    /// <param name="courseId">Id курса</param>
    /// <returns>
    /// Возвращает курс.
    /// Если курсы не найден возвращает <see cref="ActionResult"/> с кодом 404.
    /// </returns>
    [HttpGet("{courseId}")]
    [Authorize]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> GetCourseByIdAsync([FromRoute] string courseId)
    {
        var dto = await _service.GetCourseByIdAsync(courseId);

        if (dto is null) return NotFound();

        return Ok(dto);
    }

    /// <summary>
    /// Получить все лекции курса по courseId
    /// </summary>
    /// <param name="courseId">Id курса</param>
    /// <returns>
    /// Возвращает коллекцию лекций курса, отсортированных по возрастанию OrderNo.
    /// Если лекции не найдены — возвращает пустую коллекцию.
    /// </returns>
    [HttpGet("{courseId}/lectures")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<LectureListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LectureListItemDto>>> GetAllLecturesByCourseIdOrderedAsync(
        [FromRoute] string courseId)
    {
        var dtos = await _service.GetAllLecturesByCourseIdAsync(courseId);

        return Ok(dtos);
    }
    
}
