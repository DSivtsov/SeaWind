namespace Application.ModelsTime;

/// <summary>
/// Tester - внутренняя модель приложения (domain/application level)
/// сейчас по составу полей совпадает с TesterDto
/// со временем может получить поля PasswordHash, Role, CreatedAt и т.п.
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="Age"></param>
public class Tester
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }

    // Обязательный пустой конструктор для EF
    public Tester() { }

    // Удобный конструктор для ручного создания сущности
    public Tester(Guid id, string name, int age)
    {
        Id = id;
        Name = name;
        Age = age;
    }
}
