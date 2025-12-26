using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.UseCasesCourse;
using AutoFixture;
using Moq;

namespace Application.UnitTests.UseCases.Course;

public class CourseServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly ICourseService _courseService;

    public CourseServiceTests()
    {
        _fixture = new Fixture();
        _courseRepositoryMock = _fixture.Freeze<Mock<ICourseRepository>>();
        _courseService = new CourseService(_courseRepositoryMock.Object);
    }

    /// <summary>
    /// Проверяет, что метод GetAllAsync сервиса курсов возвращает корректные данные из репозитория.
    /// </summary>
    /// <returns>
    /// Подтверждение того, что возвращаемые курсы соответствуют ожидаемым данным из репозитория
    /// и что метод репозитория был вызван ровно один раз.
    /// </returns>
    [Fact]
    public async Task CourseService_GetAllAsync_ReturnsCourses_FromRepository()
    {
        // Arrange
        var expectedCourses = _fixture.CreateMany<Models.Course>(3).ToList();
        _courseRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedCourses);
        // Act
        var result = await _courseService.GetAllAsync();
        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(expectedCourses.Count, resultList.Count);
        for (int i = 0; i < expectedCourses.Count; i++)
        {
            Assert.Equal(expectedCourses[i].Id, resultList[i].Id);
            Assert.Equal(expectedCourses[i].Title, resultList[i].Title);
            Assert.Equal(expectedCourses[i].Description, resultList[i].Description);
        }
        _courseRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
}
