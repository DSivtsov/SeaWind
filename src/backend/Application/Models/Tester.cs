namespace Application.Models;

/// <summary>
/// Tester - внутренняя модель приложения (domain/application level)
/// сейчас по составу полей совпадает с TesterDto
/// со временем может получить поля PasswordHash, Role, CreatedAt и т.п.
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="Age"></param>
public record Tester(Guid Id, string Name, int Age);
