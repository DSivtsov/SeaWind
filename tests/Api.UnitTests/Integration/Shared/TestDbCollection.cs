namespace Api.UnitTests.Integration.Shared;

[CollectionDefinition("TestDb collection")]
public class TestDbCollection : ICollectionFixture<TestDbFixture>
{
    // пусто — только связывает фикстуру с коллекцией
}

