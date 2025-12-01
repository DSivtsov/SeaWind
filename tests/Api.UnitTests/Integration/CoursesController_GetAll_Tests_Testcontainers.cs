using Api.UnitTests.Integration.Shared;
using Application.DtoCourse;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace Api.UnitTests.Integration;

[Collection("ContainerDb collection")]
public class CoursesController_GetAll_Tests_Testcontainers
{
    private readonly HttpClient _client;

    private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public CoursesController_GetAll_Tests_Testcontainers(ContainerDbFixture testcontainerFixture)
    {
        if (testcontainerFixture.Client is not null)
        {
            _client = testcontainerFixture.Client;
        }
        else
            throw new NotImplementedException();
    }

    [Fact]
    public async Task GetAll_returns_200_and_valid_payload()
    {
        // Arrange in TestMainDbFixture

        // Act
        var response = await _client.GetAsync("/api/courses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();

        var dto = JsonSerializer.Deserialize<IEnumerable<CourseDto>>(json, _jsonOptions);

        dto.Should().HaveCount(2);
    }
}