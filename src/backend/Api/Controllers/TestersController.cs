using Api.Exceptions;
using Application.Abstractions.Services;
using Application.DtoTester;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public sealed class TestersController : ControllerBase
{
    private readonly ITesterService _svc;
    public TestersController(ITesterService svc) => _svc = svc;

    /// <summary>List users</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TesterDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<TesterDto>> GetAll([FromQuery] int? minAge)
    {
        if (minAge is < 0)
            throw new BadRequestException("minAge must be >= 0");

        var users = _svc.GetAll(minAge);
        return Ok(users);
    }

    /// <summary>Get user by id</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TesterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<TesterDto> GetById([FromRoute] Guid id)
    {
        var user = _svc.Get(id);
        return user is null ? throw new NotFoundException("minAge must be >= 0") : Ok(user);
    }

    /// <summary>Create user</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TesterDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<TesterDto> Create([FromBody] CreateTesterRequest body)
    {
        if (string.IsNullOrWhiteSpace(body.Name))
            throw new BadRequestException("Name is required");

        if (body.Age < 0 || body.Age > 150)
            throw new BadRequestException("Age must be between 0 and 150");

        var result = _svc.Create(body);
        
        if (result.IsConflict)
            return Conflict(result.Error);

        if (result.IsBadReq)
            throw new BadRequestException(result.Error);

        var created = result.Value!;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Update user</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update([FromRoute] Guid id, [FromBody] UpdateTesterRequest body)
    {
        if (string.IsNullOrWhiteSpace(body.Name))
            throw new BadRequestException("Name is required");

        if (body.Age < 0 || body.Age > 150)
            throw new BadRequestException("Age must be between 0 and 150");

        var ok = _svc.Update(id, body);
        return ok ? NoContent() : throw new NotFoundException("Not found User for update");
    }

    /// <summary>Delete user</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete([FromRoute] Guid id)
    {
        var ok = _svc.Delete(id);
        return ok ? NoContent() : throw new NotFoundException("Not found User for delete");
    }

    /// <summary>Export user as CSV</summary>
    [HttpGet("{id:guid}/export")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileResult))]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [Produces("text/csv")]
    public ActionResult<FileResult> ExportCsv([FromRoute] Guid id)
    {
        var user = _svc.Get(id);
        if (user is null)
            throw new NotFoundException("Not found User for export");

        var csv = $"Id,Name,Age{Environment.NewLine}{user.Id},{user.Name},{user.Age}";
        var bytes = System.Text.Encoding.UTF8.GetBytes(csv);

        return File(bytes, "text/csv", $"user-{id}.csv");
    }
}