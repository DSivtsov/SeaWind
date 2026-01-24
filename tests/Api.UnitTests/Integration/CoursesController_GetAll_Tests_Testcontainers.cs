using Api.UnitTests.Integration.Shared;
using Application.DtoCourse;
using Application.Models;
using FluentAssertions;
using Infrastructure.Postgres.Main;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;

namespace Api.UnitTests.Integration;

[Collection("ContainerDb collection")]
public class CoursesController_GetAll_Tests_Testcontainers
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ContainerDbFixture _testcontainerFixture;
    private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public CoursesController_GetAll_Tests_Testcontainers(ContainerDbFixture testcontainerFixture)
    {
        if (testcontainerFixture.Client is not null && testcontainerFixture.Factory is not null)
        {
            _client = testcontainerFixture.Client;
            _factory = testcontainerFixture.Factory;
        }
        else
            throw new NotImplementedException();

        _testcontainerFixture = testcontainerFixture;
    }

    [Fact]
    public async Task GetAll_Smoke_CleanDb_Returns_EmptyList()
    {
        // No Arrange Empty DB
        await _testcontainerFixture.ResetMainDbAsync();
        // Act
        var response = await _client.GetAsync("/api/courses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();

        var dto = JsonSerializer.Deserialize<IEnumerable<CourseDto>>(json, _jsonOptions);

        dto.Should().NotBeNull();
        dto.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetAllLecturesByCourseIdOrdered_ReturnsOrderedByOrderNo()
    {
        // Arrange
        await _testcontainerFixture.ResetMainDbAsync();

        using var scope = _factory.Services.CreateScope();
        var mainDbContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();

        var courseId = "course-test-1";

        mainDbContext.Courses.Add(new Course(courseId, "Test course", null));

        mainDbContext.Lectures.AddRange(
            new Lecture(Guid.NewGuid(), courseId, 2, "Lecture 2", null, null),
            new Lecture(Guid.NewGuid(), courseId, 1, "Lecture 1", null, null)
        );

        await mainDbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/courses/{courseId}/lectures");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();

        var dto = JsonSerializer.Deserialize<IEnumerable<LectureListItemDto>>(json, _jsonOptions);

        dto.Should().NotBeNull();
        dto.Should().HaveCount(2);
        dto.Should().BeInAscendingOrder(lec => lec.OrderNo);
    }
}
