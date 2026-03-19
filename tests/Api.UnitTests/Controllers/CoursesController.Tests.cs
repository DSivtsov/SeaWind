using Api.Controllers;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DtoCourse;
using Application.Models;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.UnitTests.Controllers;

public class CoursesControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    public CoursesControllerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _courseServiceMock = _fixture.Freeze<Mock<ICourseService>>();
        _courseRepositoryMock = _fixture.Freeze<Mock<ICourseRepository>>();
        _currentUserServiceMock = _fixture.Freeze<Mock<ICurrentUserService>>();
    }

    /// <summary>
    /// Проверяет, что GetAll возвращает HTTP 200 OK и пустую коллекцию,
    /// когда сервис курсов не возвращает элементов.
    /// </summary>
    [Fact]
    public async Task GetAll_ReturnsOk_WithCourses()
    {
        // Arrange
        var courses = _fixture.CreateMany<CourseDto>(2).ToArray();
        _courseServiceMock.Setup(s => s.GetAllCoursesAsync()).ReturnsAsync(courses);

        var controller = new CoursesController(_courseServiceMock.Object, _courseRepositoryMock.Object,
            _currentUserServiceMock.Object);

        // Act
        var result = await controller.GetAllCoursesAsync();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsAssignableFrom<IEnumerable<CourseDto>>(ok.Value);

        Assert.Equal(courses, value);
    }


    /// <summary>
    /// Проверяет, что действие GetAll возвращает ответ HTTP 200 OK с пустым массивом.
    /// </summary>
    [Fact]
    public async Task GetAll_ReturnsOk_WithEmptyArray_WhenServiceReturnsEmpty()
    {
        // Arrange
        _courseServiceMock.Setup(srv => srv.GetAllCoursesAsync()).ReturnsAsync(Array.Empty<CourseDto>());
        var controller = new CoursesController(_courseServiceMock.Object, _courseRepositoryMock.Object,
            _currentUserServiceMock.Object);

        // Act
        var result = await controller.GetAllCoursesAsync();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsAssignableFrom<IEnumerable<CourseDto>>(ok.Value);
        Assert.Empty(value);
    }
}
