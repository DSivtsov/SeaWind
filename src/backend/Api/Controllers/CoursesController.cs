using Api.Models;
using Application.Abstractions.Services;
using Application.Dto.Course;
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
    [ProducesResponseType(typeof(IEnumerable<CourseShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CourseShortResponse>>> GetAll()
    {
        IEnumerable<CourseDto>? courses = await _service.GetAllAsync();

        if (courses is null)
        {
            return Ok(Array.Empty<CourseShortResponse>());
        }

        var coursesResponse = courses
            .Select(c => new CourseShortResponse(c.Id, c.Title, c?.Description));

        return Ok(coursesResponse);
    }
}
