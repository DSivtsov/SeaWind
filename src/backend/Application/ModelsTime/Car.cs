namespace Application.ModelsTime;

public class Car
{
    public Guid Id { get; set; }
    public string Model { get; set; } = null!;
    public Guid Owner { get; set; }
    public string RegNumber { get; set; } = null!;

    // Обязательный пустой конструктор для EF
    public Car() { }

    // Удобный конструктор для ручного создания сущности
    public Car(Guid id, string model, Guid owner, string regNumber)
    {
        Id = id;
        Model = model;
        Owner = owner;
        RegNumber = regNumber;
    }
}
