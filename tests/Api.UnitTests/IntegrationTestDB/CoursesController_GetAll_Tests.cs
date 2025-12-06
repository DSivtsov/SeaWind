using Api.UnitTests.IntegrationTestDB.Shared;
using Application.DtoCourse;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace Api.UnitTests.IntegrationTestDB;

[Collection("TestDb collection")]
public class CoursesController_GetAll_Tests
{
    private readonly TestDbFixture _testMainDbFixture;

    private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public CoursesController_GetAll_Tests(TestDbFixture testMainDbFixture)
    {
        _testMainDbFixture = testMainDbFixture;
    }

    [Fact]
    public async Task GetAll_returns_200_and_valid_payload()
    {
        // Arrange in TestMainDbFixture

        // Act
        var response = await _testMainDbFixture.Client.GetAsync("/api/courses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();

        var dto = JsonSerializer.Deserialize<IEnumerable<CourseDto>>(json, _jsonOptions);

        dto.Should().HaveCount(2);
    }

}
