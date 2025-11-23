namespace Infrastructure.UnitTests.Seeding.IntegrationTestsSeeders.Shared;

public class TestEntity
{
    public Guid Id { get; set; }
    public string Value { get; set; } = null!;

    // Обязательный пустой конструктор для EF
    public TestEntity() { }

    // Удобный конструктор для ручного создания сущности
    public TestEntity(Guid id, string value)
    {
        Id = id;
        Value = value;
    }
}
