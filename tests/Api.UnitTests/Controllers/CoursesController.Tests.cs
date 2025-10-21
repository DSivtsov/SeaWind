using Api.Controllers;
using Application.Abstractions.Services;
using Application.Dto.Course;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.UnitTests.Controllers;

public class CoursesControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly CoursesController _coursesController;

    public CoursesControllerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _courseServiceMock = _fixture.Freeze<Mock<ICourseService>>();
        _coursesController = _fixture.Build<CoursesController>().OmitAutoProperties().Create();
    }

    /// <summary>
    /// Проверяет, что GetAll контроллера возвращает OkObjectResult, содержащий коллекцию объектов CourseDto, когда курсы доступны.
    /// </summary>
    /// <remarks>
    /// Этот тест гарантирует, что метод GetAll контроллера отвечает HTTP 200 OK 
    /// и включает ожидаемое количество объектов курса с допустимыми свойствами.
    /// Он проверяет как тип ответа, так и целостность возвращаемых данных.
    /// </remarks>
    /// <returns></returns>
    [Fact]
    public async Task GetAll_ReturnsOk_WithCourses()
    {
        // Arrange
        int countCourses = 2;
        IEnumerable<CourseDto> courses = _fixture
            .Build<CourseDto>()
            .CreateMany(countCourses);

        _courseServiceMock.Setup(srv => srv.GetAllAsync()).ReturnsAsync(courses);

        // Act
        var actionResult = await _coursesController.GetAll();

        // Assert
        OkObjectResult coursesResultType = Assert.IsType<OkObjectResult>(actionResult.Result);
        IEnumerable<CourseDto> coursesResult = Assert.IsAssignableFrom<IEnumerable<CourseDto>>(coursesResultType.Value);

        Assert.Equal(countCourses, coursesResult.Count());

        foreach (var course in coursesResult)
        {
            Assert.NotEqual(Guid.Empty, course.Id);
            Assert.False(string.IsNullOrWhiteSpace(course.Title));
            Assert.False(string.IsNullOrWhiteSpace(course.Description));
        }
    }


    /// <summary>
    /// Проверяет, что действие GetAll возвращает ответ HTTP 200 OK с пустым массивом, когда сервис возвращает значение NULL.
    /// </summary>
    /// <remarks>
    /// Этот тест гарантирует, что контроллер нормализует нулевой результат службы в пустой массив в ответе, 
    /// обеспечивая согласованное поведение API для клиентов.
    /// </remarks>
    /// <returns></returns>
    [Fact]
    public async Task GetAll_ReturnsOk_WithEmptyArray_WhenServiceReturnsNull()
    {
        // Arrange
        _courseServiceMock.Setup(srv => srv.GetAllAsync()).ReturnsAsync(() => null!);

        // Act
        var actionResult = await _coursesController.GetAll();

        // Assert
        OkObjectResult coursesResultType = Assert.IsType<OkObjectResult>(actionResult.Result);
        IEnumerable<CourseDto> coursesResult = Assert.IsAssignableFrom<IEnumerable<CourseDto>>(coursesResultType.Value);

        Assert.Empty(coursesResult);
    }
}